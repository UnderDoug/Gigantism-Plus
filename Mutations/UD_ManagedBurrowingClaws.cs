using System;
using System.Collections.Generic;
using System.Linq;

using XRL.Rules;
using XRL.Language;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

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

        public virtual List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods => GetNaturalEquipmentMods();
        public virtual ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod => GetNaturalEquipmentMod();

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

        public static ModBurrowingNaturalWeapon NewBurrowingWeaponMod(UD_ManagedBurrowingClaws assigningPart)
        {
            ModBurrowingNaturalWeapon burrowingClawsMod = new()
            {
                AssigningPart = assigningPart,
                BodyPartType = "Hand",

                ModPriority = 80,
                DescriptionPriority = 80,

                ForceNoun = true,
                Noun = "claw",

                Adjective = "burrowing",
                AdjectiveColor = "W",
                AdjectiveColorFallback = "y",

                Adjustments = new(),

                AddedParts = new()
                {
                    nameof(DiggingTool),
                },

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked" },
                },
            };
            burrowingClawsMod.AddSkillAdjustment("ShortBlades", true);

            burrowingClawsMod.AddNounAdjustment(true);

            burrowingClawsMod.AddTileAdjustment("Creatures/natural-weapon-claw.bmp", true);
            burrowingClawsMod.AddColorStringAdjustment("&w", true);
            burrowingClawsMod.AddTileColorAdjustment("&w", true);
            burrowingClawsMod.AddDetailColorAdjustment("W", true);
            return burrowingClawsMod;
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, string Parts)
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
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            Props.ParseProps(out NaturalEquipmentMod.AddedStringProps, out NaturalEquipmentMod.AddedIntProps);
            return !NaturalEquipmentMod.AddedStringProps.IsNullOrEmpty() || !NaturalEquipmentMod.AddedIntProps.IsNullOrEmpty();
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
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
        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasCrystallinity))
            {
                return 1;
            }
            return 0;
        }
        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedParts;
        }
        public virtual Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedStringProps;
        }
        public virtual Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedIntProps;
        }
        public virtual ModNaturalEquipment<UD_ManagedBurrowingClaws> GetNaturalEquipmentMod(Predicate<ModNaturalEquipment<UD_ManagedBurrowingClaws>> Filter = null, UD_ManagedBurrowingClaws NewAssigner = null)
        {
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = NewBurrowingWeaponMod(NewAssigner ?? this);
            return Filter == null || Filter(naturalEquipmentMod) ? naturalEquipmentMod : null;
        }
        public virtual List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<UD_ManagedBurrowingClaws>> Filter = null, UD_ManagedBurrowingClaws NewAssigner = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(GetNaturalEquipmentMods)}("
                + $"{nameof(Filter)}, "
                + $"{nameof(NewAssigner)}: {NewAssigner?.Name})",
                Indent: indent + 1, Toggle: getDoDebug());

            NewAssigner ??= this;
            List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> naturalEquipmentModsList = new();
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = GetNaturalEquipmentMod(Filter, NewAssigner);
            if (naturalEquipmentMod != null)
            {
                naturalEquipmentModsList.Add(naturalEquipmentMod);
            }

            Debug.LastIndent = indent;
            return naturalEquipmentModsList;
        }
        public virtual ModNaturalEquipment<UD_ManagedBurrowingClaws> UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{nameof(UD_ManagedBurrowingClaws)}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);

            NaturalEquipmentMod.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

            Debug.Entry(4,
                $"x {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{nameof(UD_ManagedBurrowingClaws)}], "
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
                + $"{nameof(NaturalEquipmentMods)}[{nameof(UD_ManagedBurrowingClaws)}], "
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
                + $"{nameof(NaturalEquipmentMods)}[{nameof(UD_ManagedBurrowingClaws)}], "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMods;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            // GO.RegisterEvent(this, ManageDefaultEquipmentEvent.ManagerID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            NeedPartSupportEvent.Send(GO, "Digging");
            // GO.UnregisterEvent(this, ManageDefaultEquipmentEvent.ManagerID);
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
        public virtual void OnManageDefaultNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject?.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, 
                $"{nameof(UD_ManagedBurrowingClaws)}", 
                $"{nameof(OnManageDefaultNaturalEquipment)}(body)", 
                Toggle: getDoDebug());
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{nameof(UD_ManagedBurrowingClaws)}",
                $"{nameof(OnManageDefaultNaturalEquipment)}" +
                $"(body of: {ParentObject?.Blueprint})", 
                Toggle: getDoDebug());
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
                || ID == ManageDefaultNaturalEquipmentEvent.ID
                || ID == PartSupportEvent.ID;
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
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetPrioritisedNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            List<ModNaturalEquipment<UD_ManagedBurrowingClaws>> naturalEquipmentMods = 
                UpdateNaturalEquipmentMods(GetNaturalEquipmentMods(
                    mod => mod.BodyPartType == E.TargetBodyPart.Type), 
                    Level);

            foreach (ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod in naturalEquipmentMods)
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
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ManageDefaultNaturalEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

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
        public override bool HandleEvent(PartSupportEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(PartSupportEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Skip != this && E.Type == "Digging" && IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID))
            {
                return false;
            }
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
            UD_ManagedBurrowingClaws mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;

            return mutation;
        }
    } //!-- public class UD_ManagedBurrowingClaws : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedBurrowingClaws>
}
