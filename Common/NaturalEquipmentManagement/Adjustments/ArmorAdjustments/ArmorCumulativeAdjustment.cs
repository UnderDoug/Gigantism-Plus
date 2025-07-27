using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public abstract class ArmorCumulativeAdjustment : ArmorAdjustment
    {
        public bool NeedsShifter;

        public string Verb;

        public string AffectedStat;

        public string Effect;

        public ArmorCumulativeAdjustment()
            : base()
        {
            Prioritize = false;
            NeedsShifter = true;
            Amount = 0;
            Verb = "have";
            AffectedStat = null;
            Effect = null;
        }
        public ArmorCumulativeAdjustment(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            Prioritize = false;
            Amount = Source.Amount;
        }
        public ArmorCumulativeAdjustment(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            Prioritize = false;
            this.Amount = Amount;
        }

        /*
        public override DescriptionElement GetGeneralDescriptionElement()
        {
            if (AffectedStat != null)
            {
                Effect = $"";
            }
            return new(Verb, Effect);
        }
        */

        public override bool AfterApply(GameObject Subject)
        {
            if (NeedsShifter)
            {
                Subject?.GetPart<Armor>()?.UpdateStatShifts();
            }
            return base.AfterApply(Subject);
        }
    }
}
