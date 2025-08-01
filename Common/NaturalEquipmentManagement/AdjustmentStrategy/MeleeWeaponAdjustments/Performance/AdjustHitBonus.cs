using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustHitBonus : MeleeWeaponPerformanceAdjustment
    {
        public AdjustHitBonus()
            : base("hit")
        {
        }
        public AdjustHitBonus(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustHitBonus(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustHitBonus(MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "hit";
        }
        public AdjustHitBonus(int Amount, MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustHitBonus(MeleeWeapon Source)
            : this(Source.HitBonus)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().HitBonus += (int)Amount;
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
                return new(DescriptionElement.ORDER_ADJUST_EXTREMELY_EARLY + 4, Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }
    }
}
