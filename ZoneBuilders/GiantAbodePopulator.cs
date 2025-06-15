using System;
using System.Collections.Generic;
using System.Linq;

using Genkit;
using Qud.API;

using XRL.Rules;
using XRL.World.AI.Pathfinding;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;

using static XRL.Core.XRLCore;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.ZoneBuilders
{
    public class GiantAbodePopulator
        : ZoneBuilderSandbox
    {
        private static bool doDebug => getClassDoDebug(nameof(GiantAbodePopulator));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                '!',    // Warn
            };
            List<object> dontList = new()
            {
                "CH",   // Cell Highlighting
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }


        public const string INNER = "Inner";
        public const string OUTER = "Outer";
        public const string DOOR = "Door";
        public const string POPULATION = "Population";

        public Dictionary<string, Dictionary<string, List<Cell>>> Regions;

        public string GiantID;

        public GiantAbodePopulator()
        {
            Regions = new();
            GiantID = "";
        }

        public bool BuildZone(Zone Z)
        {
            Debug.Entry(4,
                $"\u2229 {nameof(GiantAbodePopulator)}." +
                $"{nameof(BuildZone)}(Zone Z: {Z.ZoneID})",
                Indent: 0, Toggle: getDoDebug());

            zone = Z;

            List<Cell> GraniteCells = Z.GetCellsWithObject("Granite");

            List<Cell> regionCells = new();
            List<GameObject> trashCan = new();

            Cell giantOvenCell = null;

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
                int doorXRnd = Stat.Roll(x1 + 1, x2 - 1);
                int doorYRnd = Stat.Roll(y1 + 1, y2 - 1);
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

                foreach (Cell outerCell in Region[OUTER])
                {
                    if (Region[INNER].Contains(outerCell))
                        Region[INNER].Remove(outerCell);
                    regionCells.Add(outerCell);
                    outerCell.ClearAndAddObject(8.in100() ? "WallOrDebrisLimestoneNoSmall" : Wall);
                }
                foreach (Cell innerCell in Region[INNER])
                {
                    regionCells.Add(innerCell);
                    PaintCell(innerCell.Clear(), Floor);
                }
                Cell doorCell = Region[DOOR][0];
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
                Region.Add(POPULATION, popCells);

                doorCell.Clear();
                foreach (Cell adjacentCell in doorCell.GetCardinalAdjacentCells())
                {
                    if (!Region[OUTER].Contains(adjacentCell) && !Region[INNER].Contains(adjacentCell))
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
                Debug.Entry(4, $"populationRegion: {popRegionString}", Indent: 1, Toggle: getDoDebug());
                Debug.Entry(4,
                    $"> foreach (PopulationResult item in ContentsTable: {ContentsTable.Quote()})",
                    Indent: 1, Toggle: getDoDebug());
                foreach (PopulationResult item in PopulationManager.Generate(ContentsTable, "zonetier", Z.NewTier.ToString()))
                {
                    Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

                    Debug.Entry(4, $"item: {item.Blueprint}, number: {item.Number}", Indent: 1, Toggle: getDoDebug());

                    Debug.Entry(4,
                        $"> for (int num = 0; num < item.Number; num++)",
                        Indent: 2);
                    for (int num = 0; num < item.Number; num++)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: getDoDebug());
                        Debug.Entry(4, 
                            $"item: {item.Blueprint}, " + 
                            $"number: {num + 1}/{item.Number}, " + 
                            $"hint: {item.Hint.Quote()}", 
                            Indent: 2, Toggle: getDoDebug());

                        GameObject gameObject = GameObjectFactory.Factory.CreateObject(item.Blueprint);
                        if (!PlaceObjectInArea(Z, new LocationList(popArea), gameObject, 0, 0, item.Hint))
                        {
                            Debug.CheckNah(4, $"Failed to place [{num + 1}]{item.Blueprint}", Indent: 3, Toggle: getDoDebug());
                        }
                        else
                        {
                            Debug.CheckYeh(4, $"[{num + 1}]{item.Blueprint} placed successfully", Indent: 3, Toggle: getDoDebug());
                            if (isUnique && item.Blueprint == "Gigantic Oven")
                            {
                                giantOvenCell = gameObject?.CurrentCell;
                                Debug.CheckYeh(4, $"Giant Oven location stored", Indent: 4, Toggle: getDoDebug());
                            }
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: getDoDebug());
                    Debug.Entry(4,
                        $"x for (int num = 0; num < item.Number; num++) >//",
                        Indent: 2, Toggle: getDoDebug());
                }
                Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());
                Debug.Entry(4, $"x foreach (PopulationResult item in ContentsTable: {ContentsTable.Quote()}) >//", Indent: 1, Toggle: getDoDebug());

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
                Cell doorCell = region[DOOR][0];
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
                    if (getDoDebug("CH"))
                    {
                        step.HighlightBlue(12);
                    }
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

            if (getDoDebug("CH"))
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

            GameObject UniqueGiant = The.ZoneManager.GetCachedObjects(GiantID);
            if (UniqueGiant == null)
            {
                Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to retreive Unique {nameof(WrassleGiantHero)} from cache " +
                    $"in zone {zone?.ZoneID}",
                    Indent: 1);

                UniqueGiant = SecretGiantWhoCooksBuilderExtension.GetTheGiant();
            }

            List<Cell> emptyInnerCells =
                    (from c in Regions["Abode:Cook"][INNER]
                     where c.IsEmptyFor(UniqueGiant)
                     select c).ToList();

            Cell giantLocation = 
                giantOvenCell?.GetEmptyAdjacentCells()?.GetRandomElement()
             ?? emptyInnerCells?.GetRandomElement()
             ?? zone?.FindFirstObject("Gigantic Oven")?.CurrentCell.GetEmptyAdjacentCells()?.GetRandomElement()
             ?? nonRegionEmptyCells?.GetRandomElement() 
             ?? zone?.GetEmptyCells()?.GetRandomElement();

            if (UniqueGiant != null)
            {
                if (giantLocation != null)
                {
                    giantLocation.AddObject(UniqueGiant);
                    if (UniqueGiant.Brain != null)
                    {
                        UniqueGiant.Brain.StartingCell = new();
                        UniqueGiant.Brain.StartingCell.SetCell(giantLocation);
                        UniqueGiant.Brain.Wanders = true;
                        UniqueGiant.Brain.WandersRandomly = true;
                    }
                    if (UniqueGiant.TryGetPart(out StewBelly stewBelly))
                    {
                        stewBelly.ProcessStartingStews();
                    }
                }
                else
                {
                    Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to find suitable cell" +
                    $"in zone {zone?.ZoneID} " +
                    $"for Unique {nameof(WrassleGiantHero)} {UniqueGiant?.DebugName ?? NULL}",
                    Indent: 1);
                }
            }
            else 
            {
                Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to retreive Unique {nameof(WrassleGiantHero)} from cache " +
                    $"in zone {zone?.ZoneID} " + 
                    $"for cell [{giantLocation?.Location}]",
                    Indent: 1);
            }

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
