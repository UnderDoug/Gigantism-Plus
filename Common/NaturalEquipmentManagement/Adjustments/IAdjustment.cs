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
        public AnyConditions<GameObject> AnyConditions;

        [NonSerialized]
        public AllConditions<GameObject> AllConditions;

        public IAdjustment()
        {
            Applied = false;
            Source = null;
            Prioritize = true;
            Priority = 0;
            Condition = null;
            AnyConditions = new();
            AllConditions = new();
        }

        public IAdjustment(bool Prioritize, int AdjustmentPriority)
            : this()
        {
            this.Prioritize = Prioritize;
            this.Priority = AdjustmentPriority;
        }

        public IAdjustment(bool Prioritize, int AdjustmentPriority, ICondition<GameObject> Condition = null, AnyConditions<GameObject> AnyConditions = null, AllConditions<GameObject> AllConditions = null)
            : this(Prioritize, AdjustmentPriority)
        {
            this.Condition = Condition;
            this.Condition = AnyConditions ?? new();
            this.Condition = AllConditions ?? new();
        }

        public IAdjustment(IAdjustment Source)
            : this(Source.Prioritize, Source.Priority, Source.Condition, Source.AnyConditions, Source.AllConditions)
        {
        }

        public virtual void ResetApplied()
        {
            Applied = false;
        }

        public virtual bool GetApplied()
        {
            return Applied;
        }

        public virtual bool CheckCondition(GameObject Subject)
        {
            return Subject == null || Condition == null || Condition[Subject];
        }
        public virtual bool CheckAnyConditions(GameObject Subject = null)
        {
            return Subject == null || AnyConditions.IsNullOrEmpty() || AnyConditions[Subject];
        }
        public virtual bool CheckAllConditions(GameObject Subject = null)
        {
            return Subject == null || AllConditions.IsNullOrEmpty() || AllConditions[Subject];
        }

        public virtual bool Check(GameObject Subject = null)
        {
            return Subject == null || (CheckCondition(Subject) && CheckAnyConditions(Subject) && CheckAllConditions(Subject));
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
            if (!Applied && Subject != null && Check(Subject) && BeforeApplyAdjustmentEvent.CheckFor(Subject, Source, this))
            {
                Applied = true;
            }
            return Applied;
        }

        public virtual void Write(SerializationWriter Writer)
        {
            Writer.Write(Applied);
            Writer.WriteObject(Source);
            Writer.Write(Prioritize);
            Writer.Write(Priority);
            Writer.WriteObject(Condition);
            Writer.WriteObject(AnyConditions);
            Writer.WriteObject(AllConditions);
        }
        public virtual void Read(SerializationReader Reader)
        {
            Applied = Reader.ReadBoolean();
            Source = Reader.ReadObject() as Type;
            Prioritize = Reader.ReadBoolean();
            Priority = Reader.ReadInt32();
            Condition = Reader.ReadObject() as ICondition<GameObject>;
            AnyConditions = Reader.ReadObject() as AnyConditions<GameObject>;
            AllConditions = Reader.ReadObject() as AllConditions<GameObject>;
        }
    }
}
