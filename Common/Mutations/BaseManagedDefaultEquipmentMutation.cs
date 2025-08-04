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
using System.Reflection;
using System.Linq;

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

        public NaturalEquipmentManager NaturalEquipmentManager => ParentObject?.RequirePart<NaturalEquipmentManager>();

        public virtual List<ModNaturalEquipment<T>> NaturalEquipmentMods => GetNaturalEquipmentMods();

        public BaseManagedDefaultEquipmentMutation()
        {
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

                    Debug.Divider(4, HONLY, Indent: indent + 3, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(managedMethod)}: {managedMethod.Name}", Indent: indent + 3, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(managedMethod.IsPublic)}: {managedMethod.IsPublic}", 
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.IsStatic)}: {managedMethod.IsStatic}", 
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.ReturnType)}: {managedMethod.ReturnType.Name}", 
                        Indent: indent + 4, Toggle: getDoDebug());

                    if (managedMethod.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<T>), Silent: false)
                        && managedMethod.IsStatic
                        && managedMethod.IsPublic)
                    {
                        ParameterInfo[] parameters = managedMethod.GetParameters();
                        if (parameters.Length == 1
                            && parameters[0].ParameterType.InheritsFrom(typeof(NaturalEquipmentManager), Silent: false))
                        {
                            Debug.CheckYeh(4, 
                                $"public static {managedMethod.ReturnType.Name} " +
                                $"{managedMethod.Name}(" +
                                $"{parameters[0].ParameterType.Name} {parameters[0].Name})", 
                                Indent: indent + 5, Toggle: getDoDebug());

                            if (managedMethod.Invoke(null, new object[1] { NewManager }) is ModNaturalEquipment<T> naturalEquipmentMod)
                            {
                                Debug.CheckYeh(4, $"Successful {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: getDoDebug());
                                if (naturalEquipmentMod != null && Filter(naturalEquipmentMod))
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
                                Debug.CheckNah(4, $"Failed {nameof(managedMethod.Invoke)} (May be that the mod is conditionally produced)", Indent: indent + 3, Toggle: getDoDebug());
                            }
                        }
                    }
                }
                Debug.Divider(4, HONLY, Indent: indent + 3, Toggle: getDoDebug());
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

            NaturalEquipmentMod?.AdjustMeleeDamageDieCount(GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageDieSize(GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageBonus(GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level))
                ?.AdjustMeleeHitBonus(GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level))
                ?.AdjustPenBonus(GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level))
                    
                ?.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

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
        public virtual void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, 
                $"{typeof(T).Name}", 
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}(body)", 
                Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}" +
                $"(body of: {ParentObject.Blueprint})", 
                Toggle: getDoDebug('M'));
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            Registrar.Register("CookedAt");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetPrioritisedNaturalEquipmentModsEvent.ID
                || ID == BeforeManageDefaultNaturalEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)} E)",
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
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeManageDefaultNaturalEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug('M'));

            if (E.Creature == ParentObject)
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
            else if (E.ID == "CookedAt")
            {
                if (E.GetParameter("Actor") is GameObject Actor && Actor == ParentObject && Actor.Body != null)
                {
                    Actor.Body.UpdateBodyParts();
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
