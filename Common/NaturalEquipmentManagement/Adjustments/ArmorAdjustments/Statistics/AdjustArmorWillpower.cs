using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorWillpower : AdjustArmorStatistic
    {
        public AdjustArmorWillpower()
            : base(nameof(Armor.Willpower))
        {
        }
        public AdjustArmorWillpower(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorWillpower(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorWillpower(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.Willpower);
        }
        public AdjustArmorWillpower(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorWillpower(Armor Source)
            : this(Source.Willpower)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Willpower += (int)Amount;
                return true;
            }
            return false;
        }
    }
}
