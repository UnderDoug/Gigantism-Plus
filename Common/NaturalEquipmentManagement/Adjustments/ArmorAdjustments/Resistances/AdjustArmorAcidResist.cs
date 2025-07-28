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
        public AdjustArmorAcidResist(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public AdjustArmorAcidResist(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
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
            return IsApplied();
        }
    }
}
