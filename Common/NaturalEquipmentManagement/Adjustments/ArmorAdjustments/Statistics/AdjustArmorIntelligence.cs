using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorIntelligence : ArmorCumulativeAdjustment
    {
        public AdjustArmorIntelligence()
            : base()
        {
        }
        public AdjustArmorIntelligence(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorIntelligence(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorIntelligence(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorIntelligence(Armor Source)
            : this(Source.Intelligence)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Intelligence += (int)Amount;
            }
            return GetApplied();
        }
    }
}
