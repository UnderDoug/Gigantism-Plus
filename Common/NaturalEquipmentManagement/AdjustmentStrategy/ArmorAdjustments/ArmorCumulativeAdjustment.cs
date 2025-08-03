using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class ArmorCumulativeAdjustment : ArmorAdjustment
    {
        public bool NeedsShifter;

        public string AffectedParameter;

        public ArmorCumulativeAdjustment()
            : base()
        {
            Amount = 0;
            Effect = null;
            AffectedParameter = null;
        }
        public ArmorCumulativeAdjustment(string AffectedParameter)
            : this()
        {
            this.AffectedParameter = AffectedParameter;
        }
        public ArmorCumulativeAdjustment(Type Source, int Amount, string AffectedParameter)
            : this(AffectedParameter)
        {
            this.Source = Source;
            this.Amount = Amount;
        }
        public ArmorCumulativeAdjustment(Type Source, int Amount, string Verb, string Effect, string AffectedParameter = null)
            : this(Source, Amount, AffectedParameter)
        {
            this.Verb = Verb;
            this.Effect = Effect;
        }
        public ArmorCumulativeAdjustment(ArmorCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = SourceAdjustment.AffectedParameter;
        }
        public ArmorCumulativeAdjustment(int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public ArmorCumulativeAdjustment(Type Source, int Amount, ArmorCumulativeAdjustment SourceAdjustment)
            : this(Amount, SourceAdjustment)
        {
            this.Source = Source;
        }

        public override void Configure()
        {
            base.Configure();
            Verb = "have";
            Prioritize = false;
            NeedsShifter = true;
        }

        public override bool Check(GameObject Subject)
        {
            return !Amount.IsNullOrZero() 
                && base.Check(Subject);
        }

        public override void AfterApply(GameObject Subject)
        {
            if (NeedsShifter && !Amount.IsNullOrZero())
            {
                GameObject who = null;
                if (Subject.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator)
                    && naturalEquipmentOperator.Wielder is GameObject wielder)
                {
                    who = wielder;
                }
                Subject?.GetPart<Armor>()?.UpdateStatShifts(who);
            }
            base.AfterApply(Subject);
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {bonusPenalty} to {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }
    }
}
