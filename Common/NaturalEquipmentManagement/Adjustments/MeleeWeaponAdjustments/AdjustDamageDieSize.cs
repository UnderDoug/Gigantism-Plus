using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustDamageDieSize : MeleeWeaponCumulativeAdjustment
    {
        public AdjustDamageDieSize()
            : base()
        {
        }
        public AdjustDamageDieSize(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustDamageDieSize(MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustDamageDieSize(int Amount, MeleeWeaponCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieSize((int)Amount);
                return true;
            }
            return false;
        }
    }
}
