using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorAcidResist : AdjustArmorResistance
    {
        public AdjustArmorAcidResist()
            : base(Statistic.GetStatCapitalizedDisplayName("AcidResistance"))
        {
        }
        public AdjustArmorAcidResist(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAcidResist(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorAcidResist(AdjustArmorResistance SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName("AcidResistance");
        }
        public AdjustArmorAcidResist(int Amount, AdjustArmorResistance SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAcidResist(Armor Source)
            : this(Source.Acid)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Acid += (int)Amount;
            }
            return IsApplied();
        }
    }
}
