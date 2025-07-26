using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeTile : RenderAdjustment
    {
        public string Tile;

        public ChangeTile()
            : base()
        {
            Tile = null;
        }

        public ChangeTile(string Tile = null)
            : this()
        {
            this.Tile = Tile;
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
                Subject.Render.Tile = Tile;
                return true;
            }
            return false;
        }
    }
}
