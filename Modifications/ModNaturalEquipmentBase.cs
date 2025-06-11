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
using static XRL.World.Parts.ModNaturalEquipmentBase;
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
        public class PartAdjustment : IScribedPart
        {
            private static bool doDebug => getClassDoDebug(nameof(PartAdjustment));

            [NonSerialized]
            public Guid ID;

            [NonSerialized]
            public bool Applied; // Whether the adjustment has been applied.

            [NonSerialized]
            public string ParentNaturalEquipmentMod; // String name of the NaturalEquipmentMod this part adjust belongs to.

            [NonSerialized]
            public Type Target; // Render, MeleeWeapon, Armor, etc.

            [NonSerialized]
            public string Field; // Field/Property to adjust

            [NonSerialized]
            public int AdjustmentPriority; // Priority of adjustment, lower number = higher priority

            [NonSerialized]
            public object Value; // Value to adjust the Field to.

            [NonSerialized]
            public Func<GameObject, bool> Condition;

            public PartAdjustment()
            {
                ID = Guid.NewGuid();
                Applied = false;
                ParentNaturalEquipmentMod = string.Empty;
                Target = null;
                Field = string.Empty;
                AdjustmentPriority = 0;
                Value = null;
                Condition = null;
            }

            public PartAdjustment(string ParentNaturalEquipmentMod, Type Target, string Field, int Priority, object Value, Func<GameObject, bool> Condition)
                : this ()
            {
                this.ParentNaturalEquipmentMod = ParentNaturalEquipmentMod;
                this.Target = Target;
                this.Field = Field;
                this.AdjustmentPriority = Priority;
                this.Value = Value;
                this.Condition = Condition;
            }

            public PartAdjustment(PartAdjustment Source)
                : this (Source.ParentNaturalEquipmentMod, Source.Target, Source.Field, Source.AdjustmentPriority, Source.Value, Source.Condition)
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
                this.AdjustmentPriority = Priority;
                this.Value = Value;
            }

            public override string ToString()
            {
                string output = string.Empty;
                output += $"({(AdjustmentPriority != 0 ? AdjustmentPriority : "AdjustmentPriorityUnset")})";
                output += $"<{(Applied ? "Applied" : "Unapplied")}> ";
                output += $"{Target?.Name ?? "NoTarget?"}.";
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
                    output += $"[{(ID != null ? ID : "No ManagerID")}::{ParentNaturalEquipmentMod}]";
                }
                output += ToString();
                return output;
            }

            public virtual bool CheckCondition(GameObject Equipment = null)
            {
                return Equipment == null || Condition == null || Condition.Invoke(Equipment);
            }

            public virtual bool HasSameTargetAs(PartAdjustment OtherAdjustment)
            {
                return GetAddress() == OtherAdjustment.GetAddress();
            }

            public virtual bool IsTruerThan(GameObject Equipment, PartAdjustment OtherAdjustment)
            {
                int indent = Debug.LastIndent;

                if (Equipment == null || OtherAdjustment == null) return true;

                bool otherCondition = true;
                bool condition = true;
                try
                {
                    condition = CheckCondition(Equipment);
                    Debug.LoopItem(4, $"{nameof(Condition)} Checked", $"{condition}", Good: condition, Indent: indent + 1, Toggle: doDebug);
                }
                catch (Exception e)
                {
                    Debug.CheckNah(4, $"{nameof(Condition)} Checked", $"{nameof(Exception)}", Indent: indent + 2, Toggle: doDebug);
                    MetricsManager.LogModError(ThisMod, e);
                }
                try
                {
                    otherCondition = OtherAdjustment.CheckCondition(Equipment);
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

            public virtual bool TryGetHigherPriorityAdjustment(GameObject Equipment, PartAdjustment OtherAdjustment, out PartAdjustment HigherProrityAdjustment)
            {
                HigherProrityAdjustment = null;
                if (HasSameTargetAs(OtherAdjustment) && IsTruerThan(Equipment, OtherAdjustment))
                {
                    HigherProrityAdjustment = AdjustmentPriority < OtherAdjustment.AdjustmentPriority ? this : OtherAdjustment;
                    return true;
                }
                return false;
            }

            public override bool SameAs(IPart p)
            {
                if (p is PartAdjustment a)
                {
                    return ID == a.ID;
                }
                return false;
            }

            public virtual string GetAddress()
            {
                return $"{Target?.Name ?? "null"}.{Field ?? "null"}";
            }

            public virtual bool SetAddress(string Address)
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

            public virtual bool Apply(GameObject Equipment)
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
                    Debug.Entry(4, $"{nameof(Target)}: {targetPart?.GetType()?.Name ?? NULL}", Indent: indent + 2, Toggle: doDebug);
                    if (targetPart != null)
                    {
                        if (BeforeApplyPartAdjustmentEvent.Send(Equipment, ParentNaturalEquipmentMod, Target, Field, ref Value, Condition) && Value != null)
                        {
                            Debug.CheckYeh(4, $"Have {targetPart.GetType().Name}", Indent: indent + 2, Toggle: doDebug);
                            Traverse targetPartTraverse = new(targetPart);
                            Traverse targetProperty = targetPartTraverse.Property(Field);
                            Traverse targetField = targetPartTraverse.Field(Field);
                            Type valueType = Value.GetType();
                            Debug.Entry(4, $"Value Type: {valueType?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            Debug.Entry(4, $"{nameof(targetProperty)}.GetValueType: {targetProperty?.GetValueType()?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            Debug.Entry(4, $"{nameof(targetField)}.GetValueType: {targetField?.GetValueType()?.Name ?? NULL}", Indent: indent + 3, Toggle: doDebug);
                            try
                            {
                                bool condition = CheckCondition(Equipment);
                                Debug.CheckYeh(4, $"{nameof(Condition)} Checked", $"{condition}", Indent: indent + 3, Toggle: doDebug);
                                if (condition && targetProperty.PropertyExists() && targetProperty.GetValueType() == valueType)
                                {
                                    Debug.CheckYeh(4, $"{nameof(targetProperty)}", Indent: indent + 3, Toggle: doDebug);
                                    Debug.Entry(4, $"Property Type: {targetProperty.GetValueType().Name}", Indent: indent + 4, Toggle: doDebug);
                                    targetProperty.SetValue(Value);

                                    Applied = true;
                                    Debug.Entry(4, $"Value: {targetProperty.GetValue()}", Indent: indent + 4, Toggle: doDebug);
                                    Debug.LastIndent = indent;
                                    return targetProperty.GetValue().Equals(Value);
                                }
                                if (condition && targetField.FieldExists() && targetField.GetValueType() == valueType)
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
                        Debug.CheckNah(4, $"Object lacks, and is not itself, a {Target.Name}", Indent: indent + 2, Toggle: doDebug);
                        Applied = true;
                    }
                    Debug.LastIndent = indent;
                }
                return false;
            }
            public override void Write(GameObject Basis, SerializationWriter Writer)
            {
                base.Write(Basis, Writer);

                Writer.Write(ID);
                Writer.Write(Applied);
                Writer.Write(ParentNaturalEquipmentMod);
                Writer.WriteObject(Target);
                Writer.Write(Field);
                Writer.Write(AdjustmentPriority);
                Writer.WriteObject(Value);
                Writer.WriteObject(Condition);
            }
            public override void Read(GameObject Basis, SerializationReader Reader)
            {
                base.Read(Basis, Reader);

                ID = Reader.ReadGuid();
                Applied = Reader.ReadBoolean();
                ParentNaturalEquipmentMod = Reader.ReadString();
                Target = Reader.ReadObject() as Type;
                Field = Reader.ReadString();
                AdjustmentPriority = Reader.ReadInt32();
                Value = Reader.ReadObject();
                Condition = Reader.ReadObject() as Func<GameObject, bool>;
            }
            public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
            {
                PartAdjustment partAdjustment = base.DeepCopy(Parent, MapInv) as PartAdjustment;

                partAdjustment.ID = Guid.NewGuid();

                return partAdjustment;
            }
        }

        private GameObject _wielder = null;
        public GameObject Wielder
        {
            get => _wielder ??= ParentObject?.Equipped;
            set => _wielder = value;
        }

        private NaturalEquipmentManager _manager = null;
        public NaturalEquipmentManager Manager
        {
            get => _manager ??= ParentObject?.GetPart<NaturalEquipmentManager>();
            set => _manager = value;
        }

        [NonSerialized]
        public List<PartAdjustment> Adjustments;

        public string BodyPartType;

        public int ModPriority;
        public int DescriptionPriority;

        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;
        public int PenBonus;

        public bool ForceNoun;
        public string Noun;

        public string Adjective;
        public string AdjectiveColor;
        public string AdjectiveColorFallback;
        public bool ExludeFromDynamicTile;

        [NonSerialized]
        public List<string> AddedParts;

        [NonSerialized]
        public Dictionary<string, string> AddedStringProps;

        [NonSerialized]
        public Dictionary<string, int> AddedIntProps;

        public ModNaturalEquipmentBase()
        {
            Adjustments = new();
            ForceNoun = false;
            ExludeFromDynamicTile = false;
            AddedParts = new();
            AddedStringProps = new();
            AddedIntProps = new();
        }
        public ModNaturalEquipmentBase(int Tier)
            : base(Tier)
        {
            Adjustments = new();
        }
        public ModNaturalEquipmentBase(ModNaturalEquipmentBase Source)
            : this()
        {
            BodyPartType = Source.BodyPartType;

            Adjustments = new(Source.Adjustments ??= new());

            ModPriority = Source.ModPriority;
            DescriptionPriority = Source.DescriptionPriority;

            DamageDieCount = Source.DamageDieCount;
            DamageDieSize = Source.DamageDieSize;
            DamageBonus = Source.DamageBonus;
            HitBonus = Source.HitBonus;
            PenBonus = Source.PenBonus;

            ForceNoun = Source.ForceNoun;
            Noun = Source.Noun;

            Adjective = Source.Adjective;
            AdjectiveColor = Source.AdjectiveColor;
            AdjectiveColorFallback = Source.AdjectiveColorFallback;
            ExludeFromDynamicTile = Source.ExludeFromDynamicTile;

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

        public virtual Guid AddAdjustment(Type Target, string Field, object Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            PartAdjustment adjustment = new(GetType().Name, Target, Field, Priority, Value, Condition);
            Adjustments ??= new();
            Adjustments.Add(adjustment);
            return adjustment.ID;
        }
        public virtual Guid AddAdjustment(Type Target, string Field, object Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddAdjustment(Target, Field, Value, modPriority, Condition);
        }

        public virtual Guid AddNounAdjustment(int Priority, Func<GameObject, bool> Condition = null)
        {
            string noun = GetNoun();
            if (noun != null)
            {
                return AddAdjustment(RENDER, "DisplayName", noun, Priority, Condition);
            }
            return Guid.Empty;
        }
        public virtual Guid AddNounAdjustment(bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddNounAdjustment(modPriority, Condition);
        }

        public virtual Guid AddSkillAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(MELEEWEAPON, "Skill", Value, Priority, Condition);
        }
        public virtual Guid AddSkillAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddSkillAdjustment(Value, modPriority, Condition);
        }

        public virtual Guid AddStatAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(MELEEWEAPON, "Stat", Value, Priority, Condition);
        }
        public virtual Guid AddStatAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddStatAdjustment(Value, modPriority, Condition);
        }

        public virtual Guid AddTileAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(RENDER, "Tile", Value, Priority, Condition);
        }
        public virtual Guid AddTileAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddTileAdjustment(Value, modPriority, Condition);
        }

        public virtual Guid AddColorStringAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(RENDER, "ColorString", Value, Priority, Condition);
        }
        public virtual Guid AddColorStringAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddColorStringAdjustment(Value, modPriority, Condition);
        }

        public virtual Guid AddTileColorAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(RENDER, "TileColor", Value, Priority, Condition);
        }
        public virtual Guid AddTileColorAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddTileColorAdjustment(Value, modPriority, Condition);
        }

        public virtual Guid AddDetailColorAdjustment(string Value, int Priority, Func<GameObject, bool> Condition = null)
        {
            return AddAdjustment(RENDER, "DetailColor", Value, Priority, Condition);
        }
        public virtual Guid AddDetailColorAdjustment(string Value, bool FlipPriority = false, Func<GameObject, bool> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddDetailColorAdjustment(Value, modPriority, Condition);
        }

        public virtual int GetDamageDieCount()
        {
            return DamageDieCount;
        }
        public virtual int GetDamageDieSize()
        {
            return DamageDieSize;
        }

        public virtual int GetDamageBonus()
        {
            return DamageBonus;
        }

        public virtual int GetHitBonus()
        {
            return HitBonus;
        }
        public virtual int GetPenBonus()
        {
            return PenBonus;
        }

        public virtual string GetNoun()
        {
            return Noun;
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
            bool sameNoun = Noun == m.Noun;
            bool sameAdjective = Adjective == m.Adjective;
            bool sameSource = GetSource() == m.GetSource();

            bool sameGenerally = sameBodyPartType && sameNoun && sameAdjective && sameSource;
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

            naturalEquipmentMod.ForceNoun = ForceNoun;
            naturalEquipmentMod.Noun = Noun;

            naturalEquipmentMod.Adjective = Adjective;
            naturalEquipmentMod.AdjectiveColor = AdjectiveColor;
            naturalEquipmentMod.AdjectiveColorFallback = AdjectiveColorFallback;
            naturalEquipmentMod.ExludeFromDynamicTile = ExludeFromDynamicTile;

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