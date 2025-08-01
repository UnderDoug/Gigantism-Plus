using HNPS_GigantismPlus;
using Sheeter;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModNaturalEquipment<T> 
        : ModNaturalEquipmentBase
        , IDescribeModificationHandler<ModNaturalEquipment<T>>
        // , IModEventHandler<BeforeDescribeModificationEvent<ModNaturalEquipment<T>>>
        // , IModEventHandler<DescribeModificationEvent<ModNaturalEquipment<T>>>
        where T 
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("ModNaturalEquipment");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
                "AM",    // Apply Mod
                "APP",   // Apply Part & Prop
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        private T _assigningPart = null;

        public T AssigningPart
        {
            get => _assigningPart ??= Operator?.Manager?.GetManagedNaturalEquipmentCompatiblePart<T>();
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

        public override void AddAdjustment(IAdjustment Adjustment, ICondition<GameObject> Condition = null, AllConditions<GameObject> AllConditions = null, AnyConditions<GameObject> AnyConditions = null)
        {
            int indent = Debug.LastIndent;
            Adjustments ??= new();
            Adjustment.Source ??= GetType();
            base.AddAdjustment(Adjustment, Condition, AllConditions, AnyConditions);
            Debug.LastIndent = indent;
        }

        public virtual void ApplyPartAndPropChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyPartAndPropChanges)}(GameObject Object)", Indent: 4, Toggle: getDoDebug("APP"));
            Debug.Entry(4, $"{AssigningPart?.Name}; Level: {(int)AssigningPart?.Level}", Indent: 5, Toggle: getDoDebug("APP"));

            if (AddedParts != null)
            {
                Debug.Entry(4, "> foreach (string part in NaturalEquipmentMod.GetAddedParts())", Indent: 5, Toggle: getDoDebug("APP"));
                foreach (string part in AddedParts)
                {
                    Debug.LoopItem(4, "part", part, Indent: 6, Toggle: getDoDebug("APP"));
                    Object.RequirePart(part);
                }
                Debug.Entry(4, $"x foreach (string part in NaturalEquipmentMod.GetAddedParts()) >//", Indent: 5, Toggle: getDoDebug("APP"));
            }

            if (AddedStringProps != null)
            {
                bool priorityPropExists = Object.HasIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY);
                int priorityPropValue = Object.GetIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY);
                bool priorityPropBeaten = priorityPropValue > -ModPriority;
                if (!priorityPropExists || priorityPropBeaten)
                {
                    Object.SetIntProperty(NATEQUIPMANAGER_STRINGPROP_PRIORITY, -ModPriority);

                    Debug.Entry(4, "> foreach ((string Name, string Value) in AddedStringProps)", Indent: 5, Toggle: getDoDebug("APP"));
                    foreach ((string Name, string Value) in AddedStringProps)
                    {
                        Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6, Toggle: getDoDebug("APP"));
                        Object.SetStringProperty(Name: Name, Value: Value, RemoveIfNull: true);
                    }
                    Debug.Entry(4, $"x foreach ((string Name, string Value) in AddedStringProps) >//", Indent: 5, Toggle: getDoDebug("APP"));
                }
                else
                {
                    Debug.CheckNah(4,
                        $"{NATEQUIPMANAGER_STRINGPROP_PRIORITY} ({priorityPropValue}) <= ModPriority {-ModPriority}",
                        Indent: 5, Toggle: getDoDebug("APP"));
                }
            }
            else
            {
                Debug.CheckNah(4, $"No StringProps", Indent: 5, Toggle: getDoDebug("APP"));
            }

            if (AddedIntProps != null)
            {
                bool priorityPropExists = Object.HasIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY);
                int priorityPropValue = Object.GetIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY);
                bool priorityPropBeaten = priorityPropValue > -ModPriority;
                if (!priorityPropExists || priorityPropBeaten)
                {
                    Object.SetIntProperty(NATEQUIPMANAGER_INTPROP_PRIORITY, -ModPriority);

                    Debug.Entry(4, $"> foreach ((string Name, int Value) in AddedIntProps", Indent: 5, Toggle: getDoDebug("APP"));
                    foreach ((string Name, int Value) in AddedIntProps)
                    {
                        Debug.CheckYeh(4, $"{Name}", $"{Value}", Indent: 6, Toggle: getDoDebug("APP"));
                        Object.SetIntProperty(Name: Name, Value: Value, RemoveIfZero: true);
                    }
                    Debug.Entry(4, $"x foreach ((string Name, int Value) in AddedIntProps) >//", Indent: 5, Toggle: getDoDebug("APP"));
                }
                else
                {
                    Debug.CheckNah(4, 
                        $"{NATEQUIPMANAGER_INTPROP_PRIORITY} ({priorityPropValue}) <= ModPriority {-ModPriority}", 
                        Indent: 5, Toggle: getDoDebug("APP"));
                }
            }
            else
            {
                Debug.CheckNah(4, $"No IntProps", Indent: 5, Toggle: getDoDebug("APP"));
            }

            Debug.Entry(4, $"x {nameof(ApplyPartAndPropChanges)}(GameObject Object) *//", Indent: 4, Toggle: getDoDebug("APP"));
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            if(AssigningPart.Is(null))
            {
               AssigningPart = who.GetPart<T>();
            }
            if(AssigningPart.Is(null))
            {
                Debug.Warn(2,
                    $"{typeof(ModNaturalEquipment<T>).Name}<{GetSource()}>",
                    $"{nameof(BeingAppliedBy)}(" +
                    $"obj: {obj?.DebugName ?? NULL}, " +
                    $"who: {who?.DebugName ?? NULL})",
                    $"Failed to assign {GetSource()} as AssigningPart",
                    Indent: 0);
            }
            return base.BeingAppliedBy(obj, who);
        }
        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, 
                $"@ {Name}[{GetSource()}]."
                + $"{nameof(ApplyModification)}"
                + $"(Object: \"{Object.BaseDisplayName}\")", 
                Indent: 3, Toggle: getDoDebug("AM"));
            
            ApplyPartAndPropChanges(Object);

            Debug.Entry(4, 
                $"x {Name}[{GetSource()}]."
                + $"{nameof(ApplyModification)}"
                + $"(Object: \"{Object.BaseDisplayName}\")"
                + $" @//", 
                Indent: 3, Toggle: getDoDebug("AM"));
            base.ApplyModification(Object);
        }
        public override string GetSource()
        {
            return typeof(T).Name;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(BeforeDescribeModificationEvent<ModNaturalEquipment<T>>.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(DescribeModificationEvent<ModNaturalEquipment<T>>.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade);
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetColoredAdjective(), ModPriority);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeDescribeModificationEvent<ModNaturalEquipment<T>> E)
        {
            if (EnablePrereleaseContent && E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                int indent = Debug.LastIndent;
                Debug.LoopItem(4, 
                    $"{GetType().Name}.{nameof(HandleEvent)}({nameof(BeforeDescribeModificationEvent<ModNaturalEquipment<T>>)} E)", 
                    Indent: indent + 1, Toggle: true);

                if (!Adjustments.IsNullOrEmpty() && !Adjustments.GetApplied().IsNullOrEmpty())
                {
                    Debug.CheckYeh(4, $"Have Adjustments", Indent: indent + 2, Toggle: true);
                    foreach (IAdjustment adjustment in Adjustments.GetApplied())
                    {
                        Debug.Entry(4, $"{adjustment.ToString(true)}", Indent: indent + 3, Toggle: true);
                        if (adjustment.TryGetDescriptionElements(ParentObject, out List<DescriptionElement> weaponElements, out List<DescriptionElement> generalElements))
                        {
                            if (!weaponElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(weaponElements)}", Indent: indent + 4, Toggle: true);
                                foreach (DescriptionElement element in weaponElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: true);
                                }
                                E.WeaponDescriptions.AddRange(weaponElements);
                            }
                            if (!generalElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(generalElements)}", Indent: indent + 4, Toggle: true);
                                foreach (DescriptionElement element in generalElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: true);
                                }
                                E.GeneralDescriptions.AddRange(generalElements);
                            }
                        }
                    }
                }

                Debug.LastIndent = indent;
            }
            else if (E.Adjective == GetColoredAdjective() && E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                int dieCount = GetDamageDieCount();
                int dieSize = GetDamageDieSize();
                int damageBonus = GetDamageBonus();
                int hitBonus = GetHitBonus();
                int penBonus = GetPenBonus();

                if (dieCount != 0)
                {
                    E.AddWeaponElement("gain", $"{dieCount} additional damage die");
                }
                if (dieSize != 0)
                {
                    E.AddWeaponElement("gain", $"{dieSize.Signed()} damage die size");
                }
                if (damageBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (hitBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}");
                }
                if (penBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {penBonus.Signed()} penetration {penBonus.Signed().BonusOrPenalty()}");
                }
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<T>> E)
        {
            return base.HandleEvent(E);
        }

        public override string GetAdjective()
        {
            return Adjective ?? typeof(T).Name;
        }
        public override string GetInstanceDescription(GameObject Object = null)
        {
            return DescribeModificationEvent<ModNaturalEquipment<T>>
                .Send(Object, GetColoredAdjective(), Context: NATURAL_EQUIPMENT)
                .Process();
        }
        
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipment<T> modNaturalEquipment = base.DeepCopy(Parent, MapInv) as ModNaturalEquipment<T>;
            return ClearForCopy(modNaturalEquipment);
        }
        public override IPart DeepCopy(GameObject Parent)
        {
            ModNaturalEquipment<T> naturalEquipmentMod = base.DeepCopy(Parent) as ModNaturalEquipment<T>;
            return ClearForCopy(naturalEquipmentMod);
        }
        public static ModNaturalEquipment<T> ClearForCopy(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            NaturalEquipmentMod.AssigningPart = null;
            return NaturalEquipmentMod;
        }

    }
}