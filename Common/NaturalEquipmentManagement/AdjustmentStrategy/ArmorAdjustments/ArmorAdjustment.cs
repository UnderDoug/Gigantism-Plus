using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ArmorAdjustment : IAdjustment
    {
        public ArmorAdjustment()
            : base()
        {
        }
        public ArmorAdjustment(ArmorAdjustment Source)
            : base(Source)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return Subject.HasPart<Armor>() 
                && base.Check(Subject);
        }
    }
}
