using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorStatistic : ArmorCumulativeAdjustment
    {
        public AdjustArmorStatistic()
            : base()
        {
            Verb = "confer";
        }
        public AdjustArmorStatistic(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public AdjustArmorStatistic(int Amount, string AffectedParameter = null)
            : this(AffectedParameter)
        {
            this.Amount = Amount;
        }
        public AdjustArmorStatistic(AdjustArmorStatistic Source)
            : base(Source)
        {
            Verb = "confer";
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null)
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {bonusPenalty} of {amount} to {AffectedParameter}";
            }
            return new(Verb, Effect);
        }
    }
}
