using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModNaturalEquipment<T> 
        : ModNaturalEquipmentBase
        where T 
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("ModNaturalEquipment");

        [NonSerialized]
        private T _assigningPart = null;

        public T AssigningPart
        {
            get => _assigningPart ??= ParentObject?.Equipped?.GetManagedNaturalEquipmentCompatiblePart<T>();
            set => _assigningPart = value;
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
            Adjustment adjustment = new(Target, Field, Priority, Value);
            Adjustments ??= new();
            Adjustments.Add(adjustment);
            return adjustment.ID;
        }
        public override Guid AddAdjustment(string Target, string Field, string Value, bool FlipPriority = false)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
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
            Debug.Entry(4, $"* {nameof(ApplyPartAndPropChanges)}(GameObject Object)", Indent: 4, Toggle: doDebug);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {AssigningPart.Level}", Indent: 5, Toggle: doDebug);

            if (AddedParts != null)
            {
                Debug.Entry(4, "> foreach (string part in NaturalEquipmentMod.GetAddedParts())", Indent: 5, Toggle: doDebug);
                foreach (string part in AddedParts)
                {
                    Debug.LoopItem(4, "part", part, Indent: 6, Toggle: doDebug);
                    Object.RequirePart(part);
                }
                Debug.Entry(4, $"x foreach (string part in NaturalEquipmentMod.GetAddedParts()) >//", Indent: 5, Toggle: doDebug);
            }

            if (AddedStringProps != null)
            {
                bool priorityPropExists = Object.HasIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY);
                int priorityPropValue = Object.GetIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY);
                bool priorityPropBeaten = priorityPropValue > -ModPriority;
                if (!priorityPropExists || priorityPropBeaten)
                {
                    Object.SetIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY, -ModPriority);

                    Debug.Entry(4, "> foreach ((string Name, string Value) in AddedStringProps)", Indent: 5, Toggle: doDebug);
                    foreach ((string Name, string Value) in AddedStringProps)
                    {
                        Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6, Toggle: doDebug);
                        Object.SetStringProperty(Name: Name, Value: Value, RemoveIfNull: true);
                    }
                    Debug.Entry(4, $"x foreach ((string Name, string Value) in AddedStringProps) >//", Indent: 5, Toggle: doDebug);
                }
                else
                {
                    Debug.CheckNah(4,
                        $"{NATEQUIPMANAGER_STRINGPROP_PRIORITY} ({priorityPropValue}) <= ModPriority {-ModPriority}",
                        Indent: 5, Toggle: doDebug);
                }
            }
            else
            {
                Debug.CheckNah(4, $"No StringProps", Indent: 5);
            }

            if (AddedIntProps != null)
            {
                bool priorityPropExists = Object.HasIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY);
                int priorityPropValue = Object.GetIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY);
                bool priorityPropBeaten = priorityPropValue > -ModPriority;
                if (!priorityPropExists || priorityPropBeaten)
                {
                    Object.SetIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY, -ModPriority);

                    Debug.Entry(4, $"> foreach ((string Name, int Value) in AddedIntProps", Indent: 5, Toggle: doDebug);
                    foreach ((string Name, int Value) in AddedIntProps)
                    {
                        Debug.CheckYeh(4, $"{Name}", $"{Value}", Indent: 6, Toggle: doDebug);
                        Object.SetIntProperty(Name: Name, Value: Value, RemoveIfZero: true);
                    }
                    Debug.Entry(4, $"x foreach ((string Name, int Value) in AddedIntProps) >//", Indent: 5, Toggle: doDebug);
                }
                else
                {
                    Debug.CheckNah(4, 
                        $"{NATEQUIPMANAGER_INTPROP_PRIORITY} ({priorityPropValue}) <= ModPriority {-ModPriority}", 
                        Indent: 5, Toggle: doDebug);
                }
            }
            else
            {
                Debug.CheckNah(4, $"No IntProps", Indent: 5);
            }

            Debug.Entry(4, $"x {nameof(ApplyPartAndPropChanges)}(GameObject Object) *//", Indent: 4, Toggle: doDebug);
        }


        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            if(AssigningPart.Is(null))
            {
               AssigningPart = who.GetPart<T>();
            }
            if(AssigningPart.Is(null))
            {
                Debug.Entry(2,
                    $"WARN",
                    $"{typeof(ModNaturalEquipment<T>).Name}<{typeof(T).Name}>.{nameof(BeingAppliedBy)} (" +
                    $"GameObject obj: {obj.ID}:{obj.ShortDisplayNameStripped}, " +
                    $"GameObject who: {who.ID}:{who.ShortDisplayNameStripped}) - " +
                    $"Failed to assign {GetSource()} as AssigningPart",
                    Indent: 0, Toggle: doDebug);
            }
            return base.BeingAppliedBy(obj, who);
        }
        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, 
                $"@ {Name}[{GetSource()}]." + 
                $"{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\")", 
                Indent: 3, Toggle: doDebug);

            Object.GetPart<NaturalEquipmentManager>()
                .AddShortDescriptionEntry(this);
            
            ApplyPartAndPropChanges(Object);

            Debug.Entry(4, 
                $"x {Name}[{GetSource()}]." + 
                $"{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\") @//", 
                Indent: 3, Toggle: doDebug);
            base.ApplyModification(Object);
        }
        public override string GetSource()
        {
            return typeof(T).Name;
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

        public override string GetAdjective()
        {
            return Adjective ?? typeof(T).Name;
        }
        public override string GetInstanceDescription()
        {
            string text = ParentObject.GetObjectNoun();
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            int dieCount = GetDamageDieCount();
            int dieSize = GetDamageDieSize();
            int damageBonus = GetDamageBonus();
            int hitBonus = GetHitBonus();
            int penBonus = GetPenBonus();
            string description = $"{descriptionName}: ";
            description += ParentObject.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " ")
                        : ("This " + text + " ");
            
            List<List<string>> descriptions = new();
            if (dieCount != 0) descriptions
                    .Add(new() { "gain", $"{dieCount} additional damage die" });
            if (dieSize != 0) descriptions
                    .Add(new() { "gain", $"{dieSize.Signed()} damage die size" });
            if (damageBonus != 0) descriptions
                    .Add(new() { "have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage" });
            if (hitBonus != 0) descriptions
                    .Add(new() { "have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}" });
            if (penBonus != 0) descriptions
                    .Add(new() { "have", $"a {penBonus.Signed()} penetration {penBonus.Signed().BonusOrPenalty()}" });
            if (descriptions.IsNullOrEmpty()) descriptions
                    .Add(new() { "gain", "some manner of adjustments" });


            List<string> processedDescriptions = new();
            foreach (List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, ParentObject));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
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

    } //!-- public class ModNaturalEquipment<E> : ModNaturalEquipmentBase where E : IPart, IManagedDefaultNaturalEquipment<E>, new()
}