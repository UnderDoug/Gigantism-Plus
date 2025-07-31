using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ChangeRenderDisplayName : RenderAdjustment
    {
        public ChangeRenderDisplayName()
            : base()
        {
        }

        public ChangeRenderDisplayName(string DisplayName = null)
            : this()
        {
            Value = DisplayName;
        }
        public ChangeRenderDisplayName(ChangeRenderDisplayName SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public ChangeRenderDisplayName(Render Source)
            : this(Source.DisplayName)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return Subject.Render.DisplayName != Value 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.DisplayName = Value;
            }
            return IsApplied();
        }
    }
}
