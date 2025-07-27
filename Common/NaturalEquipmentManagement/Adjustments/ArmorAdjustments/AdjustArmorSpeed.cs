using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorSpeed : ArmorCumulativeAdjustment
    {
        public AdjustArmorSpeed()
            : base()
        {
        }
        public AdjustArmorSpeed(int Amount = 0)
            : base()
        {
            this.Amount = Amount;
        }
        public AdjustArmorSpeed(ArmorCumulativeAdjustment Source)
            : base(Source)
        {
        }
        public AdjustArmorSpeed(int Amount, ArmorCumulativeAdjustment Source)
            : base(Source)
        {
            this.Amount = Amount;
        }
        public AdjustArmorSpeed(Armor Source)
            : this(Source.SpeedBonus - Source.SpeedPenalty)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Armor armor = Subject.GetPart<Armor>();
                int amount = Math.Abs((int)Amount);
                if (Amount > 0)
                {
                    armor.SpeedBonus += amount;
                }
                else 
                {
                    armor.SpeedPenalty += amount;
                }
            }
            return GetApplied();
        }
    }
}
