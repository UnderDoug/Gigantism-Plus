using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeTile : RenderAdjustment
    {
        public ChangeTile()
            : base()
        {
        }

        public ChangeTile(string Tile = null)
            : this()
        {
            Value = Tile;
        }
        public ChangeTile(ChangeTile Source)
            : base(Source)
        {
        }
        public ChangeTile(Render Source)
            : this(Source.Tile)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.Tile = Value;
                return true;
            }
            return false;
        }
    }
}
