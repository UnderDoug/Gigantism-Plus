using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentCybernetic<T> 
        : IPart
        , IModEventHandler<ManageDefaultEquipmentEvent>
        , IManagedDefaultNaturalEquipment<T>
        where T 
        : BaseManagedDefaultEquipmentCybernetic<T>
        , IModEventHandler<ManageDefaultEquipmentEvent>
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }
        public int Level { get; set; }
        public List<string> BodyPartTypesWantingManaged { get; set; }

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
            BodyPartTypesWantingManaged = new();
            NaturalEquipmentMods = new();
        }

        // Takes an existing NaturalEquipmentMods Dictionary
        public BaseManagedDefaultEquipmentCybernetic(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, T NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<T>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<T> NaturalEquipmentMod) in naturalEquipmentMods)
            {
                ModNaturalEquipment<T> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public BaseManagedDefaultEquipmentCybernetic(ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
            : this()
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
        }

        public BaseManagedDefaultEquipmentCybernetic(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
            : this(naturalEquipmentMods, NewParent)
        {
            NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
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
            // List<string> vomitCats = new() { "Meta", "Damage", "Additions", "Render" };
            // NaturalEquipmentMod.Vomit(4, "| Before", vomitCats, Indent: 2);
            Debug.Divider(4, "\u2500", 40, Indent: 2);

            //NaturalEquipmentMod.Level = Level;
            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageDieCount", true, Indent: 3);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageDieSize", true, Indent: 3);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageBonus", true, Indent: 3);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "HitBonus", true, Indent: 3);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "PenBonus", true, Indent: 3);

            Debug.Divider(4, "\u2500", 40, Indent: 2);
            // NaturalEquipmentMod.Vomit(4, "|  After", vomitCats, Indent: 2);
            return true;
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1);

            Body body = Implantee?.Body;
            if (body != null)
            {
                string targetType = TargetBodyPart.Type;
                foreach (BodyPart bodyPart in body.LoopPart(targetType))
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Type}]", Indent: 2);

                    ModNaturalEquipment<T> naturalEquipmentMod = null;
                    if (NaturalEquipmentMod != null)
                    {
                        naturalEquipmentMod = new(NaturalEquipmentMod);
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Property", Indent: 3);
                    }
                    else if (NaturalEquipmentMods.ContainsKey(targetType))
                    {
                        naturalEquipmentMod = new(NaturalEquipmentMods[targetType]);
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Dictionary", Indent: 3);
                    }
                    else
                    {
                        Debug.Entry(4, $"No NaturalEquipmentMod for this BodyPart", Indent: 3);
                    }

                    if (naturalEquipmentMod == null) continue;

                    Debug.Entry(4, $"modNaturalWeapon: {naturalEquipmentMod?.Name}", Indent: 3);

                    Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1);
            return true;
        }

        public virtual void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee.RegisterEvent(this, ManageDefaultEquipmentEvent.ID, 0, Serialize: true);
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            Implantee.UnregisterEvent(this, ManageDefaultEquipmentEvent.ID);
        } //!--- public override void OnUnimplanted(GameObject Object)
        public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = Implantee.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnManageNaturalEquipment)}(body)");
            Debug.Entry(4, $"TARGET {Implantee.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);
            
            Body body = Implantee.Body;
            if (body != null)
                ProcessNaturalEquipment(Manager, TargetBodyPart);

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body: {Implantee.Blueprint})");
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
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
            OnUnimplanted(E.Implantee, E.Item);
            return base.HandleEvent(E);
        }
        public bool HandEvent(ManageDefaultEquipmentEvent E)
        {
            if (E.Wielder == Implantee)
            {
                OnManageNaturalEquipment(E.Manager, E.BodyPart);
            }
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
                if (Actor == ParentObject)
                {
                    // Do Code?
                }
            }
            else if (E.ID == "MutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == ParentObject)
                {
                    // ProcessNaturalEquipment(Actor?.Body);
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
            cybernetic.NaturalEquipmentMod = new(NaturalEquipmentMod, (T)cybernetic);
            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;
            return cybernetic;
        }
    }
}
