using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Genkit;
using HistoryKit;
using Qud.API;

using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;
using static XRL.World.ZoneBuilderPriority;
using XRL.World.ObjectBuilders;
using XRL.World.Conversations;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;
using XRL.World.Skills.Cooking;
using XRL.Language;
using XRL.Names;
using XRL.World.Capabilities;
using XRL.Rules;
using XRL.UI;
using XRL.Wish;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [HasWishCommand]
    [JoppaWorldBuilderExtension]
    public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
    {
        public static string SecretZoneId = string.Empty;
        public static JournalMapNote SecretMapNote = null;

        public static List<string> FactionAdmirationBag = new()
        {
            "cooking them a seriously thick stew",
            "clearing their roofs of clutter",
            "teaching them patience",
            "shifting a giant boulder for them",
            "taking the time to listen to their woes",
            "wreslting a salt kraken for fun",
            "=subject.possessive= vibrant nanoweave suit",
            "teaching them to wrestle",
        };
        public static List<string> ThiccBoisBag = new()
        {
            "Trolls",
            "Cragmensch",
        };
        public static List<string> EpithetsBag = new()
        {
            $"the {"Giant".OptionalColorYuge(Colorfulness)}",
            $"the {"Thicc".OptionalColorYuge(Colorfulness)}",
            $"the {"Swole".OptionalColorYuge(Colorfulness)}",
            $"of the {"Gains".OptionalColorYuge(Colorfulness)}",
            $"of the {"Stew".OptionalColorYuge(Colorfulness)}",
            $"who {"Cooks".OptionalColorYuge(Colorfulness)}",
            $"who is {"Immense".OptionalColorYuge(Colorfulness)}",
            $"who {"Hankers".OptionalColorYuge(Colorfulness)}",
            $"{"Hanker's Bane".OptionalColorYuge(Colorfulness)}",
        };

        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Debug.Entry(4,
                $"\u2666 {typeof(SecretGiantWhoCooksBuilderExtension).Name}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0);

            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: true);
            SecretZoneId = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            if (JournalAPI.GetMapNote(SECRET_GIANTID).Is(null))
            {
                JournalAPI.AddMapNote(
                    ZoneID: SecretZoneId,
                    text: SECRET_GIANTLOCATION_TEXT,
                    category: SECRET_GIANTLOCATION_CATEGORY,
                    attributes: new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" },
                    secretId: SECRET_GIANTID
                );
            }
            SecretMapNote = JournalAPI.GetMapNote(SECRET_GIANTID);
            SecretMapNote.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            zoneManager.ClearZoneBuilders(SecretZoneId);
            zoneManager.AddZoneBuilder(SecretZoneId, LATE, nameof(CreateGiantCrater));
            // zoneManager.AddZoneBuilder(SecretZoneId, LATE + ADJUST_MEDIUM, nameof(Ruiner));
            // zoneManager.AddZoneBuilder(SecretZoneId, VERY_LATE, nameof(RoadBuilder));

            Debug.Entry(4, $"Listing Zone Builder Blueprints", Indent: 1);
            foreach (ZoneBuilderBlueprint builderBlueprint in zoneManager.GetBuildersFor(SecretZoneId))
            {
                Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                Debug.Entry(4, $"{builderBlueprint.Class}", Indent: 2);
            }
            Debug.Divider(4, HONLY, Count: 25, Indent: 2);

            Debug.Entry(4, $"Listing Zone Parts", Indent: 1);
            ZonePartCollection zonePartCollection = new();
            if (zoneManager.ZoneParts.ContainsKey(SecretZoneId))
            {
                zonePartCollection = zoneManager.ZoneParts[SecretZoneId];
            }

            if (!zonePartCollection.IsNullOrEmpty())
            {
                foreach (ZonePartBlueprint partBlueprint in zonePartCollection.Members)
                {
                    Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                    Debug.Entry(4, $"{partBlueprint.Name}", Indent: 2);
                }
                Debug.Divider(4, HONLY, Count: 25, Indent: 2);
            }
            else
            {
                Debug.Entry(4, $"None", Indent: 2);
            }

            ZoneBlueprint zoneBlueprint = zoneManager.GetZoneBlueprint(SecretZoneId);
            Debug.Entry(4, $"ZoneBlueprint is {zoneBlueprint.Name}", Indent: 1);

            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(Hills));
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(FactionEncounters));

            zoneManager.AddZonePostBuilder(SecretZoneId, "Music", "Track", "Music/Barathrums Study");
            zoneManager.SetZoneProperty(SecretZoneId, "SkipTerrainBuilders", true);

            zoneManager.SetZoneName(SecretZoneId, SECRET_GIANTLOCATION_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneId, false);
            zoneManager.SetZoneProperty(SecretZoneId, "NoBiomes", "Yes");

            zoneManager.AddZoneBuilder(
                ZoneID: SecretZoneId, 
                Priority: MID + ADJUST_LARGE, 
                Class: nameof(AddObjectBuilder), 
                Key1: "Object", 
                Value1: zoneManager.CacheObject(GetTheGiant()));
        }
        public static GameObject GetTheGiant()
        {
            return GetAGiant(Unique: true);
        }
        public static GameObject GetAGiant(bool Unique = false)
        {
            string giantName = NameStyles.Generate(Culture: "Giant");

            if (Unique)
            {
                Unique = !The.Game.HasStringGameState(SECRET_GIANT_UNIQUE_STATE);
            }

            List<string> oldFactions = 
                (from faction in Factions.Loop()
                 where faction.Old
                 select faction.Name).ToList();
            GameObjectBlueprint creatureBlueprint = EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => EncountersAPI.IsLegendaryEligible(blueprint)
                && (blueprint.HasPart(nameof(Body)) || blueprint.HasTagOrProperty("BodySubstitute"))
                && (blueprint.HasPart(nameof(Combat)) || blueprint.HasTagOrProperty("BodySubstitute"))
                && oldFactions.Contains(blueprint.GetPrimaryFaction())
                && !blueprint.HasTag("BaseObject")
                && !blueprint.HasTag("NoLibrarian")
                && !blueprint.HasPart(nameof(Burrowing))
                && !blueprint.HasPart(nameof(WallWalker))
                && !blueprint.HasProperName()
                && !blueprint.DescendsFrom("BaseTrueKin")
                && !blueprint.DescendsFrom("BasePlant")
                && !(blueprint.DescendsFrom("BaseRobot")
                     && !(blueprint.Name.Is("Chrome Pyramid")
                       || blueprint.Name.Is("Leering Stalker")))
                && !(blueprint.TryGetTag("Species", out string species) && species.Is("mecha")));

            GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
            {
                Name = nameof(Gigantified),
                ChanceOneIn = 1,
            };
            creatureBlueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;

            creatureBlueprint.Tags["Culture"] = "Giant";

            if (Unique)
            {
                if (!creatureBlueprint.Tags.ContainsKey("Role")) creatureBlueprint.Tags.Add("Role", "");
                creatureBlueprint.Tags["Role"] = $"Leader";
            }

            if (creatureBlueprint.Tags.ContainsKey("NoHateFactions")) creatureBlueprint.Tags.Remove("NoHateFactions");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction1")) creatureBlueprint.Tags.Remove("staticFaction1");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction2")) creatureBlueprint.Tags.Remove("staticFaction2");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction3")) creatureBlueprint.Tags.Remove("staticFaction3");

            if (!creatureBlueprint.Tags.ContainsKey("SharesRecipe")) creatureBlueprint.Tags.Add("SharesRecipe", "");

            // Could possibly look at gigantifying them instead...
            creatureBlueprint.Parts.RemoveAll((KeyValuePair<string, GamePartBlueprint> entry)
                => entry.Key == typeof(DromadCaravan).Name
                || entry.Key == typeof(HasGuards).Name
                || entry.Key == typeof(HasThralls).Name
                || entry.Key == typeof(HasSlaves).Name
                || entry.Key == typeof(Followers).Name);

            GameObject creature;
            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: creatureBlueprint,
                    BonusModChance: 0,
                    SetModNumber: 0,
                    AutoMod: null,
                    BeforeObjectCreated: null,
                    AfterObjectCreated: null,
                    Context: null,
                    ProvideInventory: null);

            creature.SetStringProperty("SharesRecipe", SECRET_GIANTRECIPE);
            creature.SetStringProperty("SharesRecipeWithTrueKin", "false");
            // creature.SetStringProperty("SharesRecipeText", "");

            if (creature.TryGetPart(out GameUnique gameUnique))
            {
                creature.RemovePart(gameUnique);
            }
            if (Unique)
            {
                gameUnique = creature.RequirePart<GameUnique>();
                gameUnique.State = SECRET_GIANT_UNIQUE_STATE;
            }

            creature.RequirePart<Calming>();

            if (!creature.TryGetPart(out GivesRep givesRep))
            {
                givesRep = creature.RequirePart<GivesRep>();
            }
            givesRep.repValue = 400;

            creature.SetStringProperty("NoHateFactions", $"Giants,Trolls,Cragmensch,Wardens,Dromad,Mechanimists");

            List<string> factionAdmiration = new(FactionAdmirationBag);
            string factionAdmiration1 = factionAdmiration.DrawRandomElement();
            // Get a bag of different height/size/weight related reasons, draw a random one each
            creature.SetStringProperty("staticFaction1", 
                $",friend,{factionAdmiration1}");

            string staticFaction2Faction = string.Empty;
            string staticFaction2Admiration = factionAdmiration.DrawRandomElement();
            if (Unique || "1d4".Roll().Is(4))
            {
                staticFaction2Faction = ThiccBoisBag.GetRandomElement();
                staticFaction2Admiration = $"=pronouns.possessive= impressive gains";
            }
            creature.SetStringProperty("staticFaction2", $"{staticFaction2Faction},friend,{staticFaction2Admiration}");

            if (Unique)
            {
                creature.SetStringProperty("staticFaction3", $"GiantTemplar,hate,suplexing their warleader in the ring");
            }

            if (creature.TryGetPart(out GigantismPlus gigantism))
            {
                int starting = gigantism.Level + (Unique ? 3 : 0);
                if (starting > 10)
                {
                    starting -= 10;
                }
                gigantism.Level = Unique ? 10 : 5;
                gigantism.SetRapidLevelAmount((int)Math.Max(1, Math.Ceiling(starting / 3.0)) * 3);
            }
            creature.Brain.Mobile = true;
            creature.Brain.Wanders = true;
            creature.Brain.Factions = "Giants";
            creature.Brain.Allegiance.Clear();
            creature.Brain.Allegiance.Add("Giants", 700);
            creature.RequirePart<Interesting>();

            if (!creature.TryGetPart(out Description description))
            {
                description = creature.RequirePart<Description>();
            }
            string creatureNoun = creatureBlueprint.DisplayName() ?? creatureBlueprint.Name;
            string creatureArticle = Grammar.IndefiniteArticle(creatureNoun).Capitalize();
            string preDesc = SECRET_GIANTPREDESC.Replace(SECRET_GIANTPREDESC_REPLACE, $"{creatureArticle} {creatureNoun}");
            description.Short = preDesc + description._Short;

            Epithets epithets = creature.RequirePart<Epithets>();
            epithets.Primary = EpithetsBag.GetRandomElement().Color("y");

            if (creature.TryGetPart(out DisplayNameAdjectives displayNameAdjectives))
            {
                displayNameAdjectives.RemoveAdjective(Gigantified.GetNamePrefix());
            }

            if (Unique)
            {
                if (!creature.TryGetPart(out ConversationScript conversationScript))
                {
                    conversationScript = creature.RequirePart<ConversationScript>();
                }
                conversationScript.ConversationID = SECRET_GIANTCONVSCRIPT_ID;
            }

            // These are the skills the highest tier GiantSlayer has
            creature.AddSkill("Acrobatics");
            creature.AddSkill("Acrobatics_Jump");
            creature.AddSkill("Endurance");
            creature.AddSkill("Endurance_ShakeItOff");
            if (Unique) creature.AddSkill("Endurance_Weathered");
            if (Unique) creature.AddSkill("Endurance_Calloused");
            creature.AddSkill("Tactics");
            creature.AddSkill("Tactics_Charge");
            creature.AddSkill("Cudgel");
            creature.AddSkill("Cudgel_Expertise");
            creature.AddSkill("Cudgel_Bludgeon");
            creature.AddSkill("Cudgel_Slam");
            creature.AddSkill("Cudgel_ChargingStrike");
            if (Unique) creature.AddSkill("Cudgel_Backswing");
            if (Unique) creature.AddSkill("Cudgel_Conk");
            if (Unique) creature.AddSkill("Cudgel_SmashUp");
            creature.AddSkill("SingleWeaponFighting");
            creature.AddSkill("SingleWeaponFighting_OpportuneAttacks");
            if (Unique) creature.AddSkill("SingleWeaponFighting_WeaponExpertise");
            if (Unique) creature.AddSkill("SingleWeaponFighting_PenetratingStrikes");

            if (Unique)
            {
                SecretRevealer secretRevealer = creature.RequirePart<SecretRevealer>();
                secretRevealer.id = SECRET_GIANTID;
                secretRevealer.text = SECRET_GIANTLOCATION_TEXT;
                secretRevealer.message = $"You have discovered {secretRevealer.text}!";
                secretRevealer.category = SECRET_GIANTLOCATION_CATEGORY;
                secretRevealer.adjectives = string.Join(",", 
                    new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });
            }

            creature.GiveProperName(giantName);
            string heroTemplate = Unique ? SECRET_GIANT_HEROTEMPLATE : GIANT_HEROTEMPLATE;
            int heroTier = Unique ? 8 : -1;
            creature = HeroMaker.MakeHero(creature, heroTemplate, heroTier, "Giant");

            if (creature.TryGetPart(out HasMakersMark hasMakersMark)) creature.RemovePart(hasMakersMark);

            if (creature.TryGetPart(out DromadCaravan dromadCaravan)) creature.RemovePart(dromadCaravan);
            if (creature.TryGetPart(out HasGuards hasGuards)) creature.RemovePart(hasGuards);
            if (creature.TryGetPart(out HasThralls hasThralls)) creature.RemovePart(hasThralls);
            if (creature.TryGetPart(out HasSlaves hasSlaves)) creature.RemovePart(hasSlaves);
            if (creature.TryGetPart(out Followers followers)) creature.RemovePart(followers);

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
            Cell cell = The.Player.CurrentCell.GetEmptyAdjacentCells().GetRandomElement();
            if (cell.Is(null))
            {
                Popup.Show($"No empty cells nearby to spawn giant {giant.DisplayNameStripped}");
                return;
            }
            cell.AddObject(giant);
        }
    } //!-- public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
}
