using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorDV : ArmorCumulativeAdjustment
    {
        public AdjustArmorDV()
            : base()
        {
        }
        public AdjustArmorDV(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorDV(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(Armor Source)
            : this(Source.DV)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().DV += (int)Amount;
            }
            return GetApplied();
        }
    }
}
