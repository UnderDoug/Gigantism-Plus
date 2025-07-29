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
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {GetType().Name}.{nameof(Apply)}()", Indent: indent + 1, Toggle: true);

            bool baseApply = Subject.HasPart<MeleeWeapon>() && base.Apply(Subject);

            Debug.Entry(4, $"x {GetType().Name}.{nameof(Apply)}() *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return baseApply;
        }
    }
}
