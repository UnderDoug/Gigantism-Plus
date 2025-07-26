using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustDamageDieCount : MeleeWeaponCumulativeAdjustment
    {
        public AdjustDamageDieCount()
            : base()
        {
        }
        public AdjustDamageDieCount(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustDamageDieCount(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustDamageDieCount(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieCount(Amount);
                return true;
            }
            return false;
        }
    }
}
