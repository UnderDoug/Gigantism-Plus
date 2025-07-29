using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustDamageBonus : MeleeWeaponCumulativeAdjustment
    {
        public AdjustDamageBonus()
            : base("damage")
        {
        }
        public AdjustDamageBonus(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustDamageBonus(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustDamageBonus(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage";
        }
        public AdjustDamageBonus(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamage((int)Amount);
                return true;
            }
            return false;
        }
    }
}
