using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorColdResist : AdjustArmorResistance
    {
        public AdjustArmorColdResist()
            : base(Statistic.GetStatCapitalizedDisplayName("ColdResistance"))
        {
        }
        public AdjustArmorColdResist(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorColdResist(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorColdResist(AdjustArmorResistance SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName("ColdResistance");
        }
        public AdjustArmorColdResist(int Amount, AdjustArmorResistance SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorColdResist(Armor Source)
            : this(Source.Cold)
        {
        }

        public override void Configure()
        {
            base.Configure();
            DescriptionOrder += 2;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Cold += (int)Amount;
            }
            return IsApplied();
        }
    }
}
