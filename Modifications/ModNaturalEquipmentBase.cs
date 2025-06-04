using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalEquipmentBase : IModification
    {
        private static bool doDebug => getClassDoDebug(nameof(ModNaturalEquipmentBase));

        [Serializable]
        public class HNPS_Adjustment : IScribedPart, IComposite
        {
            public Guid ID;

            public string Target; // GameObject (the equipment itself), Render, MeleeWeapon, Armor

            public string Field; // Field/Property to adjust

            public new int Priority; // Priority of adjustment, lower number = higher priority

            public string Value; // Value of the adjustment.

            public HNPS_Adjustment()
            {
                ID = Guid.Empty;
                Target = string.Empty;
                Field = string.Empty;
                Priority = 0;
                Value = string.Empty;
            }

            public HNPS_Adjustment(HNPS_Adjustment Source)
            {
                ID = Guid.NewGuid();
                Target = new(Source.Target);
                Field = new(Source.Field);
                Priority = Source.Priority;
                Value = new(Source.Value);
            }

            public HNPS_Adjustment(string Target, string Field, int Priority, string Value)
            {
                ID = Guid.NewGuid();
                this.Target = Target;
                this.Field = Field;
                this.Priority = Priority;
                this.Value = Value;
            }

            public override string ToString()
            {
                string output = string.Empty;
                output += $"({(Priority != 0 ? Priority : "PriorityUnset")})";
                output += $"{Target ?? "NoTarget?"}.";
                output += $"{Field ?? "NoField?"} = \"";
                output += Value ?? "Value?";
                output += "\"";
                return output;
            }
            public string ToString(bool ShowID)
            {
                string output = string.Empty;
                if (ShowID)
                    output += $"[{(ID != null ? ID : "No ID")}]";
                output += ToString();
                return output;
            }

            public override bool SameAs(IPart p)
            {
                if (p is HNPS_Adjustment a)
                {
                    return ID == a.ID && base.SameAs(p);
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
        public abstract ModNaturalEquipmentBase Copy();

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

        public abstract Guid AddAdjustment(string Target, string Field, string Value, int Priority);
        public abstract Guid AddAdjustment(string Target, string Field, string Value, bool FlipPriority = false);
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