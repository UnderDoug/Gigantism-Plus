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
        public string DetailColor;

        public ChangeDetailColor()
            : base()
        {
            DetailColor = null;
        }

        public ChangeDetailColor(string DetailColor = null)
            : this()
        {
            this.DetailColor = DetailColor;
        }
        public ChangeDetailColor(ChangeDetailColor Source)
            : base(Source)
        {
        }
        public ChangeDetailColor(ChangeTileColor Source)
            : base(Source)
        {
            DetailColor = Source.TileColor;
        }
        public ChangeDetailColor(Render Source)
            : this(Source.DetailColor)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.DetailColor = DetailColor;
                return true;
            }
            return false;
        }
    }
}
