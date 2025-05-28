using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.Rules;
using XRL.UI;
using XRL.Wish;
using XRL.World.Parts.Mutation;
using XRL.World.Skills.Cooking;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class StewBelly : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(StewBelly));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
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

        [SerializeField]
        private int _Stews;
        public int Stews // total number of times SeriouslyThickStew has been consumed.
        {
            get => _Stews;
            set 
            {
                _Stews = value;
                CalculateGainsAndHankering(value, out int Gains, out int Hankering);
                if (this.Gains != Gains)
                {
                    this.Gains = Gains;
                    OnGained();
                }
                this.Hankering = Hankering;
            }
        }
        public bool StartingStewsPocessed { get; private set; }

        [SerializeField]
        private int _Gains;
        public int Gains // number of bonus GigantismPlus levels awarded.
        {
            get => _Gains;
            private set => _Gains = Math.Max(0, value);
        }
        public Guid mutationMod = Guid.Empty;

        [SerializeField]
        private int _StartingHankering;
        public int StartingHankering
        {
            get => _StartingHankering != 0 ? _StartingHankering : 2;
            set 
            {
                _StartingHankering = value;
                Hankering += _StartingHankering;
            }
        }

        [SerializeField]
        private int _Hankering;
        public int Hankering // number of additional SeriouslyThickStews need to be consumed to get a Gain.
        {
            get => _Hankering;
            private set => _Hankering = Math.Max(0, value);
        }

        public bool Grumble;
        public int TurnsTillGrumble;

        public StewBelly()
        {
            Stews = 0;
            Gains = 0;
            StartingHankering = 2;
            Hankering = StartingHankering;
            Grumble = false;
            TurnsTillGrumble = GetTurnsTillGrumble();
            StartingStewsPocessed = false;
        }
        public StewBelly(int StartingHankering)
            : this()
        {
            this.StartingHankering = StartingHankering;
            Hankering = this.StartingHankering;
        }

        public int GetNextHankering()
        {
            return Gains + 1;
        }
        public int GetLastHankering()
        {
            return Gains;
        }

        public void OnGained()
        {   
            RemoveMutationMod(ParentObject, ref mutationMod);
            Mutations mutations = ParentObject.RequirePart<Mutations>(); 
            if (Gains > 0)
            {
                mutationMod = mutations.AddMutationMod(
                    Mutation: typeof(GigantismPlus),
                    Variant: null,
                    Level: Gains,
                    SourceType: Mutations.MutationModifierTracker.SourceType.Unknown,
                    SourceName: $"{Stews.Things("Helping")} of {new SeriouslyThickStew().GetDisplayName()}");
            }
            ParentObject.CheckEquipmentSlots();
        }
        public static Guid RemoveMutationMod(GameObject Object, ref Guid mutationMod)
        {
            if (Object == null) return Guid.Empty;
            Mutations mutations = Object.RequirePart<Mutations>();
            if (mutationMod != Guid.Empty)
            {
                mutations.RemoveMutationMod(mutationMod);
            }
            mutationMod = Guid.Empty;
            Object.CheckEquipmentSlots();
            return mutationMod;
        }
        public Guid RemoveMutationMod()
        {
            return RemoveMutationMod(ParentObject, ref mutationMod);
        }

        public override void AddedAfterCreation()
        {
            mutationMod = RemoveMutationMod(ParentObject, ref mutationMod);
            foreach (IPart part in ParentObject.PartsList)
            {
                if (part.Name == nameof(StewBelly) && this != part as StewBelly)
                {
                    StewBelly stewBelly = part as StewBelly;
                    stewBelly.mutationMod = RemoveMutationMod(ParentObject, ref stewBelly.mutationMod);
                    ParentObject.RemovePart(stewBelly);
                    StartingStewsPocessed = false;
                    ProcessStartingStews();
                }
            }
            base.AddedAfterCreation();
        }
        public override void Attach()
        {
            foreach (IPart part in ParentObject.PartsList)
            {
                if (part.Name == nameof(StewBelly) && this != part as StewBelly)
                {
                    StewBelly stewBelly = part as StewBelly;
                    stewBelly.mutationMod = RemoveMutationMod(ParentObject, ref stewBelly.mutationMod);
                    ParentObject.RemovePart(stewBelly);
                    StartingStewsPocessed = false;
                    ProcessStartingStews();
                }
            }
            base.Attach();
        }
        public override void Remove()
        {
            mutationMod = RemoveMutationMod(ParentObject, ref mutationMod);
            base.Remove();
        }

        public static void CalculateGainsAndHankering(int Stews, out int Gains, out int Hankering)
        {
            Gains = 0;
            Hankering = 1;
            while (Stews > 0)
            {
                int leftOvers = Stews - Hankering;
                Hankering -= Stews;
                if (Hankering <= 0) Hankering = ++Gains;
                Stews = Math.Max(0,leftOvers);
            };
        }
        public int CalculateGains()
        {
            CalculateGainsAndHankering(Stews, out int Gains, out _);
            return Gains;
        }
        public int CalculateHankering()
        {
            CalculateGainsAndHankering(Stews, out _, out int Hankering);
            return Hankering;
        }

        public static int CalculateStews(int Gains, int Hankering = 0, int StartingHankering = 2)
        {
            int stews = Hankering > 0 ? Hankering : 0;
            while (Gains > 0)
            {
                stews += Gains--;
            }
            stews += StartingHankering - 1;
            return stews;
        }

        public int CalculateStews()
        {
            return CalculateStews(Gains, Hankering, StartingHankering);
        }

        public void SatiateHankering()
        {
            Stews += Hankering;
        }

        public void EatStew()
        {
            Stews++;
        }
        public bool ProcessStartingStews()
        {
            bool did = false;
            string StartingStewsProperty = ParentObject.GetPropertyOrTag(GNT_START_STEWS_PROPLABEL, "0");
            int StartingStews = 0;
            if (StartingStewsProperty.Contains("-"))
            {
                if (StartingStewsProperty.StartsWith("-") || StartingStewsProperty.EndsWith("-"))
                {
                    StartingStewsProperty.Replace("-", "");
                }
                else
                {
                    string[] startingStewsPieces = StartingStewsProperty.Split("-");
                    int Low = Math.Min(int.Parse(startingStewsPieces[0]), int.Parse(startingStewsPieces[1]));
                    int High = Math.Max(int.Parse(startingStewsPieces[0]), int.Parse(startingStewsPieces[1]));
                    StartingStewsProperty = $"{Stat.Roll(Low, High)}";
                }
            }
            if (StartingStewsProperty.Contains("d"))
            {
                if (StartingStewsProperty.StartsWith("d") || StartingStewsProperty.EndsWith("d"))
                {
                    StartingStewsProperty.Replace("d", "");
                }
                else
                {
                    StartingStewsProperty = $"{Stat.Roll(StartingStewsProperty)}";
                }
            }
            if (int.TryParse(StartingStewsProperty, out int startingStews))
            {
                StartingStews = startingStews;
            }
            if (StartingStews > 0)
            {
                if (Stews < StartingStews)
                    Stews += StartingStews;
                did = true;
            }
            StartingStewsPocessed = Stews >= StartingStews;
            return StartingStewsPocessed && did;
        }

        public int GetTurnsTillGrumble()
        {
            return RndGP.Next(1200, 8000);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == EndTurnEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || ((!StartingStewsPocessed || Stews <= 0) && ID == ObjectEnteredCellEvent.ID);
        }
        public override bool HandleEvent(EndTurnEvent E)
        {
            if (Grumble)
            {
                DidX("seriously hanker", "for more stew", "!", $"{Grammar.MakePossessive(ParentObject.ShortDisplayName)} Stew Belly", UseVisibilityOf: ParentObject);
                int nearness = 7;
                if (!ParentObject.IsPlayer())
                {
                    nearness -= Math.Max(0, ParentObject.CurrentCell.CosmeticDistanceto(The.Player.CurrentCell.Location));
                }
                if (ParentObject.IsVisible() || ParentObject.IsPlayer())
                {
                    Rumble(nearness, 0.2f);
                }
                Grumble = false;
                TurnsTillGrumble = GetTurnsTillGrumble();
            }
            else
            {
                if (--TurnsTillGrumble == 0) Grumble = true;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            Debug.Entry(4,
                $"{nameof(StewBelly)}" + 
                $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E)", 
                Indent: 0);

            Debug.Entry(4, $"Stews", $"{Stews}", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"StartingStewsPocessed", $"{StartingStewsPocessed}", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"StartingHankering", $"{StartingHankering}", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"Hankering", $"{Hankering}", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"Gains", $"{Gains}", Indent: 1, Toggle: getDoDebug());
            Debug.Entry(4, $"mutationMod", $"{mutationMod}", Indent: 1, Toggle: getDoDebug());

            if (Stews > 0)
            {
                if (!E.Postfix.IsNullOrEmpty())
                {
                    E.Postfix.AppendLine();
                }
                E.Postfix.AppendRules(
                    $"{"Stew Belly".OptionalColorYuge(Colorfulness)}: " + 
                    $"This creature has achieved {Gains.Things("Gain")} " 
                    + $"from the {Stews.Things("hepling")} of {new SeriouslyThickStew().GetDisplayName()} they've eaten! " + 
                    $"Talk about a hankering!");
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ObjectEnteredCellEvent E)
        {
            ProcessStartingStews();
            return base.HandleEvent(E);
        }

        public void Slap()
        {
            if (ParentObject != null && ParentObject.IsPlayer())
            {
                int stews = Math.Max(1, Stews);
                Rumble(stews, 0.2f);
                if (Stews < 1)
                    Popup.Show(
                        $"You slap your belly. It will hold much stew yet. It grumbles with a serious hankering. " +
                        $"You reckon {Hankering.Things("more helping")} to see any gains.");
                else
                    Popup.Show(
                        $"You slap your belly. It holds much stew. About {Stews.Things("helping")}. " +
                        $"You hanker for more, though! You reckon {Hankering.Things("more helping")} will see additional gains.");
            }
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            StewBelly stewBelly = base.DeepCopy(Parent, MapInv) as StewBelly;
            mutationMod = RemoveMutationMod(ParentObject, ref mutationMod);
            stewBelly.mutationMod = RemoveMutationMod(Parent, ref stewBelly.mutationMod);
            stewBelly.StartingStewsPocessed = false;
            return stewBelly;
        }

        [WishCommand(Command = "Slap Belly")]
        public static void SlapBellyWish()
        {
            StewBelly stewBelly = The.Player.RequirePart<StewBelly>();
            stewBelly.Slap();
        }
    }
}
