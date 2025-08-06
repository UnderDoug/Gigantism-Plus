using Genkit;
using HistoryKit;
using HNPS_GigantismPlus;
using Qud.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRL;
using XRL.Language;
using XRL.Names;
using XRL.Rules;
using XRL.UI;
using XRL.Wish;
using XRL.World.AI.GoalHandlers;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.ZoneBuilders;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.ObjectBuilders.WrassleGiantHero;

namespace XRL.World.WorldBuilders
{
    [HasWishCommand]
    [JoppaWorldBuilderExtension]
    public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
    {
        private static bool doDebug => getClassDoDebug(nameof(SecretGiantWhoCooksBuilderExtension));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                'X',    // Trace
                '!',    // Alert
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public JoppaWorldBuilder Builder = null;

        public static string SecretZoneID = string.Empty;
        public static JournalMapNote SecretMapNote = null;

        public Zone SecretZone => The.ZoneManager.GetZone(SecretZoneID);

        public override void OnAfterBuild(JoppaWorldBuilder Builder)
        {
            this.Builder = Builder;
            MetricsManager.rngCheckpoint("gigantify");
            Builder.BuildStep("Gigantifying wrasslers", GigantifyWrasslers);
        }
        public void GigantifyWrasslers(string WorldID)
        {
            if (!(WorldID == "JoppaWorld"))
            {
                return;
            }
            WorldCreationProgress.StepProgress("Gigantifying wrasslers...");

            Debug.Entry(4,
                $"\u2666 {nameof(SecretGiantWhoCooksBuilderExtension)}." +
                $"{nameof(GigantifyWrasslers)}(WorldID: {WorldID})",
                Indent: 0, Toggle: getDoDebug());

            Debug.Entry(4, $"Waking up Unique Giant...", Indent: 1, Toggle: getDoDebug());
            GameObject UniqueGiant = GetTheGiant();

            if (UniqueGiant == null)
            {
                Debug.Warn(2,
                    $"{nameof(SecretGiantWhoCooksBuilderExtension)}",
                    $"{nameof(GigantifyWrasslers)}(JoppaWorldBuilder builder) ",
                    $"failed to instantiate {nameof(UniqueGiant)}. Placement aborted.",
                    Indent: 0);
                return;
            }

            Debug.Entry(4, $"Getting coordinates...", Indent: 1, Toggle: getDoDebug());
            Location2D location = Builder.popMutableLocationOfTerrain("Mountains", l => l.X > 60, centerOnly: true);
            Debug.LoopItem(4, $"{nameof(location)}: [{location}]", Indent: 2, Toggle: getDoDebug());

            Debug.Entry(4, $"Getting ZoneID from {nameof(location)}...", Indent: 1, Toggle: getDoDebug());
            SecretZoneID = Builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);
            Debug.Entry(4, $"{nameof(SecretZoneID)} Set", $"{SecretZoneID}", Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"Checking MapNote isn't already set...", Indent: 1, Toggle: getDoDebug());
            if (JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID) == null)
            {
                Debug.CheckYeh(4, $"MapNote not set", Indent: 2, Toggle: getDoDebug());
                Debug.Entry(4, $"Setting MapNote...", Indent: 1, Toggle: getDoDebug());
                JournalAPI.AddMapNote(
                    ZoneID: SecretZoneID,
                    text: SCRT_GNT_LCTN_TEXT,
                    category: SCRT_GNT_LCTN_CATEGORY,
                    attributes: SecretAttributes,
                    secretId: SCRT_GNT_SCRT_ID
                );
            }
            Debug.Entry(4, $"Setting MapNote Weight...", Indent: 1, Toggle: getDoDebug());
            SecretMapNote = JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID);
            SecretMapNote.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            Debug.Entry(4, $"Removing undesired ZoneBuilders...", Indent: 1, Toggle: getDoDebug());
            zoneManager.RemoveZoneBuilders(SecretZoneID, nameof(Hills));
            zoneManager.RemoveZoneBuilders(SecretZoneID, nameof(FactionEncounters));
            zoneManager.ClearZoneBuilders(SecretZoneID);

            string MapFileName = SCRT_GNT_ZONE_MAP2_CENTRE;
            Debug.Entry(4, $"Setting {nameof(MapFileName)}...", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"{nameof(MapFileName)} Set", $"{MapFileName}", Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"Assigning MapFile to MapBuilder...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneID, nameof(MapBuilder), "FileName", $"{MapFileName}");

            Debug.Entry(4, $"Setting Music...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneID, "Music", "Track", "Music/Barathrums Study");

            Debug.Entry(4, $"Setting Zone to Checkpoint...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneID, nameof(IsCheckpoint), "Key", SecretZoneID);

            Debug.Entry(4, $"Skipping TerainBuilders and flaggign NoBiomes...", Indent: 1, Toggle: getDoDebug());
            zoneManager.SetZoneProperty(SecretZoneID, "SkipTerrainBuilders", true);
            zoneManager.SetZoneProperty(SecretZoneID, "NoBiomes", "Yes");
            zoneManager.SetZoneProperty(SecretZoneID, "faction", SCRT_GNT_VLG_FCT);

            Debug.Entry(4, $"Setting ZoneName...", Indent: 1, Toggle: getDoDebug());
            zoneManager.SetZoneName(SecretZoneID, SCRT_GNT_LCTN_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneID, false);

            int approxZoneTier = (int)((location.X + 3) / 3.0) / 10; // 80 parasangs wide, 8 Tiers, divide parasangs by 10
            Tier.Constrain(ref approxZoneTier);
            Debug.Entry(4, $"Setting ZoneTierOverride to {nameof(approxZoneTier)} ({approxZoneTier})...", Indent: 1, Toggle: getDoDebug());
            zoneManager.SetZoneProperty(SecretZoneID, "ZoneTierOverride", approxZoneTier.ToString());

            TerrainTravel pTravel = Builder.terrainComponents[Location2D.Get(location.X/3, location.Y/3)];
            if (UI.Options.ShowOverlandEncounters && pTravel != null)
            {
                Debug.Entry(4, $"Setting up OverLandEncounters option...", Indent: 1, Toggle: getDoDebug());
                pTravel.ParentObject.Render.RenderString = "G";
                pTravel.ParentObject.Render.SetForegroundColor('Z');
            }

            string wrasslerColor = null;
            WrassleID wrassleID = null;
            string wrassleRingColor = null;

            Debug.Entry(4, $"Storing {nameof(wrasslerColor)}...", Indent: 1, Toggle: getDoDebug());
            if (!UniqueGiant.TryGetPart(out Wrassler wrassler))
            {
                Debug.CheckYeh(4, $"{nameof(UniqueGiant)} lacks {nameof(Wrassler)} part, registering with {nameof(UD_QWE)}",
                    Indent: 1, Toggle: getDoDebug());
                wrassler = UD_QWE.MakeWrassler(UniqueGiant, "UniqueGiant");
            }
            else
            {
                Debug.CheckYeh(4, $"{nameof(UniqueGiant)} has {nameof(Wrassler)} part",
                    Indent: 1, Toggle: getDoDebug());
            }
            wrassleID = wrassler?.GetWrassleID();

            if (wrassleID == null)
            {
                Debug.Warn(2,
                    $"{nameof(SecretGiantWhoCooksBuilderExtension)}",
                    $"{nameof(GigantifyWrasslers)}(JoppaWorldBuilder builder) ",
                    $"failed to instantiate {nameof(wrassleID)}. Ring sync aborted.",
                    Indent: 0);
                return;
            }

            Debug.Entry(4, $"{nameof(wrassleID)}.{nameof(wrassleID.ID)}", $"{wrassleID.GetID(Silent: true)}", Indent: 1, Toggle: getDoDebug());

            wrasslerColor = wrassleID?.SecondaryColor;
            wrassleRingColor = wrasslerColor 
                ?? UniqueGiant.GetStringProperty("WrassleColor", null) 
                ?? UD_QWE.WrassleRingColors.GetRandomElement();
            Debug.Entry(4, $"{nameof(wrassleRingColor)} is {wrassleRingColor}", Indent: 1, Toggle: getDoDebug());

            /*
            wrassleRingColor ??= UD_QWE.WrassleRingColors.GetRandomElement();
            Debug.Entry(4, $"{nameof(EnablePrereleaseContent)} is {EnablePrereleaseContent}...", Indent: 1, Toggle: getDoDebug());
            Debug.CheckNah(4, $"Skipping Wrassler Content", Indent: 2, Toggle: getDoDebug());
            */

            Debug.Entry(4, $"Assigning GiantAbodePopulator if it's necessary...", Indent: 1, Toggle: getDoDebug());
            if (MapFileName == SCRT_GNT_ZONE_MAP2_CENTRE) // This specific map has the widgets necessary for the specified builder to work
            {
                Debug.CheckYeh(4, $"Map is correct, adding {nameof(GiantAbodePopulator)}...", Indent: 2, Toggle: getDoDebug());
                if (TryGenerateGiantVillagers(approxZoneTier, wrassleRingColor,
                    out GameObject tinkerGiant,
                    out GameObject apothecaryGiant,
                    out GameObject dromadGiant,
                    out GameObject gutsmongerGiant,
                    out GameObject petGiant,
                    out List<GameObject> residentGiants))
                {
                    string uniqueGiantID = zoneManager.CacheObject(UniqueGiant);
                    Dictionary<string, string> merchantGiantsIDs = new()
                    {
                        { "TinkerID", zoneManager.CacheObject(tinkerGiant) },
                        { "ApothecaryID", zoneManager.CacheObject(apothecaryGiant) },
                        { "DromadID", zoneManager.CacheObject(dromadGiant) },
                        { "GutsmongerID", zoneManager.CacheObject(gutsmongerGiant) },
                    };
                    string petID = zoneManager.CacheObject(petGiant);
                    List<string> residentGiantsIDs = new();
                    if (!residentGiants.IsNullOrEmpty())
                    {
                        foreach (GameObject residentGiant in residentGiants)
                        {
                            residentGiantsIDs.Add(zoneManager.CacheObject(residentGiant));
                        }
                    }
                    zoneManager.AddZonePostBuilder(
                        ZoneID: SecretZoneID,
                        Class: nameof(GiantAbodePopulator),
                        Key1: nameof(GiantAbodePopulator.GiantID), Value1: uniqueGiantID,
                        Key2: nameof(GiantAbodePopulator.MerchantIDs), Value2: merchantGiantsIDs,
                        Key3: nameof(GiantAbodePopulator.PetID), Value3: petID,
                        Key4: nameof(GiantAbodePopulator.ResidentIDs), Value4: residentGiantsIDs);
                }
                else
                {
                    Debug.Warn(2,
                        $"{nameof(SecretGiantWhoCooksBuilderExtension)}",
                        $"{nameof(GigantifyWrasslers)}",
                        $"Failed to instantiate one or more of the Giant Villagers, " +
                        $"allowing {nameof(GiantAbodePopulator)} to attempt Villager generation",
                        Indent: 1);

                    zoneManager.AddZonePostBuilder(
                        ZoneID: SecretZoneID,
                        Class: nameof(GiantAbodePopulator),
                        Key1: "GiantID",
                        Value1: zoneManager.CacheObject(UniqueGiant));
                }
            }
            else
            {
                Debug.CheckNah(4, $"Map is incorrect, adding {nameof(AddObjectBuilder)}...", Indent: 2, Toggle: getDoDebug());
                zoneManager.AddZonePostBuilder(
                    ZoneID: SecretZoneID,
                    Class: nameof(AddObjectBuilder),
                    Key1: "Object",
                    Value1: zoneManager.CacheObject(UniqueGiant));
            }

            Debug.Entry(4, $"Getting Ropes and Attempting to assign Color...", Indent: 1, Toggle: getDoDebug());
            List<GameObject> ropesList = zoneManager.GetZone(SecretZoneID).GetObjectsThatInheritFrom("WrassleRingRopes");
            if (!ropesList.IsNullOrEmpty())
            {
                Debug.CheckYeh(4, $"Got Ropes", Indent: 2, Toggle: getDoDebug());
                foreach (GameObject rope in ropesList)
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(rope)}: {rope?.DebugName}", Indent: 2, Toggle: getDoDebug());
                    if (rope.TryGetPart(out WrassleGear wrassleGear))
                    {
                        Debug.CheckYeh(4, $"{nameof(rope)} has {nameof(WrassleGear)}", Indent: 3, Toggle: getDoDebug());
                        Debug.Entry(4, $"Attempting to sync WrassleIDs...", Indent: 3, Toggle: getDoDebug());
                        if (UD_QWE.TrySyncWrassleID(UniqueGiant, rope))
                        {
                            Debug.CheckYeh(4, $"Wrassle ID's synched", $"{UniqueGiant.WrassleIDString()}", 
                                Indent: 3, Toggle: getDoDebug());
                            // wrassleGear.SetDetailColor(Force: true); // Might not need this now.
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Wrassle ID's failed to sync",
                                Indent: 3, Toggle: getDoDebug());
                        }
                    }
                    else
                    {
                        Debug.CheckNah(4, $"{nameof(rope)} lacks {nameof(WrassleGear)}", Indent: 3, Toggle: getDoDebug());
                        Debug.Entry(4, $"Setting color to preselected {wrassleRingColor.Quote()} via {nameof(Render)}...", 
                            Indent: 3, Toggle: getDoDebug());

                        rope.Render.DetailColor = wrassleRingColor;
                    }
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"No Ropes", Indent: 2, Toggle: getDoDebug());
            }

            Debug.Entry(4, $"Getting Folding Chairs and Attempting to assign Color...", Indent: 1, Toggle: getDoDebug());
            List<GameObject> chairsList = zoneManager.GetZone(SecretZoneID).GetObjectsThatInheritFrom("FoldingChair");
            if (!chairsList.IsNullOrEmpty())
            {
                Debug.CheckYeh(4, $"Got Chairs", Indent: 2, Toggle: getDoDebug());
                foreach (GameObject chair in chairsList)
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(chair)}: {chair?.DebugName}", Indent: 2, Toggle: getDoDebug());
                    if (!chair.IsGiganticEquipment)
                    {
                        Debug.CheckNah(4, $"{nameof(chair)} not {nameof(chair.IsGiganticEquipment)}, skipping", Indent: 3, Toggle: getDoDebug());
                        continue;
                    }
                    if (chair.TryGetPart(out WrassleGear wrassleGear))
                    {
                        Debug.CheckYeh(4, $"{nameof(chair)} has {nameof(WrassleGear)}", Indent: 3, Toggle: getDoDebug());
                        Debug.Entry(4, $"Attempting to sync WrassleIDs...", Indent: 3, Toggle: getDoDebug());
                        if (UD_QWE.TrySyncWrassleID(UniqueGiant, chair))
                        {
                            Debug.CheckYeh(4, $"Wrassle ID's synched", $"{UniqueGiant.WrassleIDString()}", 
                                Indent: 3, Toggle: getDoDebug());
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Wrassle ID's failed to sync", Indent: 3, Toggle: getDoDebug());
                        }
                    }
                    else
                    {
                        Debug.CheckNah(4, $"{nameof(chair)} lacks {nameof(WrassleGear)}", Indent: 3, Toggle: getDoDebug());
                    }
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"No Chairs", Indent: 2, Toggle: getDoDebug());
            }
        } //!-- public override void OnAfterBuild(JoppaWorldBuilder builder)

        public static GameObject GetTheGiant()
        {
            return GetAGiant(Unique: true);
        }
        public static GameObject GetAGiant(bool Unique = false)
        {
            GameObject creature;
            GameObjectBlueprint creatureBlueprint = Unique 
                ? GetAUniqueGiantHeroBlueprintModel()
                : GetAGiantHeroBlueprintModel()
                ;

            void ApplyBuilder(GameObject Creature) 
            {
                UD_QWE.WrassleGiantHeroBuilder.Apply(Creature, Context: Unique ? "Unique" : "Hero"); 
            }
            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: creatureBlueprint,
                    BeforeObjectCreated: ApplyBuilder,
                    Context: Unique ? "Unique" : "Hero",
                    ProvideInventory: null);

            return creature;
        }
        public static void SetGiantVillagerStews(GameObject Villager)
        {
            DieRoll dieRoll = new(Villager.GetStringProperty(GNT_START_STEWS_PROPLABEL, "1d1"));
            dieRoll.AdjustDieCount(Stat.RandomCosmetic(0, 1));
            dieRoll.AdjustDieSize(Stat.RandomCosmetic(1, 3));
            Villager.SetStringProperty(GNT_START_STEWS_PROPLABEL, dieRoll.ToString());
        }
        public static void GigantifyMerchant(GameObject Merchant)
        {
            SetGiantVillagerStews(Merchant);
            Gigantifier.Apply(Merchant, "GiantVillageMerchant");
        }
        public static void GigantifyResident(GameObject Resident)
        {
            SetGiantVillagerStews(Resident);
            Gigantifier.Apply(Resident, "GiantVillageResident");
        }
        public static bool TryGenerateGiantVillagers(int ApproxZoneTier, string HeroDetailColor, out GameObject TinkerGiant, out  GameObject ApothecaryGiant, out GameObject DromadGiant, out GameObject GutsmongerGiant, out GameObject PetGiant, out List<GameObject> Residents)
        {
            string tinkerBlueprint = $"HumanTinker{ApproxZoneTier}";
            string apothecaryBlueprint = $"HumanApothecary{ApproxZoneTier}";
            string dromadBlueprint = $"DromadTrader{ApproxZoneTier}";
            string gutsmongerBlueprint = $"Bophades";
            string petBlueprint = PopulationManager.RollOneFrom($"DynamicInheritsTable:BaseAnimal:Tier{ApproxZoneTier}").Blueprint;

            If.d100(10, () => tinkerBlueprint = GetOldGiantEligibleBlueprint());
            If.d100(10, () => apothecaryBlueprint = GetOldGiantEligibleBlueprint());

            TinkerGiant = null;
            ApothecaryGiant = null;
            DromadGiant = null;
            PetGiant = null;

            string xContext = $"{nameof(SecretGiantWhoCooksBuilderExtension)}.{nameof(TryGenerateGiantVillagers)}() ";

            Debug.Entry(4, $"Crafting up {nameof(TinkerGiant)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                TinkerGiant = GameObjectFactory.Factory.CreateObject(tinkerBlueprint, AfterObjectCreated: GigantifyMerchant);
                PrepareGiantVillager(TinkerGiant, "Tinker", ApproxZoneTier, HeroDetailColor);
            }
            catch (Exception x)
            {
                TinkerGiant = null;
                MetricsManager.LogException(xContext + "Tinker", x);
            }

            Debug.Entry(4, $"Teaching {nameof(ApothecaryGiant)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                ApothecaryGiant = GameObjectFactory.Factory.CreateObject(apothecaryBlueprint, AfterObjectCreated: GigantifyMerchant);
                PrepareGiantVillager(ApothecaryGiant, "Apothecary", ApproxZoneTier, HeroDetailColor);
            }
            catch (Exception x)
            {
                ApothecaryGiant = null;
                MetricsManager.LogException(xContext + "Apothecary", x);
            }

            Debug.Entry(4, $"Finding {nameof(DromadGiant)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                DromadGiant = GameObjectFactory.Factory.CreateObject(dromadBlueprint, AfterObjectCreated: GigantifyMerchant);
                PrepareGiantVillager(DromadGiant, "Merchant", ApproxZoneTier, HeroDetailColor);
            }
            catch (Exception x)
            {
                DromadGiant = null;
                MetricsManager.LogException(xContext + "Merchant", x);
            }

            Debug.Entry(4, $"Smuggling {nameof(GutsmongerGiant)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                void gigantifyGutsmonger(GameObject Gutsmonger)
                {
                    Gutsmonger.GetStat("Level").BaseValue = 5 * ApproxZoneTier - 1;
                    Gutsmonger.Body.GetBody().Implant(GameObjectFactory.Factory.CreateObject("GiganticExoframeSigma"));
                    GigantifyMerchant(Gutsmonger);
                }
                GutsmongerGiant = GameObjectFactory.Factory.CreateObject(gutsmongerBlueprint, AfterObjectCreated: gigantifyGutsmonger);
                PrepareGiantVillager(GutsmongerGiant, "Gutsmonger", ApproxZoneTier, HeroDetailColor);
            }
            catch (Exception x)
            {
                GutsmongerGiant = null;
                MetricsManager.LogException(xContext + "Gutsmonger", x);
            }

            Debug.Entry(4, $"Adopting {nameof(PetGiant)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                PetGiant = GameObjectFactory.Factory.CreateObject(petBlueprint, AfterObjectCreated: GigantifyMerchant);
                PrepareGiantVillager(PetGiant, "Pet", ApproxZoneTier, HeroDetailColor);
            }
            catch (Exception x)
            {
                PetGiant = null;
                MetricsManager.LogException(xContext + "Pet", x);
            }

            int residentsRoll = Stat.Random(3, 6);
            Debug.Entry(4, $"Immigrating {residentsRoll} {nameof(Residents)}...", Indent: 1, Toggle: getDoDebug());
            try
            {
                Residents = Event.NewGameObjectList();
                GameObject resident = null;
                for (int i = 0; i < residentsRoll; i++)
                {
                    resident = GameObjectFactory.Factory.CreateObject(GetGiantHeroEligibleBlueprint(), AfterObjectCreated: GigantifyResident);
                    if (resident != null && Residents.TryAdd(PrepareGiantVillager(resident, "Resident", ApproxZoneTier, HeroDetailColor)))
                    {
                        Debug.CheckYeh(4, $"{nameof(resident)} {resident} Added", Indent: 2, Toggle: getDoDebug());
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Failed to instantiate {nameof(resident)}", Indent: 2, Toggle: getDoDebug());
                    }
                }
            }
            catch (Exception x)
            {
                Residents = null;
                MetricsManager.LogException(xContext + "Resident", x);
            }

            return TinkerGiant != null && ApothecaryGiant != null && DromadGiant != null && PetGiant != null && Residents != null;
        }
        public static GameObject PrepareGiantVillager(GameObject Villager, string Context = null, int Tier = 1, string HeroDetailColor = null)
        {
            int indent = Debug.LastIndent;

            bool isTinker = Context == "Tinker";
            bool isApothecary = Context == "Apothecary";
            bool isDromad = Context == "Merchant";
            bool isGutsmonger = Context == "Gutsmonger";
            bool isPet = Context == "Pet";
            bool isResident = Context == "Resident";

            Villager.Brain = Villager.RequirePart<Brain>();

            Villager.RemovePart<Lovely>();
            Villager.RemovePart<Breeder>();
            Villager.RemovePart<SecretObject>();
            Villager.RemovePart<ConvertSpawner>();
            Villager.RemovePart<Rummager>();
            Villager.RemovePart<AIShopper>();
            Villager.RemovePart<AIPilgrim>();
            Villager.RemovePart<ConversationScript>();

            string noHateFactions = Villager.GetPropertyOrTag("NoHateFactions");
            if (noHateFactions.IsNullOrEmpty() || !noHateFactions.Contains("Wardens"))
            {
                noHateFactions = !noHateFactions.IsNullOrEmpty() ? $"{noHateFactions},Wardens" : "Wardens";
            }
            if (noHateFactions.IsNullOrEmpty() || !noHateFactions.Contains("Dromad"))
            {
                noHateFactions = !noHateFactions.IsNullOrEmpty() ? $"{noHateFactions},Dromad" : "Dromad";
            }
            if (noHateFactions.IsNullOrEmpty() || !noHateFactions.Contains("SCRT_GNT_VLG_FCT"))
            {
                noHateFactions = !noHateFactions.IsNullOrEmpty() ? $"{noHateFactions},{SCRT_GNT_VLG_FCT}" : SCRT_GNT_VLG_FCT;
            }
            if (noHateFactions.IsNullOrEmpty() || !noHateFactions.Contains("Giants"))
            {
                noHateFactions = !noHateFactions.IsNullOrEmpty() ? $"{noHateFactions},Giants" : "Giants";
            }
            Villager.SetStringProperty("NoHateFactions", noHateFactions);

            Villager.Brain.Factions = "";
            Villager.Brain.Allegiance.Clear();
            Villager.Brain.Allegiance.Add("Giants", 100);
            if (!isResident)
            {
                Villager.Brain.Allegiance.Add(SCRT_GNT_VLG_FCT, 50);
            }
            Villager.Brain.Allegiance.Hostile = false;
            Villager.Brain.Allegiance.Calm = true;
            Villager.Brain.Wanders = true;
            Villager.Brain.WandersRandomly = true;

            GenericInventoryRestocker inventoryRestocker = null;
            ConversationScript conversationScript = null;
            Interesting interesting = null;

            Villager.SetIntProperty("SuppressSimpleConversation", 1);
            Villager.SetIntProperty("ParticipantVillager", 1);

            if (!isResident)
            {
                Villager.SetIntProperty("SecretGiantVillager", 1);
                Villager.SetStringProperty("HeroNameColor", "Y");
                // Villager.SetStringProperty("HeroColorString", "same");
                // Villager.SetStringProperty("HeroTileColor", "same");
                Villager.SetStringProperty("HeroDetailColor", HeroDetailColor);
                if (isGutsmonger && HeroDetailColor == "B")
                {
                    List<string> colors = new()
                    {
                        "Y",
                        "y",
                        "C",
                        "c",
                        "b",
                    };
                    Villager.SetStringProperty("HeroDetailColor", colors.GetRandomElement());
                }
                interesting = Villager.RequirePart<Interesting>();
            }
            else
            {
                if (Villager.Render != null)
                {
                    Villager.Render.DetailColor = HeroDetailColor;
                }
            }

            string conversationScriptID = null;

            if (!isPet && !isResident)
            {
                Villager.SetStringProperty("Merchant", "You betcha!");
                inventoryRestocker = Villager.RequirePart<GenericInventoryRestocker>();
                inventoryRestocker.Clear();
                inventoryRestocker.AddTable($"Giant {Context} Wares");
                conversationScript = Villager.RequirePart<ConversationScript>();
            }

            Statistic hitpoints = Villager.GetStat("Hitpoints");
            Statistic level = Villager.GetStat("Level");
            Statistic xP = Villager.GetStat("XP");
            string baseVillagerBlueprintName = null;
            string heroTemplate = Context;
            if (isTinker)
            {
                conversationScriptID = "tinker";
                if (!Villager.InheritsFrom("HumanTinker"))
                {
                    baseVillagerBlueprintName = "HumanTinker";
                    ConversationsAPI.addSimpleConversationToObject(
                        Object: Villager, 
                        Text: "Need a gadget repaired or identified, =player.formalAddressTerm=? " +
                        "Or if you're a tinker =player.reflexive=, perhaps you'd like to peruse my schematics?", 
                        Goodbye: "Live and drink, tinker.", 
                        ClearLost: true);
                }
            }
            if (isApothecary)
            {
                baseVillagerBlueprintName = "HumanApothecary";
                conversationScriptID = "herbalist";
                ConversationsAPI.addSimpleConversationToObject(
                    Object: Villager, 
                    Text: "I've the cure for what ails you.~You don't look so good. You need more yuckwheat and honey in your diet.~" +
                    "Cook your meals with yuckwheat if you feel sick. Catch a disease early enough and you can kill it.~" +
                    "\"Ease the pain, addle the brain.\" Be careful when you chew witchwood bark.~" +
                    "In the market for a tonic, =player.formalAddressTerm=? Spend water now or blood later, your choice.~" +
                    "Prickly-boons and yuckwheat for trade.~" +
                    "If you came for the humble pie, you had best not have led any mind-hunters here.~" +
                    "Have you got enough tonics?", 
                    Goodbye: "Live and drink.", 
                    ClearLost: true);
            }

            if ((isTinker && !Villager.InheritsFrom("HumanTinker")) || (isApothecary && !Villager.InheritsFrom("HumanApothecary")))
            {
                GameObjectBlueprint baseMerchantBlueprint = GameObjectFactory.Factory.GetBlueprintIfExists($"{baseVillagerBlueprintName}{Tier}");

                inventoryRestocker.Table = $"Village {Context} {Tier}";

                // Villager.SetIntProperty("SuppressSimpleConversation", 1);

                int blueprintHitpoints = baseMerchantBlueprint.Stats["Hitpoints"].BaseValue;
                int blueprintLevel = baseMerchantBlueprint.Stats["Level"].BaseValue;
                int blueprintXP = baseMerchantBlueprint.Stats["XP"].BaseValue;

                if (hitpoints.BaseValue < blueprintHitpoints)
                {
                    hitpoints.BaseValue = blueprintHitpoints;
                }
                if (level.BaseValue < blueprintLevel)
                {
                    level.BaseValue = blueprintLevel;
                }

                if (isTinker)
                {

                    Villager.GetStat("Intelligence").BaseValue = Math.Max(baseMerchantBlueprint.Stats["Intelligence"].BaseValue, 16);
                }
                if(isApothecary)
                {
                    Villager.SetStringProperty("Role", "Skirmisher");
                    Villager.GetStat("Intelligence").BaseValue = Math.Max(baseMerchantBlueprint.Stats["Intelligence"].BaseValue, 15);
                    Villager.GetStat("Toughness").BaseValue = Math.Max(baseMerchantBlueprint.Stats["Toughness"].BaseValue, 15);
                }

                if (baseMerchantBlueprint != null)
                {
                    if (!baseMerchantBlueprint.Skills.IsNullOrEmpty())
                    {
                        foreach ((string name, GamePartBlueprint blueprint) in baseMerchantBlueprint.Skills)
                        {
                            Villager.AddSkill(name);
                        }
                    }
                    if (!baseMerchantBlueprint.Inventory.IsNullOrEmpty())
                    {
                        foreach (InventoryObject inventoryObject in baseMerchantBlueprint.Inventory)
                        {
                            if (inventoryObject.Chance.in100())
                            {
                                int itemAmount = Stat.Roll(inventoryObject.Number);
                                string autoMod = nameof(ModGigantic);
                                Villager.ReceiveObject(Blueprint: inventoryObject.Blueprint, Number: itemAmount, AutoMod: autoMod);
                            }
                        }
                    }
                }
            }
            if (isDromad)
            {
                conversationScriptID = "DromadTrader";
                Villager.SetStringProperty("HeroTileColor", "&w");
                if (Villager.TryGetPart(out DromadCaravan dromadCaravan))
                {
                    Villager.RemovePart(dromadCaravan);
                }
                ConversationsAPI.addSimpleConversationToObject(
                        Object: Villager,
                        Text: "Welcome, =player.species=. What do you desire?",
                        Goodbye: "Live and drink.",
                        ClearLost: true);

                for (int i = 0; i <= 2 && Tier > i; i++)
                {
                    // inventoryRestocker.AddTable($"Tier{(Tier - i).ToStringCached()}Wares");
                }
                heroTemplate = $"Dromad{Context}";
                string dromadTitle = NameMaker.MakeTitle(For: Villager, Special: Context);
                if (!dromadTitle.IsNullOrEmpty())
                {
                    Villager.RequirePart<Titles>().AddTitle(dromadTitle, -5);
                }
                if (Villager.Brain.Allegiance.IsNullOrEmpty())
                {
                    Villager.Brain.Factions = $"Giants-100";
                    Villager.Brain.Factions = $"{SCRT_GNT_VLG_FCT}-50";
                }
                else if (!Villager.Brain.Allegiance.ContainsKey(SCRT_GNT_VLG_FCT))
                {
                    Villager.Brain.Allegiance["Giants"] = 75;
                    Villager.Brain.Allegiance[SCRT_GNT_VLG_FCT] = 25;
                }
            }
            if (isGutsmonger)
            {
                // Villager.SetIntProperty("SuppressSimpleConversation", 1);
                Villager.SetStringProperty("HeroTileColor", "&B");

                conversationScriptID = "gutsmonger";

                string conversationText =
                    "Oi, =player.species=... Guts??~" +

                    "Go' anuver... \"shipmen\" 'rivin soon. " +
                    "\n\nDon' ask... Heh.~" +

                    "Der's two fins Oim good a', en Oim notoriously skint on d'means for wun-uv 'em! " +
                    "\n\n Har har har! Oim a funnee wun!~" +

                    "{{emote|*cracks knuckles obnoxously*}}~" +

                    "'s a family name, 'roight? " +
                    "\n\nOid change et, bu' folk 'round 'er' know me by it.~" +

                    "Oiv go' a speshol se' o' skiows det make me a noightmer fo' a certain kin'na folk wot fink we 'er're danjrus... " +
                    "\n\n'an maybe Oi em... Ta dem.~" +

                    "Dey'w tell yah 's cus Oi took fings wot we'ren moine an' sold 'em wivow permitch.. " +
                    "\n\nwivow persismz... " +
                    "\n\n...wen Oi wa'n' opposed to! " +
                    "\n\nBu' Oi say if'n dey we'ren moine... how'd Oi take 'em??";

                ConversationsAPI.addSimpleConversationToObject(
                        Object: Villager,
                        Text: conversationText,
                        Goodbye: "Live and, uh... Drink?",
                        ClearLost: true);

                string gutsmongerTitle = NameMaker.MakeTitle(For: Villager, Special: Context);
                if (!gutsmongerTitle.IsNullOrEmpty())
                {
                    Villager.RequirePart<Titles>().AddTitle(gutsmongerTitle, -5);
                }
                if (Villager.Brain.Allegiance.IsNullOrEmpty())
                {
                    Villager.Brain.Factions = $"Giants-100";
                    Villager.Brain.Factions = $"{SCRT_GNT_VLG_FCT}-50";
                }
                else if (!Villager.Brain.Allegiance.ContainsKey(SCRT_GNT_VLG_FCT))
                {
                    Villager.Brain.Allegiance["Giants"] = 75;
                    Villager.Brain.Allegiance[SCRT_GNT_VLG_FCT] = 25;
                }
            }
            if (isPet)
            {
                Villager.SetStringProperty("HeroTileColor", "&z");
                string petTitle = NameMaker.MakeTitle(Villager);
                Villager.GiveProperName();
                if (!petTitle.IsNullOrEmpty())
                {
                    Villager.RequirePart<Titles>().AddTitle(petTitle, -5);
                }
                Villager.RequirePart<SmartuseForceTwiddles>();

                Villager.RemovePart<Pettable>();
                Pettable pettable = Villager.RequirePart<Pettable>();
                pettable.PettableIfPositiveFeeling = true;
                pettable.UseFactionForFeelingFloor = SCRT_GNT_VLG_FCT;

                Villager.SetIntProperty("VillagePet", 1);
                Villager.RequirePart<Interesting>().Key = "VillagePet";

                ConversationsAPI.addSimpleConversationToObject(
                    Object: Villager, 
                    Text: Villager.GetTag("SimpleConversation", "*does not react*"), 
                    Goodbye: "Live and drink.");
            }
            if (isResident)
            {
                string response = Villager.GetTag("SimpleConversation",
                    "Moon and Sun. Wisdom and will.~" +
                    "May the earth yield for us this season.~" +
                    "Peace, =player.formalAddressTerm=.");
                ConversationsAPI.addSimpleConversationToObject(
                    Object: Villager,
                    Text: response,
                    Goodbye: "Live and drink.");
            }

            if (!conversationScriptID.IsNullOrEmpty() && conversationScript != null)
            {
                conversationScript.ConversationID = conversationScriptID;
            }

            Villager.SetStringProperty("Culture", isResident ? "Giant" : "WrassleGiant");
            if (!isGutsmonger)
            {
                Villager.SetStringProperty("Species", "Giant");
            }

            if (!isResident)
            {
                Villager = HeroMaker.MakeHero(Villager, $"SpecialVillagerHeroTemplate_{heroTemplate}", -1, Context);

                string epithetContext = !isGutsmonger ? Context : "Warden";

                string villagerEpithet = NameMaker.MakeEpithet(
                    For: null,
                    Genotype: null,
                    Subtype: null,
                    Species: null,
                    Culture: null,
                    Faction: "WrassleGiants",
                    Region: null,
                    Gender: null,
                    Mutations: null,
                    Tag: null,
                    Special: epithetContext,
                    NamingContext: null,
                    SpecialFaildown: true,
                    HasHonorific: null,
                    HasEpithet: null);

                Debug.LoopItem(4, $"{nameof(villagerEpithet)}", villagerEpithet ?? NULL, Good: villagerEpithet != null, Indent: indent + 2, Toggle: getDoDebug());

                string villagerName = null;

                if (!isGutsmonger)
                {
                    villagerName = NameMaker.MakeName(
                        For: null,
                        Genotype: null,
                        Subtype: null,
                        Species: null,
                        Culture: null,
                        Faction: "WrassleGiants",
                        Region: null,
                        Gender: null,
                        Mutations: null,
                        Tag: null,
                        Special: Context,
                        NamingContext: null,
                        SpecialFaildown: true,
                        HasHonorific: null,
                        HasEpithet: null);
                }

                Debug.LoopItem(4, $"{nameof(villagerName)}", villagerName ?? NULL, Good: villagerName != null, Indent: indent + 2, Toggle: getDoDebug());

                if (!villagerName.IsNullOrEmpty() && villagerName.Contains("NameGenFail"))
                {
                    villagerName = null;
                }
                else
                {
                    villagerName = villagerName.OptionalColorYuge();
                }

                if (!villagerName.IsNullOrEmpty() && !isGutsmonger)
                {
                    Villager.GiveProperName(
                        Name: villagerName,
                        Force: true,
                        Special: "Hero",
                        SpecialFaildown: true,
                        HasHonorific: null,
                        HasEpithet: null,
                        NamingContext: null);
                }

                if (!villagerEpithet.IsNullOrEmpty())
                {
                    if (Villager.TryGetPart(out Epithets epithets))
                    {
                        Villager.RemovePart(epithets);
                    }
                    epithets = Villager.RequirePart<Epithets>();
                    epithets.Primary = GameText.VariableReplace(villagerEpithet).Color("y");
                }
                Villager.SetIntProperty($"Village{Context}", 1);
                Villager.SetIntProperty("NamedVillager", 1);
                TakeOnRoleEvent.Send(Villager, Context);
                inventoryRestocker?.PerformRestock(Silent: true);
            }
            else
            {
                Villager.SetIntProperty("Villager", 1);
            }

            int xPThisLevel = Leveler.GetXPForLevel(level.BaseValue);
            int xPNextLevel = Leveler.GetXPForLevel(level.BaseValue + 1);
            xP.BaseValue = Stat.RandomCosmetic(xPThisLevel, xPNextLevel);

            Villager.FireEvent("VillageInit");
            return Villager;
        }

        [WishCommand(Command = "go2giant")]
        public static void GoToGiantWish()
        {
            Zone Z = The.ZoneManager.GetZone(SecretZoneID);
            The.Player.Physics.CurrentCell.RemoveObject(The.Player.Physics.ParentObject);
            Z.GetEmptyCells().GetRandomElement().AddObject(The.Player);
            The.ZoneManager.SetActiveZone(Z);
            The.ZoneManager.ProcessGoToPartyLeader();
        }

        [WishCommand(Command = "GetAGiant")]
        public static void GetAGiantWish()
        {
            GameObject giant = GetAGiant();
            Cell cell = The.Player.CurrentCell.getClosestEmptyCell();
            if (cell == null)
            {
                Popup.Show($"No empty cells nearby to spawn giant {giant?.DebugName ?? NULL}");
                return;
            }
            cell.AddObject(giant);
        }

        [WishCommand(Command = "hanker giants")]
        public static void HankerGiantsWish()
        {
            Zone Z = The.Player.CurrentZone;
            foreach (GameObject Object in Z.GetObjectsWithPart(typeof(GigantismPlus).Name))
            {
                if (Object.IsPlayer()) continue;
                Object.Die(null ,null, "insufficient stew", "insufficient stew", true, DeathVerb: "hanker");
            }
        }
    } //!-- public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
}
