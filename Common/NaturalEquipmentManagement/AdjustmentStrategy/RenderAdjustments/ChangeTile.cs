using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
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

        public override bool Check(GameObject Subject)
        {
            return Subject.Render.Tile != Value 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.Render.Tile = Value;
            }
            return IsApplied();
        }
    }
}
