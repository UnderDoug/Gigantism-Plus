using Genkit;
using Qud.API;

using System.Collections.Generic;
using System.Linq;

using XRL.Rules;
using XRL.World.AI.Pathfinding;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using XRL.World.Parts;

namespace XRL.World.ZoneBuilders
{
	public class GiantAbodePopulator
        : ZoneBuilderSandbox
    {
        List<Dictionary<string, List<Cell>>> Regions;
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
            foreach (GameObject abodeSpawner in zone.GetObjectsThatInheritFrom("GiantAbodeSpawner"))
            {
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
                int doorXRnd = RndGP.Next(x1+1, x2-1);
                int doorYRnd = RndGP.Next(y1+1, y2-1);
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
                Regions.Add(Region);

                foreach (Cell outerCell in Region["Outer"])
                {
                    if (Region["Inner"].Contains(outerCell))
                        Region["Inner"].Remove(outerCell);
                    regionCells.Add(outerCell);
                    outerCell.ClearAndAddObject(Wall);
                }
                
                List<Location2D> innerRegion = new();
                foreach (Cell innerCell in Region["Inner"])
                {
                    bool noAdjacentDoors = true;
                    foreach (Cell adjacentCell in innerCell.GetAdjacentCells())
                    {
                        if (!cell.GetObjectsThatInheritFrom("Door").IsNullOrEmpty())
                        {
                            noAdjacentDoors = false;
                            break;
                        }
                    }
                    regionCells.Add(innerCell);
                    PaintCell(innerCell.Clear(), Floor);
                    if (noAdjacentDoors)
                    {
                        innerRegion.Add(innerCell.Location);
                    }
                }

                Cell doorCell = Region["Door"][0];

                doorCell.Clear();
                foreach (Cell adjacentCell in doorCell.GetAdjacentCells())
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

                foreach (PopulationResult item in PopulationManager.Generate(ContentsTable, "zonetier", Z.NewTier.ToString()))
                {
                    for (int num = 0; num < item.Number; num++)
                    {
                        PlaceObjectInArea(Z, new LocationList(innerRegion), GameObject.Create(item.Blueprint), 0, 0, item.Hint);
                    }
                }

            }
            foreach(GameObject trash in trashCan)
            {
                trash.Obliterate(null, true);
            }

            List<Cell> nonRegionEmptyCells = new();
            foreach (Cell emptyCell in Z.GetEmptyCells())
            {
                if (!regionCells.Contains(emptyCell)) nonRegionEmptyCells.Add(emptyCell);
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
                        cell.Clear().AddObject("WallOrDebrisGranite");
                    }
                }
            }
            foreach (Dictionary<string,List<Cell>> region in Regions)
            {
                Cell nearestEmptyCell = null;
                Cell doorCell = region["Door"][0];
                foreach (Cell emptyCell in nonRegionEmptyCells)
                {
                    nearestEmptyCell ??= emptyCell;
                    if (doorCell.CosmeticDistanceTo(emptyCell.X, emptyCell.Y) < doorCell.CosmeticDistanceTo(emptyCell.X, emptyCell.Y))
                        nearestEmptyCell = emptyCell;
                }
                FindPath path = new(doorCell, nearestEmptyCell);
                foreach (Cell step in path.Steps)
                {
                    if (step == doorCell) continue;
                    step.Clear();
                    if (85.in100())
                    {
                        step.RequireObject("DirtPath");
                    }
                }
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
    }
}
