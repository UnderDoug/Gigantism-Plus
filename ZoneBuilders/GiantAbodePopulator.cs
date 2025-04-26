using Genkit;
using Qud.API;

using System.Collections.Generic;
using System.Linq;

using XRL.Rules;
using XRL.World.Parts;
using XRL.World.AI.Pathfinding;
using static XRL.Core.XRLCore;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.ZoneBuilders
{
    public class GiantAbodePopulator
        : ZoneBuilderSandbox
    {
        Dictionary<string, Dictionary<string, List<Cell>>> Regions;

        public string GiantID;

        public GiantAbodePopulator()
        {
            Regions = new();
        }

        public bool BuildZone(Zone Z)
        {
            Debug.Entry(4,
                $"\u2229 {typeof(GiantAbodePopulator).Name}." +
                $"{nameof(BuildZone)}(Zone Z: {Z.ZoneID})",
                Indent: 0);

            zone = Z;

            List<Cell> GraniteCells = Z.GetCellsWithObject("Granite");

            List<Cell> regionCells = new();
            List<GameObject> trashCan = new();
            int abodeNumber = 0;
            foreach (GameObject abodeSpawner in zone.GetObjectsThatInheritFrom("GiantAbodeSpawner"))
            {
                bool isUnique = abodeSpawner.Blueprint == "GiantAbodeSpawner Cook";
                string abodeLabel = $"HNPS_GigantismPlus::Abode:{(isUnique ? "Cook" : ++abodeNumber)}::";
                string DoorDirection = abodeSpawner.GetTagOrStringProperty("DoorDirection");
                string ContentsTable = abodeSpawner.GetTagOrStringProperty("ContentsTable");
                string Floor = abodeSpawner.GetTagOrStringProperty("Floor");
                string Wall = abodeSpawner.GetTagOrStringProperty("Wall");
                int radius = abodeSpawner.GetIntProperty("Radius");
                bool DoorRandom = abodeSpawner.HasTagOrStringProperty("DoorRandom");
                bool Constrained = DoorRandom && abodeSpawner.GetTagOrStringProperty("DoorRandom") == "Constrained";
                radius = radius != 0 ? radius : 2;

                trashCan.Add(abodeSpawner);

                Cell cell = abodeSpawner.CurrentCell;
                cell.RemoveObject(abodeSpawner, Forced: true, Silent: true);

                cell.Clear();
                int x1 = cell.X - radius;
                int y1 = cell.Y - radius;
                int x2 = cell.X + radius;
                int y2 = cell.Y + radius;
                int doorXRnd = RndGP.Next(x1 + 1, x2 - 1);
                int doorYRnd = RndGP.Next(y1 + 1, y2 - 1);
                int doorX = Constrained ? doorXRnd : cell.X;
                int doorY = Constrained ? doorYRnd : cell.Y;
                Point2D doorLocation = DoorDirection switch
                {
                    "N" => new Point2D(doorX, y1),
                    "S" => new Point2D(doorX, y2),
                    "E" => new Point2D(x2, doorY),
                    "W" => new Point2D(x1, doorY),
                    "NW" => new Point2D(x1, y1),
                    "NE" => new Point2D(x2, y1),
                    "SW" => new Point2D(x1, y2),
                    "SE" => new Point2D(x2, y2),
                    _ => new Point2D(),
                };
                if (DoorRandom && !Constrained)
                {
                    doorLocation = new(doorXRnd, doorYRnd);
                }
                Rect2D R = new(x1, y1, x2, y2, doorLocation);

                Dictionary<string, List<Cell>> Region = Z.GetHutRegion(R, true);
                Regions.Add($"Abode:{(isUnique ? "Cook" : abodeNumber)}", Region);

                foreach (Cell outerCell in Region["Outer"])
                {
                    if (Region["Inner"].Contains(outerCell))
                        Region["Inner"].Remove(outerCell);
                    regionCells.Add(outerCell);
                    outerCell.ClearAndAddObject(8.in100() ? "WallOrDebrisLimestoneNoSmall" : Wall);
                }
                foreach (Cell innerCell in Region["Inner"])
                {
                    regionCells.Add(innerCell);
                    PaintCell(innerCell.Clear(), Floor);
                }
                Cell doorCell = Region["Door"][0];
                R.Door.x = doorCell.X;
                R.Door.y = doorCell.Y;

                Rect2D P = R.GetCellSide(R.Door) switch
                {
                    "N" => new(R.x1, R.y1 + 1, R.x2, R.y2, R.Door),
                    "S" => new(R.x1, R.y1, R.x2, R.y2 - 1, R.Door),
                    "E" => new(R.x1, R.y1, R.x2 - 1, R.y2, R.Door),
                    "W" => new(R.x1 + 1, R.y1, R.x2, R.y2, R.Door),
                    _ => R,
                };

                List<Location2D> popArea = new();
                List<Cell> popCells = new();
                foreach (Point2D point in P.ReduceBy(1,1).getPoints())
                {
                    Cell pointCell = Z.GetCell(point);
                    Location2D pointLocation = pointCell.Location;
                    if (!popArea.Contains(pointLocation))
                        popArea.Add(pointLocation);
                    if (!popCells.Contains(pointCell))
                        popCells.Add(pointCell);
                }
                Region.Add("Population", popCells);

                doorCell.Clear();
                foreach (Cell adjacentCell in doorCell.GetCardinalAdjacentCells())
                {
                    if (!Region["Outer"].Contains(adjacentCell) && !Region["Inner"].Contains(adjacentCell))
                    {
                        adjacentCell.Clear().RequireObject("DirtPath");
                    }
                }

                GameObject door = EncountersAPI.GetAnObject((GameObjectBlueprint blueprint)
                => blueprint.InheritsFrom("Door")
                && !blueprint.HasTag("BaseObject")
                && !blueprint.Name.Contains("Double")
                && blueprint.Parts.ContainsKey("ModGigantic")
                && blueprint.Tier < 3
                && !blueprint.Name.Contains("Gate"));
                if (door != null)
                    doorCell.AddObject(door);

                string popRegionString = string.Empty;
                foreach (Location2D popLocation in popArea)
                {
                    popRegionString += popRegionString == string.Empty ? $"[{popLocation}]" : $",[{popLocation}]";
                }
                Debug.Entry(4, $"populationRegion: {popRegionString}", Indent: 1);
                Debug.Entry(4,
                    $"> foreach (PopulationResult item in ContentsTable: {ContentsTable.Quote()})",
                    Indent: 1);
                foreach (PopulationResult item in PopulationManager.Generate(ContentsTable, "zonetier", Z.NewTier.ToString()))
                {
                    Debug.Divider(4, HONLY, Count: 25, Indent: 1);

                    Debug.Entry(4, $"item: {item.Blueprint}, number: {item.Number}", Indent: 1);

                    Debug.Entry(4,
                        $"> for (int num = 0; num < item.Number; num++)",
                        Indent: 2);
                    for (int num = 0; num < item.Number; num++)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                        Debug.Entry(4, 
                            $"item: {item.Blueprint}, " + 
                            $"number: {num + 1}/{item.Number}, " + 
                            $"hint: {item.Hint.Quote()}", 
                            Indent: 3);

                        GameObject gameObject = GameObjectFactory.Factory.CreateObject(item.Blueprint);
                        if (!PlaceObjectInArea(Z, new LocationList(popArea), gameObject, 0, 0, item.Hint))
                            Debug.CheckNah(4, $"Failed to place [{num + 1}]{item.Blueprint}", Indent: 3);

                        else Debug.CheckYeh(4, $"[{num + 1}]{item.Blueprint} placed successfully", Indent: 3);
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                    Debug.Entry(4,
                        $"x for (int num = 0; num < item.Number; num++) >//",
                        Indent: 2);
                }
                Debug.Divider(4, HONLY, Count: 25, Indent: 1);
                Debug.Entry(4, $"x foreach (PopulationResult item in ContentsTable: {ContentsTable.Quote()}) >//", Indent: 1);

                string abodeRegionString = string.Empty;
                foreach ((string regionLabel, List<Cell> cells) in Region)
                {
                    zone.SetZoneProperty(abodeLabel+regionLabel, cells.ToStringList().Join(";"));
                }
            }
            zone.SetZoneProperty($"HNPS_GigantismPlus::Abodes", $"{abodeNumber}");
            foreach (GameObject trash in trashCan)
            {
                trash.Obliterate(null, true);
            }

            List<Cell> nonRegionEmptyCells = new();
            foreach (Cell emptyCell in Z.GetEmptyCells())
            {
                if (!regionCells.Contains(emptyCell)) nonRegionEmptyCells.Add(emptyCell);
            }

            foreach ((_,Dictionary<string, List<Cell>> region) in Regions)
            {
                Cell nearestEmptyCell = null;
                Cell doorCell = region["Door"][0];
                foreach (Cell emptyCell in nonRegionEmptyCells)
                {
                    nearestEmptyCell ??= emptyCell;
                    if (doorCell.CosmeticDistanceTo(emptyCell.X, emptyCell.Y) < doorCell.CosmeticDistanceTo(emptyCell.X, emptyCell.Y))
                        nearestEmptyCell = emptyCell;
                }
                CleanQueue<SortPoint> avoidCells = new();
                foreach(Cell avoidCell in regionCells)
                {
                    SortPoint avoidPoint = new(avoidCell.X, avoidCell.Y);
                    if (!avoidCells.Contains(avoidPoint)) avoidCells.Enqueue(avoidPoint);
                }
                FindPath path = new(doorCell, nearestEmptyCell, Avoid: avoidCells);
                foreach (Cell step in path.Steps)
                {
                    if (step == doorCell) continue;
                    step.Clear();
                    if (85.in100())
                    {
                        step.RequireObject("DirtPath");
                    }
                    // step.HighlightBlue(12);
                }
            }

            foreach (Cell cell in GraniteCells)
            {
                if (!cell.GetObjectsThatInheritFrom("Wall").IsNullOrEmpty() && !cell.HasObject("Granite"))
                {
                    bool doRemplacement = true;
                    foreach (Cell ordinalCell in cell.GetOrdinalAdjacentCells())
                    {
                        if (!ordinalCell.GetObjectsThatInheritFrom("Door").IsNullOrEmpty())
                        {
                            doRemplacement = false;
                            break;
                        }
                    }
                    if (doRemplacement)
                    {
                        cell.Clear().AddObject("WallOrDebrisGraniteNoSmall");
                    }
                }
            }

            if (false) // this is just to have an easy toggle
            {
                foreach ((_, Dictionary<string, List<Cell>> Region) in Regions)
                {
                    foreach ((string label, List<Cell> subregion) in Region)
                    {
                        foreach (Cell cell in subregion)
                        {
                            switch (label)
                            {
                                case "Inner":
                                    cell.HighlightCyan(5);
                                    break;
                                case "Outer":
                                    cell.HighlightPurple(3);
                                    break;
                                case "Door":
                                    cell.HighlightRed(8);
                                    break;
                                case "Population":
                                    cell.HighlightGreen(10);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            Cell giantLocation = 
                zone?.FindFirstObject("Gigantic Oven")?.CurrentCell?.GetEmptyAdjacentCells()?.GetRandomElement()
             ?? nonRegionEmptyCells?.GetRandomElement() 
             ?? zone?.GetEmptyCells()?.GetRandomElement();

            GameObject UniqueGiant = The.ZoneManager.GetCachedObjects(GiantID) ?? SecretGiantWhoCooksBuilderExtension.GetTheGiant();

            if (UniqueGiant != null)
                giantLocation.AddObject(UniqueGiant);

            return true;
        } //!-- public bool BuildZone(Zone Z)

        public static void PaintCell(Cell C, string Floor = null, bool Overwrite = true)
        {
            string paintColorString = "&y";
            string paintTile = "Tiles/tile-dirt1.png";
            string paintDetailColor = "k";
            string paintTileColor = paintColorString;
            string paintRenderString = "ú";
            GameObject floorSample = GameObjectFactory.Factory.CreateSampleObject(Floor);
            if (floorSample != null && floorSample.TryGetPart(out Render floorRender))
            {
                paintColorString = floorRender.ColorString;
                paintTile = floorRender.Tile;
                paintDetailColor = floorRender.DetailColor;
                paintTileColor = floorRender.TileColor;
                paintRenderString = floorRender.RenderString;
            }
            if (Overwrite || string.IsNullOrEmpty(C.PaintTile))
            {
                C.PaintColorString = paintColorString;
                C.PaintTile = paintTile;
                C.PaintDetailColor = paintDetailColor;
                C.PaintTileColor = paintTileColor;
                C.PaintRenderString = paintRenderString;
            }
        }
    } //!-- public class GiantAbodePopulator : ZoneBuilderSandbox
}
