using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorAV : ArmorCumulativeAdjustment
    {
        public AdjustArmorAV()
            : base()
        {
        }
        public AdjustArmorAV(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAV(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorAV(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAV(Armor Source)
            : this(Source.AV)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().AV += (int)Amount;
            }
            return IsApplied();
        }
    }
}
