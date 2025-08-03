using System;
using System.Collections.Generic;
using System.Reflection;

using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentCybernetic<T> 
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        where T 
        : BaseManagedDefaultEquipmentCybernetic<T>
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("BaseManagedDefaultEquipmentCybernetic");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "getMods",
                'M',    // Manage
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public NaturalEquipmentManager NaturalEquipmentManager => Implantee?.RequirePart<NaturalEquipmentManager>();

        public virtual List<ModNaturalEquipment<T>> NaturalEquipmentMods => GetNaturalEquipmentMods();

        public int Level { get; set; }

        private GameObject _implantee = null;
        public GameObject Implantee
        {
            get => _implantee ??= ImplantObject?.Implantee;
            set => _implantee = value == null ? null : _implantee;
        }

        private GameObject _implantObject = null;
        public GameObject ImplantObject
        {
            get =>_implantObject ??= ParentObject;
            set => _implantObject = value == null ? null : _implantObject;
        }

        public BaseManagedDefaultEquipmentCybernetic()
        {
            Level = 1;
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }

        public List<ModNaturalEquipment<T>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<T>> Filter = null, NaturalEquipmentManager NewManager = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(GetNaturalEquipmentMods)}("
                + $"{nameof(Filter)}, "
                + $"{nameof(NewManager)})",
                Indent: indent + 1, Toggle: getDoDebug());

            NewManager ??= NaturalEquipmentManager;
            List<ModNaturalEquipment<T>> naturalEquipmentModsList = new();

            List<MethodInfo> managedBaseMethods = new(typeof(T).GetMethods());
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                managedBaseMethods.RemoveAll(m => !m.IsStatic || !m.IsPublic || !m.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<T>)));
            }
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                Debug.CheckYeh(4, $"Have Methods", Indent: indent + 2, Toggle: getDoDebug());
                foreach (MethodInfo managedMethod in managedBaseMethods)
                {
                    if (!managedMethod.IsStatic || !managedMethod.IsPublic)
                    {
                        continue;
                    }
                    Debug.LoopItem(4, $"{nameof(managedMethod)}: {managedMethod.Name}", Indent: indent + 3, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(managedMethod.IsPublic)}: {managedMethod.IsPublic}",
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.IsStatic)}: {managedMethod.IsStatic}",
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.ReturnType)}: {managedMethod.ReturnType.Name}",
                        Indent: indent + 4, Toggle: getDoDebug());

                    if (managedMethod.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<T>))
                        && managedMethod.IsStatic
                        && managedMethod.IsPublic)
                    {
                        ParameterInfo[] parameters = managedMethod.GetParameters();
                        if (parameters.Length == 1
                            && parameters[0].ParameterType.InheritsFrom(typeof(NaturalEquipmentManager)))
                        {
                            Debug.CheckYeh(4,
                                $"public static {managedMethod.ReturnType.Name} " +
                                $"{managedMethod.Name}(" +
                                $"{parameters[0].ParameterType.Name} {parameters[0].Name})",
                                Indent: indent + 5, Toggle: getDoDebug());

                            if (managedMethod.Invoke(null, new object[1] { NewManager }) is ModNaturalEquipment<T> naturalEquipmentMod)
                            {
                                Debug.CheckYeh(4, $"Successful {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: getDoDebug());
                                if (Filter(naturalEquipmentMod))
                                {
                                    Debug.CheckYeh(4, $"Passed {nameof(Filter)}, added to List", Indent: indent + 3, Toggle: getDoDebug());
                                    naturalEquipmentModsList.Add(naturalEquipmentMod);
                                }
                                else
                                {
                                    Debug.CheckNah(4, $"Failed {nameof(Filter)}", Indent: indent + 3, Toggle: getDoDebug());
                                }
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Failed {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: getDoDebug());
                            }
                        }
                    }
                }
            }
            Debug.LastIndent = indent;
            return naturalEquipmentModsList;
        }

        public virtual ModNaturalEquipment<T> UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{typeof(T).Name}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            NaturalEquipmentMod?.AddDamageDieCountAdjustment(GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level))
                ?.AddDamageDieSizeAdjustment(GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level))
                ?.AddDamageBonusAdjustment(GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level))
                ?.AddHitBonusAdjustment(GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level))
                ?.AddPenBonusAdjustment(GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level))
                
                ?.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{typeof(T).Name}], "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMod;
        }
        public virtual List<ModNaturalEquipment<T>> UpdateNaturalEquipmentMods(List<ModNaturalEquipment<T>> NaturalEquipmentMods, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}[{typeof(T).Name}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipment<T> naturalEquipmentMod in NaturalEquipmentMods)
                {
                    UpdateNaturalEquipmentMod(naturalEquipmentMod, Level);
                }
            }

            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}[{typeof(T).Name}], "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMods;
        }

        public virtual void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee?.MakeUnderstood();
            Implantee?.Body?.UpdateBodyParts();
        } //!-- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee?.Body?.UpdateBodyParts();
        } //!-- public override void OnUnimplanted(GameObject Object)

        public virtual void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = Implantee.GetCurrentZone();
            string InstanceObjectZoneID = InstanceObjectZone?.ZoneID ?? "[Pre-build]";

            Debug.Header(4, 
                $"{typeof(T).Name}", 
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}" +
                $"(body)", Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {Implantee?.DebugName ?? NULL} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}" +
                $"(body of: {Implantee.Blueprint})", Toggle: getDoDebug('M'));
        }

        public virtual List<int> GetImplanteeRegisteredEventIDs()
        {
            return new()
            {
                GetPrioritisedNaturalEquipmentModsEvent.ID,
                BeforeManageDefaultNaturalEquipmentEvent.ID,
            };
        }
        public virtual void RegisterImplanteeEvents(GameObject Implantee)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"{nameof(RegisterImplanteeEvents)}", Indent: indent + 1, Toggle: getDoDebug('R'));

            List<int> eventIDs = GetImplanteeRegisteredEventIDs();
            if (!eventIDs.IsNullOrEmpty())
            {
                foreach (int eventID in eventIDs)
                {
                    Implantee?.RegisterEvent(this, eventID);
                    Debug.LoopItem(4, $"Registered {nameof(eventID)}: {eventID}]", Indent: indent + 2, Toggle: getDoDebug('R'));
                }
            }

            Debug.LastIndent = indent;
        }
        public virtual void UnregisterImplanteeEvents(GameObject Implantee)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"{nameof(UnregisterImplanteeEvents)}", Indent: indent + 1, Toggle: getDoDebug('R'));

            List<int> eventIDs = GetImplanteeRegisteredEventIDs();
            if (!eventIDs.IsNullOrEmpty())
            {
                foreach (int eventID in eventIDs)
                {
                    Implantee.UnregisterEvent(this, eventID);
                    Debug.LoopItem(4, $"Unregistered {nameof(eventID)}: {eventID}]", Indent: indent + 2, Toggle: getDoDebug('R'));
                }
            }

            Debug.LastIndent = indent;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID;
        }
        public override bool HandleEvent(ImplantedEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ImplantedEvent)} E)"
                + $" {nameof(E.Implantee)}: {E.Implantee?.DebugName ?? NULL}"
                + $" {nameof(E.Item)}: {E.Item?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());

            RegisterImplanteeEvents(E.Implantee);

            OnImplanted(E.Implantee, E.Item);

            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ImplantedEvent)} E)"
                + $" {nameof(E.Implantee)}: {E.Implantee?.DebugName ?? NULL}"
                + $" {nameof(E.Item)}: {E.Item?.DebugName ?? NULL}"
                + $" @//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnimplantedEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(UnimplantedEvent)} E)"
                + $" {nameof(E.Implantee)}: {E.Implantee?.DebugName ?? NULL}"
                + $" {nameof(E.Item)}: {E.Item?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());

            UnregisterImplanteeEvents(E.Implantee);

            OnUnimplanted(E.Implantee, E.Item);

            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(UnimplantedEvent)} E)"
                + $" {nameof(E.Implantee)}: {E.Implantee?.DebugName ?? NULL}"
                + $" {nameof(E.Item)}: {E.Item?.DebugName ?? NULL}"
                + $" @//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(GetPrioritisedNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetPrioritisedNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug("getMods"));

            List<ModNaturalEquipment<T>> naturalEquipmentMods = 
                UpdateNaturalEquipmentMods(GetNaturalEquipmentMods(
                    mod => mod.BodyPartType == E.TargetBodyPart.Type), 
                    Level);

            foreach (ModNaturalEquipment<T> naturalEquipmentMod in naturalEquipmentMods)
            {
                E.AddNaturalEquipmentMod(naturalEquipmentMod);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeManageDefaultNaturalEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeManageDefaultNaturalEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug('M'));

            if (E.Creature == Implantee && E.Equipment.HasPart<NaturalEquipmentOperator>())
            {
                OnBeforeManageDefaultNaturalEquipment(E.Operator, E.BodyPart);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(ManageDefaultNaturalEquipmentEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterManageDefaultNaturalEquipmentEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeRapidAdvancementEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterRapidAdvancementEvent E)
        {
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("CanBeDisassembled"); // This prevents the cybernetic from being disassembled.
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            Registrar.Register("CookedAt");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "CanBeDisassembled")
            {
                return false; // This prevents the cybernetic from being disassembled.
            }

            if (E.ID == "BeforeMutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == Implantee)
                {
                    /*
                    Debug.Entry(4, $"> foreach (ModNaturalEquipment<E> naturalEquipmentMod in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
                    foreach (ModNaturalEquipment<T> NaturalEquipmentMod in NaturalEquipmentMods)
                    {
                        // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    }
                    if (NaturalEquipmentMod != null) // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    Debug.Entry(4, $"x foreach (ModNaturalEquipment<E> naturalEquipmentMod in NaturalEquipmentMods) >//", Indent: 1, Toggle: getDoDebug());
                    */
                }
            }
            else if (E.ID == "MutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == Implantee)
                {
                    // do code?
                }
            }
            else if (E.ID == "CookedAt")
            {
                if (E.GetParameter("Actor") is GameObject Actor && Actor == ParentObject && Actor.Body != null)
                {
                    Actor.Body.UpdateBodyParts();
                }
            }
            return base.FireEvent(E);
        }
        // This prevents the cybernetic from being disassembled.
        public virtual void CanBeDisassembled()
        {
            Event CanBeDisassembled = Event.New("CanBeDisassembled");
            ParentObject.FireEvent(CanBeDisassembled);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentCybernetic<T> cybernetic = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentCybernetic<T>;

            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;

            return cybernetic;
        }
    }
}
