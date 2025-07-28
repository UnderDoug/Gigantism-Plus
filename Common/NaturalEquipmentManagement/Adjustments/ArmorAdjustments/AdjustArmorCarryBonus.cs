using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorCarryBonus : ArmorCumulativeAdjustment
    {
        public AdjustArmorCarryBonus()
            : base()
        {
        }
        public AdjustArmorCarryBonus(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorCarryBonus(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorCarryBonus(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorCarryBonus(Armor Source)
            : this(Source.CarryBonus)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                NeedsShifter = false;
                Subject.GetPart<Armor>().CarryBonus += (int)Amount;
            }
            return IsApplied();
        }
    }
}
