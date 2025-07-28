using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorHeatResist : ArmorCumulativeAdjustment
    {
        public AdjustArmorHeatResist()
            : base()
        {
        }
        public AdjustArmorHeatResist(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorHeatResist(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorHeatResist(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorHeatResist(Armor Source)
            : this(Source.Heat)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Heat += (int)Amount;
            }
            return IsApplied();
        }
    }
}
