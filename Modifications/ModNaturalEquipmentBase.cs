using HarmonyLib;
using HNPS_GigantismPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
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
    public abstract class ModNaturalEquipmentBase : IModification
    {
        private static bool doDebug => getClassDoDebug(nameof(ModNaturalEquipmentBase));

        [Serializable]
        public class HNPS_Adjustment : IComposite
        {
            public Guid ID;

            public bool Applied; // Whether the adjustment has been applied.

            public string Target; // Render, MeleeWeapon, Armor, etc.

            public string Field; // Field/Property to adjust

            public int Priority; // Priority of adjustment, lower number = higher priority

            public object Value; // Value to adjust the Field to.

            public HNPS_Adjustment()
            {
                ID = Guid.NewGuid();
                Applied = false;
                Target = string.Empty;
                Field = string.Empty;
                Priority = 0;
                Value = null;
            }

            public HNPS_Adjustment(string Target, string Field, int Priority, object Value)
                : this ()
            {
                this.Target = Target;
                this.Field = Field;
                this.Priority = Priority;
                this.Value = Value;
            }

            public HNPS_Adjustment(HNPS_Adjustment Source)
                : this (Source.Target, Source.Field, Source.Priority, Source.Value)
            {
            }
            public HNPS_Adjustment(string Address)
                : this ()
            {
                SetAddress(Address);
            }
            public HNPS_Adjustment(string Address, int Priority, object Value)
                : this (Address)
            {
                this.Priority = Priority;
                this.Value = Value;
            }

            public override string ToString()
            {
                string output = string.Empty;
                output += $"({(Priority != 0 ? Priority : "PriorityUnset")})";
                output += $"{Target ?? "NoTarget?"}.";
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
                    output += $"[{(ID != null ? ID : "No ID")}]";
                output += ToString();
                return output;
            }

            public bool HasSameTargetAs(HNPS_Adjustment OtherAdjustment)
            {
                return GetAddress() == OtherAdjustment.GetAddress();
            }

            public bool TryGetHigherPriorityAdjustment(HNPS_Adjustment OtherAdjustment, out HNPS_Adjustment HigherProrityAdjustment)
            {
                HigherProrityAdjustment = null;
                if (HasSameTargetAs(OtherAdjustment))
                {
                    HigherProrityAdjustment = Priority < OtherAdjustment.Priority ? this : OtherAdjustment;
                    return true;
                }
                return false;
            }

            public bool SameAs(HNPS_Adjustment a)
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
                    string[] pieces = Address.Split('.');
                    Target = pieces[0];
                    Field = pieces[1];
                    return true;
                }
                return false;
            }

            public bool Apply(GameObject Object)
            {
                if (Object != null && !Applied)
                {
                    IPart targetPart = Object.GetPart(Target);
                    if (targetPart != null)
                    {
                        Traverse targetPartTraverse = new(targetPart);
                        Traverse targetProperty = targetPartTraverse.Property(Field);
                        Traverse targetField = targetPartTraverse.Field(Field);
                        Type valueType = Value.GetType();
                        try
                        {
                            if (targetProperty.PropertyExists() && targetProperty.GetValueType() == valueType)
                            {
                                targetProperty.SetValue(Value);

                                Applied = true;
                                return targetProperty.GetValue().Equals(Value);
                            }
                            if (targetField.PropertyExists() && targetField.GetValueType() == valueType)
                            {
                                targetField.SetValue(Value);

                                Applied = true;
                                return targetField.GetValue().Equals(Value);
                            }
                        }
                        catch (Exception e)
                        {
                            MetricsManager.LogModError(ThisMod, e);
                            return false;
                        }
                    }
                    Applied = true;
                }
                return false;
            }



        }

        [NonSerialized]
        public List<HNPS_Adjustment> Adjustments;

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

        public abstract Guid AddAdjustment(string Target, string Field, object Value, int Priority);
        public abstract Guid AddAdjustment(string Target, string Field, object Value, bool FlipPriority = false);
        public abstract int GetDamageDieCount();
        public abstract int GetDamageDieSize();
        public abstract int GetDamageBonus();
        public abstract int GetHitBonus();
        public abstract int GetPenBonus();

        public override void ApplyModification(GameObject Object)
        {
            base.ApplyModification(Object);
        }
        public abstract string GetSource();

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
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

            Adjustments = Reader.ReadList<HNPS_Adjustment>() ?? new();
            AddedParts = Reader.ReadList<string>() ?? new();
            AddedStringProps = Reader.ReadDictionary<string, string>() ?? new();
            AddedIntProps = Reader.ReadDictionary<string, int>() ?? new();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent, MapInv) as ModNaturalEquipmentBase;

            naturalEquipmentMod.Adjustments = new();
            Adjustments ??= new();
            if (!Adjustments.IsNullOrEmpty())
            {
                foreach (HNPS_Adjustment adjustment in Adjustments)
                {
                    naturalEquipmentMod.Adjustments.Add(new(adjustment));
                }
            }
            
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

    } //!-- public class ModNaturalEquipmentBase : IModification
}