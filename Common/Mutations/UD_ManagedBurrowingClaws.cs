using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.NaturalEquipmentConditions;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_ManagedBurrowingClaws 
        : BurrowingClaws
        , IManagedDefaultNaturalEquipment<UD_ManagedBurrowingClaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_ManagedBurrowingClaws));
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

        public NaturalEquipmentManager NaturalEquipmentManager => ParentObject?.RequirePart<NaturalEquipmentManager>();

        public bool HasGigantism =>  ParentObject != null && ParentObject.HasPart<GigantismPlus>();

        public bool HasElongated => ParentObject != null && ParentObject.HasPart<ElongatedPaws>();

        public bool HasCrystallinity => ParentObject != null && ParentObject.HasPartDescendedFrom<Crystallinity>();

        public UD_ManagedBurrowingClaws()
            : base()
        {
        }

        public UD_ManagedBurrowingClaws(BurrowingClaws BurrowingClaws)
            : this()
        {
            Level = BurrowingClaws.Level;
            DigUpActivatedAbilityID = BurrowingClaws.DigUpActivatedAbilityID;
            DigDownActivatedAbilityID = BurrowingClaws.DigDownActivatedAbilityID;
            EnableActivatedAbilityID = BurrowingClaws.EnableActivatedAbilityID;
        }

        public static ModBurrowingNaturalWeapon NewBurrowingHandMod(NaturalEquipmentManager NewManager)
        {
            return new(NewManager);
        }

        public static ModBurrowingNaturalWeapon NewBurrowingArmMod(NaturalEquipmentManager NewManager)
        {
            // This is for Gigantic Scorpiock, and any other creature that uses Arm-slot melee weapons.
            return new(NewManager)
            {
                BodyPartType = "Arm",
            };
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod = null, int Level = 1)
        {
            if (HasGigantism)
            {
                if (HasElongated || HasCrystallinity)
                {
                    return 0;
                }
                return 1;
            }
            DieRoll baseDamage = new(GetClawsDamage(Level));
            return baseDamage.RightValue-2;
        }
        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod = null, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasCrystallinity))
            {
                return 1;
            }
            return 0;
        }
        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }

        public List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<UD_ManagedBurrowingClaws>> Filter = null)
        {
            return NaturalEquipmentManager.GetNaturalEquipmentMods(Filter);
        }

        public virtual ModNaturalEquipment<UD_ManagedBurrowingClaws> UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}<{nameof(UD_ManagedBurrowingClaws)}>, "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            NaturalEquipmentMod?.AdjustMeleeDamageDieCount(GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageDieSize(GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageBonus(GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level))
                ?.AdjustMeleeHitBonus(GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level))
                ?.AdjustPenBonus(GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level))
                
                ?.AddAdjustment(new AddBurrowingClawsProperties(GetWallBonusPenetration(), GetWallBonusPercentage()), true)
                
                ?.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

            Debug.Entry(4,
                $"x {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}<{nameof(UD_ManagedBurrowingClaws)}>, "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMod;
        }
        public virtual List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> UpdateNaturalEquipmentMods(List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}<{nameof(UD_ManagedBurrowingClaws)}>, "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod in NaturalEquipmentMods)
                {
                    UpdateNaturalEquipmentMod(naturalEquipmentMod, Level);
                }
            }

            Debug.Entry(4,
                $"x {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}<{nameof(UD_ManagedBurrowingClaws)}>, "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMods;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            // GO.RegisterEvent(this, ManageDefaultEquipmentEvent.OperatorID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            NeedPartSupportEvent.Send(GO, "Digging");
            // GO.UnregisterEvent(this, ManageDefaultEquipmentEvent.OperatorID);
            return base.Unmutate(GO);
        }
        public override void Remove()
        {
            NeedPartSupportEvent.Send(ParentObject, "Digging");
            base.Remove();
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            // base.OnRegenerateDefaultEquipment(body);
            return;
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }
        public virtual void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject?.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, 
                $"{nameof(UD_ManagedBurrowingClaws)}", 
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}(body)", 
                Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{nameof(UD_ManagedBurrowingClaws)}",
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}" +
                $"(body of: {ParentObject?.Blueprint})", 
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
                || ID == GetNaturalEquipmentModsEvent.ID
                || ID == BeforeManageDefaultNaturalEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(GetNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug("getMods"));

            E.AddNaturalEquipmentMods(UpdateNaturalEquipmentMods(GetNaturalEquipmentMods(
                m => m.BodyPartType == E.TargetBodyPart.Type),
                Level));

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeManageDefaultNaturalEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
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
            UD_ManagedBurrowingClaws mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;

            return mutation;
        }
    } //!-- public class UD_ManagedBurrowingClaws : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedBurrowingClaws>
}
