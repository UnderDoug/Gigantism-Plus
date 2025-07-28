using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using HarmonyLib;

using XRL;
using XRL.World;
using XRL.Language;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using SerializeField = UnityEngine.SerializeField;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class IAdjustment : IComposite
    {
        private static bool doDebug => getClassDoDebug(nameof(IAdjustment));

        [NonSerialized]
        private bool SendAfterEvent; // Whether Apply() should send AfterApplyAdjustmentEvent.

        [NonSerialized]
        private bool Applied; // Whether the adjustment has been applied.

        [NonSerialized]
        public Type Source; // The source of the adjustment, 

        [NonSerialized]
        public bool Prioritize; // Whether or not the adjust is subject to prioritization

        [NonSerialized]
        public int Priority; // Priority of adjustment, lower number = higher priority
       
        [NonSerialized]
        public ICondition<GameObject> Condition;

        [NonSerialized]
        public AllConditions<GameObject> AllConditions;

        [NonSerialized]
        public AnyConditions<GameObject> AnyConditions;

        [NonSerialized]
        public string Value;

        [NonSerialized]
        public int? Amount;

        [NonSerialized]
        public bool? State;

        [NonSerialized]
        public string Verb;

        [NonSerialized]
        public string Effect;

        public IAdjustment()
        {
            SendAfterEvent = false;

            Applied = false;
            Source = null;
            Prioritize = true;
            Priority = 0;

            Condition = null;
            AllConditions = new();
            AnyConditions = new();

            Value = null;
            Amount = null;
            State = null;

            Verb = null;
            Effect = null;
        }
        public IAdjustment(Type Source, bool Prioritize, int Priority, ICondition<GameObject> Condition = null, AllConditions<GameObject> AllConditions = null, AnyConditions<GameObject> AnyConditions = null, string Value = null, int? Amount = null, bool? State = null, string Verb = null, string Effect = null)
            : this()
        {
            this.Source = Source;
            this.Prioritize = Prioritize;
            this.Priority = Priority;
            this.Condition = Condition;
            this.AllConditions = AllConditions ?? new();
            this.AnyConditions = AnyConditions ?? new();
            this.Value = Value;
            this.Amount = Amount;
            this.State = State;
            this.Verb = Verb;
            this.Effect = Effect;
        }
        public IAdjustment(IAdjustment SourceAdjustment)
            : this(
                  Source: SourceAdjustment.Source,
                  Prioritize: SourceAdjustment.Prioritize,
                  Priority: SourceAdjustment.Priority,
                  Condition: SourceAdjustment.Condition,
                  AllConditions: SourceAdjustment.AllConditions,
                  AnyConditions: SourceAdjustment.AnyConditions,
                  Value: SourceAdjustment.Value,
                  Amount: SourceAdjustment.Amount,
                  State: SourceAdjustment.State,
                  Verb: SourceAdjustment.Verb,
                  Effect: SourceAdjustment.Effect)
        {
        }

        public virtual void ResetApplied()
        {
            Applied = false;
        }

        public virtual bool IsApplied()
        {
            return Applied;
        }

        public virtual bool GetSendAfterEvent()
        {
            return SendAfterEvent;
        }

        public override string ToString()
        {
            return $"{Source.Name}.{GetType().Name}";
        }

        public string ToString(bool ShowApplied, bool Short = false)
        {
            string applied = ShowApplied ? $"[{(Applied ? SQR : MTY)}]" : null;
            string addToString = !Short ? AddToString() : null;
            return $"{applied}{ToString()}{addToString}";
        }

        public virtual string AddToString()
        {
            List<string> outputList = new();
            
            if (Value != null)
            {
                string valueString = Value.Quote();
                outputList.Add(valueString);
            }
            int amount;
            if (Amount != null)
            {
                amount = (int)Amount;
                string amountString = amount.Signed();
                outputList.Add(amountString);
            }
            bool state;
            if (State != null)
            {
                state = (bool)State;
                string stateString = Quote($"{state}");
                outputList.Add(stateString);
            }
            string output = null;
            if (!outputList.IsNullOrEmpty())
            {
                output = ": " + outputList.Join(", ");
            } 
            return output;
        }

        public virtual DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            return DescriptionElement.Empty;
        }
        public virtual List<DescriptionElement> GetWeaponDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = new();

            DescriptionElement descriptionElement = GetWeaponDescriptionElement(Subject);

            if (descriptionElement != DescriptionElement.Empty)
            {
                descriptionElements.Add(descriptionElement);
            }
            return new();
        }

        public virtual DescriptionElement GetGeneralDescriptionElement(GameObject Subject)
        {
            return DescriptionElement.Empty;
        }
        public virtual List<DescriptionElement> GetGeneralDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = new();

            DescriptionElement descriptionElement = GetGeneralDescriptionElement(Subject);

            if (descriptionElement != DescriptionElement.Empty)
            {
                descriptionElements.Add(descriptionElement);
            }
            return new();
        }

        public bool TryGetDescriptionElements(GameObject Subject, out List<DescriptionElement> WeaponDescriptionElements, out List<DescriptionElement> GeneralDescriptionElements)
        {
            WeaponDescriptionElements = new();
            GeneralDescriptionElements = new();

            List<DescriptionElement> descriptionElements = GetWeaponDescriptionElements(Subject);
            if (!descriptionElements.IsNullOrEmpty())
            {
                WeaponDescriptionElements.AddRange(descriptionElements);
            }

            descriptionElements = GetGeneralDescriptionElements(Subject);
            if (!descriptionElements.IsNullOrEmpty())
            {
                GeneralDescriptionElements.AddRange(descriptionElements);
            }
            return !WeaponDescriptionElements.IsNullOrEmpty() || !GeneralDescriptionElements.IsNullOrEmpty();
        }

        public virtual bool CheckCondition(GameObject Subject)
        {
            return Subject == null || Condition == null || Condition[Subject];
        }
        public virtual bool CheckAllConditions(GameObject Subject)
        {
            return Subject == null || AllConditions.IsNullOrEmpty() || AllConditions[Subject];
        }
        public virtual bool CheckAnyConditions(GameObject Subject)
        {
            return Subject == null || AnyConditions.IsNullOrEmpty() || AnyConditions[Subject];
        }

        public virtual bool Check(GameObject Subject)
        {
            return Subject == null || (CheckCondition(Subject) && CheckAllConditions(Subject) && CheckAnyConditions(Subject));
        }

        public virtual bool IsTruerThan(GameObject Subject, IAdjustment OtherAdjustment)
        {
            int indent = Debug.LastIndent;

            if (Subject == null || OtherAdjustment == null) return true;

            bool otherCondition = true;
            bool condition = true;
            try
            {
                condition = Check(Subject);
                Debug.LoopItem(4, $"{nameof(Condition)} Checked", $"{condition}", Good: condition, Indent: indent + 1, Toggle: doDebug);
            }
            catch (Exception e)
            {
                Debug.CheckNah(4, $"{nameof(Condition)} Checked", $"{nameof(Exception)}", Indent: indent + 2, Toggle: doDebug);
                MetricsManager.LogModError(ThisMod, e);
            }
            try
            {
                otherCondition = OtherAdjustment.Check(Subject);
                Debug.LoopItem(4, $"{nameof(otherCondition)} Checked", $"{otherCondition}", Good: otherCondition, Indent: indent + 1, Toggle: doDebug);
            }
            catch (Exception e)
            {
                Debug.CheckNah(4, $"{nameof(otherCondition)} Checked", $"{nameof(Exception)}", Indent: indent + 2, Toggle: doDebug);
                MetricsManager.LogModError(ThisMod, e);
            }

            Debug.LastIndent = indent;
            return condition || !otherCondition || condition == otherCondition;
        }

        public virtual bool TryGetHigherPriorityAdjustment(IAdjustment OtherAdjustment, out IAdjustment HigherProrityAdjustment)
        {
            HigherProrityAdjustment = null;
            if (OtherAdjustment != null)
            {
                if (SameAs(OtherAdjustment) && (Prioritize || OtherAdjustment.Prioritize))
                {
                    if (!OtherAdjustment.Prioritize)
                    {
                        HigherProrityAdjustment = this;
                    }
                    if (!Prioritize)
                    {
                        HigherProrityAdjustment = OtherAdjustment;
                    }
                    if (Prioritize && OtherAdjustment.Prioritize)
                    {
                        HigherProrityAdjustment = Priority < OtherAdjustment.Priority ? this : OtherAdjustment;
                    }
                }
            }
            return HigherProrityAdjustment != null;
        }
        public virtual bool TryGetHigherPriorityAdjustment(GameObject Subject, IAdjustment OtherAdjustment, out IAdjustment HigherProrityAdjustment)
        {
            HigherProrityAdjustment = null;
            return IsTruerThan(Subject, OtherAdjustment) && TryGetHigherPriorityAdjustment(OtherAdjustment, out HigherProrityAdjustment) && HigherProrityAdjustment != null;
        }

        public virtual bool SameAs(IAdjustment OtherAdjustment)
        {
            if (OtherAdjustment == null)
            {
                return false;
            }
            if (this == OtherAdjustment)
            {
                return true;
            }
            bool sameType = GetType() == OtherAdjustment.GetType();
            bool sameSource = Source == OtherAdjustment.Source;
            if (!Prioritize && sameType)
            {
                 return sameSource;
            }
            return sameType;
        }

        public virtual bool Apply(GameObject Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {GetType().Name}.{nameof(Apply)}()", Indent: indent + 1, Toggle: doDebug);

            if (!Applied && Subject != null && Check(Subject) && BeforeApplyAdjustmentEvent.CheckFor(Subject, Source, this))
            {
                Debug.LoopItem(4, $"2] !{nameof(Applied)} and {nameof(Check)}({nameof(Subject)})", Indent: indent + 2, Toggle: doDebug);

                Applied = true;
                // SendAfterEvent = true;

                Debug.CheckYeh(4, $"{nameof(Applied)}: {Applied}", Indent: indent + 3, Toggle: doDebug);
                // Debug.CheckYeh(4, $"{nameof(SendAfterEvent)}: {SendAfterEvent}", Indent: indent + 3, Toggle: doDebug);

                Apply(Subject);
                EarlyAfterApplyAdjustmentEvent.Send(Subject, Source, this);
                AfterApply(Subject);
                return false;
            }
            Debug.Entry(4, $"x {GetType().Name}.{nameof(Apply)}() *//", Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return true;
        }

        public virtual bool AfterApply(GameObject Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"! {GetType().Name}.{nameof(AfterApply)}()", Indent: indent + 1, Toggle: doDebug);

            Debug.CheckYeh(4, $"Doing AfterApply", Indent: indent + 2, Toggle: true);
            AfterApplyAdjustmentEvent.Send(Subject, Source, this);
            if (SendAfterEvent)
            {
                // Debug.CheckYeh(4, $"Doing AfterApply", Indent: indent + 2, Toggle: true);
                // AfterApplyAdjustmentEvent.Send(Subject, Source, this);
            }
            else
            {
                // Debug.CheckNah(4, $"Doing AfterApply", Indent: indent + 2, Toggle: true);
            }
            Debug.Entry(4, $"x {GetType().Name}.{nameof(AfterApply)}() !//", Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return SendAfterEvent;
        }

        public virtual void Write(SerializationWriter Writer)
        {
            Writer.Write(SendAfterEvent);
            Writer.Write(Applied);
            Writer.WriteObject(Source);
            Writer.Write(Prioritize);
            Writer.Write(Priority);
            Writer.WriteObject(Condition);
            Writer.WriteObject(AllConditions);
            Writer.WriteObject(AnyConditions);
            Writer.WriteOptimized(Value);
            Writer.WriteNullable(Amount);
            Writer.WriteNullable(State);
            Writer.WriteOptimized(Verb);
            Writer.WriteOptimized(Effect);
        }
        public virtual void Read(SerializationReader Reader)
        {
            SendAfterEvent = Reader.ReadBoolean();
            Applied = Reader.ReadBoolean();
            Source = Reader.ReadObject() as Type;
            Prioritize = Reader.ReadBoolean();
            Priority = Reader.ReadInt32();
            Condition = Reader.ReadObject() as ICondition<GameObject>;
            AllConditions = Reader.ReadObject() as AllConditions<GameObject>;
            AnyConditions = Reader.ReadObject() as AnyConditions<GameObject>;
            Value = Reader.ReadOptimizedString();
            Amount = Reader.ReadObject() as int?;
            State = Reader.ReadObject() as bool?;
            Verb = Reader.ReadOptimizedString();
            Effect = Reader.ReadOptimizedString();
        }
    }
}
