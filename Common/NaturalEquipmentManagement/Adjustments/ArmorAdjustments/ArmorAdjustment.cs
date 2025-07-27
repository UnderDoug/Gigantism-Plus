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
            return Subject.HasPart<Armor>() && base.Apply(Subject);
        }
    }
}
