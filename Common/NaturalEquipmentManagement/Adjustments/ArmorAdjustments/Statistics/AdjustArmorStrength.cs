using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorStrength : AdjustArmorStatistic
    {
        public AdjustArmorStrength()
            : base(Statistic.GetStatCapitalizedDisplayName(nameof(Armor.Strength)))
        {
        }
        public AdjustArmorStrength(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorStrength(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorStrength(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName(nameof(Armor.Strength));
        }
        public AdjustArmorStrength(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorStrength(Armor Source)
            : this(Source.Strength)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Strength += (int)Amount;
            }
            return IsApplied();
        }
    }
}
