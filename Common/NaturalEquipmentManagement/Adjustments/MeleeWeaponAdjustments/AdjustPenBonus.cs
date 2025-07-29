using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustPenBonus : MeleeWeaponCumulativeAdjustment
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
        public AdjustPenBonus(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "penetration";
        }
        public AdjustPenBonus(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustPenBonus(MeleeWeapon Source)
            : this(Source.PenBonus)
        {
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {AffectedParameter} {bonusPenalty}";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && !Amount.IsNullOrZero())
            {
                Subject.GetPart<MeleeWeapon>().PenBonus += (int)Amount;
                return true;
            }
            return false;
        }
    }
}
