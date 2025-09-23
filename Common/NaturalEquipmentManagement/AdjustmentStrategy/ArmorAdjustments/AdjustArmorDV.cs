using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorDV : ArmorCumulativeAdjustment
    {
        public AdjustArmorDV()
            : base(nameof(Armor.DV))
        {
        }
        public AdjustArmorDV(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorDV(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.DV);
        }
        public AdjustArmorDV(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(Armor Source)
            : this(Source.DV)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return !Amount.IsNullOrZero()
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().DV += (int)Amount;
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {bonusPenalty} to {AffectedParameter}";
                return new(DescriptionElement.ORDER_ADJUST_SLIGHTLY_LATE + 2, Verb, Effect);
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject = null)
        {
            return default;
        }
    }
}
