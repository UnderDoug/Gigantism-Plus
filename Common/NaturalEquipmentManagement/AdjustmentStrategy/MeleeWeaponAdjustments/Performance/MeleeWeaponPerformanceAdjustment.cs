using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class MeleeWeaponPerformanceAdjustment : MeleeWeaponCumulativeAdjustment
    {
        public MeleeWeaponPerformanceAdjustment()
            : base()
        {
        }
        public MeleeWeaponPerformanceAdjustment(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public MeleeWeaponPerformanceAdjustment(Type Source, int Amount, string AffectedParameter)
            : this(AffectedParameter)
        {
            this.Source = Source;
            this.Amount = Amount;
        }
        public MeleeWeaponPerformanceAdjustment(Type Source, int Amount, string Verb, string AffectedParameter = null)
            : this(Source, Amount, AffectedParameter)
        {
            this.Verb = Verb;
        }
        public MeleeWeaponPerformanceAdjustment(MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            Verb = "have";
            AffectedParameter = SourceAdjustment.AffectedParameter;
        }
        public MeleeWeaponPerformanceAdjustment(int Amount, MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public MeleeWeaponPerformanceAdjustment(Type Source, int Amount, MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : this(Amount, SourceAdjustment)
        {
            this.Source = Source;
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
        }

        public override bool Check(GameObject Subject)
        {
            return !Amount.IsNullOrZero() 
                && base.Check(Subject);
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
