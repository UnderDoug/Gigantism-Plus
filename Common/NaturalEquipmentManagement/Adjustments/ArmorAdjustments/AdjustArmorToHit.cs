using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorToHit : ArmorCumulativeAdjustment
    {
        public AdjustArmorToHit()
            : base()
        {
        }
        public AdjustArmorToHit(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToHit(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorToHit(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorToHit(Armor Source)
            : this(Source.ToHit)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                NeedsShifter = false;
                Subject.GetPart<Armor>().ToHit += (int)Amount;
            }
            return GetApplied();
        }
    }
}
