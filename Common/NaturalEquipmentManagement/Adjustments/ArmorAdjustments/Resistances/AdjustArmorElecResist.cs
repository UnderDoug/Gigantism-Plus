using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorElecResist : AdjustArmorResistance
    {
        public AdjustArmorElecResist()
            : base(Statistic.GetStatCapitalizedDisplayName("ElectricResistance"))
        {
        }
        public AdjustArmorElecResist(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorElecResist(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorElecResist(AdjustArmorResistance SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName("ElectricResistance");
        }
        public AdjustArmorElecResist(int Amount, AdjustArmorResistance SourceAdjustment)
            : this(SourceAdjustment)
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
            return IsApplied();
        }
    }
}
