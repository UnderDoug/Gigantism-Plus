using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorAcidResist : ArmorCumulativeAdjustment
    {
        public AdjustArmorAcidResist()
            : base()
        {
        }
        public AdjustArmorAcidResist(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAcidResist(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorAcidResist(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAcidResist(Armor Source)
            : this(Source.Acid)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Acid += (int)Amount;
            }
            return GetApplied();
        }
    }
}
