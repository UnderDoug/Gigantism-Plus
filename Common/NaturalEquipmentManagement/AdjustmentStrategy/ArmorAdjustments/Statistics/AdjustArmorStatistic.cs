using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class AdjustArmorStatistic : ArmorCumulativeAdjustment
    {
        public AdjustArmorStatistic()
            : base()
        {
        }
        public AdjustArmorStatistic(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public AdjustArmorStatistic(int Amount, string AffectedParameter = null)
            : this(AffectedParameter)
        {
            this.Amount = Amount;
        }
        public AdjustArmorStatistic(AdjustArmorStatistic Source)
            : base(Source)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Verb = "confer";
        }

        public override bool Check(GameObject Subject)
        {
            return !Amount.IsNullOrZero() 
                && base.Check(Subject);
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {bonusPenalty} of {amount} {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetGeneralDescriptionElement(Subject);
        }
    }
}
