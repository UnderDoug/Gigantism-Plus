using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorStrength : ArmorCumulativeAdjustment
    {
        public AdjustArmorStrength()
            : base()
        {
        }
        public AdjustArmorStrength(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorStrength(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorStrength(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorStrength(Armor Source)
            : this(Source.Strength)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Strength += (int)Amount;
            }
            return GetApplied();
        }
    }
}
