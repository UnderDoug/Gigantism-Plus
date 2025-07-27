using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorColdResist : ArmorCumulativeAdjustment
    {
        public AdjustArmorColdResist()
            : base()
        {
        }
        public AdjustArmorColdResist(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorColdResist(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorColdResist(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorColdResist(Armor Source)
            : this(Source.Cold)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Cold += (int)Amount;
            }
            return GetApplied();
        }
    }
}
