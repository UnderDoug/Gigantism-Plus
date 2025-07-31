using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class RenderAdjustment : IAdjustment
    {
        public RenderAdjustment()
            : base()
        {
        }
        public RenderAdjustment(RenderAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return Subject.Render != null 
                && !Value.IsNullOrEmpty() 
                && base.Check(Subject);
        }
    }
}
