using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public abstract class MeleeWeaponCumulativeAdjustment : MeleeWeaponAdjustment
    {
        public MeleeWeaponCumulativeAdjustment()
            : base()
        {
            Prioritize = false;
            Amount = 0;
        }
        public MeleeWeaponCumulativeAdjustment(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            Prioritize = false;
            Amount = Source.Amount;
        }
        public MeleeWeaponCumulativeAdjustment(int Amount, MeleeWeaponAdjustment Source)
            : base(Source)
        {
            Prioritize = false;
            this.Amount = Amount;
        }
    }
}
