using Genkit;
using HistoryKit;

using System.Collections.Generic;

using XRL.Rules;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using System.Linq;

namespace XRL.World.ZoneBuilders
{
	public class AbandonedSettlementUnder
        : ZoneBuilderSandbox
    {
        public bool BuildZone(Zone Z)
        {
            zone = Z;
            Z.SetZoneProperty("DisableForcedConnections", "Yes");
            Z.SetZoneProperty("relaxedbiomes", "true");
            List<Location2D> locations = new();
            foreach (Cell cell in Z.GetCells())
            {
                cell.ClearObjectsWithPart("Combat");
            }
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
            return true;
        }
    }
}
