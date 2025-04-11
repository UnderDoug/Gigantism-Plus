using Genkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;
using static XRL.World.ZoneBuilderPriority;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.Names;
using XRL.Wish;
using HistoryKit;
using Qud.API;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using XRL.World.Capabilities;

namespace HNPS_GigantismPlus
{
    [HasWishCommand]
    [JoppaWorldBuilderExtension]
    public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
    {
        public static string SecretZoneId = string.Empty;
        public static JournalMapNote SecretMapNote = null;
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Debug.Entry(4,
                $"\u2666 {typeof(SecretGiantWhoCooksBuilderExtension).Name}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0);

            string giantName = NameStyles.Generate(Culture: "Giant");

            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: false);
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

            zoneManager.AddZoneBuilder(SecretZoneId, NORMAL, nameof(Connecter));
            zoneManager.AddZoneBuilder(SecretZoneId, MID, nameof(RoadBuilder));
            zoneManager.AddZoneBuilder(SecretZoneId, LATE, nameof(OverlandRuins));
            zoneManager.AddZoneBuilder(SecretZoneId, LATE + ADJUST_MEDIUM, nameof(Connecter));

            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(PopTableZoneBuilder));
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(Population));
            zoneManager.RemoveZoneBuilders(SecretZoneId, nameof(FactionEncounters));

            GameObjectBlueprint creatureBlueprint = EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => EncountersAPI.IsLegendaryEligible(blueprint)
                && (blueprint.HasPart("Body") || blueprint.HasTagOrProperty("BodySubstitute"))
                && (blueprint.HasPart("Combat") || blueprint.HasTagOrProperty("BodySubstitute"))
                && !blueprint.HasPart("NoLibrarian")
                && !blueprint.HasPart("ExcludeFromVillagePopulations")
                && !blueprint.DescendsFrom("BaseTrueKin")
                && !(blueprint.TryGetTag("Species", out string species) && species.Is("mecha")));

            GameObject creature;
            GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
            {
                Name = nameof(Gigantified),
                ChanceOneIn = 1,
            };
            creatureBlueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;

            creatureBlueprint.Tags["Culture"] = "Giant";

            if (!creatureBlueprint.Tags.ContainsKey("Role")) creatureBlueprint.Tags.Add("Role", "");
            creatureBlueprint.Tags["Role"] = $"Leader";

            if (creatureBlueprint.Tags.ContainsKey("NoHateFactions")) creatureBlueprint.Tags.Remove("NoHateFactions");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction1")) creatureBlueprint.Tags.Remove("staticFaction1");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction2")) creatureBlueprint.Tags.Remove("staticFaction2");
            if (creatureBlueprint.Tags.ContainsKey("staticFaction3")) creatureBlueprint.Tags.Remove("staticFaction3");

            if (!creatureBlueprint.Tags.ContainsKey("SharesRecipe")) creatureBlueprint.Tags.Add("SharesRecipe", "");
            if (!creatureBlueprint.Tags.ContainsKey("SharesRecipeText")) creatureBlueprint.Tags.Add("SharesRecipeText", "");
            if (!creatureBlueprint.Tags.ContainsKey("SharesRecipeWithTrueKin")) creatureBlueprint.Tags.Add("SharesRecipeWithTrueKin", "");

            List<string> giantRecipeAsk = new()
            {
                $"I've got a serious hankering for that stew, {giantName}.\n",
                $"Please, {giantName}, I need to know how you got swole!\n",
                $"Impressive gains, {giantName}. What's the secret?\n",
            };
            creatureBlueprint.Tags["SharesRecipe"] = SECRET_GIANTRECIPE;
            creatureBlueprint.Tags["SharesRecipeText"] = giantRecipeAsk.GetRandomElement();
            creatureBlueprint.Tags["SharesRecipeWithTrueKin"] = "false";

            // Could possibly look at gigantifying them instead...
            creatureBlueprint.Parts.RemoveAll((KeyValuePair<string, GamePartBlueprint> entry)
                => entry.Key == typeof(HasGuards).Name
                || entry.Key == typeof(HasThralls).Name
                || entry.Key == typeof(HasSlaves).Name);

            if (!creatureBlueprint.Parts.ContainsKey(typeof(SpawnBlocker).Name))
            {
                creatureBlueprint.Parts.Add(typeof(SpawnBlocker).Name, new(typeof(SpawnBlocker).Name));
            }

            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: creatureBlueprint,
                    BonusModChance: 0,
                    SetModNumber: 0,
                    AutoMod: null,
                    BeforeObjectCreated: null,
                    AfterObjectCreated: null,
                    Context: null,
                    ProvideInventory: null);

            if (creature.TryGetPart(out GameUnique gameUnique))
            {
                creature.RemovePart(gameUnique);
            }
            gameUnique = creature.RequirePart<GameUnique>();
            gameUnique.State = SECRET_GIANT_STATE;

            creature.RequirePart<Calming>();
            if (!creature.TryGetPart(out GivesRep givesRep))
            {
                givesRep = creature.RequirePart<GivesRep>();
            }
            givesRep.repValue = 400;

            creature.SetStringProperty("NoHateFactions", $"Giants,Trolls,Cragmensch,Wardens,Dromad,Mechanimists");

            List<string> factionAdmirationBag = new()
            {
                "cooking them a seriously thick stew",
                "clearing their roofs of clutter",
                "teaching them patience",
                "shifting a giant boulder for them",
                "taking the time to listen to their woes",
                "wreslting a salt kraken for fun",
                "having a vibrant nanoweave suit",
                "teaching them to wrestle",
            };
            string factionAdmiration1 = factionAdmirationBag.DrawRandomElement();
            // Get a bag of different height/size/weight related reasons, draw a random one each
            creature.SetStringProperty("staticFaction1", $",friend,{factionAdmiration1}");

            creature.SetStringProperty("staticFaction2", $"{(RndGP.Next() % 2 == 0 ? "Trolls" : "Cragmensch")},friend,=pronouns.possessive= impressive gains");

            // Keep this one static
            creature.SetStringProperty("staticFaction3", $"GiantTemplar,hate,suplexing their warleader in the ring");

            if (creature.TryGetPart(out GigantismPlus gigantism))
            {
                int starting = gigantism.Level + 3;
                if (starting > 10)
                {
                    starting -= 10;
                }
                gigantism.Level = 10;
                gigantism.SetRapidLevelAmount((int)Math.Max(1, Math.Ceiling(starting / 3.0))*3);
            }
            creature.Brain.Mobile = true;
            creature.Brain.Wanders = true;
            creature.Brain.Factions = "";
            creature.Brain.Allegiance.Clear();
            creature.Brain.Allegiance.Add("Giants", 700);
            creature.RequirePart<Interesting>();

            if (!creature.TryGetPart(out Description description))
            {
                description = creature.RequirePart<Description>();
            }
            string preDesc = SECRET_GIANTPREDESC.Replace(SECRET_GIANTPREDESC_REPLACE, creature.Blueprint);
            description.Short = preDesc + description._Short;

            List<string> epithetsBag = new()
            {
                $"the {"giant".OptionalColorGigantic(Colorfulness)}",
            };
            Epithets epithets = creature.RequirePart<Epithets>();
            epithets.Primary = epithetsBag.GetRandomElement();

            if (creature.TryGetPart(out DisplayNameAdjectives displayNameAdjectives))
            {
                displayNameAdjectives.RemoveAdjective(Gigantified.GetNamePrefix());
            }

            /* 
            creature.SetIntProperty("SuppressSimpleConversation", 1);
            string adventurer = HistoricStringExpander.ExpandString("<spice.commonPhrases.adventurer.!random>");
            string simpleConversation = $"Greetings {adventurer}. Can you smell what th- I am cooking?";
            ConversationsAPI.addSimpleConversationToObject(
                Object: creature,
                Text: simpleConversation, 
                Goodbye: "Live and drink.",
                Filter: null, FilterExtras: null, Append: null,
                ClearLost: true);
            */

            if (!creature.TryGetPart(out ConversationScript conversationScript))
            {
                conversationScript = creature.RequirePart<ConversationScript>();
            }
            conversationScript.ConversationID = SECRET_GIANTCONVSCRIPT_ID;

            // These are the skills the highest tier GiantSlayer has
            creature.AddSkill("Acrobatics");
            creature.AddSkill("Acrobatics_Jump");
            creature.AddSkill("Endurance");
            creature.AddSkill("Endurance_ShakeItOff");
            creature.AddSkill("Endurance_Weathered");
            creature.AddSkill("Endurance_Calloused");
            creature.AddSkill("Tactics");
            creature.AddSkill("Tactics_Charge");
            creature.AddSkill("Cudgel");
            creature.AddSkill("Cudgel_Expertise");
            creature.AddSkill("Cudgel_Bludgeon");
            creature.AddSkill("Cudgel_Slam");
            creature.AddSkill("Cudgel_ChargingStrike");
            creature.AddSkill("Cudgel_Backswing");
            creature.AddSkill("Cudgel_Conk");
            creature.AddSkill("Cudgel_SmashUp");
            creature.AddSkill("SingleWeaponFighting");
            creature.AddSkill("SingleWeaponFighting_OpportuneAttacks");
            creature.AddSkill("SingleWeaponFighting_WeaponExpertise");
            creature.AddSkill("SingleWeaponFighting_PenetratingStrikes");

            SecretRevealer secretRevealer = creature.RequirePart<SecretRevealer>();
            secretRevealer.id = SECRET_GIANTID;
            secretRevealer.text = SECRET_GIANTLOCATION_TEXT;
            secretRevealer.message = $"You have discovered {secretRevealer.text}!";
            secretRevealer.category = SECRET_GIANTLOCATION_CATEGORY;
            secretRevealer.adjectives = string.Join(",", new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });

            creature.GiveProperName(giantName);
            creature = HeroMaker.MakeHero(creature, "HNPS_SpecialHeroTemplate_SecretGiant", 8, "Giant");

            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));

            zoneManager.SetZoneName(SecretZoneId, SECRET_GIANTLOCATION_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneId, false);
            zoneManager.SetZoneProperty(SecretZoneId, "NoBiomes", "Yes");
        }

        [WishCommand(Command = "Secret Giant")]
        [WishCommand(Command = "secret giant")]
        [WishCommand(Command = "SecretGiant")]
        [WishCommand(Command = "secretgiant")]
        [WishCommand(Command = "go2giant")]
        public static void GoToGiant()
        {
            Wishing.HandleWish(The.Player, "goto:" + SecretMapNote.ZoneID);
        }
    } //!-- public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
}
