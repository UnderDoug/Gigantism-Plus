using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class MeleeWeaponAdjustment : IAdjustment
    {
        public MeleeWeaponAdjustment()
            : base()
        {
        }
        public MeleeWeaponAdjustment(MeleeWeaponAdjustment Source)
            : base(Source)
        {
        }
        public override bool Check(GameObject Subject)
        {
            return Subject.HasPart<MeleeWeapon>() 
                && base.Check(Subject);
        }
    }
}
