using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorIntelligence : AdjustArmorStatistic
    {
        public AdjustArmorIntelligence()
            : base(nameof(Armor.Intelligence))
        {
        }
        public AdjustArmorIntelligence(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorIntelligence(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorIntelligence(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.Intelligence);
        }
        public AdjustArmorIntelligence(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorIntelligence(Armor Source)
            : this(Source.Intelligence)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().Intelligence += (int)Amount;
                return true;
            }
            return false;
        }
    }
}
