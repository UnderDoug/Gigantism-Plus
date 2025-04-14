using System;
using SerializeField = UnityEngine.SerializeField;

using XRL.Wish;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using XRL.World.Parts.Skill;
using System.Text.RegularExpressions;
using XRL.UI;
using XRL.Language;
using XRL.World.Capabilities;
using XRL.World.Skills.Cooking;
using XRL.Messages;

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
        private int _Hankering;
        public int Hankering // number of additional SeriouslyThickStews need to be consumed to get a Gain.
        {
            get => _Hankering;
            private set => _Hankering = Math.Max(0, value);
        }

        public Guid mutationMod = Guid.Empty;

        public bool Grumble;
        public int TurnsTillGrumble;

        public StewBelly()
        { 
            Stews = 0;
            Gains = 0;
            Hankering = 2;
            Grumble = false;
            TurnsTillGrumble = GetTurnsTillGrumble();
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
            Mutations mutations = ParentObject.RequirePart<Mutations>();
            if (mutationMod != Guid.Empty)
            {
                mutations.RemoveMutationMod(mutationMod);
            }
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

        public override void Remove()
        {
            Mutations mutations = ParentObject.RequirePart<Mutations>();
            if (mutationMod != Guid.Empty)
            {
                mutations.RemoveMutationMod(mutationMod);
            }
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

        public static int CalculateStews(int Gains, int Hankering = 0)
        {
            int stews = Hankering > 0 ? Hankering : 0;
            while (Gains > 0)
            {
                stews += Gains--;
            }
            return stews;
        }

        public int CalculateStews()
        {
            return CalculateStews(Gains, Hankering);
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
                || ID == EndTurnEvent.ID;
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

        public void Slap()
        {
            if(ParentObject != null && ParentObject.IsPlayer())
            {
                Popup.Show(
                    $"You slap your belly. It holds much stew. About {Stews.Things("helping")}. " + 
                    $"You hanker for more, though! You reckon {Hankering.Things("more helping")} will see additional gains.");
            }
        }

        [WishCommand(Command = "Slap Belly")]
        public static void SlapBellyWish()
        {
            StewBelly stewBelly = The.Player.RequirePart<StewBelly>();
            stewBelly.Slap();
        }
    }
}
