using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorWillpower : ArmorCumulativeAdjustment
    {
        public AdjustArmorWillpower()
            : base()
        {
        }
        public AdjustArmorWillpower(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorWillpower(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorWillpower(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorWillpower(Armor Source)
            : this(Source.Willpower)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Willpower += (int)Amount;
            }
            return GetApplied();
        }
    }
}
