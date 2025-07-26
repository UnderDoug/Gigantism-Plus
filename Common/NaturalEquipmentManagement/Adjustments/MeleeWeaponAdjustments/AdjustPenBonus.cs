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
            : base()
        {
        }
        public AdjustPenBonus(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustPenBonus(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustPenBonus(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().PenBonus += Amount;
                return true;
            }
            return false;
        }
    }
}
