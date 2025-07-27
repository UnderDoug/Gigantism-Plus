using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Tilemaps;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
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
                Subject.Render.DisplayName = Value;
                return true;
            }
            return false;
        }
    }
}
