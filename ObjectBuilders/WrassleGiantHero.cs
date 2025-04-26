using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Qud.API;
using XRL.Names;
using XRL.World.ObjectBuilders;
using XRL.World.Parts.Mutation;
using XRL.World.Parts;
using XRL.Language;
using XRL.Rules;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Debug;

namespace XRL.World.ObjectBuilders
{
    public class WrassleGiantHero : IObjectBuilder
    {
        public static List<string> FactionAdmirationBag
        {
            get
            {
                List<string> bag = new(GNT_ADMIREREASON_BOOK.BookPagesAsList());
                bag.AddRange(SCRT_GNT_GNT_ADMIREREASON_BOOK.BookPagesAsList());
                return bag;
            }
        }
        public static List<string> ThiccBoisBag => GNT_THICCBOI_BOOK.BookPagesAsList();
        public static List<string> NoHateFactionsList => GNT_NOHATEFACTION_BOOK.BookPagesAsList();
        // These are the skills the highest tier GiantSlayer has
        public static List<string> HeroSkills => new()
        {
            "Acrobatics",
            "Acrobatics_Jump",
            "Endurance",
            "Endurance_ShakeItOff",
            "Tactics",
            "Tactics_Charge",
            "Cudgel",
            "Cudgel_Expertise",
            "Cudgel_Bludgeon",
            "Cudgel_Slam",
            "Cudgel_ChargingStrike",
            "SingleWeaponFighting",
            "SingleWeaponFighting_OpportuneAttacks",
        };
        public static List<string> UniqueHeroSkills => new()
        {
            // "Acrobatics",
            // "Acrobatics_Jump",
            // "Endurance",
            // "Endurance_ShakeItOff",
            "Endurance_Weathered",
            "Endurance_Calloused",
            // "Tactics",
            // "Tactics_Charge",
            // "Cudgel",
            // "Cudgel_Expertise",
            // "Cudgel_Bludgeon",
            // "Cudgel_Slam",
            // "Cudgel_ChargingStrike",
            "Cudgel_Backswing",
            "Cudgel_Conk",
            "Cudgel_SmashUp",
            // "SingleWeaponFighting",
            // "SingleWeaponFighting_OpportuneAttacks",
            "SingleWeaponFighting_WeaponExpertise",
            "SingleWeaponFighting_PenetratingStrikes",
        };

        public string Context;

        public WrassleGiantHero()
        {
            Context = "Hero";
        }

        public override void Apply(GameObject Creature, string Context)
        {
            Context ??= this.Context;

            bool Unique = Context == "Unique";

            if (!Creature.TryGetPart(out Wrassler wrassler))
            {
                wrassler = Creature.RequirePart<Wrassler>();
            }

            List<string> noHateFactionsList = new(NoHateFactionsList);
            if (Creature.TryGetStringProperty("NoHateFactions", out string existingNoHateFactions))
            {
                if (existingNoHateFactions.TrySplitToList(",", out List<string> existingNoHateFactionsList))
                {
                    foreach (string existingNoHateFaction in existingNoHateFactionsList)
                    {
                        if (!noHateFactionsList.Contains(existingNoHateFaction))
                            noHateFactionsList.Add(existingNoHateFaction);
                    }
                }
                else
                {
                    if (!noHateFactionsList.Contains(existingNoHateFactions))
                        noHateFactionsList.Add(existingNoHateFactions);
                }
            }
            Creature.SetStringProperty("NoHateFactions", noHateFactionsList.Join(","));

            List<string> factionAdmiration = new(FactionAdmirationBag);
            string factionAdmiration1 = factionAdmiration.DrawRandomElement();
            // Get a bag of different height/size/weight related reasons, draw a random one each
            Creature.SetStringProperty("staticFaction1", $",friend,{factionAdmiration1}");
            Debug.Entry(4, "staticFaction1", $",friend,{factionAdmiration1}", Indent: 0);

            string staticFaction2Faction = string.Empty;
            string staticFaction2Admiration = factionAdmiration.DrawRandomElement();
            if (Unique || wrassler.WrassleID.SeededRandomBool())
            {
                staticFaction2Faction = ThiccBoisBag.GetRandomElement();
                staticFaction2Admiration = GNT_THICCBOI_ADMIREREASON;
            }
            Creature.SetStringProperty("staticFaction2", $"{staticFaction2Faction},friend,{staticFaction2Admiration}");
            Debug.Entry(4, "staticFaction2", $"{staticFaction2Faction},friend,{staticFaction2Admiration}", Indent: 0);

            if (Unique)
            {
                Creature.SetStringProperty("staticFaction3", SCRT_GNT_UNQ_TEMPLAR_HATEREASON);
                Debug.Entry(4, "staticFaction3", SCRT_GNT_UNQ_TEMPLAR_HATEREASON, Indent: 0);
            }

            if (Creature.TryGetPart(out GameUnique gameUnique))
            {
                Creature.RemovePart(gameUnique);
            }

            if (Unique)
            {
                Creature.SetStringProperty("SharesRecipe", SCRT_GNT_RECIPE);
                Creature.SetStringProperty("SharesRecipeWithTrueKin", "false");

                gameUnique = Creature.RequirePart<GameUnique>();
                gameUnique.State = SCRT_GNT_UNQ_STATE;

                SecretRevealer secretRevealer = Creature.RequirePart<SecretRevealer>();
                secretRevealer.id = SCRT_GNT_SCRT_ID;
                secretRevealer.text = SCRT_GNT_LCTN_TEXT;
                secretRevealer.message = $"You have discovered {secretRevealer.text}!";
                secretRevealer.category = SCRT_GNT_LCTN_CATEGORY;
                secretRevealer.adjectives = string.Join(",",
                    new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" });
            }

            Creature.Brain.Mobile = true;
            Creature.Brain.Wanders = true;
            Creature.Brain.WandersRandomly = true;
            Creature.Brain.Factions = "";
            Creature.Brain.Allegiance.Clear();
            Creature.Brain.Allegiance.Add("WrassleGiants", 800);
            Creature.Brain.Allegiance.Add("Giants", 600);

            int MentalMutations = 0;
            int PhysicalMutations = 0;
            bool MakeChimera = false;
            string Epithet = NameMaker.MakeEpithet(
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

            string CreatureName = NameMaker.MakeName(
                For: Creature,
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

            Creature.GiveProperName(
                Name: CreatureName, 
                Force: true, 
                Special: Context, 
                SpecialFaildown: true, 
                HasHonorific: null, 
                HasEpithet: null, 
                NamingContext: null);

            int Stews = Stat.Roll("4d4");

            Creature.RequirePart<DisplayNameColor>().SetColorByPriority("yuge", 40);

            if (!Epithet.IsNullOrEmpty() && !Unique)
            {
                Creature.RequirePart<Epithets>().Primary = GameText.VariableReplace(Epithet);

                if (Epithet.IsGivingStewful())
                    Stews += Stat.Roll("2d4");

                if (Epithet.IsGivingStewless())
                    Stews -= Stat.Roll("1d4");

                if (Epithet.IsGivingThoughtful())
                {
                    Creature.AddBaseStat("Intelligence", Stat.Roll("1d4"));
                    MentalMutations += Stat.Roll("1d2");
                    PhysicalMutations -= Stat.Roll("1d2");
                }

                if (Epithet.IsGivingTough())
                {
                    Creature.AddBaseStat("Toughness", Stat.Roll("1d4"));
                    PhysicalMutations += Stat.Roll("1d2");
                }

                if (Epithet.IsGivingStrong())
                {
                    Creature.AddBaseStat("Strength", Stat.Roll("1d4"));
                    PhysicalMutations += Stat.Roll("1d2");
                }

                if (Epithet.IsGivingResilient())
                {
                    Creature.AddBaseStat("Willpower", Stat.Roll("1d4"));
                    MentalMutations += Stat.Roll("1d2");
                }

                if (Epithet.IsGivingTrulyImmense())
                {
                    Creature.MultiplyStat("Hitpoints", 2);
                    if (Stat.Roll("1d3") == 3)
                    {
                        MakeChimera = true;
                        PhysicalMutations += 1;
                    }
                    PhysicalMutations += Stat.Roll("1d2");
                }

                if (Epithet.IsGivingWrassler())
                    Creature.ReceiveObject("Gigantic FoldingChair");

                if (!Epithet.IsGivingPopular())
                {
                    if (Creature.TryGetPart(out Leader leader)) Creature.RemovePart(leader);
                    if (Creature.TryGetPart(out Followers followers)) Creature.RemovePart(followers);
                    if (Creature.TryGetPart(out DromadCaravan dromadCaravan)) Creature.RemovePart(dromadCaravan);
                    if (Creature.TryGetPart(out HasGuards hasGuards)) Creature.RemovePart(hasGuards);
                    if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1)) Creature.RemovePart(snapjawPack1);
                    if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack)) Creature.RemovePart(baboonHero1Pack);
                    if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1)) Creature.RemovePart(eyelessKingCrabSkuttle1);
                    if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1)) Creature.RemovePart(goatfolkClan1);
                }
            }
            else if (Unique)
            {
                Stews += Stat.Roll("2d4");
                Creature.AddBaseStat("Strength", Stat.Roll("3d4"));
                Creature.AddBaseStat("Agility", Stat.Roll("2d4"));
                Creature.AddBaseStat("Toughness", Stat.Roll("3d3"));
                Creature.AddBaseStat("Intelligence", Stat.Roll("2d3"));
                Creature.AddBaseStat("Willpower", Stat.Roll("3d4"));
                Creature.AddBaseStat("Ego", Stat.Roll("2d3"));
                if (Stat.Roll("1d3") == 3)
                {
                    MakeChimera = true;
                    PhysicalMutations += 1;
                }
                PhysicalMutations += Stat.Roll("2d2");
                MentalMutations += Stat.Roll("1d3");
                if (Creature.TryGetPart(out Leader leader)) Creature.RemovePart(leader);
                if (Creature.TryGetPart(out Followers followers)) Creature.RemovePart(followers);
                if (Creature.TryGetPart(out DromadCaravan dromadCaravan)) Creature.RemovePart(dromadCaravan);
                if (Creature.TryGetPart(out HasGuards hasGuards)) Creature.RemovePart(hasGuards);
                if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1)) Creature.RemovePart(snapjawPack1);
                if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack)) Creature.RemovePart(baboonHero1Pack);
                if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1)) Creature.RemovePart(eyelessKingCrabSkuttle1);
                if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1)) Creature.RemovePart(goatfolkClan1);

                Creature.MultiplyStat("Hitpoints", 3);
                Creature.SetStringProperty("SharesRecipe", SCRT_GNT_RECIPE);
                Creature.SetStringProperty("SharesRecipeWithTrueKin", "false");

                if (!Creature.TryGetPart(out HasMakersMark hasMakersMark))
                {
                    hasMakersMark = Creature.RequirePart<HasMakersMark>();
                }
                hasMakersMark.Mark = $"\u00A3";
                hasMakersMark.Color = $"Z";
                MakersMark.RecordUsage(hasMakersMark.Mark);
            }

            int level = Unique ? 35 : 20;
            Creature.GetStat("Level").BaseValue = level + Stat.Roll("3d3");

            int extraXP = Stat.Roll("1d18") * Stat.Roll("18d18");
            Creature.GetStat("XP").BaseValue = Leveler.GetXPForLevel(Creature.GetStat("Level").Value) + extraXP;

            Creature.AddBaseStat("XPValue", Stews * 75);

            if (Unique) Creature.MultiplyStat("XPValue", 2);

            for (int i = 0; i < Stews; i++)
            {
                Creature.AddBaseStat("Hitpoints", Stat.Roll(Unique ? "2d8" : "1d10"));
            }

            int GigantismLevel = (int)Math.Floor(Creature.Level / 3.0);
            Gigantified.GigantifyMutant(Creature, GigantismLevel, Stews, null, Context);

            Mutations mutations = Creature.RequirePart<Mutations>();

            if (MakeChimera)
            {
                BaseMutation Chimera = MutationFactory.GetMutationEntryByName("Chimera")?.CreateInstance();
                if (Chimera != null)
                {
                    mutations.AddMutation(Chimera);
                    PhysicalMutations += Math.Max(0, (int)Math.Floor(MentalMutations / 2.0));
                    MentalMutations = 0;
                }
            }

            int MentalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(MentalMutations / 2.0));
            string MentalMutationLevelDie = $"1d{MentalMutationLevelHigh}";
            for (int i = 0; i < MentalMutations; i++)
            {
                BaseMutation randomMentalMutation;
                do
                {
                    randomMentalMutation = MutationFactory.GetRandomMutation("Mental");
                }
                while (randomMentalMutation != null && mutations.HasMutation(randomMentalMutation));
                if (randomMentalMutation != null)
                {
                    mutations.AddMutation(randomMentalMutation, Stat.Roll(MentalMutationLevelDie));
                }
            }

            int PhysicalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(PhysicalMutations / 2.0));
            string PhysicalMutationLevelDie = $"1d{PhysicalMutationLevelHigh}";
            for (int i = 0; i < PhysicalMutations; i++)
            {
                BaseMutation randomPhysicalMutation;
                do
                {
                    randomPhysicalMutation = MutationFactory.GetRandomMutation("Physical");
                }
                while (randomPhysicalMutation != null && mutations.HasMutation(randomPhysicalMutation));
                if (randomPhysicalMutation != null)
                {
                    mutations.AddMutation(randomPhysicalMutation, Stat.Roll(PhysicalMutationLevelDie));
                    if (mutations.HasMutation("Chimera") && "1d7".RollCached() > 4) mutations.AddChimericBodyPart();
                }
            }

            Creature.RequirePart<Calming>();
            if (!Creature.TryGetPart(out GivesRep givesRep))
            {
                givesRep = Creature.RequirePart<GivesRep>();
            }
            givesRep.repValue = Unique ? 400 : 200;

            // if (Creature.TryGetPart(out HasMakersMark hasMakersMark)) Creature.RemovePart(hasMakersMark);
            if (Creature.TryGetPart(out GreaterVoider greaterVoider)) Creature.RemovePart(greaterVoider);
            if (Creature.TryGetPart(out Rummager rummager)) Creature.RemovePart(rummager);
            if (Creature.TryGetPart(out Breeder breeder)) Creature.RemovePart(breeder);

            if (Creature.TryGetPart(out DisplayNameAdjectives displayNameAdjectives))
            {
                displayNameAdjectives.RemoveAdjective(Gigantified.GetNamePrefix());
            }

            if (Unique)
            {
                if (!Creature.TryGetPart(out ConversationScript conversationScript))
                {
                    conversationScript = Creature.RequirePart<ConversationScript>();
                }
                conversationScript.ConversationID = SCRT_GNT_UNQ_CONVSCRPT_ID;
            }

            Creature.AddSkills(HeroSkills);
            if (Unique) 
                Creature.AddSkills(UniqueHeroSkills);

            Creature.RequirePart<Interesting>();

            if (!Creature.TryGetPart(out Description description))
            {
                description = Creature.RequirePart<Description>();
            }
            string creatureNoun = $"{Creature.GetBlueprint().DisplayName()}".Color("y");
            string creatureArticle = Grammar.IndefiniteArticle(creatureNoun);
            creatureArticle = Unique ? creatureArticle.Capitalize() : creatureArticle;
            string preDesc = Unique ? SCRT_GNT_UNQ_PREDESC : GNT_PREDESC;
            preDesc = preDesc.Replace(GNT_PREDESC_RPLC, $"{creatureArticle} {creatureNoun}");
            description.Short = preDesc + description._Short;
        }

        public static GameObjectBlueprint GetGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null, bool Old = false, bool Unique = false)
        {
            GameObjectBlueprint creatureObjectBlueprint =
                EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => IsWrassleGiantEligible(blueprint, filter, Old, Unique));
            return creatureObjectBlueprint;
        }
        public static GameObjectBlueprint GetOldGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetGiantEligibleBlueprint(filter, Old: true);
        }
        public static GameObjectBlueprint GetUniqueGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetGiantEligibleBlueprint(filter, Unique: true);
        }
        public static GameObjectBlueprint PrepareGiantBlueprint(GameObjectBlueprint CreatureBlueprint)
        {
            GamePartBlueprint WrassleGiantHeroPartBlueprint = new("XRL.World.ObjectBuilders", nameof(WrassleGiantHero))
            {
                Name = nameof(WrassleGiantHero),
                ChanceOneIn = 1,
            };
            CreatureBlueprint.Builders[WrassleGiantHeroPartBlueprint.Name] = WrassleGiantHeroPartBlueprint;

            CreatureBlueprint.Tags["Culture"] = "Giant";

            if (!CreatureBlueprint.Tags.ContainsKey("Role")) CreatureBlueprint.Tags.Add("Role", "");
            CreatureBlueprint.Tags["Role"] = $"Hero";

            if (CreatureBlueprint.Tags.ContainsKey("NoHateFactions")) CreatureBlueprint.Tags.Remove("NoHateFactions");
            if (CreatureBlueprint.Tags.ContainsKey("staticFaction1")) CreatureBlueprint.Tags.Remove("staticFaction1");
            if (CreatureBlueprint.Tags.ContainsKey("staticFaction2")) CreatureBlueprint.Tags.Remove("staticFaction2");
            if (CreatureBlueprint.Tags.ContainsKey("staticFaction3")) CreatureBlueprint.Tags.Remove("staticFaction3");

            if (!CreatureBlueprint.Tags.ContainsKey("SharesRecipe")) CreatureBlueprint.Tags.Add("SharesRecipe", "");

            // Could possibly look at gigantifying them instead...
            /*
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
            */

            return CreatureBlueprint;
        }
        public static GameObjectBlueprint PrepareUniqueGiantBlueprint(GameObjectBlueprint CreatureBlueprint)
        {
            PrepareGiantBlueprint(CreatureBlueprint);

            CreatureBlueprint.Tags["Culture"] = "WrassleGiant";

            if (!CreatureBlueprint.Tags.ContainsKey("Role")) CreatureBlueprint.Tags.Add("Role", "");
            CreatureBlueprint.Tags["Role"] = $"Leader";

            return CreatureBlueprint;
        }
        public static GameObjectBlueprint GetAGiantHeroBluePrintModel(Predicate<GameObjectBlueprint> filter = null, bool Old = true)
        {
            GameObjectBlueprint creatureBlueprint = GetGiantEligibleBlueprint(filter, Old);
            return PrepareGiantBlueprint(creatureBlueprint);
        }
        public static GameObjectBlueprint GetAUniqueGiantHeroBluePrintModel(Predicate<GameObjectBlueprint> filter = null)
        {
            GameObjectBlueprint creatureBlueprint = GetUniqueGiantEligibleBlueprint(filter);
            return PrepareUniqueGiantBlueprint(creatureBlueprint);
        }

        public static bool IsWrassleGiantEligible(GameObjectBlueprint Blueprint, Predicate<GameObjectBlueprint> filter = null, bool Old = false, bool Unique = false)
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

            bool mustBeOld = Unique || Old;
            if (mustBeOld && !oldFactions.Contains(Blueprint.GetPrimaryFaction()))
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

            if (filter != null && !filter(Blueprint))
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
    }
}
