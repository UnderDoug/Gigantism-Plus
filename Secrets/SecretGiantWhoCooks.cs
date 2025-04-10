using Genkit;
using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;
using static XRL.World.ZoneBuilderPriority;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using Qud.API;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [JoppaWorldBuilderExtension]
    public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
    {
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Debug.Entry(4,
                $"\u2666 {typeof(SecretGiantWhoCooksBuilderExtension).Name}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0);

            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: false);
            string zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            if (JournalAPI.GetMapNote(SECRET_GIANTRECIPE).Is(null))
            {
                JournalAPI.AddMapNote(
                    ZoneID: zoneID,
                    text: SECRET_GIANTLOCATION_TEXT,
                    category: SECRET_GIANTLOCATION_CATEGORY,
                    attributes: new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" },
                    secretId: SECRET_GIANTRECIPE
                );
            }
            JournalMapNote note = JournalAPI.GetMapNote(SECRET_GIANTRECIPE);
            note.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            zoneManager.AddZoneBuilder(zoneID, NORMAL, nameof(Connecter));
            zoneManager.AddZoneBuilder(zoneID, MID, nameof(RoadBuilder));
            zoneManager.AddZoneBuilder(zoneID, LATE, nameof(OverlandRuins));
            zoneManager.AddZoneBuilder(zoneID, LATE + ADJUST_MEDIUM, nameof(Connecter));

            zoneManager.RemoveZoneBuilders(zoneID, nameof(PopTableZoneBuilder));
            zoneManager.RemoveZoneBuilders(zoneID, nameof(Population));
            zoneManager.RemoveZoneBuilders(zoneID, nameof(FactionEncounters));

            GameObjectBlueprint gameObjectBlueprint = EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => EncountersAPI.IsEligibleForDynamicEncounters(blueprint)
                && (blueprint.HasPart("Body") || blueprint.HasTagOrProperty("BodySubstitute"))
                && (blueprint.HasPart("Combat") || blueprint.HasTagOrProperty("BodySubstitute"))
                && !blueprint.DescendsFrom("BaseTrueKin")
                && !blueprint.HasPart("MentalShield"));

            GameObject creature;
            GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
            {
                Name = nameof(Gigantified),
                ChanceOneIn = 1,
            };
            gameObjectBlueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;

            gameObjectBlueprint.Tags["Culture"] = "Giant";

            string noHateFactions = "Wardens,Dromad";
            if (gameObjectBlueprint.Tags.ContainsKey("NoHateFactions"))
            {
                noHateFactions += $",{gameObjectBlueprint.Tags["NoHateFactions"]}";
            }
            gameObjectBlueprint.Tags["NoHateFactions"] = $"{noHateFactions}";

            if (!gameObjectBlueprint.Tags.ContainsKey("staticFaction1")) gameObjectBlueprint.Tags.Add("staticFaction1", "");
            if (!gameObjectBlueprint.Tags.ContainsKey("staticFaction2")) gameObjectBlueprint.Tags.Add("staticFaction2", "");
            if (!gameObjectBlueprint.Tags.ContainsKey("staticFaction3")) gameObjectBlueprint.Tags.Add("staticFaction3", "");

            // Get a bag of different height/size/weight related reasons, draw a random one each
            gameObjectBlueprint.Tags["staticFaction1"] = $",friend,cooking a seriously thick stew";
            gameObjectBlueprint.Tags["staticFaction2"] = $",friend,teaching them patience";

            // Keep this one static
            gameObjectBlueprint.Tags["staticFaction3"] = $"GiantTemplar,hate,suplexing their warleader in the ring";

            // Need to remove the tag/part that gives merchants guards and seekers thralls.
            // Although True Kin as a genotype are excluded, we'll remove slaves, too.
            // Could possibly look at gigantifying them instead...

            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: gameObjectBlueprint,
                    BonusModChance: 0,
                    SetModNumber: 0,
                    AutoMod: null,
                    BeforeObjectCreated: null,
                    AfterObjectCreated: null,
                    Context: null,
                    ProvideInventory: null);

            if (creature.TryGetPart(out GigantismPlus gigantism))
            {
                int starting = gigantism.Level + 3;
                if (starting > 10)
                {
                    starting -= 10;
                }
                gigantism.Level = 10;
                gigantism.SetRapidLevelAmount((int)Math.Ceiling(starting / 3.0));
            }
            creature.Brain.Mobile = true;
            creature.Brain.Factions = "";
            creature.Brain.Allegiance.Clear();
            creature.Brain.Allegiance.Hostile = false;
            creature.Brain.Allegiance.Add("Giants", 700);
            creature.RequirePart<Interesting>();
            if (creature.TryGetPart(out Description description))
            {
                string preDesc = SECRET_GIANTPREDESC.Replace(SECRET_GIANTPREDESC_REPLACE, creature.Blueprint);
                description.Short = preDesc + description.Short;
            }

            /* 
            if (!creature.TryGetPart(out ConversationScript conversationScript))
            {
                conversationScript = creature.RequirePart<ConversationScript>();
            }
            conversationScript.ConversationID = SECRET_GIANTCONVSCRIPT_ID;
            */

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
            secretRevealer.id = SECRET_GIANTRECIPE;
            secretRevealer.text = SECRET_GIANTLOCATION_TEXT;
            secretRevealer.message = $"You have discovered {secretRevealer.text}!";
            secretRevealer.category = SECRET_GIANTLOCATION_CATEGORY;
            secretRevealer.adjectives = string.Join(",", new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });

            creature = HeroMaker.MakeHero(creature, "HNPS_SpecialHeroTemplate_SecretGiant", 8, "Giant");

            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));

            zoneManager.SetZoneName(zoneID, SECRET_GIANTLOCATION_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }
    }
}
