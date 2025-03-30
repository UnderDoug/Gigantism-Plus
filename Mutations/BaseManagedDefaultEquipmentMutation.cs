using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentMutation<T> : BaseDefaultEquipmentMutation, IManagedDefaultNaturalEquipment<T> 
        where T : BaseManagedDefaultEquipmentMutation<T>, IManagedDefaultNaturalEquipment<T>, new()
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }

        public BaseManagedDefaultEquipmentMutation()
        {
            NaturalEquipmentMods = new();
        }

        // Takes an existing NaturalEquipmentMods Dictionary
        public BaseManagedDefaultEquipmentMutation(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, T NewParent)
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

        public BaseManagedDefaultEquipmentMutation(ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
            : this()
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
        }

        public BaseManagedDefaultEquipmentMutation(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
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
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}(ModNaturalEquipment<{typeof(T).Name}> NaturalEquipmentMod: {NaturalEquipmentMod.Name}, int Level: {Level})",
                Indent: 2);

            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);

            return true;
        }
        public override bool ChangeLevel(int NewLevel)
        {
            Debug.Header(4, $"BaseManagedDefaultEquipmentMutation<{typeof(T).Name}>", $"{nameof(ChangeLevel)}");

            List<string> bodyPartTypesWantingManaged = new();
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
                if (!bodyPartTypesWantingManaged.Contains(NaturalEquipmentMod.BodyPartType))
                {
                    Debug.Entry(4, $"{typeof(T).Name} added \"{NaturalEquipmentMod.BodyPartType}\" to WantingManaged list.", Indent: 2);
                    bodyPartTypesWantingManaged.Add(NaturalEquipmentMod.BodyPartType);
                }
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);
            Debug.Entry(4, $"> foreach (BodyPart bodyPart in ParentObject.Body.LoopParts())", Indent: 1);
            foreach (BodyPart bodyPart in ParentObject.Body.LoopParts())
            {
                if (bodyPartTypesWantingManaged.Contains(bodyPart.Type))
                {
                    NaturalEquipmentManager manager = bodyPart.DefaultBehavior?.GetPart<NaturalEquipmentManager>();
                    if (manager != null)
                    {
                        Debug.Entry(4, $"[{bodyPart.DefaultBehavior.ID}:{bodyPart.DefaultBehavior.Render.DisplayName}] {typeof(NaturalEquipmentManager).Name} told it WantsToManage", Indent: 2);
                        manager.WantToManage = true;
                    }
                }
            }
            Debug.Entry(4, $"x foreach (BodyPart bodyPart in ParentObject.Body.LoopParts()) >//", Indent: 1);
            Debug.Header(4, $"BaseManagedDefaultEquipmentMutation<{typeof(T).Name}>", $"{nameof(ChangeLevel)}");
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1);

            Body body = ParentObject?.Body;
            if (body != null)
            {
                string targetType = TargetBodyPart.Type;
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart bodyPart in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Type}]", Indent: 2);

                    if (bodyPart.Type != targetType) continue;

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
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnManageNaturalEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1);

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body: {ParentObject.Blueprint})");
        }

        public override bool Unmutate(GameObject GO)
        {
            return base.Unmutate(GO);
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

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<ManageDefaultEquipmentEvent>.ID;
        }
        public virtual bool HandEvent(ManageDefaultEquipmentEvent E)
        {
            if (E.Wielder == ParentObject)
            {
                OnManageNaturalEquipment(E.Manager, E.BodyPart);
            }
            return base.HandleEvent(E);
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
            BaseManagedDefaultEquipmentMutation<T> mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation<T>;
            mutation.NaturalEquipmentMods = new();
            foreach ((string bodyPartType, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, (T)mutation));
            }
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, (T)mutation);
            return mutation;
        }
    }
}
