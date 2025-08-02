using System;
using System.Collections.Generic;
using System.Text;

using HarmonyLib;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustArbitraryPart<T> : IAdjustment where T : IPart, new()
    {
        public string Parameter;

        [NonSerialized]
        public new object Value;

        public AdjustArbitraryPart()
            : base()
        {
            Parameter = null;
            Value = null;
        }
        public AdjustArbitraryPart(string Parameter, object Value, string Verb = null, string Effect = null)
            : this()
        {
            this.Parameter = Parameter;
            this.Value = Value;
            this.Verb = Verb;
            this.Effect = Effect;
        }
        public AdjustArbitraryPart(AdjustArbitraryPart<T> SourceAdjustment)
            : base(SourceAdjustment)
        {
            Parameter = null;
            Value = null;
        }

        public bool IsPartWeapon()
        {
            T samplePart = new();
            return samplePart is MeleeWeapon
                || samplePart is MissileWeapon
                || samplePart is ThrownWeapon;
        }

        public bool TryGetPartParameter(GameObject Subject, out Traverse PartParameter)
        {
            T targetPart = Subject?.GetPart<T>();
            PartParameter = null;
            if (targetPart != null && !Parameter.IsNullOrEmpty() && Value != null)
            {
                Traverse targetPartTraverse = new(targetPart);
                Type ValueType = Value.GetType();

                bool partParamaterExists = 
                    (PartParameter = targetPartTraverse.Property(Parameter)).PropertyExists() 
                 || (PartParameter = targetPartTraverse.Field(Parameter)).FieldExists();

                if (!partParamaterExists || PartParameter.GetValueType() != ValueType)
                {
                    PartParameter = null;
                }
            }
            return PartParameter != null;
        }

        public override bool SameAs(IAdjustment OtherAdjustment)
        {
            return OtherAdjustment is AdjustArbitraryPart<T> aAP 
                && aAP.Parameter == Parameter
                && base.SameAs(OtherAdjustment);
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && TryGetPartParameter(Subject, out _);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && TryGetPartParameter(Subject, out Traverse partParameter))
            {
                partParameter.SetValue(Value);
            }
            return IsApplied();
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            DescriptionElement element = DescriptionElement.Empty;
            T samplePart = new();
            if (!Effect.IsNullOrEmpty() && IsPartWeapon())
            {
                element = new(Verb, Effect);
            }
            return element;
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject)
        {
            DescriptionElement element = DescriptionElement.Empty;
            T samplePart = new();
            if (!Effect.IsNullOrEmpty() && !IsPartWeapon())
            {
                element = new(Verb, Effect);
            }
            return element;
        }

        public override void Write(SerializationWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteObject(Value);
        }

        public override void Read(SerializationReader Reader)
        {
            base.Read(Reader);
            Value = Reader.ReadObject();
        }
    }
}
