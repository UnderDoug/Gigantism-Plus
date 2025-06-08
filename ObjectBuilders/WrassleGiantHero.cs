using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HistoryKit;
using Qud.API;

using XRL.Language;
using XRL.Names;
using XRL.Rules;
using XRL.UI;
using XRL.Wish;
using XRL.World.Capabilities;
using XRL.World.Loaders;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
    public class WrassleGiantHero : IObjectBuilder
    {
        private static bool doDebug => getClassDoDebug(nameof(WrassleGiantHero));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                '!',    // Warn
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
            /* 
             * Commented skills are the ones that appear in the above list 
             */
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
        public static string[] SecretAttributes = new[] { "giant", "humanoid", "settlement", "mountains", "recipe", "oddity" };

        public string Context;

        public WrassleGiantHero()
        {
            Context = "Hero";
        }

        public override void Apply(GameObject Creature, string Context = null)
        {
            Debug.Entry(4, 
                $"{nameof(WrassleGiantHero)}." +
                $"{nameof(Apply)}(" +
                $"GameObject Creature: {Creature?.DebugName ?? NULL}, " + 
                $"string Context: {Context.Quote()})",
                Indent: 0, Toggle: getDoDebug());

            Context ??= this.Context;

            bool Unique = Context == "Unique";
            if (Unique && The.Game.HasStringGameState(SCRT_GNT_UNQ_STATE) && false)
            {
                Debug.Warn(4, 
                    $"{nameof(WrassleGiantHero)}", 
                    $"{nameof(Apply)}", 
                    $"Attempted to create Unique {nameof(WrassleGiantHero)} while one already exists", 
                    Indent: Debug.LastIndent + 1);

                Context = "Hero";
                Unique = false;
            }

            Debug.LoopItem(4, $"Unique?", Good: Unique, Indent: 1, Toggle: getDoDebug());

            string nameSpecial = Unique ? "Unique" : "Hero";

            Debug.CheckYeh(4, $"nameSpecial", $"{nameSpecial}", Indent: 1, Toggle: getDoDebug());

            Creature.SetStringProperty("Culture", "WrassleGiant");

            Creature.SetStringProperty("Role", Unique ? "Leader" : "Hero");

            Creature.SetIntProperty("WrassleGearBestowChance", 100);

            Creature.SetStringProperty("staticFaction1", null);
            Creature.SetStringProperty("staticFaction2", null);
            Creature.SetStringProperty("staticFaction3", null);

            if (Creature.TryGetPart(out GameUnique gameUnique))
            {
                Creature.RemovePart(gameUnique);
            }
            gameUnique = new()
            {
                State = SCRT_GNT_UNQ_STATE,
            };
            Creature.AddPart(gameUnique, Creation: true);

            if (!Creature.TryGetPart(out Wrassler wrassler))
            {
                Debug.CheckNah(4, $"<Wrassler> missing, Adding", Indent: 1, Toggle: getDoDebug());
                wrassler = Creature.RequirePart<Wrassler>();
            }
            Debug.LoopItem(4, $"Have <Wrassler>?", Good: wrassler != null, Indent: 1, Toggle: getDoDebug());

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
            Debug.LoopItem(4, 
                $"NoHateFactions: {Creature.GetStringProperty("NoHateFactions")}", 
                Good: !Creature.GetStringProperty("NoHateFactions").IsNullOrEmpty(), 
                Indent: 1, Toggle: getDoDebug());

            int StaticFactionAdmirations = Unique || wrassler.WrassleID.SeededRandomBool() ? 3 : 2;
            bool ThiccBoisAdmire = Unique || wrassler.WrassleID.SeededRandomBool(3);
            string ThiccBois = ThiccBoisBag.GetRandomElement();
            int ThiccBoisIndex = 
                Unique 
                ? StaticFactionAdmirations - 1 
                : ThiccBoisAdmire 
                    ? StaticFactionAdmirations
                    : 0
                ;
            List<string> factionAdmirationBag = new(FactionAdmirationBag);
            Dictionary<int, string> factionAdmiration = new()
            {
                { 1, factionAdmirationBag.DrawRandomElement() },
                { 2, factionAdmirationBag.DrawRandomElement() },
                { 3, factionAdmirationBag.DrawRandomElement() },
            };

            for (int i = 1; i <= StaticFactionAdmirations; i++)
            {
                string faction = "";
                string feeling = "friend";
                string reason = factionAdmirationBag[i];

                if (i == ThiccBoisIndex)
                {
                    faction = ThiccBois;
                    reason = GNT_THICCBOI_ADMIREREASON;
                }
                if (Unique && i == StaticFactionAdmirations)
                {
                    faction = "GiantTemplar";
                    feeling = "hate";
                    reason = SCRT_GNT_UNQ_TEMPLAR_HATEREASON;
                }
                Creature.SetStringProperty($"staticFaction{i}", $"{faction},{feeling},{reason}");
                Debug.Entry(4, $"staticFaction{i}", $"{faction},{feeling},{reason}", Indent: 1, Toggle: getDoDebug());
            }

            if (Unique)
            {
                Creature.SetStringProperty("SharesRecipe", SCRT_GNT_RECIPE);
                Creature.SetStringProperty("SharesRecipeWithTrueKin", "false");
                Debug.LoopItem(4, 
                    $"SharesRecipe",
                    Creature.GetStringProperty("SharesRecipe"),
                    Good: Creature.HasStringProperty("SharesRecipe"), 
                    Indent: 1, Toggle: getDoDebug());
                Debug.LoopItem(4, 
                    $"SharesRecipeWithTrueKin",
                    Creature.GetStringProperty("SharesRecipeWithTrueKin"),
                    Good: Creature.HasStringProperty("SharesRecipeWithTrueKin"), 
                    Indent: 1, Toggle: getDoDebug());

                SecretRevealer secretRevealer = Creature.RequirePart<SecretRevealer>();
                secretRevealer.id = SCRT_GNT_SCRT_ID;
                secretRevealer.text = SCRT_GNT_LCTN_TEXT;
                secretRevealer.message = $"You have discovered {secretRevealer.text}!";
                secretRevealer.category = SCRT_GNT_LCTN_CATEGORY;
                secretRevealer.adjectives = SecretAttributes.ToList().Join(",");

                Debug.LoopItem(4,
                    $"<SecretRevealer>?",
                    Good: secretRevealer != null,
                    Indent: 1, Toggle: getDoDebug());
                if (secretRevealer != null)
                {
                    Debug.LoopItem(4,
                        $"secretRevealer.id",
                        secretRevealer.id,
                        Good: secretRevealer.id == SCRT_GNT_SCRT_ID,
                        Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4,
                        $"secretRevealer.text",
                        secretRevealer.text,
                        Good: secretRevealer.text == SCRT_GNT_LCTN_TEXT,
                        Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4,
                        $"secretRevealer.message",
                        secretRevealer.message,
                        Good: secretRevealer.message == $"You have discovered {secretRevealer.text}!",
                        Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4,
                        $"secretRevealer.category",
                        secretRevealer.category,
                        Good: secretRevealer.category == SCRT_GNT_LCTN_CATEGORY,
                        Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4,
                        $"secretRevealer.adjectives",
                        secretRevealer.adjectives,
                        Good: secretRevealer.adjectives == SecretAttributes.ToList().Join(","),
                        Indent: 2, Toggle: getDoDebug());
                }
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
                Special: nameSpecial, 
                NamingContext: null, 
                SpecialFaildown: true, 
                HasHonorific: null, 
                HasEpithet: null);

            Debug.LoopItem(4, $"Epithet", Epithet ?? "null", Good: Epithet != null, Indent: 1, Toggle: getDoDebug());

            string CreatureName = NameMaker.MakeName(
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
                Special: nameSpecial,
                NamingContext: null,
                SpecialFaildown: true,
                HasHonorific: null,
                HasEpithet: null);

            Debug.LoopItem(4, $"CreatureName", CreatureName ?? "null", Good: CreatureName != null, Indent: 1, Toggle: getDoDebug());

            Creature.GiveProperName(
                Name: CreatureName.OptionalColorYuge(),
                Force: true,
                Special: nameSpecial,
                SpecialFaildown: true,
                HasHonorific: null,
                HasEpithet: null,
                NamingContext: null);

            int Stews = Stat.Roll("4d4");
            Debug.CheckYeh(4, $"{"4d4".Quote()} Stews", $"{Stews}", Indent: 1, Toggle: getDoDebug());

            if (!Epithet.IsNullOrEmpty())
            {
                if (Creature.TryGetPart(out Epithets epithets))
                {
                    Creature.RemovePart(epithets);
                }
                epithets = Creature.RequirePart<Epithets>();
                epithets.Primary = GameText.VariableReplace(Epithet).Color("y");
            }
            if (!Epithet.IsNullOrEmpty() && !Unique)
            {
                Debug.Entry(4, $"Beginning WrassleGiant Runway", Indent: 1, Toggle: getDoDebug());

                if (Epithet.IsGivingStewful())
                {
                    int extraStews = Stat.Roll("2d4");
                    Stews += extraStews;
                    Debug.CheckYeh(4, $"Epithet.IsGivingStewful() {"2d4".Quote()} extraStews", $"{extraStews}", Indent: 2, Toggle: getDoDebug());
                }

                if (Epithet.IsGivingStewless())
                {
                    int fewerStews = Stat.Roll("1d4");
                    Stews -= fewerStews;
                    Debug.CheckYeh(4, $"Epithet.IsGivingStewless() {"1d4".Quote()} fewerStews", $"{fewerStews}", Indent: 2, Toggle: getDoDebug());
                }
                    
                if (Epithet.IsGivingThoughtful())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingThoughtful()", Indent: 2, Toggle: getDoDebug());

                    int extraInt = Stat.Roll("1d4");
                    Creature.AddBaseStat("Intelligence", extraInt);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraInt", $"{extraInt}", Indent: 3, Toggle: getDoDebug());

                    int extraMentalMutations = Stat.Roll("1d2");
                    MentalMutations += extraMentalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 3, Toggle: getDoDebug());

                    int fewerPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations -= fewerPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} fewerPhysicalMutations", $"{fewerPhysicalMutations}", Indent: 3, Toggle: getDoDebug());
                }

                if (Epithet.IsGivingTough())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingTough()", Indent: 2, Toggle: getDoDebug());

                    int extraTou = Stat.Roll("1d4");
                    Creature.AddBaseStat("Toughness", extraTou);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraTou", $"{extraTou}", Indent: 3, Toggle: getDoDebug());

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3, Toggle: getDoDebug());
                }

                if (Epithet.IsGivingStrong())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingStrong()", Indent: 2, Toggle: getDoDebug());

                    int extraStr = Stat.Roll("1d4");
                    Creature.AddBaseStat("Strength", extraStr);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraStr", $"{extraStr}", Indent: 3, Toggle: getDoDebug());

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3, Toggle: getDoDebug());
                }

                if (Epithet.IsGivingResilient())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingResilient()", Indent: 2, Toggle: getDoDebug());

                    int extraWil = Stat.Roll("1d4");
                    Creature.AddBaseStat("Willpower", extraWil);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraWil", $"{extraWil}", Indent: 3, Toggle: getDoDebug());

                    int extraMentalMutations = Stat.Roll("1d2");
                    MentalMutations += extraMentalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 3, Toggle: getDoDebug());
                }

                if (Epithet.IsGivingTrulyImmense())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingTrulyImmense() Hitpoints", $"x2", Indent: 2, Toggle: getDoDebug());
                    
                    Creature.MultiplyStat("Hitpoints", 2);
                    Debug.LoopItem(4, $"Hitpoints", $"x2", Indent: 3, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"Hitpoints new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 3, Toggle: getDoDebug());

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3, Toggle: getDoDebug());

                    int cimeraRoll = Stat.Roll("1d3");
                    bool makeChimera = cimeraRoll == 3;
                    Debug.LoopItem(4, $"{"1d3".Quote()} cimeraRoll", $"{cimeraRoll}", Good: makeChimera, Indent: 3, Toggle: getDoDebug());
                    if (makeChimera)
                    {
                        MakeChimera = true;
                        PhysicalMutations += 1;
                        Debug.LoopItem(4, $"Chimera extraPhysicalMutations", $"{1}", Indent: 4, Toggle: getDoDebug());
                    }
                }

                if (Epithet.IsGivingWrassler())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingWrassler() Gigantic FoldingChair", $"Give", Indent: 2, Toggle: getDoDebug());
                    Creature.ReceiveObject("Gigantic FoldingChair");
                }

                if (!Epithet.IsGivingPopular())
                {
                    Debug.CheckNah(4, $"Epithet.IsGivingPopular() Followers", $"Dismiss", Indent: 2, Toggle: getDoDebug());

                    if (Creature.TryGetPart(out Leader leader))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(Leader)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(leader);
                    }
                    if (Creature.TryGetPart(out Followers followers))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(Followers)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(followers);
                    }
                    if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(DromadCaravan)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(dromadCaravan);
                    }
                    if (Creature.TryGetPart(out HasGuards hasGuards))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(HasGuards)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(hasGuards);
                    }
                    if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(SnapjawPack1)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(snapjawPack1);
                    }
                    if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(BaboonHero1Pack)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(baboonHero1Pack);
                    }
                    if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(GoatfolkClan1)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(goatfolkClan1);
                    }
                    if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(EyelessKingCrabSkuttle1)}", Indent: 3, Toggle: getDoDebug());
                        Creature.RemovePart(eyelessKingCrabSkuttle1);
                    }
                }
                else
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingPopular() Followers", $"Keep", Indent: 2, Toggle: getDoDebug());
                    if (Creature.TryGetPart(out Leader leader))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(Leader)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out Followers followers))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(Followers)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(DromadCaravan)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out HasGuards hasGuards))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(HasGuards)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(SnapjawPack1)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(BaboonHero1Pack)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(GoatfolkClan1)}", Indent: 3, Toggle: getDoDebug());
                    }
                    if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(EyelessKingCrabSkuttle1)}", Indent: 3, Toggle: getDoDebug());
                    }
                }
            }
            else if (Unique || Epithet.IsNullOrEmpty())
            {
                Debug.Entry(4, $"Beginning Unique WrassleGiant Runway", Indent: 1, Toggle: getDoDebug());

                int extraStews = Stat.Roll("2d4");
                Stews += extraStews;
                Debug.CheckYeh(4, $"{"2d4".Quote()} extraStews", $"{extraStews}", Indent: 2, Toggle: getDoDebug());

                Dictionary<string, (string die, int roll)> statRolls = new()
                {
                    { "extraStr", ("3d4", Stat.Roll("3d4")) },
                    { "extraAgi", ("2d4", Stat.Roll("2d4")) },
                    { "extraTou", ("3d3", Stat.Roll("3d3")) },
                    { "extraInt", ("2d3", Stat.Roll("2d3")) },
                    { "extraWil", ("3d4", Stat.Roll("3d4")) },
                    { "extraEgo", ("2d3", Stat.Roll("2d3")) },
                };

                Debug.CheckYeh(4, $"Extra Stats", Indent: 2, Toggle: getDoDebug());
                Creature.AddBaseStat("Strength",     statRolls["extraStr"].roll);
                Creature.AddBaseStat("Agility",      statRolls["extraAgi"].roll);
                Creature.AddBaseStat("Toughness",    statRolls["extraTou"].roll);
                Creature.AddBaseStat("Intelligence", statRolls["extraInt"].roll);
                Creature.AddBaseStat("Willpower",    statRolls["extraWil"].roll);
                Creature.AddBaseStat("Ego",          statRolls["extraEgo"].roll);

                Debug.LoopItem(4, $"{statRolls["extraStr"].die.Quote()} extraStr", $"{statRolls["extraStr"].roll}", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"{statRolls["extraAgi"].die.Quote()} extraAgi", $"{statRolls["extraAgi"].roll}", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"{statRolls["extraTou"].die.Quote()} extraTou", $"{statRolls["extraTou"].roll}", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"{statRolls["extraInt"].die.Quote()} extraInt", $"{statRolls["extraInt"].roll}", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"{statRolls["extraWil"].die.Quote()} extraWil", $"{statRolls["extraWil"].roll}", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"{statRolls["extraEgo"].die.Quote()} extraEgo", $"{statRolls["extraEgo"].roll}", Indent: 3, Toggle: getDoDebug());

                int cimeraRoll = Stat.Roll("1d3");
                bool makeChimera = cimeraRoll == 3;
                Debug.LoopItem(4, $"{"1d3".Quote()} cimeraRoll", $"{cimeraRoll}", Good: makeChimera, Indent: 2, Toggle: getDoDebug());
                if (makeChimera)
                {
                    MakeChimera = true;
                    PhysicalMutations += 1;
                    Debug.LoopItem(4, $"Chimera extraPhysicalMutations", $"{1}", Indent: 3, Toggle: getDoDebug());
                }

                int extraPhysicalMutations = Stat.Roll("2d2");
                PhysicalMutations += extraPhysicalMutations;
                Debug.CheckYeh(4, $"{"2d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 2, Toggle: getDoDebug());

                int extraMentalMutations = Stat.Roll("1d3");
                MentalMutations += extraMentalMutations;
                Debug.CheckYeh(4, $"{"1d3".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 2, Toggle: getDoDebug());

                Debug.CheckYeh(4, $"Unique Followers", $"Dismiss", Indent: 2, Toggle: getDoDebug());
                if (Creature.TryGetPart(out Leader leader))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(Leader)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(leader);
                }
                if (Creature.TryGetPart(out Followers followers))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(Followers)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(followers);
                }
                if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(DromadCaravan)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(dromadCaravan);
                }
                if (Creature.TryGetPart(out HasGuards hasGuards))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(HasGuards)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(hasGuards);
                }
                if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(SnapjawPack1)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(snapjawPack1);
                }
                if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(BaboonHero1Pack)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(baboonHero1Pack);
                }
                if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(GoatfolkClan1)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(goatfolkClan1);
                }
                if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(EyelessKingCrabSkuttle1)}", Indent: 3, Toggle: getDoDebug());
                    Creature.RemovePart(eyelessKingCrabSkuttle1);
                }

                Creature.MultiplyStat("Hitpoints", 3);
                Debug.LoopItem(4, $"Hitpoints x3, new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 2, Toggle: getDoDebug());
            }

            Debug.CheckYeh(4, $"Pump Hitpoints from {Stews.Things("stew")}", Indent: 2, Toggle: getDoDebug());
            int totalStewsHP = 0;
            for (int i = 0; i < Stews; i++)
            {
                string stewsHPDie = Unique ? "2d8" : "1d10";
                int stewsHP = Stat.Roll(stewsHPDie);
                Creature.AddBaseStat("Hitpoints", stewsHP);
                totalStewsHP += stewsHP;
                Debug.LoopItem(4, $"{stewsHPDie.Quote()} stewsHP{(Unique ? " (Unique)" : "")}", $"{stewsHP}", Indent: 3, Toggle: getDoDebug());
            }
            Debug.CheckYeh(4, $"Hitpoints from {Stews.Things("stew")}", $"{totalStewsHP}", Indent: 3, Toggle: getDoDebug());
            Debug.LoopItem(4, $"Hitpoints new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 3, Toggle: getDoDebug());

            if (!Creature.TryGetPart(out HasMakersMark hasMakersMark))
            {
                hasMakersMark = Creature.RequirePart<HasMakersMark>();
            }
            Debug.LoopItem(4, $"<HasMakersMark>?", Good: Creature.HasPart<HasMakersMark>(), Indent: 2, Toggle: getDoDebug());
            List<string> usableMarks = new(MakersMark.GetUsable());
            string heroMark = usableMarks.DrawSeededElement(wrassler.WrassleID);
            hasMakersMark.Mark = Unique ? ((char)156).ToString() : heroMark;
            hasMakersMark.Color = wrassler.DetailColor;
            if (Unique) MakersMark.RecordUsage(hasMakersMark.Mark);
            Debug.LoopItem(4, $"Mark: {hasMakersMark.Mark}, Color: {hasMakersMark.Color}", Indent: 3, Toggle: getDoDebug());

            int level = Unique ? 35 : 20;
            int extraLevels = Stat.Roll("3d3");
            Creature.GetStat("Level").BaseValue = level + extraLevels;
            Debug.CheckYeh(4, $"Set Level", $"{level + extraLevels}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"starting level", $"{level}", Indent: 3, Toggle: getDoDebug());
            Debug.LoopItem(4, $"{"3d3".Quote()} extraLevels", $"{extraLevels}", Indent: 3, Toggle: getDoDebug());

            int extraMP = (Creature.GetStat("Level").BaseValue - 1);
            Creature.AddBaseStat("MP", extraMP);
            Debug.CheckYeh(4, $"Add extraMP", $"{extraMP}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"(starting Level - 1)", Indent: 3, Toggle: getDoDebug());

            int extraXP = Stat.Roll("1d18") * Stat.Roll("18d18");
            Creature.GetStat("XP").BaseValue = Leveler.GetXPForLevel(Creature.GetStat("Level").Value) + extraXP;
            Debug.CheckYeh(4, $"Set XP", $"{Creature.GetStat("XP").BaseValue}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"starting XP", $"{Leveler.GetXPForLevel(Creature.GetStat("Level").Value)}", Indent: 3, Toggle: getDoDebug());
            Debug.LoopItem(4, $"{"18d18d18".Quote()} extraXP", $"{extraXP}", Indent: 3, Toggle: getDoDebug());

            int extraXPValue = Stews * 75;
            Creature.AddBaseStat("XPValue", extraXPValue);

            if (Unique) Creature.MultiplyStat("XPValue", 2);

            Debug.CheckYeh(4, $"Configure XPValue", $"{Creature.GetStat("XPValue").BaseValue}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"Add XPValue (Stews * 75)", $"{extraXPValue}", Indent: 3, Toggle: getDoDebug());
            if (Unique) Debug.LoopItem(4, $"Multiply XPValue x2 (Unique)", Indent: 3, Toggle: getDoDebug());

            Debug.CheckYeh(4, $"Gigantify Creature", Indent: 2, Toggle: getDoDebug());
            int GigantismLevel = (int)Math.Floor(Creature.Level / 3.0);
            Debug.LoopItem(4, $"GigantismLevel (Creature.Level({Creature.Level}) / 3.0)", $"{GigantismLevel}", Indent: 3, Toggle: getDoDebug());
            Gigantified.GigantifyMutant(Creature, GigantismLevel, Stews, null, Context);

            Mutations mutations = Creature.RequirePart<Mutations>();

            if (MakeChimera)
            {
                Debug.CheckYeh(4, $"MakeChimera", Indent: 2, Toggle: getDoDebug());
                BaseMutation Chimera = MutationFactory.GetMutationEntryByName("Chimera")?.CreateInstance();
                if (Chimera != null)
                {
                    mutations.AddMutation(Chimera);
                    Debug.LoopItem(4, 
                        $"Chimera Mutation added", 
                        Good: mutations.ActiveMutationList.Contains(Chimera),
                        Indent: 3, Toggle: getDoDebug());
                    // mutations.MutationList = mutations.MutationList.OrderByDescending(x => x.Name == Chimera.Name).ToList();
                    Debug.LoopItem(4, 
                        $"Mutations list sorted to have Chimera at the top", 
                        Good: mutations.MutationList.ElementAt(0) == Chimera,
                        Indent: 3, Toggle: getDoDebug());

                    Debug.CheckYeh(4, $"Convert MentalMutations count to some number of PhysicalMutations", Indent: 3, Toggle: getDoDebug());
                    int MentalToPhysicalMutations = Math.Max(0, (int)Math.Floor(MentalMutations / 2.0));
                    PhysicalMutations += MentalToPhysicalMutations;
                    MentalMutations = 0;
                    Debug.LoopItem(4, 
                        $"Max(0, (int)Math.Floor(MentalMutations / 2.0)) extraPhysicalMutations", 
                        $"{MentalToPhysicalMutations}", 
                        Indent: 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"MentalMutations", $"{MentalMutations}", Indent: 4, Toggle: getDoDebug());
                }
                else
                {
                    Debug.CheckNah(4, $"Failed to Instantiate MakeChimera", Indent: 3, Toggle: getDoDebug());
                }
            }
            else
            {
                Debug.CheckNah(4, $"MakeChimera", Indent: 2, Toggle: getDoDebug());
            }

            Debug.CheckYeh(4, $"Final Mutation Counts", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"PhysicalMutations", $"{PhysicalMutations}", Indent: 3, Toggle: getDoDebug());
            Debug.LoopItem(4, $"MentalMutations", $"{MentalMutations}", Indent: 3, Toggle: getDoDebug());

            int MentalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(MentalMutations / 2.0));
            string MentalMutationLevelDie = $"1d{MentalMutationLevelHigh}";
            if (MentalMutations > 0)
            {
                Debug.CheckYeh(4, $"Process MentalMutations", Indent: 2, Toggle: getDoDebug());
                Debug.LoopItem(4, $"MentalMutationLevelDie", $"{MentalMutationLevelDie}", Indent: 3, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Getting {MentalMutations.Things("Random Mental Mutation")}", Indent: 3, Toggle: getDoDebug());
                Debug.Divider(4, HONLY, Count: 40, Indent: 4, Toggle: getDoDebug());
            }
            for (int i = 0; i < MentalMutations; i++)
            {
                BaseMutation randomMentalMutation;
                do
                {
                    Debug.Divider(4, HONLY, Count: 25, Indent: 5, Toggle: getDoDebug());
                    randomMentalMutation = MutationFactory.GetRandomMutation("Mental");
                    Debug.LoopItem(4, 
                        $"{randomMentalMutation.Name}", 
                        Good: !mutations.HasMutation(randomMentalMutation), 
                        Indent: 5, Toggle: getDoDebug());
                }
                while (randomMentalMutation != null && mutations.HasMutation(randomMentalMutation));
                if (randomMentalMutation != null)
                {
                    int randomMutationLevel = Stat.Roll(MentalMutationLevelDie);
                    mutations.AddMutation(randomMentalMutation, randomMutationLevel);
                    Debug.LoopItem(4, $"mutation added at level {randomMutationLevel}", Indent: 6, Toggle: getDoDebug());
                    Debug.Divider(4, HONLY, Count: 25, Indent: 5, Toggle: getDoDebug());
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 4, Toggle: getDoDebug());
            }

            int PhysicalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(PhysicalMutations / 2.0));
            string PhysicalMutationLevelDie = $"1d{PhysicalMutationLevelHigh}";
            if (PhysicalMutations > 0)
            {
                Debug.CheckYeh(4, $"Process PhysicalMutations", Indent: 2, Toggle: getDoDebug());
                Debug.LoopItem(4, $"PhysicalMutationLevelDie", $"{PhysicalMutationLevelDie}", Indent: 3, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Getting {PhysicalMutations.Things("Random Physical Mutation")}", Indent: 3, Toggle: getDoDebug());
                Debug.Divider(4, HONLY, Count: 40, Indent: 4, Toggle: getDoDebug());
            }
            for (int i = 0; i < PhysicalMutations; i++)
            {
                BaseMutation randomPhysicalMutation;
                do
                {
                    Debug.Divider(4, HONLY, Count: 25, Indent: 5, Toggle: getDoDebug());
                    randomPhysicalMutation = MutationFactory.GetRandomMutation("Physical");
                    Debug.LoopItem(4, 
                        $"{randomPhysicalMutation.Name}", 
                        Good: !mutations.HasMutation(randomPhysicalMutation), 
                        Indent: 5, Toggle: getDoDebug());
                }
                while (randomPhysicalMutation != null && mutations.HasMutation(randomPhysicalMutation));
                if (randomPhysicalMutation != null)
                {
                    int randomMutationLevel = Stat.Roll(PhysicalMutationLevelDie);
                    mutations.AddMutation(randomPhysicalMutation, randomMutationLevel);
                    Debug.LoopItem(4, $"mutation added at level {randomMutationLevel}", Indent: 6, Toggle: getDoDebug());
                    if (mutations.HasMutation("Chimera"))
                    {
                        Debug.LoopItem(4, $"Chimera Limb?", Indent: 6, Toggle: getDoDebug());
                        int chimeraLimbRoll = "1d7".RollCached();
                        bool giveChimeraLimb = chimeraLimbRoll > 4;
                        Debug.LoopItem(4,
                            $"{"1d7".Quote()} chimeraLimbRoll {chimeraLimbRoll} > 4", 
                            Good: giveChimeraLimb, 
                            Indent: 7, Toggle: getDoDebug());
                        if (giveChimeraLimb)
                        {
                            mutations.AddChimericBodyPart();
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 5, Toggle: getDoDebug());
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 4, Toggle: getDoDebug());
            }

            Creature.RequirePart<Calming>();
            Debug.LoopItem(4, $"<Calming>?", Good: Creature.HasPart<Calming>(), Indent: 2, Toggle: getDoDebug());

            if (!Creature.TryGetPart(out GivesRep givesRep))
            {
                givesRep = Creature.RequirePart<GivesRep>();
            }
            givesRep.repValue = Unique ? 400 : 200;
            Debug.LoopItem(4, 
                $"<GivesRep>?", 
                $"{(givesRep?.repValue != null ? givesRep.repValue : "")}",
                Good: Creature.HasPart<GivesRep>(), 
                Indent: 2, Toggle: getDoDebug());

            Debug.CheckYeh(4, $"Remove Problem Parts", Indent: 2, Toggle: getDoDebug());
            if (Creature.TryGetPart(out GreaterVoider greaterVoider))
            {
                Debug.CheckYeh(4, $"Removed {nameof(GreaterVoider)}", Indent: 3, Toggle: getDoDebug());
                Creature.RemovePart(greaterVoider);
            }
            if (Creature.TryGetPart(out Rummager rummager))
            {
                Debug.CheckYeh(4, $"Removed {nameof(Rummager)}", Indent: 3, Toggle: getDoDebug());
                Creature.RemovePart(rummager);
            }
            if (Creature.TryGetPart(out Breeder breeder))
            {
                Debug.CheckYeh(4, $"Removed {nameof(Breeder)}", Indent: 3, Toggle: getDoDebug());
                Creature.RemovePart(breeder);
            }
            if (Creature.TryGetPart(out ReplaceObject replaceObject))
            {
                Debug.CheckYeh(4, $"Removed {nameof(ReplaceObject)}", Indent: 3, Toggle: getDoDebug());
                Creature.RemovePart(replaceObject);
            }

            Creature.FireEvent("VillageInit");
                
            if (Creature.TryGetPart(out DisplayNameAdjectives displayNameAdjectives))
            {
                if (displayNameAdjectives.AdjectiveList.Contains(Gigantified.GetNamePrefix()))
                {
                    Debug.CheckYeh(4, $"Removed {Gigantified.GetNamePrefix()} from {nameof(DisplayNameAdjectives)}", Indent: 2, Toggle: getDoDebug());
                    displayNameAdjectives.RemoveAdjective(Gigantified.GetNamePrefix());
                }
            }

            if (Unique)
            {
                if (!Creature.TryGetPart(out ConversationScript conversationScript))
                {
                    conversationScript = Creature.RequirePart<ConversationScript>();
                }
                conversationScript.ConversationID = SCRT_GNT_UNQ_CONVSCRPT_ID;

                Debug.LoopItem(4, 
                    $"<ConversationScript>?", 
                    $"{conversationScript?.ConversationID ?? "" }",
                    Good: Creature.HasPart<ConversationScript>(),
                    Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4,
                    $"<ConversationScript>?",
                    $"Not yet written for non-unique WrassleGiants",
                    Indent: 2, Toggle: getDoDebug());
            }

            Creature.AddSkills(HeroSkills);
            if (Unique) 
                Creature.AddSkills(UniqueHeroSkills);

            Debug.CheckYeh(4, $"Skills Added", Indent: 2, Toggle: getDoDebug());
            foreach (BaseSkill skill in Creature.GetPartsDescendedFrom<BaseSkill>())
            {
                Debug.LoopItem(4, $" {skill.Name}", Indent: 3, Toggle: getDoDebug());
            }

            int startingMP = Creature.Stat("MP");
            if (startingMP > 0)
            {
                int lastRemaining = 0;
                int lastPoints = 0;
                bool stuck = false;
                int maxAttempts = 200;
                Debug.CheckYeh(4, $"Total MP to Spend", $"{startingMP}", Indent: 2, Toggle: getDoDebug());
                Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: getDoDebug());
                while (Creature.Stat("MP") > 0 && Creature.Stat("MP") != lastRemaining && !stuck && --maxAttempts > 0)
                {
                    int pointsToSpend = Stat.Roll($"1d{Math.Max(1, Math.Min(Creature.Stat("MP"), 4))}");
                    if (lastRemaining == Creature.Stat("MP"))
                    {
                        stuck = true;
                        pointsToSpend += lastPoints;
                    }
                    else
                    {
                        stuck = false;
                    }
                    lastPoints = pointsToSpend;
                    lastRemaining = Creature.Stat("MP");
                    Debug.LoopItem(4, $"Spending: {pointsToSpend}", Indent: 3, Toggle: getDoDebug());
                    Creature.RandomlySpendPoints(maxAPtospend: 0, maxSPtospend: 0, maxMPtospend: pointsToSpend);
                    Debug.LoopItem(4, $"Remaining: {Creature.Stat("MP")}", Indent: 4, Toggle: getDoDebug());
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: getDoDebug());
                }
            }
            else
            {
                Debug.CheckNah(4, $"No MP to Spend", Indent: 2, Toggle: getDoDebug());
            }
            Creature.RandomlySpendPoints();
            Debug.LoopItem(4, $"AP", $"{Creature.Stat("AP")}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"SP", $"{Creature.Stat("SP")}", Indent: 2, Toggle: getDoDebug());
            Debug.LoopItem(4, $"MP", $"{Creature.Stat("MP")}", Indent: 2, Toggle: getDoDebug());

            if (Creature.IsMutant() && !Creature.IsEsper())
            {
                Debug.CheckYeh(4, $"Attempting Rapid Advancements", Indent: 2, Toggle: getDoDebug());
                for (int i = 0; i < Creature.Level; i++)
                {
                    int iLevel = i + 1;
                    Debug.LoopItem(4, $"iLevel: {iLevel}", Indent: 3, Toggle: getDoDebug());

                    int rapidAdvancement = ((iLevel + 5) % 10 == 0) ? 3 : 0;
                    Leveler.RapidAdvancement(rapidAdvancement, Creature);
                    if (rapidAdvancement != 0)
                    {
                        Debug.CheckYeh(4, $"Rapid Advance", $"{rapidAdvancement != 0}",
                            Indent: 4, Toggle: getDoDebug());
                    }
                }
            }
            else
            {
                Debug.CheckNah(4, $"Inelligeble for Rapid Advancements", Indent: 2, Toggle: getDoDebug());
            }
            Creature.RequirePart<Interesting>();
            Debug.LoopItem(4, $"<Interesting>?", Good: Creature.HasPart<Interesting>(), Indent: 2, Toggle: getDoDebug());

            if (!Creature.TryGetPart(out Description description))
            {
                description = Creature.RequirePart<Description>();
            }

            string creatureSubtype = Creature != null && Creature.IsPlayer() ? Creature?.GetSubtype() : null;
            string creatureType = Creature?.GetCreatureType();
            string creatureBlueprint = Creature?.GetBlueprint()?.DisplayName();

            string creatureNoun = creatureSubtype ?? creatureType ?? creatureBlueprint ?? null;
            string creatureArticle = Grammar.IndefiniteArticle(creatureNoun);
            creatureArticle = Unique ? creatureArticle.Capitalize() : creatureArticle;

            string aCreature = creatureNoun != null ? $"{creatureArticle} {creatureNoun}" : "";
            aCreature = Unique
                ? $"{aCreature}, "
                : !aCreature.IsNullOrEmpty()
                    ? $", {aCreature}, "
                    : ""
                    ;

            string preDesc = Unique ? SCRT_GNT_UNQ_PREDESC : GNT_PREDESC;
            aCreature = Unique ? Creature.An(Stripped: true, BaseOnly: true) : Creature.an(Stripped: true, BaseOnly: true);
            preDesc = preDesc.Replace("*creature.an*", aCreature);

            description.Short = preDesc + description._Short;

            Debug.LoopItem(4, 
                $"<Description>?", 
                Good: description.Short.Contains($"{preDesc}"), 
                Indent: 2, Toggle: getDoDebug());
        }

        public static GameObjectBlueprint GetGiantEligibleBlueprintModel(Predicate<GameObjectBlueprint> filter = null, bool Old = false, bool Unique = false)
        {
            GameObjectBlueprint creatureObjectBlueprint =
                EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => IsWrassleGiantEligible(blueprint, filter, Old, Unique));
            GameObjectBlueprint alternateCreatureObjectBlueprint = null;

            int chance = Unique || Old ? 10 : 5;
            if (chance.in1000())
            {
                alternateCreatureObjectBlueprint = GameObjectFactory.Factory.GetBlueprint("Aleksh_TrollHero");
            }

            return alternateCreatureObjectBlueprint ?? creatureObjectBlueprint;
        }
        public static GameObjectBlueprint GetAnOldGiantBlueprintModel(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetGiantEligibleBlueprintModel(filter, Old: true);
        }
        public static GameObjectBlueprint GetAGiantHeroBlueprintModel(Predicate<GameObjectBlueprint> filter = null, bool Old = true)
        {
            return GetGiantEligibleBlueprintModel(filter, Old);
        }
        public static GameObjectBlueprint GetAUniqueGiantHeroBlueprintModel(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetGiantEligibleBlueprintModel(filter, Unique: true);
        }
        public static string GetGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null, bool Old = false, bool Unique = false)
        {
            return GetGiantEligibleBlueprintModel(filter, Old, Unique).Name;
        }
        public static string GetOldGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetAnOldGiantBlueprintModel(filter).Name;
        }
        public static string GetGiantHeroEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null, bool Old = true)
        {
            return GetAGiantHeroBlueprintModel(filter, Old).Name;
        }
        public static string GetUniqueGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null)
        {
            return GetAUniqueGiantHeroBlueprintModel(filter).Name;
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

            if (Blueprint.Parts.ContainsKey(typeof(CherubimSpawner).Name))
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

        [WishCommand(Command = "we could be heroes")]
        public static void WeCouldBeHeroes_oo_oo_oooh()
        {
            GameObject player = The.Player;
            if (player.HasStringProperty("already a hero no go today."))
            {
                Popup.Show($"You, {player.DisplayName}, are {"already".Color("yuge")} a heroic giant.");
            }
            else
            {
                WrassleGiantHeroBuilder.Apply(player, "Hero");
                Popup.Show($"Rise, {player.DisplayName}!");
                if (player.TryGetPart(out Wrassler wrassler))
                {
                    if (player.TryGetPart(out HasMakersMark hasMakersMark))
                    {
                        hasMakersMark.Mark = ((char)156).ToString();
                        hasMakersMark.Color = wrassler.DetailColor;
                    }
                    wrassler.BestowWrassleGear();
                }
                player.SetStringProperty("already a hero no go today.", "sorry buddy.");
            }
        }

        [WishCommand("wrassler", null)]
        public static void Wish(string Blueprint)
        {
            WishResult wishResult = WishSearcher.SearchForBlueprint(Blueprint);
            GameObject @object = GameObjectFactory.Factory.CreateObject(wishResult.Result, 0, 0, null, null, null, "Wish");
            @object.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

            if (@object != null)
            {
                The.PlayerCell.getClosestEmptyCell().AddObject(@object);
            }
        }
    } //!-- public class WrassleGiantHero : IObjectBuilder
} //!-- namespace XRL.World.ObjectBuilders
