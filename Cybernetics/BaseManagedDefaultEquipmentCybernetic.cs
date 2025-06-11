using HNPS_GigantismPlus;
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
            };
            List<object> dontList = new()
            {
                "getMods",
                'M',    // Manage
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }
        public virtual List<ModNaturalEquipment<T>> NaturalEquipmentMods => GetNaturalEquipmentMods();
        public virtual ModNaturalEquipment<T> NaturalEquipmentMod => GetNaturalEquipmentMod();

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

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod, string Parts)
        {
            if (Parts == null) return false;
            NaturalEquipmentMod.AddedParts ??= new();
            if (Parts.Contains(","))
            {
                string[] parts = Parts.Split(',');
                foreach (string part in parts)
                {
                    NaturalEquipmentMod.AddedParts.TryAdd(part);
                }
            }
            else
            {
                NaturalEquipmentMod.AddedParts.TryAdd(Parts);
            }
            return !NaturalEquipmentMod.AddedParts.IsNullOrEmpty();
        }
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<T> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            Props.ParseProps(out NaturalEquipmentMod.AddedStringProps, out NaturalEquipmentMod.AddedIntProps);
            return !NaturalEquipmentMod.AddedStringProps.IsNullOrEmpty() || !NaturalEquipmentMod.AddedIntProps.IsNullOrEmpty();
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

        public virtual List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedParts;
        }
        public virtual Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedStringProps;
        }
        public virtual Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedIntProps;
        }

        public virtual ModNaturalEquipment<T> GetNaturalEquipmentMod(Predicate<ModNaturalEquipment<T>> Filter = null, T NewAssigner = null)
        {
            return null;
        }
        public virtual List<ModNaturalEquipment<T>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<T>> Filter = null, T NewAssigner = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(GetNaturalEquipmentMods)}("
                + $"{nameof(Filter)}, "
                + $"{nameof(NewAssigner)}: {NewAssigner?.Name})",
                Indent: indent + 1, Toggle: getDoDebug());

            NewAssigner ??= (T)this;
            List<ModNaturalEquipment<T>> naturalEquipmentModsList = new();
            ModNaturalEquipment<T> naturalEquipmentMod = GetNaturalEquipmentMod(Filter, NewAssigner);
            if (naturalEquipmentMod != null)
            {
                naturalEquipmentModsList.Add(naturalEquipmentMod);
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

            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);

            NaturalEquipmentMod.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

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
            Implantee?.Body?.UpdateBodyParts();
        } //!-- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee?.Body?.UpdateBodyParts();
        } //!-- public override void OnUnimplanted(GameObject Object)

        public virtual void OnManageDefaultNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = Implantee.GetCurrentZone();
            string InstanceObjectZoneID = InstanceObjectZone?.ZoneID ?? "[Pre-build]";

            Debug.Header(4, 
                $"{typeof(T).Name}", 
                $"{nameof(OnManageDefaultNaturalEquipment)}" +
                $"(body)", Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {Implantee?.DebugName ?? NULL} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageDefaultNaturalEquipment)}" +
                $"(body of: {Implantee.Blueprint})", Toggle: getDoDebug('M'));
        }

        public virtual List<int> GetImplanteeRegisteredEventIDs()
        {
            return new()
            {
                GetPrioritisedNaturalEquipmentModsEvent.ID,
                ManageDefaultNaturalEquipmentEvent.ID,
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
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
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
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(ManageDefaultNaturalEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ManageDefaultNaturalEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug('M'));

            if (E.Creature == Implantee && E.Equipment.HasPart<NaturalEquipmentManager>())
            {
                OnManageDefaultNaturalEquipment(E.Manager, E.BodyPart);
            }
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
