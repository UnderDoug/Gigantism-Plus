using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

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
        // Dictionary holds a BodyPart.Type string as Key, and naturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via naturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }
        public int Level { get; set; }

        private GameObject _implantee = null;
        public GameObject Implantee
        {
            get
            {
                return _implantee ??= ImplantObject?.Equipped;
            }
            set => _implantee = value == null ? null : _implantee;
        }

        private GameObject _implantObject = null;
        public GameObject ImplantObject
        {
            get
            {
                return _implantObject ??= ParentObject;
            }
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
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalEquipmentMod.AddedParts.Add(part);
            }
            return true;
        }
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<T> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalEquipmentMod.AddedStringProps = StringProps;
                NaturalEquipmentMod.AddedIntProps = IntProps;
            }
            return true;
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
                Indent: 2);

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
                Indent: 1);

            string targetType = TargetBodyPart.Type;
            Debug.LoopItem(4, $" part", $"{TargetBodyPart.Description} [{TargetBodyPart.ID}:{TargetBodyPart.Type}]", Indent: 2);
            ModNaturalEquipment<T> naturalEquipmentMod = null;
            if (NaturalEquipmentMod != null && NaturalEquipmentMod.BodyPartType == targetType)
            {
                naturalEquipmentMod = NaturalEquipmentMod;
                Debug.CheckYeh(4, $"NaturalEquipmentMod Property contains entry for this BodyPart", Indent: 2);
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2);
            }
            else
            {
                Debug.CheckYeh(4, $"NaturalEquipmentMod Property does not contain entry for this BodyPart", Indent: 2);
            }

            if (!NaturalEquipmentMods.IsNullOrEmpty() && NaturalEquipmentMods.ContainsKey(targetType))
            {
                naturalEquipmentMod = NaturalEquipmentMods[targetType];
                Debug.CheckYeh(4, $"NaturalEquipmentMod Dictionary contains entry for this BodyPart", Indent: 2);
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2);
            }
            else
            {
                Debug.CheckYeh(4, $"NaturalEquipmentMod Dictionary does not contain entry for this BodyPart", Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1);
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
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnManageNaturalEquipment)}(body)");
            Debug.Entry(4, $"TARGET {Implantee.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1);

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {Implantee.Blueprint})");
        } //!-- public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);
        } //!-- public virtual void OnUpdateNaturalEquipmentMods()

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
                || ID == BeforeBodyPartsUpdatedEvent.ID
                || ID == UpdateNaturalEquipmentModsEvent.ID
                || ID == AfterBodyPartsUpdatedEvent.ID
                || ID == BeforeManageDefaultEquipmentEvent.ID
                || ID == ManageDefaultEquipmentEvent.ID
                || ID == AfterManageDefaultEquipmentEvent.ID
                || ID == BeforeRapidAdvancementEvent.ID
                || ID == AfterRapidAdvancementEvent.ID;
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
            OnUnimplanted(E.Implantee, E.Item);
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}({typeof(UpdateNaturalEquipmentModsEvent).Name} E)",
                Indent: 0);

            if (E.Actor == Implantee)
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
        public bool HandleEvent(ManageDefaultEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}({typeof(ManageDefaultEquipmentEvent).Name} E)",
                Indent: 0);

            if (E.Wielder.Is(Implantee) && E.Object.HasPart<NaturalEquipmentManager>())
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
                    Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
                    foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
                    {
                        // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    }
                    if (NaturalEquipmentMod != null) // UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
                    Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);
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
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentCybernetic<T> cybernetic = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentCybernetic<T>;
            cybernetic.NaturalEquipmentMods = new();
            foreach ((string type, ModNaturalEquipment<T> subpart) in NaturalEquipmentMods)
            {
                cybernetic.NaturalEquipmentMods.Add(type, new(subpart, (T)cybernetic));
            }
            NaturalEquipmentMod ??= new();
            cybernetic.NaturalEquipmentMod = new(NaturalEquipmentMod, (T)cybernetic);
            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;
            return cybernetic;
        }
    }
}
