using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustPenBonus : MeleeWeaponPerformanceAdjustment
    {
        public AdjustPenBonus()
            : base("penetration")
        {
        }
        public AdjustPenBonus(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustPenBonus(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustPenBonus(MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "penetration";
        }
        public AdjustPenBonus(int Amount, MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustPenBonus(MeleeWeapon Source)
            : this(Source.PenBonus)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().PenBonus += (int)Amount;
            }
            return IsApplied();
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {AffectedParameter} {bonusPenalty}";
                return new(DescriptionElement.ORDER_ADJUST_EXTREMELY_EARLY + 5, Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }
    }
}
