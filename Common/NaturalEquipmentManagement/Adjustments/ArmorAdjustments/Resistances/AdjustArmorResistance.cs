using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class AdjustArmorResistance : ArmorCumulativeAdjustment
    {
        public AdjustArmorResistance()
            : base()
        {
        }
        public AdjustArmorResistance(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public AdjustArmorResistance(int Amount, string AffectedParameter = null)
            : this(AffectedParameter)
        {
            this.Amount = Amount;
        }
        public AdjustArmorResistance(AdjustArmorResistance SourceAdjustment)
            : base(SourceAdjustment)
        {
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
