using System;
using System.Collections.Generic;
using System.Linq;

using Genkit;
using Qud.API;

using XRL.Rules;
using XRL.World.AI.Pathfinding;
using XRL.World.ObjectBuilders;
using XRL.World.WorldBuilders;
using XRL.World.Parts;

using static XRL.Core.XRLCore;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using XRL.Language;

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
        public string TinkerID;
        public string ApothecaryID;
        public string DromadID;
        public string GutsmongerID;
        public Dictionary<string, string> MerchantIDs;
        public string PetID;
        public List<string> ResidentIDs;

        public GiantAbodePopulator()
        {
            Regions = new();
            GiantID = null;
            TinkerID = null;
            ApothecaryID = null;
            DromadID = null;
            GutsmongerID = null;
            MerchantIDs = new()
            {
                { nameof(TinkerID), null },
                { nameof(ApothecaryID), null },
                { nameof(DromadID), null },
                { nameof(GutsmongerID), null },
            };
            PetID = null;
            ResidentIDs = new();
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
            Cell giantMulticabinetCell = null;
            Cell giantAlchemistTableCell = null;

            int abodeNumber = 0;
            int abodeNumberTotal = 0;

            List<GameObject> abodeSpawners = Event.NewGameObjectList(zone.GetObjectsThatInheritFrom("GiantAbodeSpawner"));

            Dictionary<string, int> abodeAllocations = new()
            {
                { "Tinker", 1 },
                { "Apothecary", 1 },
                { "Dromad", 1 },
                { "Gutsmonger", 1 },
            };

            if (abodeSpawners.Count - 5 > 0)
            {
                abodeAllocations.Add("Base", abodeSpawners.Count - 5);
            }

            if (!abodeSpawners.IsNullOrEmpty())
            {
                foreach (GameObject abodeSpawner in abodeSpawners)
                {
                    abodeNumberTotal++;
                    bool isUnique = abodeSpawner.Blueprint == "GiantAbodeSpawner Cook";
                    string abodeType = isUnique ? "Cook" : abodeAllocations.Draw();
                    bool isBasic = abodeType == "Base";
                    bool isTinker = abodeType == "Tinker";
                    bool isApothecary = abodeType == "Apothecary";
                    bool isDromad = abodeType == "Dromad";
                    bool isGutsmonger = abodeType == "Gutsmonger";
                    if (!isBasic && !isUnique)
                    {
                        abodeSpawner.SetStringProperty("ContentsTable", $"Giant Abode {abodeType}");
                    }
                    string abodeDesignation = $"Abode:{(!isBasic ? abodeType : ++abodeNumber)}";
                    string abodeLabel = $"HNPS_GigantismPlus::{abodeDesignation}::";

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
                    Regions.Add(abodeDesignation, Region);

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
                    foreach (Point2D point in P.ReduceBy(1, 1).getPoints())
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
                                if ((gameObject.GetBlueprint().HasTag("Furniture") || gameObject.GetBlueprint().HasTag("Vessel"))
                                    && !gameObject.InheritsFrom("Wire Extruder")
                                    && gameObject.Physics != null)
                                {
                                    string owningFaction = "WrassleGiants";
                                    gameObject.Physics.Owner = owningFaction;
                                    Debug.CheckYeh(4, $"{item.Blueprint}: owner set to {owningFaction}", Indent: 4, Toggle: getDoDebug());
                                }
                                if (isUnique && item.Blueprint == "Gigantic Oven")
                                {
                                    giantOvenCell = gameObject?.CurrentCell;
                                    Debug.CheckYeh(4, $"Giant Oven location stored", Indent: 4, Toggle: getDoDebug());
                                }
                                if (isTinker && item.Blueprint == "Gigantic Multicabinet")
                                {
                                    giantMulticabinetCell = gameObject?.CurrentCell;
                                    Debug.CheckYeh(4, $"Giant Multicabinet location stored", Indent: 4, Toggle: getDoDebug());
                                }
                                if (isApothecary && item.Blueprint == "Gigantic Alchemist Table")
                                {
                                    giantAlchemistTableCell = gameObject?.CurrentCell;
                                    Debug.CheckYeh(4, $"Giant Alchemist Table location stored", Indent: 4, Toggle: getDoDebug());
                                }
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: getDoDebug());
                        Debug.Entry(4,
                            $"x for (int num = 0; num < item.Number; num++) >//",
                            Indent: 2, Toggle: getDoDebug());
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());
                    Debug.Entry(4,
                        $"x foreach ({nameof(PopulationResult)} item in {nameof(ContentsTable)}: {ContentsTable.Quote()}) >//",
                        Indent: 1, Toggle: getDoDebug());

                    string abodeRegionString = string.Empty;
                    foreach ((string regionLabel, List<Cell> cells) in Region)
                    {
                        zone.SetZoneProperty(abodeLabel + regionLabel, cells.ToStringList().Join(";"));
                    }
                }
                zone.SetZoneProperty($"HNPS_GigantismPlus::Abodes", $"{abodeNumber}");
                zone.SetZoneProperty($"HNPS_GigantismPlus::AbodesTotal", $"{abodeNumberTotal}");
                foreach (GameObject trash in trashCan)
                {
                    trash.Obliterate(null, true);
                }

                List<Cell> nonRegionEmptyCells = new();
                foreach (Cell emptyCell in Z.GetEmptyCells())
                {
                    if (!regionCells.Contains(emptyCell)) nonRegionEmptyCells.Add(emptyCell);
                }

                foreach ((_, Dictionary<string, List<Cell>> region) in Regions)
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
                    foreach (Cell avoidCell in regionCells)
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

                GameObject UniqueGiant = The.ZoneManager.PullCachedObject(GiantID, false);
                if (UniqueGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive Unique {nameof(WrassleGiantHero)} from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    UniqueGiant = GetTheGiant();
                }
                bool failedToGetTinker = false;
                bool failedToGetApothecary = false;
                bool failedToGetDromad = false;
                bool failedToGetGutsmonger = false;
                bool failedToGetPet = false;
                bool failedToGetResidents = false;

                GameObject tinkerGiant = null;
                GameObject apothecaryGiant = null;
                GameObject dromadGiant = null;
                GameObject gutsmongerGiant = null;
                GameObject petGiant = null;
                if (!MerchantIDs.IsNullOrEmpty())
                {
                    foreach ((string LabelID, string ID) in MerchantIDs)
                    {
                        GameObject merchant = The.ZoneManager.PullCachedObject(ID, false);
                        switch (LabelID)
                        {
                            case nameof(TinkerID):
                                tinkerGiant = merchant;
                                break;
                            case nameof(ApothecaryID):
                                apothecaryGiant = merchant;
                                break;
                            case nameof(DromadID):
                                dromadGiant = merchant;
                                break;
                            case nameof(GutsmongerID):
                                gutsmongerGiant = merchant;
                                break;
                        }
                    }
                }

                tinkerGiant ??= The.ZoneManager.PullCachedObject(TinkerID, false);
                apothecaryGiant ??= The.ZoneManager.PullCachedObject(ApothecaryID, false);
                dromadGiant ??= The.ZoneManager.PullCachedObject(DromadID, false);
                gutsmongerGiant ??= The.ZoneManager.PullCachedObject(GutsmongerID, false);
                petGiant ??= The.ZoneManager.PullCachedObject(PetID, false);

                List<GameObject> residentGiants = Event.NewGameObjectList();
                if (!ResidentIDs.IsNullOrEmpty())
                {
                    foreach (string ID in ResidentIDs)
                    {
                        if (The.ZoneManager.PullCachedObject(ID, false) is GameObject residentGiant && residentGiant != null)
                        {
                            residentGiants.Add(residentGiant);
                        }
                    }
                }

                if (tinkerGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(tinkerGiant)} Villager from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetTinker = true;
                }
                if (apothecaryGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(apothecaryGiant)} Villager from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetApothecary = true;
                }
                if (dromadGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(dromadGiant)} Villager from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetDromad = true;
                }
                if (gutsmongerGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(gutsmongerGiant)} Villager from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetGutsmonger = true;
                }
                if (petGiant == null)
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(petGiant)} Villager from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetPet = true;
                }
                if (residentGiants.IsNullOrEmpty())
                {
                    Debug.Warn(2,
                        $"{nameof(GiantAbodePopulator)}",
                        $"{nameof(BuildZone)}",
                        $"Failed to retreive {nameof(residentGiants)} Villagers from cache " +
                        $"in zone {zone?.ZoneID}",
                        Indent: 1);

                    failedToGetResidents = true;
                }
                if (HNPS_SecretGiantWhoCooksBuilderExtension.TryGenerateGiantVillagers(zone.wX / 10,
                    HeroDetailColor: UniqueGiant.WrassleID()?.SecondaryColor,
                    out GameObject altTinkerGiant,
                    out GameObject altApothecaryGiant,
                    out GameObject altDromadGiant,
                    out GameObject altGutsmongerGiant,
                    out GameObject altPetGiant,
                    out List<GameObject> altResidentGiants))
                {
                    if (failedToGetTinker)
                    {
                        tinkerGiant = altTinkerGiant;
                    }
                    if (failedToGetApothecary)
                    {
                        apothecaryGiant = altApothecaryGiant;
                    }
                    if (failedToGetDromad)
                    {
                        dromadGiant = altDromadGiant;
                    }
                    if (failedToGetGutsmonger)
                    {
                        gutsmongerGiant = altGutsmongerGiant;
                    }
                    if (failedToGetPet)
                    {
                        petGiant = altPetGiant;
                    }
                    if (failedToGetResidents)
                    {
                        residentGiants = altResidentGiants;
                    }
                }

                List<Cell> uniqueAbodeEmptyInnerCells = Event.NewCellList(
                    from c in Regions["Abode:Cook"][INNER]
                    where c.IsEmptyFor(UniqueGiant)
                    select c);
                List<Cell> tinkerAbodeEmptyInnerCells = Event.NewCellList(
                    from c in Regions["Abode:Tinker"][INNER]
                    where c.IsEmptyFor(tinkerGiant)
                    select c);
                List<Cell> apothecaryAbodeEmptyInnerCells = Event.NewCellList(
                    from c in Regions["Abode:Apothecary"][INNER]
                    where c.IsEmptyFor(apothecaryGiant)
                    select c);
                List<Cell> dromadAbodeEmptyInnerCells = Event.NewCellList(
                    from c in Regions["Abode:Dromad"][INNER]
                    where c.IsEmptyFor(dromadGiant)
                    select c);
                List<Cell> gutsmongerAbodeEmptyInnerCells = Event.NewCellList(
                    from c in Regions["Abode:Gutsmonger"][INNER]
                    where c.IsEmptyFor(dromadGiant)
                    select c);

                Cell uniqueGiantLocation =
                    giantOvenCell?.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? uniqueAbodeEmptyInnerCells?.GetRandomElement()
                 ?? zone?.FindFirstObject("Gigantic Oven")?.CurrentCell.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                Cell tinkerGiantLocation =
                    giantMulticabinetCell?.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? tinkerAbodeEmptyInnerCells?.GetRandomElement()
                 ?? zone?.FindFirstObject("Gigantic Multicabinet")?.CurrentCell.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                Cell apothecaryGiantLocation =
                    giantAlchemistTableCell?.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? apothecaryAbodeEmptyInnerCells?.GetRandomElement()
                 ?? zone?.FindFirstObject("Gigantic Alchemist Table")?.CurrentCell.GetEmptyAdjacentCells()?.GetRandomElement()
                 ?? nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                Cell dromadGiantLocation =
                    dromadAbodeEmptyInnerCells?.GetRandomElement()
                 ?? nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                Cell gutsmongerGiantLocation =
                    gutsmongerAbodeEmptyInnerCells?.GetRandomElement()
                 ?? nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                Cell petGiantLocation =
                    nonRegionEmptyCells?.GetRandomElement()
                 ?? zone?.GetEmptyCells()?.GetRandomElement();

                if (UniqueGiant != null)
                {
                    if (uniqueGiantLocation != null)
                    {
                        uniqueGiantLocation.AddObject(UniqueGiant);
                        if (UniqueGiant.Brain != null)
                        {
                            UniqueGiant.Brain.StartingCell = new();
                            UniqueGiant.Brain.StartingCell.SetCell(uniqueGiantLocation);
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
                        $"for cell [{uniqueGiantLocation?.Location}]",
                        Indent: 1);
                }

                if (PlaceGiantVillagerInCell(tinkerGiant, tinkerGiantLocation, zone, "Tinker")
                    && PlaceGiantVillagerInCell(apothecaryGiant, apothecaryGiantLocation, zone, "Apothecary")
                    && PlaceGiantVillagerInCell(dromadGiant, dromadGiantLocation, zone, "Dromad")
                    && PlaceGiantVillagerInCell(gutsmongerGiant, gutsmongerGiantLocation, zone, "Gutsmonger")
                    && PlaceGiantVillagerInCell(petGiant, petGiantLocation, zone, "Pet"))
                {
                    Debug.CheckYeh(4, $"all five Giant Villager Merchants (and Pet) successfully placed",
                        Indent: 1, Toggle: getDoDebug());
                }
                else
                {
                    Debug.CheckNah(4, $"at least one Giant Villager Merchants (and Pet) failed to be placed",
                        Indent: 1, Toggle: getDoDebug());
                }

                if (!residentGiants.IsNullOrEmpty())
                {
                    bool allPlaced = true;
                    foreach (GameObject residentGiant in residentGiants)
                    {
                        Cell residentGiantLocation = nonRegionEmptyCells?.GetRandomElement()
                            ?? zone?.GetEmptyCells()?.GetRandomElement();
                        allPlaced = PlaceGiantVillagerInCell(residentGiant, residentGiantLocation, zone, "Resident") && allPlaced;
                    }
                    if (allPlaced)
                    {
                        Debug.CheckYeh(4, $"all {residentGiants.Count.AsCardinal()} Giant Villager Residents successfully placed",
                            Indent: 1, Toggle: getDoDebug());
                    }
                    else
                    {
                        Debug.CheckNah(4, $"at least one Giant Villager Residents failed to be placed",
                            Indent: 1, Toggle: getDoDebug());
                    }
                }
            }
            else
            {
                Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to find and {nameof(abodeSpawners)} " +
                    $"in zone {zone?.ZoneID}",
                    Indent: 1);
            }

            return true;
        } //!-- public bool BuildZone(Zone Z)

        public static bool PlaceGiantVillagerInCell(GameObject Villager, Cell HomeCell, Zone Zone, string Context = null)
        {
            if (Villager != null)
            {
                if (HomeCell != null)
                {
                    HomeCell.AddObject(Villager);
                    if (Villager.Brain != null && Context != "Resident" && Context != "Pet")
                    {
                        Villager.Brain.StartingCell = new();
                        Villager.Brain.StartingCell.SetCell(HomeCell);
                    }
                    if (Villager.TryGetPart(out StewBelly stewBelly))
                    {
                        stewBelly.ProcessStartingStews();
                    }
                    return true;
                }
                else
                {
                    Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to find suitable cell" +
                    $"in {nameof(Zone)} {Zone?.ZoneID} " +
                    $"for Giant Villager {Context} {Villager?.DebugName ?? NULL}",
                    Indent: 1);
                    return false;
                }
            }
            else
            {
                Debug.Warn(2,
                    $"{nameof(GiantAbodePopulator)}",
                    $"{nameof(BuildZone)}",
                    $"Failed to retreive Unique {nameof(WrassleGiantHero)} from cache " +
                    $"in zone {Zone?.ZoneID} " +
                    $"for cell [{HomeCell?.Location}]",
                    Indent: 1);
                return false;
            }
        }

        public static GameObject GetTheGiant() => HNPS_SecretGiantWhoCooksBuilderExtension.GetTheGiant();

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
