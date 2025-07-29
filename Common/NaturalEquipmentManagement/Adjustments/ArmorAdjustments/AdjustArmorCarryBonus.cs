using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorCarryBonus : ArmorCumulativeAdjustment
    {
        public AdjustArmorCarryBonus()
            : base("Carry Capacity")
        {
        }
        public AdjustArmorCarryBonus(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorCarryBonus(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorCarryBonus(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "Carry Capacity";
        }
        public AdjustArmorCarryBonus(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorCarryBonus(Armor Source)
            : this(Source.CarryBonus)
        {
        }

        public override void Configure()
        {
            base.Configure();
            NeedsShifter = false;
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount}% {bonusPenalty} to {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetGeneralDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && !Amount.IsNullOrZero())
            {
                Subject.GetPart<Armor>().CarryBonus += (int)Amount;
                return true;
            }
            return false;
        }

        public override void AfterApply(GameObject Subject)
        {
            if (Subject.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator)
                && naturalEquipmentOperator.Wielder is GameObject wielder)
            {
                wielder.FlushCarriedWeightCache();
            }
            base.AfterApply(Subject);
        }
    }
}
