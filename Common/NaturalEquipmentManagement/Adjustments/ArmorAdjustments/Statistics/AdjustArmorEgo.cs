using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorEgo : AdjustArmorStatistic
    {
        public AdjustArmorEgo()
            : base(nameof(Armor.Ego))
        {
        }
        public AdjustArmorEgo(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorEgo(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorEgo(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.Ego);
        }
        public AdjustArmorEgo(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorEgo(Armor Source)
            : this(Source.Ego)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Ego += (int)Amount;
                return false;
            }
            return true;
        }
    }
}
