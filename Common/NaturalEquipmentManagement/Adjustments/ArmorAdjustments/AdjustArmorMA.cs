using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorMA : ArmorCumulativeAdjustment
    {
        public AdjustArmorMA()
            : base()
        {
        }
        public AdjustArmorMA(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorMA(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorMA(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorMA(Armor Source)
            : this(Source.MA)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().MA += (int)Amount;
            }
            return IsApplied();
        }
    }
}
