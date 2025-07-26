using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeRenderDisplayName : RenderAdjustment
    {
        public string DisplayName;

        public ChangeRenderDisplayName()
            : base()
        {
            DisplayName = null;
        }

        public ChangeRenderDisplayName(string DisplayName = null)
            : this()
        {
            this.DisplayName = DisplayName;
        }
        public ChangeRenderDisplayName(ChangeRenderDisplayName Source)
            : base(Source)
        {
        }
        public ChangeRenderDisplayName(Render Source)
            : this(Source.DisplayName)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.DisplayName = DisplayName;
                return true;
            }
            return false;
        }
    }
}
