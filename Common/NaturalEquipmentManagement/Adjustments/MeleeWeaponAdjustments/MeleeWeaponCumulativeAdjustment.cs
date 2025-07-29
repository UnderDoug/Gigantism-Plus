using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public abstract class MeleeWeaponCumulativeAdjustment : MeleeWeaponAdjustment
    {
        public string AffectedParameter;

        public MeleeWeaponCumulativeAdjustment()
            : base()
        {
            Amount = 0;
            Verb = "have";
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
            Verb = "have";
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
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && Amount != null && Amount != 0)
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {bonusPenalty} to {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }
    }
}
