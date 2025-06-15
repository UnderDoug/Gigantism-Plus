using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

using SerializeField = UnityEngine.SerializeField;

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
                'R',    // Register
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

        public BaseManagedDefaultEquipmentMutation()
        {
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
                + $"{NaturalEquipmentMod?.GetType()?.Name}[{typeof(T).Name}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (NaturalEquipmentMod != null)
            {
                NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
                NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
                NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
                NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
                NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);

                NaturalEquipmentMod.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());
            }

            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod?.GetType()?.Name}[{typeof(T).Name}], "
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

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            return base.Unmutate(GO);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }
        public virtual void OnManageDefaultNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, 
                $"{typeof(T).Name}", 
                $"{nameof(OnManageDefaultNaturalEquipment)}(body)", 
                Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnManageDefaultNaturalEquipment)}" +
                $"(body of: {ParentObject.Blueprint})", 
                Toggle: getDoDebug('M'));
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetPrioritisedNaturalEquipmentModsEvent.ID
                || ID == ManageDefaultNaturalEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeBodyPartsUpdatedEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AfterBodyPartsUpdatedEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

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

            if (E.Creature == ParentObject)
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

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentMutation<T> mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation<T>;

            return mutation;
        }
    }
}
