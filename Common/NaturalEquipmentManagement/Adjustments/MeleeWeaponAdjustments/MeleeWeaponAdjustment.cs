using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class MeleeWeaponAdjustment : IAdjustment
    {
        public MeleeWeaponAdjustment()
            : base()
        {
        }
        public MeleeWeaponAdjustment(MeleeWeaponAdjustment Source)
            : base(Source)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            return Subject.HasPart<MeleeWeapon>() && base.Apply(Subject);
        }
    }
}
