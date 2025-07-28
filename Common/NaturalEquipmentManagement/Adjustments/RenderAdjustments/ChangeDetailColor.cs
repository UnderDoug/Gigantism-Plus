using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeDetailColor : RenderAdjustment
    {
        public ChangeDetailColor()
            : base()
        {
        }

        public ChangeDetailColor(string DetailColor = null)
            : this()
        {
            Value = DetailColor;
        }
        public ChangeDetailColor(ChangeDetailColor SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public ChangeDetailColor(ChangeTileColor SourceAdjustment)
            : base(SourceAdjustment)
        {
            Value = SourceAdjustment.Value;
        }
        public ChangeDetailColor(Render Source)
            : this(Source.DetailColor)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.DetailColor = Value;
                return true;
            }
            return false;
        }
    }
}
