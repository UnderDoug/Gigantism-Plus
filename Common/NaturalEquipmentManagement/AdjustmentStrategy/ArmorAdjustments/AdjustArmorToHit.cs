using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArmorToHit : AdjustArmorStatistic
    {
        public AdjustArmorToHit()
            : base("To Hit")
        {
        }
        public AdjustArmorToHit(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToHit(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorToHit(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "To Hit";
        }
        public AdjustArmorToHit(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorToHit(Armor Source)
            : this(Source.ToHit)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Verb = "confer";
            NeedsShifter = false;
        }

        public override bool Check(GameObject Subject)
        {
            return !Amount.IsNullOrZero()
                && base.Check(Subject);
        }
        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<Armor>().ToHit += (int)Amount;
            }
            return IsApplied();
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {AffectedParameter} {bonusPenalty}";
                return new(Verb, Effect);
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }
    }
}
