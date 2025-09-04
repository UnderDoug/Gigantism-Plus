using HNPS_GigantismPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager
        : IScribedPart
        , IModEventHandler<GetNaturalEquipmentOperatorsEvent>
        , IModEventHandler<BeforeUpdateBodyPartsEvent>
        , IModEventHandler<BodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(NaturalEquipmentManager));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "OC",   // ObjectCreation
                nameof(GetNaturalEquipmentMods),
                $"{nameof(PrioritiseNaturalEquipmentMods)}:{true}",
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "S",    // Serialisation
                nameof(EquipperEquippedEvent),
                nameof(BodyPartsUpdatedEvent),
                nameof(AfterBodyPartsUpdatedEvent),
                nameof(BeforeUpdateBodyPartsEvent),
                $"{nameof(PrioritiseNaturalEquipmentMods)}:{false}",
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        [NonSerialized]
        public static List<List<ModNaturalEquipmentBase>> NaturalEquipmentModListPool = new();

        [NonSerialized]
        public static int nNaturalEquipmentModListPoolCounter = 0;

        public List<NaturalEquipmentOperator> NaturalEquipmentOperators => GetNaturalEquipmentOperators(ParentObject, this);

        public bool WantsToManage => ParentObject != null && ParentObject.IsCreature;

        public NaturalEquipmentManager()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
        }
        public override void Remove()
        {
            base.Remove();
        }

        public static void ResetNaturalEquipmentModListPool()
        {
            if (!NaturalEquipmentModListPool.IsNullOrEmpty())
            {
                for (int i = 0; i < nNaturalEquipmentModListPoolCounter; i++)
                {
                    if (!NaturalEquipmentModListPool[i].IsNullOrEmpty())
                    {
                        NaturalEquipmentModListPool[i].Clear();
                    }
                }
            }
            nNaturalEquipmentModListPoolCounter = 0;
        }

        public static List<ModNaturalEquipmentBase> NewNaturalEquipmentModList()
        {
            NaturalEquipmentModListPool ??= new();
            while (NaturalEquipmentModListPool.Count <= nNaturalEquipmentModListPoolCounter)
            {
                NaturalEquipmentModListPool.Add(new List<ModNaturalEquipmentBase>(12));
            }
            List<ModNaturalEquipmentBase> list = NaturalEquipmentModListPool[nNaturalEquipmentModListPoolCounter++];
            list.Clear();
            return list;
        }
        public static List<ModNaturalEquipmentBase> NewNaturalEquipmentModList(List<ModNaturalEquipmentBase> List)
        {
            List<ModNaturalEquipmentBase> list = NewNaturalEquipmentModList();
            if (!List.IsNullOrEmpty())
            {
                list.AddRange(List);
            }
            return list;
        }
        public static List<ModNaturalEquipmentBase> NewNaturalEquipmentModList(List<ModNaturalEquipmentBase> List, Predicate<ModNaturalEquipmentBase> Filter)
        {
            if (List.IsNullOrEmpty())
            {
                return NewNaturalEquipmentModList();
            }
            if (Filter == null)
            {
                return NewNaturalEquipmentModList(List);
            }
            List<ModNaturalEquipmentBase> list = NewNaturalEquipmentModList();
            foreach (ModNaturalEquipmentBase item in List)
            {
                if (Filter(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public static List<ModNaturalEquipmentBase> NewNaturalEquipmentModList(IEnumerable<ModNaturalEquipmentBase> List)
        {
            List<ModNaturalEquipmentBase> list = NewNaturalEquipmentModList();
            if (!List.IsNullOrEmpty())
            {
                list.AddRange(List);
            }
            return list;
        }

        public void SyncOperators()
        {
            foreach (NaturalEquipmentOperator naturalEquipmentOperator in NaturalEquipmentOperators)
            {
                naturalEquipmentOperator.Manager = this;
            }
        }

        public static List<NaturalEquipmentOperator> GetNaturalEquipmentOperators(GameObject Creature, NaturalEquipmentManager Manager)
        {
            GetNaturalEquipmentOperatorsEvent E = GetNaturalEquipmentOperatorsEvent.FromPool();
            E.Manager = Manager;
            E.Creature = Creature;
            E.Operators = new();

            E.GetForCreature();

            // List<NaturalEquipmentOperator> naturalEquipmentOperators = GetNaturalEquipmentOperatorsEvent.GetForCreature(Creature, Manager);

            List<BodyPart> bodyParts = Creature?.Body?.GetParts();
            if (!bodyParts.IsNullOrEmpty())
            {
                foreach (BodyPart bodyPart in bodyParts)
                {
                    E.Equipment = bodyPart.DefaultBehavior;
                    if (E.Equipment != null && E.Equipment.IsNaturalEquipment())
                    {
                        E.GetForEquipment();
                        continue;
                    }
                    E.Equipment = bodyPart.Equipped;
                    if (E.Equipment != null && E.Equipment.IsNaturalEquipment())
                    {
                        E.GetForEquipment();
                        continue;
                    }
                }
            }
            List<NaturalEquipmentOperator> naturalEquipmentOperators = E.Operators;
            E.Reset();

            return naturalEquipmentOperators;
        }

        public IEnumerable<IManagedDefaultNaturalEquipment> GetManagedNaturalEquipmentCompatibleParts()
        {
            List<IManagedDefaultNaturalEquipment> managedDefaultNaturalEquipmentParts = ParentObject?.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>();
            if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
            {
                foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentPart in ParentObject.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>())
                {
                    yield return managedDefaultNaturalEquipmentPart;
                }
            }
            List<GameObject> installedCyberneticsList = Event.NewGameObjectList(ParentObject?.Body?.GetInstalledCybernetics());
            if (!installedCyberneticsList.IsNullOrEmpty())
            {
                foreach (GameObject installedCybernetic in installedCyberneticsList)
                {
                    managedDefaultNaturalEquipmentParts = installedCybernetic.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>();
                    if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
                    {
                        foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentCybernetic in managedDefaultNaturalEquipmentParts)
                        {
                            yield return managedDefaultNaturalEquipmentCybernetic;
                        }
                    }
                }
            }
            yield break;
        }

        public T GetManagedNaturalEquipmentCompatiblePart<T>()
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            List<IManagedDefaultNaturalEquipment> managedDefaultNaturalEquipmentParts = new(GetManagedNaturalEquipmentCompatibleParts());
            if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
            {
                foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentPart in managedDefaultNaturalEquipmentParts)
                {
                    if (managedDefaultNaturalEquipmentPart is T managedNaturalEquipmentCompatiblePart)
                    {
                        return managedNaturalEquipmentCompatiblePart;
                    }
                }
            }
            return null;
        }

        public static List<ModNaturalEquipment<T>> GetNaturalEquipmentMods<T>(NaturalEquipmentManager Manager, Predicate<ModNaturalEquipment<T>> Filter = null)
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(GetNaturalEquipmentMods));
            Debug.Entry(2,
                $"* {nameof(NaturalEquipmentManager)}."
                + $"{nameof(GetNaturalEquipmentMods)}<"
                + $"{typeof(T).Name}>("
                + $"{nameof(Manager)}, "
                + $"{nameof(Filter)})",
                Indent: indent + 1, Toggle: doDebug);

            List<ModNaturalEquipment<T>> naturalEquipmentModsList = new();

            List<MethodInfo> managedBaseMethods = new(typeof(T).GetMethods());
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                managedBaseMethods.RemoveAll(m => !m.IsStatic || !m.IsPublic || !m.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<T>)));
            }
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                Debug.CheckYeh(3, $"Have Methods", Indent: indent + 2, Toggle: doDebug);
                foreach (MethodInfo managedMethod in managedBaseMethods)
                {
                    if (!managedMethod.IsStatic || !managedMethod.IsPublic)
                    {
                        continue;
                    }

                    Debug.Divider(3, HONLY, Indent: indent + 3, Toggle: doDebug);

                    Debug.LoopItem(3, $"{nameof(managedMethod)}: {managedMethod.Name}", Indent: indent + 3, Toggle: doDebug);

                    Debug.LoopItem(4, $"{nameof(managedMethod.IsPublic)}: {managedMethod.IsPublic}",
                        Indent: indent + 4, Toggle: doDebug);
                    Debug.LoopItem(4, $"{nameof(managedMethod.IsStatic)}: {managedMethod.IsStatic}",
                        Indent: indent + 4, Toggle: doDebug);
                    Debug.LoopItem(4, $"{nameof(managedMethod.ReturnType)}: {managedMethod.ReturnType.Name}",
                        Indent: indent + 4, Toggle: doDebug);

                    if (managedMethod.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<T>), Silent: false)
                        && managedMethod.IsStatic
                        && managedMethod.IsPublic)
                    {
                        ParameterInfo[] parameters = managedMethod.GetParameters();
                        if (parameters.Length == 1
                            && parameters[0].ParameterType.InheritsFrom(typeof(NaturalEquipmentManager), Silent: false))
                        {
                            Debug.CheckYeh(3,
                                $"public static {managedMethod.ReturnType.Name} " +
                                $"{managedMethod.Name}(" +
                                $"{parameters[0].ParameterType.Name} {parameters[0].Name})",
                                Indent: indent + 4, Toggle: doDebug);

                            if (managedMethod.Invoke(null, new object[1] { Manager }) is ModNaturalEquipment<T> naturalEquipmentMod)
                            {
                                Debug.CheckYeh(3, $"Successful {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: doDebug);
                                if (naturalEquipmentMod != null && Filter(naturalEquipmentMod))
                                {
                                    Debug.CheckYeh(3, $"Passed {nameof(Filter)}, added to List", Indent: indent + 3, Toggle: doDebug);
                                    naturalEquipmentModsList.Add(naturalEquipmentMod);
                                }
                                else
                                {
                                    Debug.CheckNah(3, $"Failed {nameof(Filter)}", Indent: indent + 3, Toggle: doDebug);
                                }
                            }
                            else
                            {
                                Debug.CheckNah(3, $"Failed {nameof(managedMethod.Invoke)} (May be that the mod is conditionally produced)", Indent: indent + 3, Toggle: doDebug);
                            }
                        }
                    }
                }
                Debug.Divider(3, HONLY, Indent: indent + 3, Toggle: doDebug);
            }
            Debug.LastIndent = indent;
            return naturalEquipmentModsList;
        }
        public List<ModNaturalEquipment<T>> GetNaturalEquipmentMods<T>(Predicate<ModNaturalEquipment<T>> Filter = null)
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            return GetNaturalEquipmentMods(this, Filter);
        }

        public static SortedDictionary<int, ModNaturalEquipmentBase> PrioritiseNaturalEquipmentMods(List<ModNaturalEquipmentBase> NaturalEquipmentModList, bool ForDescriptions = false)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug($"{nameof(PrioritiseNaturalEquipmentMods)}:{ForDescriptions}");

            Debug.Entry(4,
                $"* {nameof(PrioritiseNaturalEquipmentMods)}"
                + $"(ForDescriptions: {ForDescriptions})",
                Indent: indent + 1, Toggle: doDebug);

            string label = ForDescriptions
                ? "Descriptions"
                : "EquipmentMods"
                ;

            Debug.Entry(4, $"{label}:", Indent: indent + 1, Toggle: doDebug);

            SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = new();
            if (!NaturalEquipmentModList.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipmentBase attachedNaturalEquipmentMod in NaturalEquipmentModList)
                {
                    int priority = ForDescriptions
                        ? attachedNaturalEquipmentMod.DescriptionPriority
                        : attachedNaturalEquipmentMod.ModPriority
                        ;
                    string priorityString = ForDescriptions
                        ? nameof(attachedNaturalEquipmentMod.DescriptionPriority)
                        : nameof(attachedNaturalEquipmentMod.ModPriority)
                        ;

                    bool doOverwrite = true;
                    if (!naturalEquipmentMods.IsNullOrEmpty())
                    {
                        foreach ((int prioritisedPriority, ModNaturalEquipmentBase prioritisedNaturalEquipmentMod) in naturalEquipmentMods)
                        {
                            if (prioritisedNaturalEquipmentMod.GetType() == attachedNaturalEquipmentMod.GetType())
                            {
                                doOverwrite = false;
                                string whichPriority = priority == prioritisedPriority ? "the current priority" : "priority";
                                Debug.Warn(2,
                                    $"{nameof(NaturalEquipmentManager)}",
                                    $"{nameof(PrioritiseNaturalEquipmentMods)}({nameof(List<ModNaturalEquipmentBase>)}, {typeof(bool).Name})",
                                    $"[{priority}]" +
                                    $"{naturalEquipmentMods[priority]} " +
                                    $"in {nameof(naturalEquipmentMods)} excluded: {prioritisedNaturalEquipmentMod} " +
                                    $"already exists in list at {whichPriority} [{prioritisedPriority}]",
                                    Indent: indent + 2);
                                break;
                            }
                        }
                    }
                    if (naturalEquipmentMods.ContainsKey(priority) && doOverwrite)
                    {
                        Debug.Warn(2,
                            $"{nameof(NaturalEquipmentManager)}",
                            $"{nameof(PrioritiseNaturalEquipmentMods)}({nameof(List<ModNaturalEquipmentBase>)}, {typeof(bool).Name})",
                            $"[{priority}]" +
                            $"{naturalEquipmentMods[priority]} " +
                            $"in {nameof(naturalEquipmentMods)} overwritten: Same {priorityString}",
                            Indent: indent + 2);
                    }
                    if (doOverwrite)
                    {
                        naturalEquipmentMods[priority] = attachedNaturalEquipmentMod;
                    }

                    Debug.LoopItem(4,
                        $"{attachedNaturalEquipmentMod.Name}" +
                        $"[{attachedNaturalEquipmentMod.GetAdjective()}]",
                        Good: naturalEquipmentMods[priority] == attachedNaturalEquipmentMod, Indent: indent + 2, Toggle: doDebug);
                }
            }

            if (!naturalEquipmentMods.IsNullOrEmpty())
            {
                Debug.Entry(4, $"{nameof(naturalEquipmentMods)}:", Indent: indent + 1, Toggle: doDebug);
                foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in naturalEquipmentMods)
                {
                    Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}:{naturalEquipmentMod.GetColoredAdjective()}",
                        Indent: indent + 2, Toggle: doDebug);
                }
            }

            Debug.LastIndent = indent;

            return naturalEquipmentMods;
        }

        public static List<string> WantStringEvents = new()
        {
        };
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            if (!WantStringEvents.IsNullOrEmpty())
            {
                foreach (string EventID in WantStringEvents)
                {
                    Registrar.Register(EventID);
                }
            }
            base.Register(Object, Registrar);
        }
        public static List<int> WantEvents = new()
        {
            EquipperEquippedEvent.ID,
            BodyPartsUpdatedEvent.ID,
            AfterBodyPartsUpdatedEvent.ID,
        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (WantsToManage && WantEvents.Contains(ID));
        }
        public override bool HandleEvent(EquipperEquippedEvent E)
        {
            if (E.Actor == ParentObject)
            {
                if (E.Item.IsNaturalEquipment() && E.Item.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator))
                {
                    int indent = Debug.LastIndent;
                    bool doDebug = getDoDebug(nameof(EquipperEquippedEvent));

                    Debug.Entry(4,
                    $"@ {nameof(NaturalEquipmentManager)}."
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(EquipperEquippedEvent)} E)",
                    Indent: indent + 1, Toggle: doDebug);

                    Debug.Entry(4,
                        $"{nameof(E.Actor)}: {E?.Actor?.DebugName ?? NULL}",
                        Indent: indent + 2, Toggle: doDebug);

                    naturalEquipmentOperator.Manager = this;

                    Debug.Entry(4,
                        $"x {nameof(NaturalEquipmentManager)}."
                        + $"{nameof(HandleEvent)}("
                        + $"{nameof(EquippedEvent)}"
                        + $" E.Creature: {E.Actor?.DebugName ?? NULL}) @//",
                        Indent: indent + 1, Toggle: doDebug);

                    Debug.LastIndent = indent;
                }
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeUpdateBodyPartsEvent E)
        {
            bool doDebug = getDoDebug(nameof(BeforeUpdateBodyPartsEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeUpdateBodyPartsEvent)} E)",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                Debug.Entry(4,
                    $"Creature: {E?.Creature?.DebugName ?? NULL}",
                    Indent: 1, Toggle: doDebug);

                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
                    {
                        BodyPart parentLimb = naturalEquipmentOperator.ParentLimb;
                        Debug.LoopItem(4,
                            $"Limb: [{parentLimb?.ID}:{parentLimb?.Type}] {parentLimb?.Description ?? NULL}",
                            Indent: 2, Toggle: doDebug);

                        naturalEquipmentOperator.ClearShortDescriptionCache();
                        naturalEquipmentOperator.HasOperated = false;
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeUpdateBodyPartsEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            bool doDebug = getDoDebug(nameof(BodyPartsUpdatedEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)} E)",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                Debug.Entry(4,
                    $"Creature: {E?.Creature?.DebugName ?? NULL}",
                    Indent: 1, Toggle: doDebug);

                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
                    {
                        BodyPart parentLimb = naturalEquipmentOperator?.ParentLimb;
                        Debug.LoopItem(4,
                            $"Limb: [{parentLimb?.ID}:{parentLimb?.Type}] {parentLimb?.Description ?? NULL}",
                            Indent: 2, Toggle: doDebug);

                        // naturalEquipmentOperator.ClearShortDescriptionCache();
                        // naturalEquipmentOperator.HasOperated = false;
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            bool doDebug = getDoDebug(nameof(AfterBodyPartsUpdatedEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AfterBodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL})",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
                    {
                        GameObject naturalEquipment = naturalEquipmentOperator.ParentObject;
                        BodyPart equipmentLimb = naturalEquipmentOperator.ParentLimb;

                        if (BeforeManageDefaultNaturalEquipmentEvent.CheckFor(
                            Equipment: naturalEquipment, 
                            Creature: ParentObject, 
                            BodyPart: equipmentLimb, 
                            Operator: naturalEquipmentOperator))
                        {
                            ManageDefaultNaturalEquipmentEvent.Send(
                                Equipment: naturalEquipment, 
                                Creature: ParentObject, 
                                BodyPart: equipmentLimb, 
                                Operator: naturalEquipmentOperator).Reset();

                            AfterManageDefaultNaturalEquipmentEvent.Send(
                                Equipment: naturalEquipment, 
                                Creature: ParentObject, 
                                BodyPart: equipmentLimb, 
                                Operator: naturalEquipmentOperator).Reset();
                        }
                    }
                }
                List<GameObject> naturalEquipmentInInventory = Event.NewGameObjectList(E.Creature.GetInventory(GO => GO.InheritsFrom("NaturalEquipment")) ?? new());
                if (!naturalEquipmentInInventory.IsNullOrEmpty())
                {
                    foreach (GameObject naturalEquipment in naturalEquipmentInInventory)
                    {
                        string bodyPartType = naturalEquipment.GetPart<MeleeWeapon>()?.Slot ?? naturalEquipment.GetPart<Armor>()?.WornOn;
                        // E.Creature.EquipObject(naturalEquipment, E.Creature.GetFirstBodyPart(BP => BP.Type == bodyPartType && BP.DefaultBehavior == null), true, 0)
                        
                        if (!E.Creature.AutoEquip(naturalEquipment, Silent: true)
                            && GameObject.Validate(naturalEquipment))
                        {
                            naturalEquipment.Obliterate();
                        }
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AfterBodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }

        public override void FinalizeRead(SerializationReader Reader)
        {
            base.FinalizeRead(Reader);
            SyncOperators();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalEquipmentManager = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            return naturalEquipmentManager;
        }
        public override void FinalizeCopyEarly(GameObject Source, bool CopyEffects, bool CopyID, Func<GameObject, GameObject> MapInv)
        {
            base.FinalizeCopyEarly(Source, CopyEffects, CopyID, MapInv);
            SyncOperators();
        }
    }
}
