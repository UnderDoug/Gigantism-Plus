using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustHitBonus : MeleeWeaponCumulativeAdjustment
    {
        public AdjustHitBonus()
            : base()
        {
        }
        public AdjustHitBonus(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustHitBonus(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustHitBonus(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().HitBonus += Amount;
                return true;
            }
            return false;
        }
    }
}
