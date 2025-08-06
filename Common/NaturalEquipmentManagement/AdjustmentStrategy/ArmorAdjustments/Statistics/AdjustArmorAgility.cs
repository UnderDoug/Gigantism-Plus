using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorAgility : AdjustArmorStatistic
    {
        public AdjustArmorAgility()
            : base(Statistic.GetStatCapitalizedDisplayName(nameof(Armor.Agility)))
        {
        }
        public AdjustArmorAgility(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorAgility(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorAgility(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = Statistic.GetStatCapitalizedDisplayName(nameof(Armor.Agility));
        }
        public AdjustArmorAgility(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorAgility(Armor Source)
            : this(Source.Agility)
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
                Subject.GetPart<Armor>().Agility += (int)Amount;
            }
            return IsApplied();
        }
    }
}
