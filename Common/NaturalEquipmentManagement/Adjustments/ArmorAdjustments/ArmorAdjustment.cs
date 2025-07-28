using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ArmorAdjustment : IAdjustment
    {
        public ArmorAdjustment()
            : base()
        {
        }
        public ArmorAdjustment(ArmorAdjustment Source)
            : base(Source)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {GetType().Name}.{nameof(Apply)}()", Indent: indent + 1, Toggle: true);

            bool baseApply = Subject.HasPart<Armor>() && base.Apply(Subject);

            Debug.Entry(4, $"x {GetType().Name}.{nameof(Apply)}() *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return baseApply;
        }
    }
}
