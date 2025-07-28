using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorToughness : AdjustArmorStatistic
    {
        public AdjustArmorToughness()
            : base(nameof(Armor.Toughness))
        {
        }
        public AdjustArmorToughness(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorToughness(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.Toughness);
        }
        public AdjustArmorToughness(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(Armor Source)
            : this(Source.Toughness)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Toughness += (int)Amount;
            }
            return IsApplied();
        }
    }
}
