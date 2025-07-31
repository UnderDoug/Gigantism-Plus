using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ChangeColorString : RenderAdjustment
    {
        public ChangeColorString()
            : base()
        {
        }
        public ChangeColorString(string TileColor = null)
            : this()
        {
            Value = TileColor;
        }
        public ChangeColorString(ChangeColorString Source)
            : base(Source)
        {
        }
        public ChangeColorString(ChangeTileColor Source)
            : base(Source)
        {
            Value = Source.Value;
        }
        public ChangeColorString(Render Source)
            : this(Source.ColorString)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return Subject.Render.ColorString != Value 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.ColorString = Value;
            }
            return IsApplied();
        }
    }
}
