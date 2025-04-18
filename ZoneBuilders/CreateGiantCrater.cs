using Genkit;
using HistoryKit;

using System.Collections.Generic;

using XRL.Rules;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using System.Linq;
using XRL.EditorFormats.Screen;

namespace XRL.World.ZoneBuilders
{
	public class CreateGiantCrater
        : ZoneBuilderSandbox
    {
        public string CraterWallMaterial;
        public string MapWallMaterial;
        public int Radius;
        public int Thickness;
        public Point2D Center;

        public List<(Location2D location, Cell cell)> CraterSpaceRegion;
        public List<(Location2D location, Cell cell)> CraterBorderRegion;

        public CreateGiantCrater()
        {
            CraterWallMaterial = "Granite";
            MapWallMaterial = "Limestone";
            Radius = 8;
            Thickness = 2;
            Center = new(40, 12);
            CraterSpaceRegion = new();
            CraterBorderRegion = new();
        }
        public CreateGiantCrater(string CraterWallMaterial, string MapWallMaterial, int Radius, int Variance)
            : this()
        {
            this.CraterWallMaterial = CraterWallMaterial;
            this.MapWallMaterial = MapWallMaterial;
            this.Radius = Radius;
            this.Thickness = Variance;
        }
        public CreateGiantCrater(string CraterWallMaterial, string MapWallMaterial, int Radius, int Variance, Point2D Center)
            : this(CraterWallMaterial, MapWallMaterial, Radius, Variance)
        {
            this.Center = Center;
        }
        public CreateGiantCrater(CreateGiantCrater Source)
            : this(Source.CraterWallMaterial, Source.MapWallMaterial, Source.Radius, Source.Thickness)
        {
            Center = new(Source.Center.x, Source.Center.y);
        }

        public bool BuildZone(Zone Z)
        {
            Debug.Entry(4,
                $"\u03C0 {typeof(CreateGiantCrater).Name}." +
                $"{nameof(BuildZone)}(Zone Z: {Z.ZoneID})",
                Indent: 0);

            zone = Z;

            SolidMaterial solidMaterial = new(MapWallMaterial);
            solidMaterial.BuildZone(Z);

            GenerateBorder();

            /*
            List<Location2D> locations = new();
            string blueprint = Z.GetAnyObjectWithTag("Wall")?.Blueprint ?? "limestone";
            List<string> dwellings = new();
            foreach (CachedZoneConnection connection in Z.ZoneConnectionCache)
            {
                if (connection.TargetDirection == "-" && connection.Type.StartsWith("burrow"))
                {
                    locations.Add(Location2D.Get(connection.X, connection.Y));
                    string[] array = connection.Type.Split(',');
                    if (array.Length > 1 && !string.IsNullOrEmpty(array[1]))
                    {
                        dwellings.Add(array[1]);
                    }
                    else
                    {
                        dwellings.Add("Villages_BuildingContents_Dwelling_*Default");
                    }
                }
            }
            foreach (ZoneConnection zoneConnection in The.ZoneManager.GetZoneConnections(Z.ZoneID))
            {
                if (zoneConnection.Type == "burrow")
                {
                    locations.Add(Location2D.Get(zoneConnection.X, zoneConnection.Y));
                    string[] array2 = zoneConnection.Type.Split(',');
                    if (array2.Length > 1 && !string.IsNullOrEmpty(array2[1]))
                    {
                        dwellings.Add(array2[1]);
                    }
                    else
                    {
                        dwellings.Add("Villages_BuildingContents_Dwelling_*Default");
                    }
                }
            }
            if (locations.Count > 0)
            {
                List<Box> rooms = new();
                foreach (Location2D location in locations)
                {
                    int num = RndGP.Next(4, 6);
                    Box box = new Box(location.X - num, location.Y - num, location.X + num, location.Y + num).clamp(1, 1, 78, 23);
                    rooms.Add(box);
                    Z.ClearBox(box);
                }
                InfluenceMap influenceMap = new(Z.Width, Z.Height);
                Z.SetInfluenceMapWalls(influenceMap.Walls);
                foreach (Box room in rooms)
                {
                    influenceMap.AddSeed(room.center, bRecalculate: false);
                }
                influenceMap.Recalculate();
                influenceMap.SeedAllUnseeded();
                while (influenceMap.LargestSeed() > 100)
                {
                    influenceMap.AddSeedAtMaxima();
                }
                for (int i = 0; i < rooms.Count; i++)
                {
                    foreach (Location2D borderCell in influenceMap.Regions[i].getBorder(2))
                    {
                        Z.GetCell(borderCell).AddObject(blueprint);
                    }
                    List<PopulationResult> populations = PopulationManager.Generate(dwellings[i]);
                    PopulationLayout populationLayout = new(zone, influenceMap.Regions[i], rooms[i].rect, locations[i]);
                    populationLayout.inside.AddRange(influenceMap.Regions[i].reducyBy(2));
                    populationLayout.insideWall.AddRange(influenceMap.Regions[i].reducyBy(2).Except(influenceMap.Regions[i].reducyBy(3)));
                    populationLayout.insideCorner.AddRange(influenceMap.Regions[i].reducyBy(2).Except(influenceMap.Regions[i].reducyBy(3)));
                    foreach (PopulationResult population in populations)
                    {
                        PlaceObjectInBuilding(GameObject.Create(population.Blueprint), populationLayout);
                    }
                }
            }
            string damageChance = RndGP.Next(5, 25).ToString();
            PowerGrid powerGrid = new();
            powerGrid.DamageChance = damageChance;
            if ((10 + Z.Tier * 3).in100())
            {
                powerGrid.MissingConsumers = "1d6";
                powerGrid.MissingProducers = "1d3";
            }
            powerGrid.BuildZone(Z);
            Hydraulics hydraulics = new();
            hydraulics.DamageChance = damageChance;
            if ((10 + Z.Tier * 3).in100())
            {
                hydraulics.MissingConsumers = "1d6";
                hydraulics.MissingProducers = "1d3";
            }
            hydraulics.BuildZone(Z);
            MechanicalPower mechanicalPower = new();
            mechanicalPower.DamageChance = damageChance;
            if ((20 - Z.Tier).in100())
            {
                mechanicalPower.MissingConsumers = "1d6";
                mechanicalPower.MissingProducers = "1d3";
            }
            mechanicalPower.BuildZone(Z);
            */
            return true;
        } //!-- public bool BuildZone(Zone Z)

        public List<(Location2D location, Cell cell)> CarveSpace(bool Inner = false)
        {
            DieRoll dieRoll = new("1d2");
            List<(Location2D location, Cell cell)> region = new();
            foreach (Cell cell in zone.GetCells())
            {
                int dist = cell.CosmeticDistanceTo(Center);
                int extraRadius = !Inner ? Thickness : 0;
                int radius = Radius + extraRadius;
                if (dist > radius)
                    continue;

                if (dist == radius)
                {
                    if (dieRoll.Resolve() == 1)
                    {
                        cell.Clear();
                        if (!region.Contains((cell.Location, cell))) 
                            region.Add((cell.Location, cell));
                    }
                    else
                    {
                        if (region.Contains((cell.Location, cell))) 
                            region.Remove((cell.Location, cell));
                        cell.Clear();
                        cell.AddObject(MapWallMaterial);
                    }
                    foreach (Cell adjacentCell in cell.GetAdjacentCells())
                    {
                        if (Inner && adjacentCell.CosmeticDistanceTo(Center) > radius)
                        {
                            if (region.Contains((adjacentCell.Location, adjacentCell)))
                                region.Remove((adjacentCell.Location, adjacentCell));
                            cell.Clear();
                            cell.AddObject(MapWallMaterial);
                            continue;
                        }
                        if (dieRoll.Resolve() == 1)
                        {
                            adjacentCell.Clear();
                            if (!region.Contains((adjacentCell.Location, adjacentCell))) 
                                region.Add((adjacentCell.Location, adjacentCell));
                        }
                        else
                        {
                            if (region.Contains((adjacentCell.Location, adjacentCell))) 
                                region.Remove((adjacentCell.Location, adjacentCell));
                            cell.Clear();
                            cell.AddObject(MapWallMaterial);
                        }
                    }
                    continue;
                }
                cell.Clear();
                if (!region.Contains((cell.Location, cell))) 
                    region.Add((cell.Location, cell));
            }
            return region;
        } //!-- public List<(Location2D location, Cell cell)> CarveSpace(bool Inner = false)

        public List<(Location2D location, Cell cell)> CarveInnerSpace()
        {
            return CarveSpace(Inner: true);
        }
        public List<(Location2D location, Cell cell)> CarveBorderSpace()
        {
            return CarveSpace(Inner: false);
        }

        public void GenerateBorder()
        {
            if (CraterBorderRegion.IsNullOrEmpty())
            {
                CraterBorderRegion = CarveBorderSpace();
            }
            foreach ((Location2D location, Cell cell) in CraterBorderRegion)
            {
                cell.Clear();
                cell.AddObject(CraterWallMaterial);
            }

            if (CraterSpaceRegion.IsNullOrEmpty())
            {
                CraterSpaceRegion = CarveInnerSpace();
            }
            foreach ((Location2D location, Cell cell) in CraterSpaceRegion)
            {
                if (CraterBorderRegion.Contains((location, cell)))
                {
                    CraterBorderRegion.Remove((location, cell));
                }
            }

            foreach ((Location2D location, Cell cell) in CraterSpaceRegion)
            {
                // cell.Clear();
                // cell.AddObject("Shale");
            }
        } //!-- public void GenerateBorder()
    }
}
