using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class MeleeWeaponCumulativeAdjustment : MeleeWeaponAdjustment
    {
        public string AffectedParameter;

        public MeleeWeaponCumulativeAdjustment()
            : base()
        {
            Amount = 0;
            AffectedParameter = null;
        }
        public MeleeWeaponCumulativeAdjustment(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public MeleeWeaponCumulativeAdjustment(Type Source, int Amount, string AffectedParameter)
            : this(AffectedParameter)
        {
            this.Source = Source;
            this.Amount = Amount;
        }
        public MeleeWeaponCumulativeAdjustment(Type Source, int Amount, string Verb, string AffectedParameter = null)
            : this(Source, Amount, AffectedParameter)
        {
            this.Verb = Verb;
        }
        public MeleeWeaponCumulativeAdjustment(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = SourceAdjustment.AffectedParameter;
        }
        public MeleeWeaponCumulativeAdjustment(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public MeleeWeaponCumulativeAdjustment(Type Source, int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(Amount, SourceAdjustment)
        {
            this.Source = Source;
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
            Verb = "have";
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {bonusPenalty} to {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetPrimaryDescriptionElement(Subject);
        }
    }
}
