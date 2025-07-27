using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorEgo : ArmorCumulativeAdjustment
    {
        public AdjustArmorEgo()
            : base()
        {
        }
        public AdjustArmorEgo(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorEgo(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorEgo(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorEgo(Armor Source)
            : this(Source.Ego)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Ego += (int)Amount;
            }
            return GetApplied();
        }
    }
}
