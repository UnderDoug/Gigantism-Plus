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
        public static string UniqueEpithet => $"who knows how to {"Cook".OptionalColorYuge(Colorfulness)}";
        public static List<string> EpithetsBag = new()
        {
            $"the {"Giant".OptionalColorYuge(Colorfulness)}",
            $"the {"Thicc".OptionalColorYuge(Colorfulness)}",
            $"the {"Swole".OptionalColorYuge(Colorfulness)}",
            $"of the {"Gains".OptionalColorYuge(Colorfulness)}",
            $"of the {"Stew".OptionalColorYuge(Colorfulness)}",
            $"who is {"Immense".OptionalColorYuge(Colorfulness)}",
            $"who {"Hankers".OptionalColorYuge(Colorfulness)}",
            $"{"Hanker's Bane".OptionalColorYuge(Colorfulness)}",
        };
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
                $"\u2666 {typeof(SecretGiantWhoCooksBuilderExtension).Name}." +
                $"{nameof(OnAfterBuild)}(JoppaWorldBuilder builder)",
                Indent: 0);

            Location2D location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: true);
            SecretZoneId = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            if (JournalAPI.GetMapNote(SCRT_GNT_SCRT_ID).Is(null))
            {
                JournalAPI.AddMapNote(
                    ZoneID: SecretZoneId,
                    text: SCRT_GNT_LCTN_TEXT,
                    category: SCRT_GNT_LCTN_CATEGORY,
                    attributes: new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" },
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

            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(MapBuilder), "FileName", $"{ThisMod.Path}/Secrets/Maps/{MapFileName}");
            zoneManager.AddZonePostBuilder(SecretZoneId, "Music", "Track", "Music/Barathrums Study");

            zoneManager.AddZonePostBuilder(SecretZoneId, nameof(IsCheckpoint), "Key", SecretZoneId);

            zoneManager.SetZoneProperty(SecretZoneId, "SkipTerrainBuilders", true);
            zoneManager.SetZoneProperty(SecretZoneId, "NoBiomes", "Yes");

            zoneManager.SetZoneName(SecretZoneId, SCRT_GNT_LCTN_TEXT, Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(SecretZoneId, false);

            if (MapFileName == SCRT_GNT_ZONE_MAP2_CENTRE)
                zoneManager.AddZonePostBuilder(ZoneID: SecretZoneId, Builder: nameof(GiantAbodePopulator));

            GameObject Giant = GetTheGiant();

            string wrasslerColor = null;
            if (Giant.TryGetPart(out Wrassler wrassler)) wrasslerColor = wrassler.DetailColor;

            zoneManager.AddZonePostBuilder(
                ZoneID: SecretZoneId, 
                Class: nameof(AddObjectBuilder), 
                Key1: "Object", 
                Value1: zoneManager.CacheObject(Giant));

            string wrassleRingColor = wrasslerColor ?? WrassleRingColors.GetRandomElement();
            foreach (GameObject rope in zoneManager.GetZone(SecretZoneId).GetObjectsThatInheritFrom("WrassleRingRopes"))
            {
                rope.Render.DetailColor = wrassleRingColor;
            }
        } //!-- public override void OnAfterBuild(JoppaWorldBuilder builder)

        public static GameObject GetTheGiant()
        {
            return GetAGiant(Unique: true);
        }
        public static GameObject GetAGiant(bool Unique = false)
        {
            string giantName = NameStyles.Generate(Culture: "Giant");

            if (Unique)
            {
                Unique = !The.Game.HasStringGameState(SCRT_GNT_UNQ_STATE);
            }

            GameObjectBlueprint creatureObjectBlueprint = 
                EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => IsGiantCookEligible(blueprint, true));

            GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
            {
                Name = nameof(Gigantified),
                ChanceOneIn = 1,
            };
            creatureObjectBlueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;

            creatureObjectBlueprint.Tags["Culture"] = "Giant";

            if (Unique)
            {
                if (!creatureObjectBlueprint.Tags.ContainsKey("Role")) creatureObjectBlueprint.Tags.Add("Role", "");
                creatureObjectBlueprint.Tags["Role"] = $"Leader";
            }

            if (creatureObjectBlueprint.Tags.ContainsKey("NoHateFactions")) creatureObjectBlueprint.Tags.Remove("NoHateFactions");
            if (creatureObjectBlueprint.Tags.ContainsKey("staticFaction1")) creatureObjectBlueprint.Tags.Remove("staticFaction1");
            if (creatureObjectBlueprint.Tags.ContainsKey("staticFaction2")) creatureObjectBlueprint.Tags.Remove("staticFaction2");
            if (creatureObjectBlueprint.Tags.ContainsKey("staticFaction3")) creatureObjectBlueprint.Tags.Remove("staticFaction3");

            if (!creatureObjectBlueprint.Tags.ContainsKey("SharesRecipe")) creatureObjectBlueprint.Tags.Add("SharesRecipe", "");

            // Could possibly look at gigantifying them instead...
            creatureObjectBlueprint.Parts.RemoveAll((KeyValuePair<string, GamePartBlueprint> entry)
                => entry.Key == typeof(DromadCaravan).Name
                || entry.Key == typeof(EyelessKingCrabSkuttle1).Name
                || entry.Key == typeof(SnapjawPack1).Name
                || entry.Key == typeof(BaboonHero1Pack).Name
                || entry.Key == typeof(GoatfolkClan1).Name
                || entry.Key == typeof(HasGuards).Name
                || entry.Key == typeof(HasThralls).Name
                || entry.Key == typeof(HasSlaves).Name
                || entry.Key == typeof(Leader).Name
                || entry.Key == typeof(Followers).Name
                || entry.Key == typeof(Breeder).Name
                || entry.Key == typeof(GreaterVoider).Name
                || entry.Key == typeof(Rummager).Name);

            GameObject creature;
            creature = GameObjectFactory.Factory.CreateObject(
                    Blueprint: creatureObjectBlueprint,
                    BonusModChance: 0,
                    SetModNumber: 0,
                    AutoMod: null,
                    BeforeObjectCreated: null,
                    AfterObjectCreated: null,
                    Context: null,
                    ProvideInventory: null);

            creature.SetStringProperty("SharesRecipe", SCRT_GNT_RECIPE);
            creature.SetStringProperty("SharesRecipeWithTrueKin", "false");
            // creature.SetStringProperty("SharesRecipeText", "");

            if (creature.TryGetPart(out GameUnique gameUnique))
            {
                creature.RemovePart(gameUnique);
            }
            if (Unique)
            {
                gameUnique = creature.RequirePart<GameUnique>();
                gameUnique.State = SCRT_GNT_UNQ_STATE;
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
                gigantism.Level = Unique ? 10 : ExplodingDie(5, new DieRoll("1d3"), Limit: 10);
                gigantism.SetRapidLevelAmount(starting.RapidAdvancementCeiling(Min: 3));
            }
            if (!creature.TryGetPart(out StewBelly stewBelly))
            {
                stewBelly = creature.RequirePart<StewBelly>();
            }
            string dieRoll = Unique ? "4d4" : "2d4";
            int startingStews = dieRoll.Roll();
            if (Unique) creature.SetIntProperty(GNT_START_STEWS_PROPLABEL, startingStews);
            else stewBelly.Stews += startingStews;

            creature.Brain.Mobile = true;
            creature.Brain.Wanders = true;
            creature.Brain.WandersRandomly = true;
            creature.Brain.Factions = "WrassleGiants";
            creature.Brain.Allegiance.Clear();
            creature.Brain.Allegiance.Add("WrassleGiants", 800);
            creature.Brain.Allegiance.Add("Giants", 600);
            creature.RequirePart<Interesting>();

            if (!creature.TryGetPart(out Description description))
            {
                description = creature.RequirePart<Description>();
            }
            string creatureNoun = $"{creature.Render?.DisplayName ?? creatureObjectBlueprint.DisplayName()}".Color("y");
            string creatureArticle = Grammar.IndefiniteArticle(creatureNoun);
            creatureArticle = Unique ? creatureArticle.Capitalize() : creatureArticle;
            string preDesc = Unique ? SCRT_GNT_UNQ_PREDESC : GNT_PREDESC;
            preDesc = preDesc.Replace(GNT_PREDESC_RPLC, $"{creatureArticle} {creatureNoun}");
            description.Short = preDesc + description._Short;

            Epithets epithets = creature.RequirePart<Epithets>();
            epithets.Primary = Unique ? UniqueEpithet.Color("y") : EpithetsBag.GetRandomElement().Color("y");

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
                conversationScript.ConversationID = SCRT_GNT_UNQ_CONVSCRPT_ID;
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
                secretRevealer.id = SCRT_GNT_SCRT_ID;
                secretRevealer.text = SCRT_GNT_LCTN_TEXT;
                secretRevealer.message = $"You have discovered {secretRevealer.text}!";
                secretRevealer.category = SCRT_GNT_LCTN_CATEGORY;
                secretRevealer.adjectives = string.Join(",", 
                    new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });
            }

            creature.GiveProperName(giantName);
            string heroTemplate = Unique ? SCRT_GNT_HERO_TMPLT : GNT_HERO_TMPLT;
            int heroTier = Unique ? 8 : -1;
            creature = HeroMaker.MakeHero(creature, heroTemplate, heroTier, "Giant");

            if (!creature.TryGetPart(out Wrassler wrassler))
            {
                wrassler = creature.RequirePart<Wrassler>();
            }
            wrassler.BestowWrassleGear();

            if (creature.TryGetPart(out HasMakersMark hasMakersMark)) creature.RemovePart(hasMakersMark);

            if (creature.TryGetPart(out DromadCaravan dromadCaravan)) creature.RemovePart(dromadCaravan);
            if (creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1)) creature.RemovePart(eyelessKingCrabSkuttle1);
            if (creature.TryGetPart(out SnapjawPack1 snapjawPack1)) creature.RemovePart(snapjawPack1);
            if (creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack)) creature.RemovePart(baboonHero1Pack);
            if (creature.TryGetPart(out GoatfolkClan1 goatfolkClan1)) creature.RemovePart(goatfolkClan1);
            if (creature.TryGetPart(out HasGuards hasGuards)) creature.RemovePart(hasGuards);
            if (creature.TryGetPart(out HasThralls hasThralls)) creature.RemovePart(hasThralls);
            if (creature.TryGetPart(out HasSlaves hasSlaves)) creature.RemovePart(hasSlaves);
            if (creature.TryGetPart(out Leader leader)) creature.RemovePart(leader);
            if (creature.TryGetPart(out Followers followers)) creature.RemovePart(followers);
            if (creature.TryGetPart(out Breeder breeder)) creature.RemovePart(breeder);
            if (creature.TryGetPart(out GreaterVoider greaterVoider)) creature.RemovePart(greaterVoider);
            if (creature.TryGetPart(out Rummager rummager)) creature.RemovePart(rummager);

            return creature;
        }

        public static bool IsGiantCookEligible(GameObjectBlueprint Blueprint, bool Unique = false)
        {
            if (false && !EncountersAPI.IsLegendaryEligible(Blueprint)) 
                return false;

            if (!Blueprint.HasPart(nameof(Body)) && !Blueprint.HasTagOrProperty("BodySubstitute"))
                return false;

            if (!Blueprint.HasPart(nameof(Combat)) && !Blueprint.HasTagOrProperty("BodySubstitute"))
                return false;

            List<string> oldFactions =
                (from faction in Factions.Loop()
                 where faction.Old
                 select faction.Name).ToList();
            if (Unique && !oldFactions.Contains(Blueprint.GetPrimaryFaction()))
                return false;

            if (Blueprint.HasTag("BaseObject"))
                return false;

            if (Blueprint.Name.StartsWith("Base"))
                return false;

            if (false && Blueprint.HasTag("NoLibrarian"))
                return false;

            if (Blueprint.InheritsFrom("BaseTrueKin"))
                return false;

            if (Blueprint.InheritsFrom("BaseNest"))
                return false;

            if (Blueprint.InheritsFrom("BasePlant"))
                return false;

            if (Blueprint.InheritsFrom("MutatedPlant"))
                return false;

            if (Blueprint.InheritsFrom("BaseSlynth"))
                return false;

            if (Blueprint.InheritsFrom("LiquidLichen"))
                return false;

            if (Blueprint.InheritsFrom("FungusPuffer"))
                return false;

            if (Blueprint.InheritsFrom("BaseUrchin"))
                return false;

            if (Blueprint.InheritsFrom("BaseRobot"))
            {
                List<string> acceptableRobots = new()
                {
                    "Chrome Pyramid",
                    "Leering Stalker",
                    "Aleksh_MetalBird",
                };
                if (!acceptableRobots.Contains(Blueprint.Name))
                    return false;
            }

            if (Blueprint.InheritsFrom("BaseGyreWight"))
                return false;

            if (Blueprint.InheritsFrom("Gyre Wight Apotheote"))
                return false;

            if (Blueprint.TryGetTag("Species", out string species) && species.Is("mecha"))
                return false;

            if (Blueprint.TryGetTag("Role", out string roll) && roll.Is("Minion"))
                return false;

            if (Blueprint.HasTagOrProperty("StartInLiquid"))
                return false;

            if (Blueprint.Parts.ContainsKey(typeof(SpawnWithLiquid).Name))
                return false;

            if (Blueprint.Parts.ContainsKey(typeof(AISitting).Name))
                return false;

            if (Blueprint.Mutations.ContainsKey(nameof(Burrowing)))
                return false;

            if (Blueprint.Mutations.ContainsKey(nameof(WallWalker)))
                return false;

            if (Blueprint.HasProperName())
                return false;

            // from the playable snapjaws mod, kept popping up.
            if (Blueprint.Name.Is("PlayableSnapjaw") || Blueprint.InheritsFrom("PlayableSnapjaw")) 
                return false;

            // from the RogueRobots mod, kept popping up.
            if (Blueprint.Name.Is("PlayerBaseRobot") || Blueprint.InheritsFrom("PlayerBaseRobot")) 
                return false;

            // from the RogueRobots mod, kept popping up.
            if (Blueprint.Name.Is("TinkerBot") || Blueprint.InheritsFrom("TinkerBot")) 
                return false;

            // from the RogueRobots mod, just in case.
            if (Blueprint.Name.Is("Agooga") || Blueprint.InheritsFrom("Agooga")) 
                return false;

            // from the Gyre White Subtype mod, kept popping up.
            if (Blueprint.Name.Is("Andrea_GyreWight_WightBody") || Blueprint.InheritsFrom("Andrea_GyreWight_WightBody")) 
                return false;

            return true;
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
