using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorElecResist : ArmorCumulativeAdjustment
    {
        public AdjustArmorElecResist()
            : base()
        {
        }
        public AdjustArmorElecResist(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorElecResist(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorElecResist(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorElecResist(Armor Source)
            : this(Source.Elec)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Elec += (int)Amount;
            }
            return GetApplied();
        }
    }
}
