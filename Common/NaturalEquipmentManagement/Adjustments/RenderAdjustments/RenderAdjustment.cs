using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class RenderAdjustment : IAdjustment
    {
        public RenderAdjustment()
            : base()
        {
        }
        public RenderAdjustment(RenderAdjustment Source)
            : base(Source)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            return Subject.Render != null && base.Apply(Subject);
        }
    }
}
