using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorHeatResist : AdjustArmorResistance
    {
        public AdjustArmorHeatResist()
            : base(Statistic.GetStatCapitalizedDisplayName("HeatResistance"))
        {
        }
        public AdjustArmorHeatResist(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorHeatResist(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorHeatResist(AdjustArmorResistance SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName("HeatResistance");
        }
        public AdjustArmorHeatResist(int Amount, AdjustArmorResistance SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorHeatResist(Armor Source)
            : this(Source.Heat)
        {
        }

        public override void Configure()
        {
            base.Configure();
            DescriptionOrder += 4;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Heat += (int)Amount;
            }
            return IsApplied();
        }
    }
}
