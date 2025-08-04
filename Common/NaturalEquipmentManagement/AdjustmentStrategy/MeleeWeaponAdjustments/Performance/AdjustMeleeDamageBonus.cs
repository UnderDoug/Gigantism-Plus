using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustMeleeDamageBonus : MeleeWeaponCumulativeAdjustment
    {
        public AdjustMeleeDamageBonus()
            : base("damage")
        {
        }
        public AdjustMeleeDamageBonus(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustMeleeDamageBonus(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustMeleeDamageBonus(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage";
        }
        public AdjustMeleeDamageBonus(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamage((int)Amount);
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
                return new(DescriptionElement.ORDER_ADJUST_EXTREMELY_EARLY + 3, Verb, Effect);
            }
            return base.GetPrimaryDescriptionElement(Subject);
        }
    }
}
