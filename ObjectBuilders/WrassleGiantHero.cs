using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Qud.API;
using XRL.UI;
using XRL.Names;
using XRL.Rules;
using XRL.Language;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.ObjectBuilders;
using XRL.World.Capabilities;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using HistoryKit;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
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
                $"GameObject Creature: {Creature.ID}, " + 
                $"string Context: {Context.Quote()})",
                Indent: 0);

            Context ??= this.Context;

            bool Unique = Context == "Unique";
            if (Unique && The.Game.HasStringGameState(SCRT_GNT_UNQ_STATE) && false)
            {
                Debug.Entry(4, 
                    $"/!\\ " + 
                    $"WARN: " + 
                    $"Attempted to create Unique {nameof(WrassleGiantHero)} while one already exists", 
                    Indent: 1);

                Context = "Hero";
                Unique = false;
            }

            Debug.LoopItem(4, $"Unique?", Good: Unique, Indent: 1);

            string nameSpecial = Unique ? "Unique" : "Hero";

            Debug.CheckYeh(4, $"nameSpecial", $"{nameSpecial}", Indent: 1);

            if (!Creature.TryGetPart(out Wrassler wrassler))
            {
                Debug.CheckNah(4, $"<Wrassler> missing, Adding", Indent: 1);
                wrassler = Creature.RequirePart<Wrassler>();
            }
            Debug.LoopItem(4, $"Have <Wrassler>?", Good: wrassler != null, Indent: 1);

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
                Indent: 1);

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
                Debug.Entry(4, $"staticFaction{i}", $"{faction},{feeling},{reason}", Indent: 1);
            }

            if (Unique)
            {
                Creature.SetStringProperty("SharesRecipe", SCRT_GNT_RECIPE);
                Creature.SetStringProperty("SharesRecipeWithTrueKin", "false");
                Debug.LoopItem(4, 
                    $"SharesRecipe",
                    Creature.GetStringProperty("SharesRecipe"),
                    Good: Creature.HasStringProperty("SharesRecipe"), 
                    Indent: 1);
                Debug.LoopItem(4, 
                    $"SharesRecipeWithTrueKin",
                    Creature.GetStringProperty("SharesRecipeWithTrueKin"),
                    Good: Creature.HasStringProperty("SharesRecipeWithTrueKin"), 
                    Indent: 1);

                SecretRevealer secretRevealer = Creature.RequirePart<SecretRevealer>();
                secretRevealer.id = SCRT_GNT_SCRT_ID;
                secretRevealer.text = SCRT_GNT_LCTN_TEXT;
                secretRevealer.message = $"You have discovered {secretRevealer.text}!";
                secretRevealer.category = SCRT_GNT_LCTN_CATEGORY;
                secretRevealer.adjectives = SecretAttributes.ToList().Join(",");

                Debug.LoopItem(4,
                    $"<SecretRevealer>?",
                    Good: secretRevealer != null,
                    Indent: 1);
                if (secretRevealer != null)
                {
                    Debug.LoopItem(4,
                        $"secretRevealer.id",
                        secretRevealer.id,
                        Good: secretRevealer.id == SCRT_GNT_SCRT_ID,
                        Indent: 2);
                    Debug.LoopItem(4,
                        $"secretRevealer.text",
                        secretRevealer.text,
                        Good: secretRevealer.text == SCRT_GNT_LCTN_TEXT,
                        Indent: 2);
                    Debug.LoopItem(4,
                        $"secretRevealer.message",
                        secretRevealer.message,
                        Good: secretRevealer.message == $"You have discovered {secretRevealer.text}!",
                        Indent: 2);
                    Debug.LoopItem(4,
                        $"secretRevealer.category",
                        secretRevealer.category,
                        Good: secretRevealer.category == SCRT_GNT_LCTN_CATEGORY,
                        Indent: 2);
                    Debug.LoopItem(4,
                        $"secretRevealer.adjectives",
                        secretRevealer.adjectives,
                        Good: secretRevealer.adjectives == SecretAttributes.ToList().Join(","),
                        Indent: 2);
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

            Debug.LoopItem(4, $"Epithet", Epithet ?? "null", Good: Epithet != null, Indent: 1);

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

            Debug.LoopItem(4, $"CreatureName", CreatureName ?? "null", Good: CreatureName != null, Indent: 1);

            Creature.GiveProperName(
                Name: CreatureName.OptionalColorYuge(),
                Force: true,
                Special: nameSpecial,
                SpecialFaildown: true,
                HasHonorific: null,
                HasEpithet: null,
                NamingContext: null);

            int Stews = Stat.Roll("4d4");
            Debug.CheckYeh(4, $"{"4d4".Quote()} Stews", $"{Stews}", Indent: 1);

            if (!Epithet.IsNullOrEmpty()) Creature.RequirePart<Epithets>().Primary = GameText.VariableReplace(Epithet).Color("y");
            if (!Epithet.IsNullOrEmpty() && !Unique)
            {
                Debug.Entry(4, $"Beginning WrassleGiant Runway", Indent: 1);

                if (Epithet.IsGivingStewful())
                {
                    int extraStews = Stat.Roll("2d4");
                    Stews += extraStews;
                    Debug.CheckYeh(4, $"Epithet.IsGivingStewful() {"2d4".Quote()} extraStews", $"{extraStews}", Indent: 2);
                }

                if (Epithet.IsGivingStewless())
                {
                    int fewerStews = Stat.Roll("1d4");
                    Stews -= fewerStews;
                    Debug.CheckYeh(4, $"Epithet.IsGivingStewless() {"1d4".Quote()} fewerStews", $"{fewerStews}", Indent: 2);
                }
                    
                if (Epithet.IsGivingThoughtful())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingThoughtful()", Indent: 2);

                    int extraInt = Stat.Roll("1d4");
                    Creature.AddBaseStat("Intelligence", extraInt);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraInt", $"{extraInt}", Indent: 3);

                    int extraMentalMutations = Stat.Roll("1d2");
                    MentalMutations += extraMentalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 3);

                    int fewerPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations -= fewerPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} fewerPhysicalMutations", $"{fewerPhysicalMutations}", Indent: 3);
                }

                if (Epithet.IsGivingTough())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingTough()", Indent: 2);

                    int extraTou = Stat.Roll("1d4");
                    Creature.AddBaseStat("Toughness", extraTou);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraTou", $"{extraTou}", Indent: 3);

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3);
                }

                if (Epithet.IsGivingStrong())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingStrong()", Indent: 2);

                    int extraStr = Stat.Roll("1d4");
                    Creature.AddBaseStat("Strength", extraStr);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraStr", $"{extraStr}", Indent: 3);

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3);
                }

                if (Epithet.IsGivingResilient())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingResilient()", Indent: 2);

                    int extraWil = Stat.Roll("1d4");
                    Creature.AddBaseStat("Willpower", extraWil);
                    Debug.LoopItem(4, $"{"1d4".Quote()} extraWil", $"{extraWil}", Indent: 3);

                    int extraMentalMutations = Stat.Roll("1d2");
                    MentalMutations += extraMentalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 3);
                }

                if (Epithet.IsGivingTrulyImmense())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingTrulyImmense() Hitpoints", $"x2", Indent: 2);
                    
                    Creature.MultiplyStat("Hitpoints", 2);
                    Debug.LoopItem(4, $"Hitpoints", $"x2", Indent: 3);
                    Debug.LoopItem(4, $"Hitpoints new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 3);

                    int extraPhysicalMutations = Stat.Roll("1d2");
                    PhysicalMutations += extraPhysicalMutations;
                    Debug.LoopItem(4, $"{"1d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 3);

                    int cimeraRoll = Stat.Roll("1d3");
                    bool makeChimera = cimeraRoll == 3;
                    Debug.LoopItem(4, $"{"1d3".Quote()} cimeraRoll", $"{cimeraRoll}", Good: makeChimera, Indent: 3);
                    if (makeChimera)
                    {
                        MakeChimera = true;
                        PhysicalMutations += 1;
                        Debug.LoopItem(4, $"Chimera extraPhysicalMutations", $"{1}", Indent: 4);
                    }
                }

                if (Epithet.IsGivingWrassler())
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingWrassler() Gigantic FoldingChair", $"Give", Indent: 2);
                    Creature.ReceiveObject("Gigantic FoldingChair");
                }

                if (!Epithet.IsGivingPopular())
                {
                    Debug.CheckNah(4, $"Epithet.IsGivingPopular() Followers", $"Dismiss", Indent: 2);

                    if (Creature.TryGetPart(out Leader leader))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(Leader)}", Indent: 3);
                        Creature.RemovePart(leader);
                    }
                    if (Creature.TryGetPart(out Followers followers))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(Followers)}", Indent: 3);
                        Creature.RemovePart(followers);
                    }
                    if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(DromadCaravan)}", Indent: 3);
                        Creature.RemovePart(dromadCaravan);
                    }
                    if (Creature.TryGetPart(out HasGuards hasGuards))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(HasGuards)}", Indent: 3);
                        Creature.RemovePart(hasGuards);
                    }
                    if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(SnapjawPack1)}", Indent: 3);
                        Creature.RemovePart(snapjawPack1);
                    }
                    if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(BaboonHero1Pack)}", Indent: 3);
                        Creature.RemovePart(baboonHero1Pack);
                    }
                    if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(GoatfolkClan1)}", Indent: 3);
                        Creature.RemovePart(goatfolkClan1);
                    }
                    if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                    {
                        Debug.CheckYeh(4, $"Removed {nameof(EyelessKingCrabSkuttle1)}", Indent: 3);
                        Creature.RemovePart(eyelessKingCrabSkuttle1);
                    }
                }
                else
                {
                    Debug.CheckYeh(4, $"Epithet.IsGivingPopular() Followers", $"Keep", Indent: 2);
                    if (Creature.TryGetPart(out Leader leader))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(Leader)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out Followers followers))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(Followers)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(DromadCaravan)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out HasGuards hasGuards))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(HasGuards)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(SnapjawPack1)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(BaboonHero1Pack)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(GoatfolkClan1)}", Indent: 3);
                    }
                    if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                    {
                        Debug.CheckYeh(4, $"Have {nameof(EyelessKingCrabSkuttle1)}", Indent: 3);
                    }
                }
            }
            else if (Unique || Epithet.IsNullOrEmpty())
            {
                Debug.Entry(4, $"Beginning Unique WrassleGiant Runway", Indent: 1);

                int extraStews = Stat.Roll("2d4");
                Stews += extraStews;
                Debug.CheckYeh(4, $"{"2d4".Quote()} extraStews", $"{extraStews}", Indent: 2);

                Dictionary<string, (string die, int roll)> statRolls = new()
                {
                    { "extraStr", ("3d4", Stat.Roll("3d4")) },
                    { "extraAgi", ("2d4", Stat.Roll("2d4")) },
                    { "extraTou", ("3d3", Stat.Roll("3d3")) },
                    { "extraInt", ("2d3", Stat.Roll("2d3")) },
                    { "extraWil", ("3d4", Stat.Roll("3d4")) },
                    { "extraEgo", ("2d3", Stat.Roll("2d3")) },
                };

                Debug.CheckYeh(4, $"Extra Stats", Indent: 2);
                Creature.AddBaseStat("Strength",     statRolls["extraStr"].roll);
                Creature.AddBaseStat("Agility",      statRolls["extraAgi"].roll);
                Creature.AddBaseStat("Toughness",    statRolls["extraTou"].roll);
                Creature.AddBaseStat("Intelligence", statRolls["extraInt"].roll);
                Creature.AddBaseStat("Willpower",    statRolls["extraWil"].roll);
                Creature.AddBaseStat("Ego",          statRolls["extraEgo"].roll);

                Debug.LoopItem(4, $"{statRolls["extraStr"].die.Quote()} extraStr", $"{statRolls["extraStr"].roll}", Indent: 3);
                Debug.LoopItem(4, $"{statRolls["extraAgi"].die.Quote()} extraAgi", $"{statRolls["extraAgi"].roll}", Indent: 3);
                Debug.LoopItem(4, $"{statRolls["extraTou"].die.Quote()} extraTou", $"{statRolls["extraTou"].roll}", Indent: 3);
                Debug.LoopItem(4, $"{statRolls["extraInt"].die.Quote()} extraInt", $"{statRolls["extraInt"].roll}", Indent: 3);
                Debug.LoopItem(4, $"{statRolls["extraWil"].die.Quote()} extraWil", $"{statRolls["extraWil"].roll}", Indent: 3);
                Debug.LoopItem(4, $"{statRolls["extraEgo"].die.Quote()} extraEgo", $"{statRolls["extraEgo"].roll}", Indent: 3);

                int cimeraRoll = Stat.Roll("1d3");
                bool makeChimera = cimeraRoll == 3;
                Debug.LoopItem(4, $"{"1d3".Quote()} cimeraRoll", $"{cimeraRoll}", Good: makeChimera, Indent: 2);
                if (makeChimera)
                {
                    MakeChimera = true;
                    PhysicalMutations += 1;
                    Debug.LoopItem(4, $"Chimera extraPhysicalMutations", $"{1}", Indent: 3);
                }

                int extraPhysicalMutations = Stat.Roll("2d2");
                PhysicalMutations += extraPhysicalMutations;
                Debug.CheckYeh(4, $"{"2d2".Quote()} extraPhysicalMutations", $"{extraPhysicalMutations}", Indent: 2);

                int extraMentalMutations = Stat.Roll("1d3");
                MentalMutations += extraMentalMutations;
                Debug.CheckYeh(4, $"{"1d3".Quote()} extraMentalMutations", $"{extraMentalMutations}", Indent: 2);

                Debug.CheckYeh(4, $"Unique Followers", $"Dismiss", Indent: 2);
                if (Creature.TryGetPart(out Leader leader))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(Leader)}", Indent: 3);
                    Creature.RemovePart(leader);
                }
                if (Creature.TryGetPart(out Followers followers))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(Followers)}", Indent: 3);
                    Creature.RemovePart(followers);
                }
                if (Creature.TryGetPart(out DromadCaravan dromadCaravan))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(DromadCaravan)}", Indent: 3);
                    Creature.RemovePart(dromadCaravan);
                }
                if (Creature.TryGetPart(out HasGuards hasGuards))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(HasGuards)}", Indent: 3);
                    Creature.RemovePart(hasGuards);
                }
                if (Creature.TryGetPart(out SnapjawPack1 snapjawPack1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(SnapjawPack1)}", Indent: 3);
                    Creature.RemovePart(snapjawPack1);
                }
                if (Creature.TryGetPart(out BaboonHero1Pack baboonHero1Pack))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(BaboonHero1Pack)}", Indent: 3);
                    Creature.RemovePart(baboonHero1Pack);
                }
                if (Creature.TryGetPart(out GoatfolkClan1 goatfolkClan1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(GoatfolkClan1)}", Indent: 3);
                    Creature.RemovePart(goatfolkClan1);
                }
                if (Creature.TryGetPart(out EyelessKingCrabSkuttle1 eyelessKingCrabSkuttle1))
                {
                    Debug.CheckYeh(4, $"Removed {nameof(EyelessKingCrabSkuttle1)}", Indent: 3);
                    Creature.RemovePart(eyelessKingCrabSkuttle1);
                }

                Creature.MultiplyStat("Hitpoints", 3);
                Debug.LoopItem(4, $"Hitpoints x3, new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 2);
            }

            Debug.CheckYeh(4, $"Pump Hitpoints from {Stews.Things("stew")}", Indent: 2);
            int totalStewsHP = 0;
            for (int i = 0; i < Stews; i++)
            {
                string stewsHPDie = Unique ? "2d8" : "1d10";
                int stewsHP = Stat.Roll(stewsHPDie);
                Creature.AddBaseStat("Hitpoints", stewsHP);
                totalStewsHP += stewsHP;
                Debug.LoopItem(4, $"{stewsHPDie.Quote()} stewsHP{(Unique ? " (Unique)" : "")}", $"{stewsHP}", Indent: 3);
            }
            Debug.CheckYeh(4, $"Hitpoints from {Stews.Things("stew")}", $"{totalStewsHP}", Indent: 3);
            Debug.LoopItem(4, $"Hitpoints new Total", $"{Creature.GetStat("Hitpoints").BaseValue}", Indent: 3);

            if (!Creature.TryGetPart(out HasMakersMark hasMakersMark))
            {
                hasMakersMark = Creature.RequirePart<HasMakersMark>();
            }
            Debug.LoopItem(4, $"<HasMakersMark>?", Good: Creature.HasPart<HasMakersMark>(), Indent: 2);
            List<string> usableMarks = new(MakersMark.GetUsable());
            string heroMark = usableMarks.DrawSeededElement(wrassler.WrassleID);
            hasMakersMark.Mark = Unique ? ((char)156).ToString() : heroMark;
            hasMakersMark.Color = wrassler.DetailColor;
            if (Unique) MakersMark.RecordUsage(hasMakersMark.Mark);
            Debug.LoopItem(4, $"Mark: {hasMakersMark.Mark}, Color: {hasMakersMark.Color}", Indent: 3);

            int level = Unique ? 35 : 20;
            int extraLevels = Stat.Roll("3d3");
            Creature.GetStat("Level").BaseValue = level + extraLevels;
            Debug.CheckYeh(4, $"Set Level", $"{level + extraLevels}", Indent: 2);
            Debug.LoopItem(4, $"starting level", $"{level}", Indent: 3);
            Debug.LoopItem(4, $"{"3d3".Quote()} extraLevels", $"{extraLevels}", Indent: 3);

            int extraMP = (Creature.GetStat("Level").BaseValue - 1) * 4;
            Creature.AddBaseStat("MP", extraMP);
            Debug.CheckYeh(4, $"Add extraMP", $"{extraMP}", Indent: 2);
            Debug.LoopItem(4, $"(starting Level - 1) x4", Indent: 3);

            int extraXP = Stat.Roll("1d18") * Stat.Roll("18d18");
            Creature.GetStat("XP").BaseValue = Leveler.GetXPForLevel(Creature.GetStat("Level").Value) + extraXP;
            Debug.CheckYeh(4, $"Set XP", $"{Creature.GetStat("XP").BaseValue}", Indent: 2);
            Debug.LoopItem(4, $"starting XP", $"{Leveler.GetXPForLevel(Creature.GetStat("Level").Value)}", Indent: 3);
            Debug.LoopItem(4, $"{"18d18d18".Quote()} extraXP", $"{extraXP}", Indent: 3);

            int extraXPValue = Stews * 75;
            Creature.AddBaseStat("XPValue", extraXPValue);

            if (Unique) Creature.MultiplyStat("XPValue", 2);

            Debug.CheckYeh(4, $"Configure XPValue", $"{Creature.GetStat("XPValue").BaseValue}", Indent: 2);
            Debug.LoopItem(4, $"Add XPValue (Stews * 75)", $"{extraXPValue}", Indent: 3);
            if (Unique) Debug.LoopItem(4, $"Multiply XPValue x2 (Unique)", Indent: 3);

            Debug.CheckYeh(4, $"Gigantify Creature", Indent: 2);
            int GigantismLevel = (int)Math.Floor(Creature.Level / 3.0);
            Debug.LoopItem(4, $"GigantismLevel (Creature.Level({Creature.Level}) / 3.0)", $"{GigantismLevel}", Indent: 3);
            Gigantified.GigantifyMutant(Creature, GigantismLevel, Stews, null, Context);

            Mutations mutations = Creature.RequirePart<Mutations>();

            if (MakeChimera)
            {
                Debug.CheckYeh(4, $"MakeChimera", Indent: 2);
                BaseMutation Chimera = MutationFactory.GetMutationEntryByName("Chimera")?.CreateInstance();
                if (Chimera != null)
                {
                    mutations.AddMutation(Chimera);
                    Debug.LoopItem(4, 
                        $"Chimera Mutation added", 
                        Good: mutations.ActiveMutationList.Contains(Chimera),
                        Indent: 3);
                    // mutations.MutationList = mutations.MutationList.OrderByDescending(x => x.Name == Chimera.Name).ToList();
                    Debug.LoopItem(4, 
                        $"Mutations list sorted to have Chimera at the top", 
                        Good: mutations.MutationList.ElementAt(0) == Chimera,
                        Indent: 3);

                    Debug.CheckYeh(4, $"Convert MentalMutations count to some number of PhysicalMutations", Indent: 3);
                    int MentalToPhysicalMutations = Math.Max(0, (int)Math.Floor(MentalMutations / 2.0));
                    PhysicalMutations += MentalToPhysicalMutations;
                    MentalMutations = 0;
                    Debug.LoopItem(4, 
                        $"Max(0, (int)Math.Floor(MentalMutations / 2.0)) extraPhysicalMutations", 
                        $"{MentalToPhysicalMutations}", 
                        Indent: 4);
                    Debug.LoopItem(4, $"MentalMutations", $"{MentalMutations}", Indent: 4);
                }
                else
                {
                    Debug.CheckNah(4, $"Failed to Instantiate MakeChimera", Indent: 3);
                }
            }
            else
            {
                Debug.CheckNah(4, $"MakeChimera", Indent: 2);
            }

            Debug.CheckYeh(4, $"Final Mutation Counts", Indent: 2);
            Debug.LoopItem(4, $"PhysicalMutations", $"{PhysicalMutations}", Indent: 3);
            Debug.LoopItem(4, $"MentalMutations", $"{MentalMutations}", Indent: 3);

            int MentalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(MentalMutations / 2.0));
            string MentalMutationLevelDie = $"1d{MentalMutationLevelHigh}";
            if (MentalMutations > 0)
            {
                Debug.CheckYeh(4, $"Process MentalMutations", Indent: 2);
                Debug.LoopItem(4, $"MentalMutationLevelDie", $"{MentalMutationLevelDie}", Indent: 3);

                Debug.LoopItem(4, $"Getting {MentalMutations.Things("Random Mental Mutation")}", Indent: 3);
                Debug.Divider(4, "-", Count: 40, Indent: 4);
            }
            for (int i = 0; i < MentalMutations; i++)
            {
                BaseMutation randomMentalMutation;
                do
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 5);
                    randomMentalMutation = MutationFactory.GetRandomMutation("Mental");
                    Debug.LoopItem(4, 
                        $"{randomMentalMutation.Name}", 
                        Good: !mutations.HasMutation(randomMentalMutation), 
                        Indent: 5);
                }
                while (randomMentalMutation != null && mutations.HasMutation(randomMentalMutation));
                if (randomMentalMutation != null)
                {
                    int randomMutationLevel = Stat.Roll(MentalMutationLevelDie);
                    mutations.AddMutation(randomMentalMutation, randomMutationLevel);
                    Debug.LoopItem(4, $"mutation added at level {randomMutationLevel}", Indent: 6);
                    Debug.Divider(4, "-", Count: 25, Indent: 5);
                }
                Debug.Divider(4, "-", Count: 40, Indent: 4);
            }

            int PhysicalMutationLevelHigh = 2 + Math.Max(1, (int)Math.Floor(PhysicalMutations / 2.0));
            string PhysicalMutationLevelDie = $"1d{PhysicalMutationLevelHigh}";
            if (PhysicalMutations > 0)
            {
                Debug.CheckYeh(4, $"Process PhysicalMutations", Indent: 2);
                Debug.LoopItem(4, $"PhysicalMutationLevelDie", $"{PhysicalMutationLevelDie}", Indent: 3);

                Debug.LoopItem(4, $"Getting {PhysicalMutations.Things("Random Physical Mutation")}", Indent: 3);
                Debug.Divider(4, "-", Count: 40, Indent: 4);
            }
            for (int i = 0; i < PhysicalMutations; i++)
            {
                BaseMutation randomPhysicalMutation;
                do
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 5);
                    randomPhysicalMutation = MutationFactory.GetRandomMutation("Physical");
                    Debug.LoopItem(4, 
                        $"{randomPhysicalMutation.Name}", 
                        Good: !mutations.HasMutation(randomPhysicalMutation), 
                        Indent: 5);
                }
                while (randomPhysicalMutation != null && mutations.HasMutation(randomPhysicalMutation));
                if (randomPhysicalMutation != null)
                {
                    int randomMutationLevel = Stat.Roll(PhysicalMutationLevelDie);
                    mutations.AddMutation(randomPhysicalMutation, randomMutationLevel);
                    Debug.LoopItem(4, $"mutation added at level {randomMutationLevel}", Indent: 6);
                    if (mutations.HasMutation("Chimera"))
                    {
                        Debug.LoopItem(4, $"Chimera Limb?", Indent: 6);
                        int chimeraLimbRoll = "1d7".RollCached();
                        bool giveChimeraLimb = chimeraLimbRoll > 4;
                        Debug.LoopItem(4,
                            $"{"1d7".Quote()} chimeraLimbRoll {chimeraLimbRoll} > 4", 
                            Good: giveChimeraLimb, 
                            Indent: 7);
                        if (giveChimeraLimb)
                        {
                            mutations.AddChimericBodyPart();
                        }
                    }
                    Debug.Divider(4, "-", Count: 25, Indent: 5);
                }
            }

            Creature.RequirePart<Calming>();
            Debug.LoopItem(4, $"<Calming>?", Good: Creature.HasPart<Calming>(), Indent: 2);

            if (!Creature.TryGetPart(out GivesRep givesRep))
            {
                givesRep = Creature.RequirePart<GivesRep>();
            }
            givesRep.repValue = Unique ? 400 : 200;
            Debug.LoopItem(4, 
                $"<GivesRep>?", 
                $"{(givesRep?.repValue != null ? givesRep.repValue : "")}",
                Good: Creature.HasPart<GivesRep>(), 
                Indent: 2);


            Debug.CheckYeh(4, $"Remove Problem Parts", Indent: 2);
            if (Creature.TryGetPart(out GreaterVoider greaterVoider))
            {
                Debug.CheckYeh(4, $"Removed {nameof(GreaterVoider)}", Indent: 3);
                Creature.RemovePart(greaterVoider);
            }
            if (Creature.TryGetPart(out Rummager rummager))
            {
                Debug.CheckYeh(4, $"Removed {nameof(Rummager)}", Indent: 3);
                Creature.RemovePart(rummager);
            }
            if (Creature.TryGetPart(out Breeder breeder))
            {
                Debug.CheckYeh(4, $"Removed {nameof(Breeder)}", Indent: 3);
                Creature.RemovePart(breeder);
            }
                
            if (Creature.TryGetPart(out DisplayNameAdjectives displayNameAdjectives))
            {
                if (displayNameAdjectives.AdjectiveList.Contains(Gigantified.GetNamePrefix()))
                {
                    Debug.CheckYeh(4, $"Removed {Gigantified.GetNamePrefix()} from {nameof(DisplayNameAdjectives)}", Indent: 2);
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
                    Indent: 2);
            }
            else
            {
                Debug.CheckNah(4,
                    $"<ConversationScript>?",
                    $"Not yet written for non-unique WrassleGiants",
                    Indent: 2);
            }

            Creature.AddSkills(HeroSkills);
            if (Unique) 
                Creature.AddSkills(UniqueHeroSkills);

            Debug.CheckYeh(4, $"Skills Added", Indent: 2);
            foreach (BaseSkill skill in Creature.GetPartsDescendedFrom<BaseSkill>())
            {
                Debug.LoopItem(4, $" {skill.Name}", Indent: 3);
            }

            Creature.RequirePart<Interesting>();
            Debug.LoopItem(4, $"<Interesting>?", Good: Creature.HasPart<Interesting>(), Indent: 2);

            if (!Creature.TryGetPart(out Description description))
            {
                description = Creature.RequirePart<Description>();
            }
            string creatureNoun = $"{Creature?.GetCreatureType() ?? Creature?.GetBlueprint()?.DisplayName() ?? "creature"}".Color("y");
            string creatureArticle = Grammar.IndefiniteArticle(creatureNoun);
            creatureArticle = Unique ? creatureArticle.Capitalize() : creatureArticle;
            string preDesc = Unique ? SCRT_GNT_UNQ_PREDESC : GNT_PREDESC;
            preDesc = preDesc.Replace(GNT_PREDESC_RPLC, $"{creatureArticle} {creatureNoun}");
            description.Short = preDesc + description._Short;

            Debug.LoopItem(4, 
                $"<Description>?", 
                Good: description.Short.Contains($"{creatureArticle} {creatureNoun}"), 
                Indent: 2);
        }

        public static GameObjectBlueprint GetGiantEligibleBlueprint(Predicate<GameObjectBlueprint> filter = null, bool Old = false, bool Unique = false)
        {
            
            GameObjectBlueprint creatureObjectBlueprint = 
                EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => IsWrassleGiantEligible(blueprint, filter, Old, Unique));
            GameObjectBlueprint alternateCreatureObjectBlueprint = null;
            
            int chance = Unique || Old ? 10 : 5;
            if (chance.in1000())
            {
                alternateCreatureObjectBlueprint = GameObjectFactory.Factory.GetBlueprint("Aleksh_TrollHero");
                alternateCreatureObjectBlueprint?.Builders.Remove("TrollHero1");
            }

            return alternateCreatureObjectBlueprint ?? creatureObjectBlueprint;
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
            GamePartBlueprint GameUniquePartBlueprint = new("XRL.World.Parts", nameof(GameUnique))
            {
                Name = nameof(GameUnique),
            };
            if (GameUniquePartBlueprint != null)
            {
                if (GameUniquePartBlueprint.Parameters.IsNullOrEmpty()) GameUniquePartBlueprint.Parameters = new();
                GameUniquePartBlueprint.Parameters.TryAdd("State", SCRT_GNT_UNQ_STATE);
            }
            
            if (CreatureBlueprint.HasPart(nameof(GameUnique)))
            {
                CreatureBlueprint.RemovePart(nameof(GameUnique));
            }
            CreatureBlueprint.Parts.TryAdd(nameof(GameUnique), GameUniquePartBlueprint);

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
            GameObject @object;
            if (GameObjectFactory.Factory.Blueprints.TryGetValue(wishResult.Result, out GameObjectBlueprint blueprint))
            {
                GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(WrassleGiantHero))
                {
                    Name = nameof(WrassleGiantHero),
                    ChanceOneIn = 1
                };
                blueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;
                @object = GameObjectFactory.Factory.CreateObject(blueprint, 0, 0, null, null, null, "Wish");
            }
            else
            {
                @object = GameObjectFactory.Factory.CreateObject(wishResult.Result, 0, 0, null, null, null, "Wish");

                WrassleGiantHeroBuilder.Apply(@object, "Hero");
                @object.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);
            }
            if (@object != null)
            {
                The.PlayerCell.getClosestEmptyCell().AddObject(@object);
            }
        }
    } //!-- public class WrassleGiantHero : IObjectBuilder
} //!-- namespace XRL.World.ObjectBuilders
