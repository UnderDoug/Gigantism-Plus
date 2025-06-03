using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentMutation<T> 
        : BaseDefaultEquipmentMutation
        , IManagedDefaultNaturalEquipment<T> 
        where T 
        : BaseManagedDefaultEquipmentMutation<T>
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("BaseManagedDefaultEquipmentMutation");
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

        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }

        public BaseManagedDefaultEquipmentMutation()
        {
            NaturalEquipmentMods = new();
            NaturalEquipmentMod = new();
        }
        public BaseManagedDefaultEquipmentMutation(T NewAssigner)
            : this()
        {
            foreach ((_, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                naturalEquipmentMod.AssigningPart = NewAssigner;
            }
            NaturalEquipmentMod.AssigningPart = NewAssigner;
        }

        // Takes an existing NaturalEquipmentMods Dictionary
        public BaseManagedDefaultEquipmentMutation(
            Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods, 
            T NewAssigner)
            : this(NewAssigner)
        {
            Dictionary<string, ModNaturalEquipment<T>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                ModNaturalEquipment<T> naturalEquipmentMod = new(NaturalEquipmentMod, NewAssigner);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            this.NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public BaseManagedDefaultEquipmentMutation(
            ModNaturalEquipment<T> NaturalEquipmentMod, 
            T NewAssigner)
            : this(NewAssigner)
        {
            this.NaturalEquipmentMod = new(NaturalEquipmentMod, NewAssigner);
        }

        public BaseManagedDefaultEquipmentMutation(
            Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods, 
            ModNaturalEquipment<T> naturalEquipmentMod, 
            T NewAssigner)
            : this(NaturalEquipmentMods, NewAssigner)
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
        
        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            return base.Unmutate(GO);
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
        public override bool ChangeLevel(int NewLevel)
        {
            /*
            Debug.Header(4, $"BaseManagedDefaultEquipmentMutation<{typeof(T).Name}>", $"{nameof(ChangeLevel)}", Toggle: doDebug);
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: doDebug);
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: doDebug);
            Debug.Header(4, $"BaseManagedDefaultEquipmentMutation<{typeof(T).Name}>", $"{nameof(ChangeLevel)}", Toggle: doDebug);
            */
            return base.ChangeLevel(NewLevel);
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

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }
        public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnManageNaturalEquipment)}(body)", Toggle: getDoDebug());
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0, Toggle: getDoDebug());

            Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {ParentObject.Blueprint})", Toggle: getDoDebug());
        }
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: getDoDebug());
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == UpdateNaturalEquipmentModsEvent.ID
                || ID == ManageDefaultEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}({nameof(UpdateNaturalEquipmentModsEvent)} E )",
                Indent: 0, Toggle: getDoDebug());

            if (E.Creature == ParentObject)
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
                + $"{nameof(HandleEvent)}({nameof(ManageDefaultEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Creature == ParentObject)
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
                    // ProcessNaturalEquipment(Actor?.Actor);
                }
            }
            return base.FireEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            NaturalEquipmentMods ??= new();
            Writer.Write(NaturalEquipmentMods.Count);
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((string bodyPartType, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    Writer.WriteOptimized(bodyPartType);
                    naturalEquipmentMod.Write(Basis, Writer);
                }
            }

            NaturalEquipmentMod ??= new();
            NaturalEquipmentMod.Write(Basis, Writer);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            NaturalEquipmentMods = new();
            int naturalEquipmentModsCount = Reader.ReadInt32();
            for (int i = 0; i < naturalEquipmentModsCount; i++)
            {
                NaturalEquipmentMods.Add(Reader.ReadOptimizedString(), (ModNaturalEquipment<T>)Reader.ReadObject());
            }

            NaturalEquipmentMod = (ModNaturalEquipment<T>)Reader.ReadObject();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentMutation<T> mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation<T>;

            mutation.NaturalEquipmentMods = new();
            NaturalEquipmentMods ??= new();
            if (NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((string bodyPartType, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, (T)mutation));
                }
            }

            NaturalEquipmentMod ??= new();
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, (T)mutation);

            return mutation;
        }
    }
}
