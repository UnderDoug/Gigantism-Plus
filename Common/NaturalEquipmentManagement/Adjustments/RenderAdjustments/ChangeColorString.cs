using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeColorString : RenderAdjustment
    {
        public string ColorString;

        public ChangeColorString()
            : base()
        {
            ColorString = null;
        }

        public ChangeColorString(string TileColor = null)
            : this()
        {
            this.ColorString = TileColor;
        }
        public ChangeColorString(ChangeColorString Source)
            : base(Source)
        {
        }
        public ChangeColorString(ChangeTileColor Source)
            : base(Source)
        {
            ColorString = Source.TileColor;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.ColorString = ColorString;
                return true;
            }
            return false;
        }
    }
}
