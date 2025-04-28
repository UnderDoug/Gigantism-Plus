using System;
using SerializeField = UnityEngine.SerializeField;

using XRL.UI;
using XRL.World.Parts.Mutation;
using XRL.World.Skills.Cooking;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class StewBelly : IScribedPart
    {
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

        [SerializeField]
        private int _Gains;
        public int Gains // number of bonus GigantismPlus levels awarded.
        {
            get => _Gains;
            private set => _Gains = Math.Max(0, value);
        }

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

        public Guid mutationMod = Guid.Empty;

        public bool Grumble;
        public int TurnsTillGrumble;
        public bool StartingStewsPocessed { get; private set; }

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
            RemoveMutationMod(ParentObject, mutationMod);
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
        }
        public static Guid RemoveMutationMod(GameObject Object, Guid mutationMod)
        {
            if (Object == null) return Guid.Empty;
            Mutations mutations = Object.RequirePart<Mutations>();
            if (mutationMod != Guid.Empty)
            {
                mutations.RemoveMutationMod(mutationMod);
            }
            mutationMod = Guid.Empty;
            return mutationMod;
        }
        public Guid RemoveMutationMod()
        {
            return RemoveMutationMod(ParentObject, mutationMod);
        }

        public override void AddedAfterCreation()
        {
            RemoveMutationMod();
            base.AddedAfterCreation();
        }
        public override void Attach()
        {
            base.Attach();
            RemoveMutationMod();
        }
        public override void Remove()
        {
            RemoveMutationMod(ParentObject, mutationMod);
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

        public int GetTurnsTillGrumble()
        {
            return RndGP.Next(1200, 8000);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == EndTurnEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || (!StartingStewsPocessed && ID == ObjectEnteredCellEvent.ID);
        }
        public override bool HandleEvent(EndTurnEvent E)
        {
            if (Grumble)
            {
                DidX("a serious hankering", "got", "!");
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
            if (int.TryParse(ParentObject.GetPropertyOrTag(GNT_START_STEWS_PROPLABEL, "0"), out int startingSews) && startingSews > 0)
            {
                if (Stews <  startingSews)
                    Stews += startingSews;
            }
            // ParentObject.SetIntProperty(GNT_START_STEWS_PROPLABEL, 0, true);
            Stews = Stews;
            StartingStewsPocessed = Stews >= startingSews;
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
            stewBelly.Stews = 0;
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
