using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorToughness : ArmorCumulativeAdjustment
    {
        public AdjustArmorToughness()
            : base()
        {
        }
        public AdjustArmorToughness(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorToughness(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(Armor Source)
            : this(Source.Toughness)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Toughness += (int)Amount;
            }
            return GetApplied();
        }
    }
}
