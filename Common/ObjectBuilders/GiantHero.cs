using System;
using System.Collections.Generic;
using System.Text;

using XRL.Names;
using XRL.Rules;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.ObjectBuilders;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Debug;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    public class GiantHero : IObjectBuilder
    {
        public static List<string> NoHateFactionsList => GNT_NOHATEFACTION_BOOK.BookPagesAsList();
        public static List<string> FactionAdmirationBag => GNT_ADMIREREASON_BOOK.BookPagesAsList();
        public static List<string> ThiccBoisBag => GNT_THICCBOI_BOOK.BookPagesAsList();

        public override void Apply(GameObject Creature, string Context)
        {
            Context ??= "Hero";

            Creature.SetStringProperty("Culture", "Giant");

            Creature.SetStringProperty("Role", "Hero");

            Creature.SetStringProperty("staticFaction1", null);
            Creature.SetStringProperty("staticFaction2", null);
            Creature.SetStringProperty("staticFaction3", null);

            int MentalMutations = 0;
            int PhysicalMutations = 0;
            bool MakeChimera = false;
            string Epithet = NameMaker.MakeEpithet(
                For: Creature,
                Genotype: null,
                Subtype: null,
                Species: "Giant",
                Culture: "Giant",
                Faction: "Giants",
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
                Species: "Giant",
                Culture: "Giant",
                Faction: "Giants",
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

            Creature.RequirePart<DisplayNameColor>().SetColorByPriority("yuge", 40);

            int Stews = Stat.Roll("2d4");

            if (!Epithet.IsNullOrEmpty())
            {
                Creature.RequirePart<Epithets>().Primary = Epithet;

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

            Creature.MultiplyStat("Hitpoints", 2);

            Creature.AddBaseStat("XPValue", Stews * 75);

            for (int i = 0; i < Stews; i++)
            {
                Creature.AddBaseStat("Hitpoints", Stat.Roll("1d10"));
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
                    mutations.AddMutation(randomPhysicalMutation, Stat.Roll(MentalMutationLevelDie));
                    if (mutations.HasMutation("Chimera") && "1d7".RollCached() > 4) mutations.AddChimericBodyPart();
                }
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
            string factionAdmiration1 = factionAdmiration.DrawRandomToken();
            // Get a bag of different height/size/weight related reasons, draw a random one each
            Creature.SetStringProperty("staticFaction1",
                $",friend,{factionAdmiration1}");

            string staticFaction2Faction = string.Empty;
            string staticFaction2Admiration = factionAdmiration.DrawRandomToken();
            if ("1d4".Roll().Is(4))
            {
                staticFaction2Faction = ThiccBoisBag.GetRandomElement();
                staticFaction2Admiration = GNT_THICCBOI_ADMIREREASON;
            }
            Creature.SetStringProperty("staticFaction2", $"{staticFaction2Faction},friend,{staticFaction2Admiration}");

            Creature.RandomlySpendPoints();
        }
    }
}
