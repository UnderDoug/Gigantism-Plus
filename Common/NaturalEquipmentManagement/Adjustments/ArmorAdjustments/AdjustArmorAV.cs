using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorAV : ArmorCumulativeAdjustment
    {
        public AdjustArmorAV()
            : base(nameof(Armor.AV))
        {
        }
        public AdjustArmorAV(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAV(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorAV(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.AV);
        }
        public AdjustArmorAV(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAV(Armor Source)
            : this(Source.AV)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && !Amount.IsNullOrZero())
            {
                Subject.GetPart<Armor>().AV += (int)Amount;
                return true;
            }
            return false;
        }
    }
}
