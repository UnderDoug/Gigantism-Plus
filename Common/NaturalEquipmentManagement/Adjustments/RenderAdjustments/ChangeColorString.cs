using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
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

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.ColorString = Value;
                return true;
            }
            return false;
        }
    }
}
