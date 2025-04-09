using Genkit;
using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;
using XRL.World.ObjectBuilders;
using Qud.API;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus
{
    [JoppaWorldBuilderExtension]
    public class SecretGiantWhoCooksBuilderExtension : IJoppaWorldBuilderExtension
    {
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: false);
            string zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            if (JournalAPI.GetMapNote(SECRET_GIANTRECIPE).Is(null))
            {
                JournalAPI.AddMapNote(
                    ZoneID: zoneID,
                    text: "the location of the giant who knows how to cook",
                    category: "Oddities",
                    attributes: new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" },
                    secretId: SECRET_GIANTRECIPE
                );
            }
            JournalMapNote note = JournalAPI.GetMapNote(SECRET_GIANTRECIPE);
            note.Weight = 25000;

            ZoneManager zoneManager = The.ZoneManager;

            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.NORMAL, nameof(Connecter));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.MID, nameof(RoadBuilder));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(OverlandRuins));

            // zoneManager.RemoveZoneBuilders(zoneID, nameof(PopTableZoneBuilder));
            // zoneManager.RemoveZoneBuilders(zoneID, nameof(Population));
            // zoneManager.RemoveZoneBuilders(zoneID, nameof(FactionEncounters));

            GameObjectBlueprint gameObjectBlueprint = EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => EncountersAPI.IsEligibleForDynamicEncounters(blueprint)
                && (blueprint.HasPart("Body") || blueprint.HasTagOrProperty("BodySubstitute"))
                && (blueprint.HasPart("Combat") || blueprint.HasTagOrProperty("BodySubstitute"))
                && !blueprint.DescendsFrom("BaseTrueKin")
                && !blueprint.HasPart("MentalShield")
                && !blueprint.HasTag("Merchant"));

            GameObject creature;
            GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
            {
                Name = nameof(Gigantified),
                ChanceOneIn = 1,
            };
            gameObjectBlueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;

            gameObjectBlueprint.Tags["Culture"] = "Giant";

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
                int starting = gigantism.Level;
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
            creature.Brain.Allegiance.Calm = true;
            creature.RequirePart<Interesting>();

            SecretRevealer secretRevealer = creature.RequirePart<SecretRevealer>();
            secretRevealer.id = SECRET_GIANTRECIPE;
            secretRevealer.text = "the location of the giant who knows how to cook";
            secretRevealer.message = $"You have discovered {secretRevealer.text}!";
            secretRevealer.category = "Oddities";
            secretRevealer.adjectives = string.Join(",", new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });

            creature = HeroMaker.MakeHero(creature, "HNPS_SpecialHeroTemplate_SecretGiant", 8, "Giant");

            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));

            zoneManager.SetZoneName(zoneID, "location of the giant who knows how to cook", Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }
    }
}
