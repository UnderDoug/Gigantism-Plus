using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Genkit;
using HistoryKit;
using Qud.API;

using XRL;
using XRL.UI;
using XRL.Rules;
using XRL.World;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.ZoneBuilders;
using XRL.World.WorldBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;
using XRL.World.Skills.Cooking;
using XRL.World.Conversations;
using XRL.Language;
using XRL.Names;
using XRL.Wish;

using static XRL.World.ZoneBuilderPriority;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
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

        public static string SecretZoneId = string.Empty;
        public static JournalMapNote SecretMapNote = null;
        public Zone SecretZone => The.ZoneManager.GetZone(SecretZoneId);

        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Debug.Entry(4,
                $"\u2666 {nameof(SecretGiantWhoCooksBuilderExtension)}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0, Toggle: getDoDebug());

            Debug.Entry(4, $"Getting coordinates...", Indent: 1, Toggle: getDoDebug());
            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: true);
            Debug.LoopItem(4, $"{nameof(location)}: [{location}]", Indent: 2, Toggle: getDoDebug());

            Debug.Entry(4, $"Getting ZoneID from {nameof(location)}...", Indent: 1, Toggle: getDoDebug());
            SecretZoneId = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);
            Debug.Entry(4, $"{nameof(SecretZoneId)} Set", $"{SecretZoneId}", Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"Checking MapNote isn't already set...", Indent: 1, Toggle: getDoDebug());
            if (JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID) == null)
            {
                Debug.CheckYeh(4, $"MapNote not set", Indent: 2, Toggle: getDoDebug());
                Debug.Entry(4, $"Setting MapNote...", Indent: 1, Toggle: getDoDebug());
                JournalAPI.AddMapNote(
                    ZoneID: SecretZoneId,
                    text: SCRT_GNT_LCTN_TEXT,
                    category: SCRT_GNT_LCTN_CATEGORY,
                    attributes: WrassleGiantHero.SecretAttributes,
                    secretId: SCRT_GNT_SCRT_ID
                );
            }
            Debug.Entry(4, $"Setting MapNote Weight...", Indent: 1, Toggle: getDoDebug());
            SecretMapNote = JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID);
            SecretMapNote.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            Debug.Entry(4, $"Removing undesired ZoneBuilders...", Indent: 1, Toggle: getDoDebug());
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(Hills));
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(FactionEncounters));
            zoneManager.ClearZoneBuilders(SecretZoneId);

            string MapFileName = SCRT_GNT_ZONE_MAP2_CENTRE;
            Debug.Entry(4, $"Setting {nameof(MapFileName)}...", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"{nameof(MapFileName)} Set", $"{MapFileName}", Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"Assigning MapFile to MapBuilder...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(MapBuilder), "FileName", $"{MapFileName}");

            Debug.Entry(4, $"Setting Music...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneId, "Music", "Track", "Music/Barathrums Study");

            Debug.Entry(4, $"Setting Zone to Checkpoint...", Indent: 1, Toggle: getDoDebug());
            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(IsCheckpoint), "Key", SecretZoneId);

            Debug.Entry(4, $"Skipping TerainBuilders and flaggign NoBiomes...", Indent: 1, Toggle: getDoDebug());
            zoneManager.SetZoneProperty(SecretZoneId, "SkipTerrainBuilders", true);
            zoneManager.SetZoneProperty(SecretZoneId, "NoBiomes", "Yes");

            Debug.Entry(4, $"Setting ZoneName...", Indent: 1, Toggle: getDoDebug());
            zoneManager.SetZoneName(SecretZoneId, SCRT_GNT_LCTN_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneId, false);

            TerrainTravel pTravel = builder.terrainComponents[Location2D.Get(location.X/3, location.Y/3)];
            if (XRL.UI.Options.ShowOverlandEncounters && pTravel != null)
            {
                Debug.Entry(4, $"Setting up OverLandEncounters option...", Indent: 1, Toggle: getDoDebug());
                pTravel.ParentObject.Render.RenderString = "G";
                pTravel.ParentObject.Render.SetForegroundColor('Z');
            }

            Debug.Entry(4, $"Waking up Unique Giant...", Indent: 1, Toggle: getDoDebug());
            GameObject UniqueGiant = GetTheGiant();

            string wrasslerColor = null;
            Debug.Entry(4, $"Storing {nameof(wrasslerColor)}...", Indent: 1, Toggle: getDoDebug());
            if (UniqueGiant.TryGetPart(out Wrassler wrassler))
            {
                Debug.CheckYeh(4, $"{nameof(UniqueGiant)} has {nameof(Wrassler)} part", Indent: 1, Toggle: getDoDebug());
                wrasslerColor = wrassler.DetailColor;
            }
            string wrassleRingColor = wrasslerColor ?? UD_QWE.WrassleRingColors.GetRandomElement();
            Debug.Entry(4, $"{nameof(wrassleRingColor)} is {wrassleRingColor}", Indent: 1, Toggle: getDoDebug());

            if (UniqueGiant == null)
            {
                Debug.Warn(2,
                    $"{nameof(SecretGiantWhoCooksBuilderExtension)}",
                    $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder) ",
                    $"failed to instantiate UniqueGiant. Placement aborted.",
                    Indent: 0);
                return;
            }

            Debug.Entry(4, $"Assigning GiantAbodePopulator if it's necessary...", Indent: 1, Toggle: getDoDebug());
            if (MapFileName == SCRT_GNT_ZONE_MAP2_CENTRE) // This specific map has the widgets necessary for the specified builder to work
            {
                Debug.CheckYeh(4, $"Map is correct, adding {nameof(GiantAbodePopulator)}...", Indent: 2, Toggle: getDoDebug());
                zoneManager.AddZonePostBuilder(
                    ZoneID: SecretZoneId,
                    Class: nameof(GiantAbodePopulator),
                    Key1: "GiantID",
                    Value1: zoneManager.CacheObject(UniqueGiant));
            }
            else
            {
                Debug.CheckNah(4, $"Map is incorrect, adding {nameof(AddObjectBuilder)}...", Indent: 2, Toggle: getDoDebug());
                zoneManager.AddZonePostBuilder(
                    ZoneID: SecretZoneId,
                    Class: nameof(AddObjectBuilder),
                    Key1: "Object",
                    Value1: zoneManager.CacheObject(UniqueGiant));
            }

            Debug.Entry(4, $"Getting Ropes and Attempting to assign Color...", Indent: 1, Toggle: getDoDebug());
            List<GameObject> ropesList = zoneManager.GetZone(SecretZoneId).GetObjectsThatInheritFrom("WrassleRingRopes");
            if (!ropesList.IsNullOrEmpty())
            {
                Debug.CheckYeh(4, $"Got Ropes", Indent: 2, Toggle: getDoDebug());
                foreach (GameObject rope in ropesList)
                {
                    Debug.LoopItem(4, $"{nameof(rope)}: {rope?.DebugName}", Indent: 2, Toggle: getDoDebug());
                    if (rope.TryGetPart(out WrassleGear wrassleGear))
                    {
                        Debug.CheckYeh(4, $"{nameof(rope)} has {nameof(WrassleGear)}", Indent: 3, Toggle: getDoDebug());
                        Debug.Entry(4, $"Attempting to sync WrassleIDs...", Indent: 3, Toggle: getDoDebug());
                        if (UD_QWE.TrySyncWrassleID(UniqueGiant, rope))
                        {
                            Debug.CheckYeh(4, $"Wrassle ID's synched", Indent: 3, Toggle: getDoDebug());
                            wrassleGear.SetDetailColor(Force: true);
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Wrassle ID's failed to sync", Indent: 3, Toggle: getDoDebug());
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
            }
            else
            {
                Debug.CheckNah(4, $"No Ropes", Indent: 2, Toggle: getDoDebug());
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
                ? WrassleGiantHero.GetAUniqueGiantHeroBlueprintModel()
                : WrassleGiantHero.GetAGiantHeroBlueprintModel()
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

        [WishCommand(Command = "go2giant")]
        public static void GoToGiantWish()
        {
            Zone Z = The.ZoneManager.GetZone(SecretZoneId);
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
