using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorMA : ArmorCumulativeAdjustment
    {
        public AdjustArmorMA()
            : base(nameof(Armor.MA))
        {
        }
        public AdjustArmorMA(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorMA(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorMA(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.MA);
        }
        public AdjustArmorMA(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorMA(Armor Source)
            : this(Source.MA)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().MA += (int)Amount;
            }
            return IsApplied();
        }
    }
}
