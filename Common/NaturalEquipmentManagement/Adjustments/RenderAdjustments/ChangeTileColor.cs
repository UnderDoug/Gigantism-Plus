using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeTileColor : RenderAdjustment
    {
        public string TileColor;

        public ChangeTileColor()
            : base()
        {
            TileColor = null;
        }

        public ChangeTileColor(string TileColor = null)
            : this()
        {
            this.TileColor = TileColor;
        }
        public ChangeTileColor(ChangeTileColor Source)
            : base(Source)
        {
        }
        public ChangeTileColor(ChangeDetailColor Source)
            : base(Source)
        {
            TileColor = Source.DetailColor;
        }
        public ChangeTileColor(Render Source)
            : this(Source.TileColor)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.TileColor = TileColor;
                return true;
            }
            return false;
        }
    }
}
