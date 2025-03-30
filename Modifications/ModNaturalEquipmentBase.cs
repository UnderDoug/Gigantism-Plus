using System;
using System.Collections.Generic;
using SerializeField = UnityEngine.SerializeField;
using XRL.Language;
using XRL.World.Anatomy;
using static XRL.World.Statistic;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{public abstract class ModNaturalEquipmentBase : IMeleeModification
    {
        [Serializable]
        public struct Adjustment
        {
            public Guid ID;

            public string Target; // GameObject (the equipment itself), Render, MeleeWeapon, Armor

            public string Field; // Field/Property to adjust

            public int Priority; // Priority of adjustment, lower number = higher priority

            public string Value; // Value of the adjustment.

            public override readonly string ToString()
            {
                string output = string.Empty;
                output += $"({(Priority != 0 ? Priority : "PriorityUnset")})";
                output += $"{Target ?? "NoTarget?"}.";
                output += $"{Field ?? "NoField?"}=";
                output += Value ?? "null?";
                return output;
            }
            public readonly string ToString(bool ShowID)
            {
                string output = string.Empty;
                if (ShowID)
                    output += $"[{(ID != null ? ID : "No ID")}]";
                output += ToString();
                return output;
            }

            public readonly void Write(SerializationWriter Writer)
            {
                Writer.Write(ID);
                Writer.Write(Target);
                Writer.Write(Field);
                Writer.WriteOptimized(Priority);
                Writer.Write(Value);
            }

            public void Read(SerializationReader Reader)
            {
                ID = Reader.ReadGuid();
                Target = Reader.ReadString();
                Field = Reader.ReadString();
                Priority = Reader.ReadOptimizedInt32();
                Value = Reader.ReadString();
            }
        }

        public List<Adjustment> Adjustments;

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

        public List<string> AddedParts = new();
        public Dictionary<string, string> AddedStringProps = new();
        public Dictionary<string, int> AddedIntProps = new();

        public ModNaturalEquipmentBase()
        {
        }
        public ModNaturalEquipmentBase(int Tier)
            : base(Tier)
        {
            base.Tier = Tier;
        }
        public ModNaturalEquipmentBase(ModNaturalEquipmentBase Source)
            : this()
        {
            Adjustments = new List<Adjustment>(Source.Adjustments);

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

            AddedParts = new List<string>(Source.AddedParts);
            AddedStringProps = new Dictionary<string, string>(Source.AddedStringProps);
            AddedIntProps = new Dictionary<string, int>(Source.AddedIntProps);
        }

        public override void Configure()
        {
            base.Configure();
            WorksOnSelf = true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<Physics>() && !Object.HasPart<NaturalEquipment>())
            {
                return false;
            }
            return true;
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
            return Adjective;
        }
        public virtual string GetColoredAdjective()
        {
            return Adjective.OptionalColor(AdjectiveColor, AdjectiveColorFallback, Colorfulness);
        }

        public abstract string GetInstanceDescription();

        public virtual int GetDescriptionPriority()
        {
            return DescriptionPriority;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent, MapInv) as ModNaturalEquipmentBase;
            return ClearForCopy(naturalEquipmentMod);
        }
        public static ModNaturalEquipmentBase ClearForCopy(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            NaturalEquipmentMod.Wielder = null;
            return NaturalEquipmentMod;
        }

    } //!-- public class ModNaturalEquipmentBase : IMeleeModification
}