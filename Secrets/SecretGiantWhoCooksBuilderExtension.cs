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

        public static List<string> WrassleRingColors = new()
        {
            $"W",
            $"M",
            $"G",
            $"B",
            $"C",
            $"R",
            $"w",
        };

        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Debug.Entry(4,
                $"\u2666 {nameof(SecretGiantWhoCooksBuilderExtension)}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0, Toggle: getDoDebug());

            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: true);
            SecretZoneId = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            if (JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID).Is(null))
            {
                JournalAPI.AddMapNote(
                    ZoneID: SecretZoneId,
                    text: SCRT_GNT_LCTN_TEXT,
                    category: SCRT_GNT_LCTN_CATEGORY,
                    attributes: WrassleGiantHero.SecretAttributes,
                    secretId: SCRT_GNT_SCRT_ID
                );
            }
            SecretMapNote = JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID);
            SecretMapNote.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(Hills));
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(FactionEncounters));
            zoneManager.ClearZoneBuilders(SecretZoneId);

            // zoneManager.AddZoneBuilder(SecretZoneId, LATE, nameof(CreateGiantCrater));

            string MapFileName = SCRT_GNT_ZONE_MAP2_CENTRE;

            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(MapBuilder), "FileName", $"{MapFileName}");
            zoneManager.AddZonePostBuilder(SecretZoneId, "Music", "Track", "Music/Barathrums Study");

            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(IsCheckpoint), "Key", SecretZoneId);

            zoneManager.SetZoneProperty(SecretZoneId, "SkipTerrainBuilders", true);
            zoneManager.SetZoneProperty(SecretZoneId, "NoBiomes", "Yes");

            zoneManager.SetZoneName(SecretZoneId, SCRT_GNT_LCTN_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneId, false);

            GameObject UniqueGiant = GetTheGiant();

            if (MapFileName == SCRT_GNT_ZONE_MAP2_CENTRE)
                zoneManager.AddZonePostBuilder(
                    ZoneID: SecretZoneId, 
                    Class: nameof(GiantAbodePopulator), 
                    Key1: "GiantID", 
                    Value1: zoneManager.CacheObject(UniqueGiant));

            if (UniqueGiant == null)
            {
                Debug.Entry(2,
                    $"WARN: {nameof(SecretGiantWhoCooksBuilderExtension)}." +
                    $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder) ",
                    $"failed to instantiate UniqueGiant. Placement aborted.",
                    Indent: 0, Toggle: getDoDebug('!'));
                return;
            }

            string wrasslerColor = null;
            if (UniqueGiant.TryGetPart(out Wrassler wrassler)) wrasslerColor = wrassler.DetailColor;

            string wrassleRingColor = wrasslerColor ?? WrassleRingColors.GetRandomElement();
            foreach (GameObject rope in zoneManager.GetZone(SecretZoneId).GetObjectsThatInheritFrom("WrassleRingRopes"))
            {
                if(rope.TryGetPart(out WrassleGear wrassleGear) && wrassler != null)
                {
                    wrassleGear.WrassleID = wrassler.WrassleID;
                    wrassleGear.ApplyFlair(doTile: false, doTileColor: false, doDetailColor: true, doColorString: false);
                }
                else
                {
                    rope.Render.DetailColor = wrassleRingColor;
                }
            }

            /*
            zoneManager.AddZonePostBuilder(
                ZoneID: SecretZoneId, 
                Class: nameof(AddObjectBuilder), 
                Key1: "Object", 
                Value1: zoneManager.CacheObject(UniqueGiant)); */


        } //!-- public override void OnAfterBuild(JoppaWorldBuilder builder)

        public static GameObject GetTheGiant()
        {
            return GetAGiant(Unique: true);
        }
        public static GameObject GetAGiant(bool Unique = false)
        {
            GameObject creature;
            GameObjectBlueprint creatureBlueprint = Unique 
                ? WrassleGiantHero.GetAUniqueGiantHeroBluePrintModel()
                : WrassleGiantHero.GetAGiantHeroBluePrintModel()
                ;

            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: creatureBlueprint,
                    BonusModChance: 0,
                    SetModNumber: 0,
                    AutoMod: null,
                    BeforeObjectCreated: null,
                    AfterObjectCreated: null,
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
                Popup.Show($"No empty cells nearby to spawn giant {giant.DisplayNameStripped}");
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
