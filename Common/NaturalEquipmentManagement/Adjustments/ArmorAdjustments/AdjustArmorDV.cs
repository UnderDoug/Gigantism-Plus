using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorDV : ArmorCumulativeAdjustment
    {
        public AdjustArmorDV()
            : base(nameof(Armor.DV))
        {
        }
        public AdjustArmorDV(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorDV(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.DV);
        }
        public AdjustArmorDV(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorDV(Armor Source)
            : this(Source.DV)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && !Amount.IsNullOrZero())
            {
                Subject.GetPart<Armor>().DV += (int)Amount;
                return true;
            }
            return false;
        }
    }
}
