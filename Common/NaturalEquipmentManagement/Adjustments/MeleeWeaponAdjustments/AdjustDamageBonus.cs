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
            : base()
        {
        }
        public AdjustDamageBonus(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustDamageBonus(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustDamageBonus(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamage(Amount);
                return true;
            }
            return false;
        }
    }
}
