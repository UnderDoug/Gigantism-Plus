using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ChangeTileColor : RenderAdjustment
    {
        public ChangeTileColor()
            : base()
        {
        }

        public ChangeTileColor(string TileColor = null)
            : this()
        {
            Value = TileColor;
        }
        public ChangeTileColor(ChangeTileColor SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public ChangeTileColor(ChangeDetailColor SourceAdjustment)
            : base(SourceAdjustment)
        {
            Value = SourceAdjustment.Value;
        }
        public ChangeTileColor(Render Source)
            : this(Source.TileColor)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && Subject.Render.TileColor != Value;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.TileColor = Value;
            }
            return IsApplied();
        }
    }
}
