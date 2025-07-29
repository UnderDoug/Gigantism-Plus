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
            : base("Speed")
        {
        }
        public AdjustArmorSpeed(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorSpeed(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorSpeed(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "Speed";
        }
        public AdjustArmorSpeed(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
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
            return IsApplied();
        }
    }
}
