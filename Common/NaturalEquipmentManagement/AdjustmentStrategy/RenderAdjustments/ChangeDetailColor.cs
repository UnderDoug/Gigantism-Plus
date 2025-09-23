using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
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

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && Subject.Render.DetailColor != Value;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.DetailColor = Value;
            }
            return IsApplied();
        }
    }
}
