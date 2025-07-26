using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustBonusCap : MeleeWeaponCumulativeAdjustment
    {
        public AdjustBonusCap()
            : base()
        {
        }
        public AdjustBonusCap(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustBonusCap(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustBonusCap(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                MeleeWeapon meleeWeapon = Subject.GetPart<MeleeWeapon>();
                if (meleeWeapon.MaxStrengthBonus < 999)
                {
                    meleeWeapon.AdjustBonusCap(Amount);
                }
                return true;
            }
            return false;
        }
    }
}
