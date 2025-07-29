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
            : base("To Hit")
        {
        }
        public AdjustArmorToHit(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToHit(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorToHit(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "To Hit";
        }
        public AdjustArmorToHit(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
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
            return IsApplied();
        }
    }
}
