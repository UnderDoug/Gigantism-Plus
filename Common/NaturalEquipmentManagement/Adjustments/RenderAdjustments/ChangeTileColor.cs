using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
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
        public ChangeTileColor(ChangeTileColor Source)
            : base(Source)
        {
        }
        public ChangeTileColor(ChangeDetailColor Source)
            : base(Source)
        {
            Value = Source.DetailColor;
        }
        public ChangeTileColor(Render Source)
            : this(Source.TileColor)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.TileColor = Value;
                return true;
            }
            return false;
        }
    }
}
