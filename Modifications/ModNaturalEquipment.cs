using System;
using System.Collections.Generic;
using SerializeField = UnityEngine.SerializeField;
using XRL.Language;
using XRL.World.Anatomy;
using static XRL.World.Statistic;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModNaturalEquipment<T> : ModNaturalEquipmentBase
        where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
    {
        private T _assigningPart = null;

        public T AssigningPart
        {
            get => _assigningPart ??= ParentObject?.GetManagedNaturalEquipmentCompatiblePart<T>();
            set => _assigningPart = null;
        }

        public ModNaturalEquipment()
        {
        }
        public ModNaturalEquipment(int Tier)
            : base(Tier)
        {
            base.Tier = Tier;
        }
        public ModNaturalEquipment(ModNaturalEquipment<T> Source)
            : base(Source)
        {
            AssigningPart = Source.AssigningPart;
        }
        public ModNaturalEquipment(ModNaturalEquipment<T> Source, T NewAssigningPart)
            : base(Source)
        {
            AssigningPart = NewAssigningPart;
        }

        public override Guid AddAdjustment(string Target, string Field, string Value, int Priority)
        {
            Guid guid = Guid.NewGuid();
            Adjustment adjustment = default;

            adjustment.ID = guid;
            adjustment.Target = Target;
            adjustment.Field = Field;
            adjustment.Value = Value;
            adjustment.Priority = Priority;

            Adjustment item = adjustment;

            Adjustments ??= new List<Adjustment>(1);

            Debug.Entry(4, $"\u263C New NaturalEquipment Adjustmnt: {typeof(T).Name} wants to adjust {ParentObject.DisplayNameOnly}'s {item.Target}.{item.Field} to {item.Value} with a priority of {item.Priority}", Indent: 0);

            Adjustments.Add(item);
            return guid;
        }
        public override Guid AddAdjustment(string Target, string Field, string Value, bool FlipPriority = false)
        {
            int modPriority = FlipPriority ? ModPriority : -ModPriority;
            return AddAdjustment(Target, Field, Value, modPriority);
        }

        public override int GetDamageDieCount()
        {
            return DamageDieCount;
        }
        public override int GetDamageDieSize()
        {
            return DamageDieSize;
        }

        public override int GetDamageBonus()
        {
            return DamageBonus;
        }

        public override int GetHitBonus()
        {
            return HitBonus;
        }
        public override int GetPenBonus()
        {
            return PenBonus;
        }

        public virtual void ApplyPartAndPropChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyPartAndPropChanges)}(GameObject Object)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {AssigningPart.Level}", Indent: 5);

            if (AddedParts != null)
            {
                Debug.Entry(4, "> foreach (string part in NaturalEquipmentMod.GetAddedParts())", Indent: 5);
                foreach (string part in AddedParts)
                {
                    Debug.LoopItem(4, "part", part, Indent: 6);
                    Object.RequirePart(part);
                }
                Debug.Entry(4, $"x foreach (string part in NaturalEquipmentMod.GetAddedParts()) >//", Indent: 5);
            }

            if (AddedStringProps != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, string> entry in NaturalEquipmentMod.GetAddedStringProps())", Indent: 5);
                foreach ((string Name, string Value) in AddedStringProps)
                {
                    Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6);
                    Object.SetStringProperty(Name: Name, Value: Value, RemoveIfNull: true);
                }
                Debug.Entry(4, $"x foreach (KeyValuePair<string, string> entry in NaturalEquipmentMod.GetAddedStringProps()) >//", Indent: 5);
            }

            if (AddedIntProps != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, int> entry in NaturalEquipmentMod.GetAddedIntProps())", Indent: 5);
                foreach ((string Name, int Value) in AddedIntProps)
                {
                    Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6);
                    Object.SetIntProperty(Name: Name, Value: Value, RemoveIfZero: true);
                }
                Debug.Entry(4, $"x foreach ((string Name, int Value) in NaturalEquipmentMod.GetAddedIntProps()) >//", Indent: 5);
            }

            Debug.Entry(4, $"x {nameof(ApplyPartAndPropChanges)}(GameObject Object) *//", Indent: 4);
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ {Name}.{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\")", Indent: 3);
            Debug.Entry(4, "? if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))", Indent: 4);
            
            ApplyPartAndPropChanges(Object);

            Debug.Entry(4, $"x {Name}.{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\") @//", Indent: 3);
            base.ApplyModification(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {

            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetColoredAdjective(), ModPriority);
            }
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            return null;
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
            ModNaturalEquipment<T> modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalEquipment<T>;
            return ClearForCopy(modNaturalWeaponBase);
        }
        public static ModNaturalEquipment<T> ClearForCopy(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            NaturalEquipmentMod.AssigningPart = null;
            return NaturalEquipmentMod;
        }

    } //!-- public class ModNaturalEquipment<T> : ModNaturalEquipmentBase where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
}