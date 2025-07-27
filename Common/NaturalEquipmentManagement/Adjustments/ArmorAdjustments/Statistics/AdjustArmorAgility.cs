using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorAgility : ArmorCumulativeAdjustment
    {
        public AdjustArmorAgility()
            : base()
        {
        }
        public AdjustArmorAgility(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAgility(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorAgility(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAgility(Armor Source)
            : this(Source.Agility)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Agility += (int)Amount;
            }
            return GetApplied();
        }
    }
}
