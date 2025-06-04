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
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        [SerializeField]
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }

        [SerializeField]
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }

        public int Level { get; set; }

        private GameObject _implantee = null;
        public GameObject Implantee
        {
            get => _implantee ??= ImplantObject?.Equipped;
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
            NaturalEquipmentMods = new();
            NaturalEquipmentMod = new();
        }

        public BaseManagedDefaultEquipmentCybernetic(T NewAssigner)
            : this()
        {
            foreach ((_, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                naturalEquipmentMod.AssigningPart = NewAssigner;
            }
            NaturalEquipmentMod.AssigningPart = NewAssigner;
        }

        // Takes an existing NaturalEquipmentMods Dictionary
        public BaseManagedDefaultEquipmentCybernetic(
            Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, 
            T NewAssigner)
            : this(NewAssigner)
        {
            Dictionary<string, ModNaturalEquipment<T>> newNaturalEquipmentMods = new();
            foreach ((string bodyPartType, ModNaturalEquipment<T> naturalEquipmentMod) in naturalEquipmentMods)
            {
                ModNaturalEquipment<T> newNaturalEquipmentMod = new(naturalEquipmentMod, NewAssigner);
                newNaturalEquipmentMods.Add(bodyPartType, newNaturalEquipmentMod);
            }
            NaturalEquipmentMods = newNaturalEquipmentMods;
        }

        public BaseManagedDefaultEquipmentCybernetic(
            ModNaturalEquipment<T> naturalEquipmentMod, 
            T NewAssigner)
            : this(NewAssigner)
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewAssigner);
        }

        public BaseManagedDefaultEquipmentCybernetic(
            Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, 
            ModNaturalEquipment<T> naturalEquipmentMod, 
            T NewAssigner)
            : this(naturalEquipmentMods, NewAssigner)
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewAssigner);
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

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
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

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level)
        {
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}(ModNaturalEquipment<{typeof(T).Name}> NaturalEquipmentMod[{NaturalEquipmentMod.BodyPartType}], int Level: {Level})",
                Indent: 2, Toggle: getDoDebug());

            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);
            return true;
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1, Toggle: getDoDebug());

            string targetType = TargetBodyPart.Type;
            Debug.LoopItem(4, $" part", $"{TargetBodyPart.Description} [{TargetBodyPart.ID}:{TargetBodyPart.Type}]", Indent: 2, Toggle: getDoDebug());
            ModNaturalEquipment<T> naturalEquipmentMod = null;
            if (NaturalEquipmentMod != null && NaturalEquipmentMod.BodyPartType == targetType)
            {
                naturalEquipmentMod = NaturalEquipmentMod;
                Debug.CheckYeh(4, $"NaturalEquipmentMod Property contains an entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"NaturalEquipmentMod Property does not contain an entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
            }

            if (!NaturalEquipmentMods.IsNullOrEmpty() && NaturalEquipmentMods.ContainsKey(targetType))
            {
                naturalEquipmentMod = NaturalEquipmentMods[targetType];
                Debug.CheckYeh(4, $"NaturalEquipmentMod Dictionary contains an entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"NaturalEquipmentMod Dictionary does not contain an entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1, Toggle: getDoDebug());
            return true;
        }

        public virtual void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee.Body.UpdateBodyParts();
        } //!-- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee.Body.UpdateBodyParts();
        } //!-- public override void OnUnimplanted(GameObject Object)

        public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = Implantee.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnManageNaturalEquipment)}(body)", Toggle: getDoDebug());
            Debug.Entry(4, $"TARGET {Implantee.DebugName} in zone {InstanceObjectZoneID}", Indent: 0, Toggle: getDoDebug());

            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {Implantee.Blueprint})", Toggle: getDoDebug());
        }
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: getDoDebug());
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ImplantedEvent.ID
                || ID == UpdateNaturalEquipmentModsEvent.ID
                || ID == ManageDefaultEquipmentEvent.ID;
        }
        public override bool HandleEvent(ImplantedEvent E)
        {
            Implantee = E.Implantee;
            ImplantObject = E.Item;
            OnImplanted(Implantee, ImplantObject);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnimplantedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}({nameof(UpdateNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Creature == Implantee)
            {
                OnUpdateNaturalEquipmentMods();
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeManageDefaultEquipmentEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(ManageDefaultEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}({typeof(ManageDefaultEquipmentEvent).Name} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Creature.Is(Implantee) && E.Equipment.HasPart<NaturalEquipmentManager>())
            {
                OnManageNaturalEquipment(E.Manager, E.BodyPart);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterManageDefaultEquipmentEvent E)
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
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeMutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == Implantee)
                {
                    Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
                    foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
                    {
                        // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    }
                    if (NaturalEquipmentMod != null) // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: getDoDebug());
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

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.Write(NaturalEquipmentMods ??= new());

            NaturalEquipmentMod ??= new();
            NaturalEquipmentMod.Write(Basis, Writer);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            NaturalEquipmentMods = Reader.ReadDictionary<string, ModNaturalEquipment<T>>() ?? new();
            NaturalEquipmentMod = (ModNaturalEquipment<T>)Reader.ReadObject() ?? new();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentCybernetic<T> cybernetic = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentCybernetic<T>;

            cybernetic.NaturalEquipmentMods = new(); 
            NaturalEquipmentMods ??= new();
            if (NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((string type, ModNaturalEquipment<T> subpart) in NaturalEquipmentMods)
                {
                    cybernetic.NaturalEquipmentMods.Add(type, new(subpart, (T)cybernetic));
                }
            }

            cybernetic.NaturalEquipmentMod = new(NaturalEquipmentMod ??= new(), (T)cybernetic);

            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;

            return cybernetic;
        }
    }
}
