using HarmonyLib;
using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Reflection;
using XRL.Language;
using XRL.World.Anatomy;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalEquipmentBase 
        : IModification
        , IModEventHandler<BeforeApplyPartAdjustmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModNaturalEquipmentBase));

        [Serializable]
        public class PartAdjustment : IComposite
        {
            private static bool doDebug => getClassDoDebug(nameof(PartAdjustment));

            public Guid ID;

            public bool Applied; // Whether the adjustment has been applied.

            public string ParentNaturalEquipmentMod; // String name of the NaturalEquipmentMod this part adjust belongs to.

            public Type Target; // Render, MeleeWeapon, Armor, etc.

            public string Field; // Field/Property to adjust

            public int Priority; // Priority of adjustment, lower number = higher priority

            public object Value; // Value to adjust the Field to.

            public PartAdjustment()
            {
                ID = Guid.NewGuid();
                Applied = false;
                ParentNaturalEquipmentMod = string.Empty;
                Target = null;
                Field = string.Empty;
                Priority = 0;
                Value = null;
            }

            public PartAdjustment(string ParentNaturalEquipmentMod, Type Target, string Field, int Priority, object Value)
                : this ()
            {
                this.ParentNaturalEquipmentMod = ParentNaturalEquipmentMod;
                this.Target = Target;
                this.Field = Field;
                this.Priority = Priority;
                this.Value = Value;
            }

            public PartAdjustment(PartAdjustment Source)
                : this (Source.ParentNaturalEquipmentMod, Source.Target, Source.Field, Source.Priority, Source.Value)
            {
            }
            public PartAdjustment(string Address)
                : this ()
            {
                SetAddress(Address);
            }
            public PartAdjustment(string Address, int Priority, object Value)
                : this (Address)
            {
                this.Priority = Priority;
                this.Value = Value;
            }

            public override string ToString()
            {
                string output = string.Empty;
                output += $"({(Priority != 0 ? Priority : "PriorityUnset")})";
                output += $"{Target.Name ?? "NoTarget?"}.";
                output += $"{Field ?? "NoField?"}";
                output += $" = \"";
                output += Value != null ? Value.ToString() : "Value?";
                output += "\"";
                return output;
            }
            public virtual string ToString(bool ShowID)
            {
                string output = string.Empty;
                if (ShowID)
                {
                    output += $"[{(ID != null ? ID : "No ID")}::{ParentNaturalEquipmentMod}]";
                }
                output += ToString();
                return output;
            }

            public bool HasSameTargetAs(PartAdjustment OtherAdjustment)
            {
                return GetAddress() == OtherAdjustment.GetAddress();
            }

            public bool TryGetHigherPriorityAdjustment(PartAdjustment OtherAdjustment, out PartAdjustment HigherProrityAdjustment)
            {
                HigherProrityAdjustment = null;
                if (HasSameTargetAs(OtherAdjustment))
                {
                    HigherProrityAdjustment = Priority < OtherAdjustment.Priority ? this : OtherAdjustment;
                    return true;
                }
                return false;
            }

            public bool SameAs(PartAdjustment a)
            {
                return ID == a.ID;
            }

            public string GetAddress()
            {
                return $"{Target}.{Field}";
            }

            public bool SetAddress(string Address)
            {
                if (!Address.IsNullOrEmpty() && Address.Contains('.'))
                {
                    Assembly asm = Target.Assembly;
                    string[] pieces = Address.Split('.');
                    Target = asm.GetType("XRL.World.Parts." + pieces[0]);
                    Field = pieces[1];
                    return true;
                }
                return false;
            }

            public bool Apply(GameObject Equipment)
            {
                if (Equipment != null && !Applied)
                {
                    int indent = Debug.LastIndent;
                    Debug.Divider(4, HONLY, Count: 60, Indent: indent, Toggle: doDebug);
                    Debug.LoopItem(4, 
                        $":] " +
                        $"{nameof(PartAdjustment)}." +
                        $"{nameof(Apply)}" +
                        $"(Object: {Equipment.DebugName})" +
                        $" {ToString()}", Indent: indent, Toggle: doDebug);

                    object targetPart = Target == typeof(GameObject) ? Equipment : Equipment.GetPart(Target);
                    Debug.Entry(4, $"{targetPart?.GetType()?.Name ?? NULL}", Indent: indent + 2, Toggle: doDebug);
                    if (targetPart != null)
                    {
                        if (BeforeApplyPartAdjustmentEvent.Send(Equipment, ParentNaturalEquipmentMod, Target, Field, ref Value))
                        {
                            Debug.CheckYeh(4, $"{targetPart.GetType().Name}", Indent: indent + 2, Toggle: doDebug);
                            Traverse targetPartTraverse = new(targetPart);
                            Traverse targetProperty = targetPartTraverse.Property(Field);
                            Traverse targetField = targetPartTraverse.Field(Field);
                            Type valueType = Value.GetType();
                            Debug.Entry(4, $"Value Type: {valueType?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            Debug.Entry(4, $"{nameof(targetProperty)}.GetValueType: {targetProperty?.GetValueType()?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            Debug.Entry(4, $"{nameof(targetField)}.GetValueType: {targetField?.GetValueType()?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            try
                            {
                                if (targetProperty.PropertyExists() && targetProperty.GetValueType() == valueType)
                                {
                                    Debug.CheckYeh(4, $"{nameof(targetProperty)}", Indent: indent + 3, Toggle: doDebug);
                                    Debug.Entry(4, $"Property Type: {targetProperty.GetValueType().Name}", Indent: indent + 4, Toggle: doDebug);
                                    targetProperty.SetValue(Value);

                                    Applied = true;
                                    Debug.Entry(4, $"Value: {targetProperty.GetValue()}", Indent: indent + 4, Toggle: doDebug);
                                    Debug.LastIndent = indent;
                                    return targetProperty.GetValue().Equals(Value);
                                }
                                if (targetField.FieldExists() && targetField.GetValueType() == valueType)
                                {
                                    Debug.CheckYeh(4, $"{nameof(targetField)}", Indent: indent + 3, Toggle: doDebug);
                                    Debug.Entry(4, $"Field Type: {targetField.GetValueType().Name}", Indent: indent + 4, Toggle: doDebug);
                                    targetField.SetValue(Value);

                                    Applied = true;
                                    Debug.Entry(4, $"Value: {targetField.GetValue()}", Indent: indent + 4, Toggle: doDebug);
                                    Debug.LastIndent = indent;
                                    return targetField.GetValue().Equals(Value);
                                }
                            }
                            catch (Exception e)
                            {
                                MetricsManager.LogModError(ThisMod, e);
                                Debug.LastIndent = indent;
                                return false;
                            }
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Blocked by {nameof(BeforeApplyPartAdjustmentEvent)}.Send() returning false", 
                                Indent: indent + 2, Toggle: doDebug);
                            Applied = true;
                        }
                    }
                    else 
                    { 
                        Debug.CheckNah(4, $"Object has no {Target.Name} IPart", Indent: indent + 2, Toggle: doDebug);
                        Applied = true;
                    }
                    Debug.LastIndent = indent;
                }
                return false;
            }
        }

        [NonSerialized]
        public List<PartAdjustment> Adjustments;

        public string BodyPartType;

        private GameObject _wielder = null;
        public GameObject Wielder
        {
            get => _wielder ??= ParentObject?.Equipped;
            set => _wielder = value;
        }

        public int ModPriority;
        public int DescriptionPriority;

        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;
        public int PenBonus;

        public string Adjective;
        public string AdjectiveColor;
        public string AdjectiveColorFallback;

        [NonSerialized]
        public List<string> AddedParts = new();

        [NonSerialized]
        public Dictionary<string, string> AddedStringProps = new();

        [NonSerialized]
        public Dictionary<string, int> AddedIntProps = new();

        public ModNaturalEquipmentBase()
        {
            Adjustments = new();
        }
        public ModNaturalEquipmentBase(int Tier)
            : base(Tier)
        {
            Adjustments = new();
        }
        public ModNaturalEquipmentBase(ModNaturalEquipmentBase Source)
            : this()
        {
            Adjustments = new(Source.Adjustments ??= new());

            BodyPartType = Source.BodyPartType;

            ModPriority = Source.ModPriority;
            DescriptionPriority = Source.DescriptionPriority;

            DamageDieCount = Source.DamageDieCount;
            DamageDieSize = Source.DamageDieSize;
            DamageBonus = Source.DamageBonus;
            HitBonus = Source.HitBonus;
            PenBonus = Source.PenBonus;

            Adjective = Source.Adjective;
            AdjectiveColor = Source.AdjectiveColor;
            AdjectiveColorFallback = Source.AdjectiveColorFallback;

            AddedParts = new(Source.AddedParts ?? new());
            AddedStringProps = new(Source.AddedStringProps ?? new());
            AddedIntProps = new(Source.AddedIntProps ?? new());
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            return Object.HasPart<Physics>() 
                && Object.IsNaturalEquipment();
        }

        public abstract Guid AddAdjustment(Type Target, string Field, object Value, int Priority);
        public abstract Guid AddAdjustment(Type Target, string Field, object Value, bool FlipPriority = false);
        public abstract int GetDamageDieCount();
        public abstract int GetDamageDieSize();
        public abstract int GetDamageBonus();
        public abstract int GetHitBonus();
        public abstract int GetPenBonus();

        public override void ApplyModification(GameObject Object)
        {
            if (Object.HasPartDescendedFrom<ModNaturalEquipmentBase>() && !Object.GetPartsDescendedFrom<ModNaturalEquipmentBase>(p => SameModification(p, false)).IsNullOrEmpty())
            {
                return;
            }
            base.ApplyModification(Object);
        }
        public virtual bool SameModification(ModNaturalEquipmentBase m, bool Strict = true, bool? DescriptionOnly = null)
        {
            bool sameBodyPartType = BodyPartType == m.BodyPartType;
            bool sameModPriority = ModPriority == m.ModPriority;
            bool sameDescriptionPriority = DescriptionPriority == m.DescriptionPriority;
            bool sameAdjective = Adjective == m.Adjective;
            bool sameSource = GetSource() == m.GetSource();

            bool sameGenerally = sameBodyPartType && sameAdjective && sameSource;
            bool sameForDescription = sameGenerally && sameDescriptionPriority;
            bool sameForMod = sameGenerally && sameModPriority;

            bool sameStrictly = sameForDescription && sameForMod;

            if (!Strict)
            {
                if (DescriptionOnly != null)
                {
                    if ((bool)DescriptionOnly)
                    {
                        return sameForDescription;
                    }
                    else
                    {
                        return sameForMod;
                    }
                }
                return sameGenerally;
            }
            return sameStrictly;
        }
        public abstract string GetSource();

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID
                || ID == BeforeApplyPartAdjustmentEvent.ID;
        }
        public virtual bool HandleEvent(BeforeApplyPartAdjustmentEvent E)
        {
            return base.HandleEvent(E);
        }

        public virtual string GetAdjective()
        {
            return Adjective ?? "adjective?";
        }
        public virtual string GetColoredAdjective()
        {
            return GetAdjective().OptionalColor(AdjectiveColor, AdjectiveColorFallback, Colorfulness);
        }

        public virtual string GetAdjectiveIndicativeNoun(GameObject Object = null)
        {
            Object ??= ParentObject;

            string adjective = Grammar.MakeTitleCase(GetColoredAdjective());

            string objectNoun = Object?.GetObjectNoun();
            objectNoun = Object != null && Object.IsPlural ? Grammar.Pluralize(objectNoun) : objectNoun;

            return $"{adjective}: {Object?.IndicativeProximal} {objectNoun} ";
        }

        public abstract string GetInstanceDescription(GameObject Object = null);

        public virtual int GetDescriptionPriority()
        {
            return DescriptionPriority;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.Write(Adjustments ??= new());
            Writer.Write(AddedParts ??= new());
            Writer.Write(AddedStringProps ??= new());
            Writer.Write(AddedIntProps ??= new());
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            Adjustments = Reader.ReadList<PartAdjustment>() ?? new();
            AddedParts = Reader.ReadList<string>() ?? new();
            AddedStringProps = Reader.ReadDictionary<string, string>() ?? new();
            AddedIntProps = Reader.ReadDictionary<string, int>() ?? new();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent, MapInv) as ModNaturalEquipmentBase;

            naturalEquipmentMod.Adjustments = new(Adjustments ??= new());

            naturalEquipmentMod.AddedParts = new(AddedParts ?? new());
            naturalEquipmentMod.AddedStringProps = new(AddedStringProps ?? new());
            naturalEquipmentMod.AddedIntProps = new(AddedIntProps ?? new());

            return ClearForCopy(naturalEquipmentMod);
        }
        public override IPart DeepCopy(GameObject Parent)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent) as ModNaturalEquipmentBase;

            naturalEquipmentMod.Adjustments = new(Adjustments ??= new());

            naturalEquipmentMod.BodyPartType = BodyPartType;

            naturalEquipmentMod.ModPriority = ModPriority;
            naturalEquipmentMod.DescriptionPriority = DescriptionPriority;

            naturalEquipmentMod.DamageDieCount = DamageDieCount;
            naturalEquipmentMod.DamageDieSize = DamageDieSize;
            naturalEquipmentMod.DamageBonus = DamageBonus;
            naturalEquipmentMod.HitBonus = HitBonus;
            naturalEquipmentMod.PenBonus = PenBonus;

            naturalEquipmentMod.Adjective = Adjective;
            naturalEquipmentMod.AdjectiveColor = AdjectiveColor;
            naturalEquipmentMod.AdjectiveColorFallback = AdjectiveColorFallback;

            naturalEquipmentMod.AddedParts = new(AddedParts ?? new());
            naturalEquipmentMod.AddedStringProps = new(AddedStringProps ?? new());
            naturalEquipmentMod.AddedIntProps = new(AddedIntProps ?? new());

            return ClearForCopy(naturalEquipmentMod);
        }
        public static ModNaturalEquipmentBase ClearForCopy(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            NaturalEquipmentMod.Wielder = null;
            return NaturalEquipmentMod;
        }

    } //!-- public class ModNaturalEquipmentBase : ModPart
}