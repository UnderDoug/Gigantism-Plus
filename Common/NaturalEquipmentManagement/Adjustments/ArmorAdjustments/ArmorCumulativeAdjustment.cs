using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public abstract class ArmorCumulativeAdjustment : ArmorAdjustment
    {
        public bool NeedsShifter;

        public string AffectedParameter;

        public ArmorCumulativeAdjustment()
            : base()
        {
            Prioritize = false;
            NeedsShifter = true;
            Amount = 0;
            Verb = "have";
            Effect = null;
            AffectedParameter = null;
        }
        public ArmorCumulativeAdjustment(Type Source, int Amount, string AffectedParameter)
            : this()
        {
            this.Source = Source;
            this.Amount = Amount;
            this.AffectedParameter = AffectedParameter;
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

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null)
            {
                string amount = ((int)Amount).Signed();
                string bonusPenalty = amount.BonusOrPenalty();
                Effect = $"a {amount} {bonusPenalty} to {AffectedParameter}";
            }
            return new(Verb, Effect);
        }

        public override bool AfterApply(GameObject Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"! {GetType().Name}.{nameof(AfterApply)}()", Indent: indent + 1, Toggle: true);

            if (NeedsShifter)
            {
                GameObject who = null;
                if (Subject.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator) && naturalEquipmentOperator.Wielder != null)
                {
                    who = naturalEquipmentOperator.Wielder;
                }
                Subject?.GetPart<Armor>()?.UpdateStatShifts(who);
            }
            bool baseAfterApply = base.AfterApply(Subject);
            Debug.Entry(4, $"x {GetType().Name}.{nameof(AfterApply)}() !//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return baseAfterApply;
        }
    }
}
