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
        public MeleeWeaponAdjustment(MeleeWeaponAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject) 
                && Subject.HasPart<MeleeWeapon>();
        }
    }
}
