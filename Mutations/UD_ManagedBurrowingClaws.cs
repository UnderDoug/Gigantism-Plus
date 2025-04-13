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
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod { get; set; }

        public UD_ManagedBurrowingClaws NaturalWeaponManager { get; set; }

        private bool _HasGigantism = false;
        public bool HasGigantism
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPart<GigantismPlus>();
                return _HasGigantism;
            }
            set
            {
                _HasGigantism = value;
            }
        }

        private bool _HasElongated = false;
        public bool HasElongated
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPart<ElongatedPaws>();
                return _HasElongated;
            }
            set
            {
                _HasElongated = value;
            }
        }

        private bool _HasCrystallinity = false;
        public bool HasCrystallinity
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPartDescendedFrom<Crystallinity>();
                return _HasCrystallinity;
            }
            set
            {
                _HasCrystallinity = value;
            }
        }

        public UD_ManagedBurrowingClaws()
            : base()
        {
            NaturalEquipmentMods = new();

            NaturalEquipmentMod = new ModBurrowingNaturalWeapon()
            {
                AssigningPart = this,
                BodyPartType = "Hand",

                ModPriority = 30,
                DescriptionPriority = 30,

                Adjective = "burrowing",
                AdjectiveColor = "W",
                AdjectiveColorFallback = "y",

                Adjustments = new(),
                
                AddedParts = new()
                {
                    "DiggingTool"
                },

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked" }
                },
            };
            NaturalEquipmentMod.AddAdjustment(MELEEWEAPON, "Skill", "ShortBlades");
            NaturalEquipmentMod.AddAdjustment(MELEEWEAPON, "Stat", "Strength");

            NaturalEquipmentMod.AddAdjustment(RENDER, "DisplayName", "claw", true);

            NaturalEquipmentMod.AddAdjustment(RENDER, "Tile", "Creatures/natural-weapon-claw.bmp", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "ColorString", "&w", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "TileColor", "&w", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "DetailColor", "W", true);
        }
        public UD_ManagedBurrowingClaws(
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods, 
            UD_ManagedBurrowingClaws NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            this.NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public UD_ManagedBurrowingClaws(
            ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, 
            UD_ManagedBurrowingClaws NewParent)
            : this()
        {
            this.NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public UD_ManagedBurrowingClaws(
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods,
            ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod,
            UD_ManagedBurrowingClaws NewParent)
            : this(NaturalEquipmentMods, NewParent)
        {
            this.NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public UD_ManagedBurrowingClaws(BurrowingClaws BurrowingClaws)
            : this()
        {
            Level = BurrowingClaws.Level;
            DigUpActivatedAbilityID = BurrowingClaws.DigUpActivatedAbilityID;
            DigDownActivatedAbilityID = BurrowingClaws.DigDownActivatedAbilityID;
            EnableActivatedAbilityID = BurrowingClaws.EnableActivatedAbilityID;
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, string Parts)
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
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalEquipmentMod.AddedStringProps = StringProps;
                NaturalEquipmentMod.AddedIntProps = IntProps;
            }
            return true;
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
                    return 0;
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

        public override bool Mutate(GameObject GO, int Level)
        {
            GO.RegisterEvent(this, ManageDefaultEquipmentEvent.ID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            GO.UnregisterEvent(this, ManageDefaultEquipmentEvent.ID);
            return base.Unmutate(GO);
        }

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level)
        {
            Debug.Entry(4,
                $"* {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(UpdateNaturalEquipmentMod)}(ModNaturalEquipment<{typeof(UD_ManagedBurrowingClaws).Name}> NaturalEquipmentMod[{NaturalEquipmentMod.BodyPartType}], int Level: {Level})",
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
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1);

            string targetType = TargetBodyPart.Type;
            Debug.LoopItem(4, $" part", $"{TargetBodyPart.Description} [{TargetBodyPart.ID}:{TargetBodyPart.Type}]", Indent: 2);
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = null;
            if (NaturalEquipmentMod != null && NaturalEquipmentMod.BodyPartType == targetType)
            {
                naturalEquipmentMod = NaturalEquipmentMod;
                Debug.CheckYeh(4, $"NaturalEquipmentMod for this BodyPart contained in Property", Indent: 2);
            }
            else if (!NaturalEquipmentMods.IsNullOrEmpty() && NaturalEquipmentMods.ContainsKey(targetType))
            {
                naturalEquipmentMod = NaturalEquipmentMods[targetType];
                Debug.CheckYeh(4, $"NaturalEquipmentMod for this BodyPart contained in Dictionary", Indent: 3);
            }
            else
            {
                Debug.CheckNah(4, $"No NaturalEquipmentMod for this BodyPart", Indent: 3);
            }
            if (naturalEquipmentMod != null)
            {
                Debug.Entry(4, $"NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2);
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
            }
            Debug.Entry(4,
                $"x {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1);
            return true;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            // base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }
        public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject?.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(UD_ManagedBurrowingClaws).Name}", $"{nameof(OnManageNaturalEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1);

            Debug.Footer(4,
                $"{typeof(UD_ManagedBurrowingClaws).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {ParentObject?.Blueprint})");
        }
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
            foreach ((_, ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ManageDefaultEquipmentEvent.ID
                || ID == UpdateNaturalEquipmentModsEvent.ID;
        }
        public bool HandleEvent(ManageDefaultEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(HandleEvent)}({typeof(ManageDefaultEquipmentEvent).Name} E)",
                Indent: 0);

            if (E.Wielder == ParentObject)
            {
                OnManageNaturalEquipment(E.Manager, E.BodyPart);
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(HandleEvent)}({typeof(UpdateNaturalEquipmentModsEvent).Name} E)",
                Indent: 0);

            if (E.Actor == ParentObject)
            {
                OnUpdateNaturalEquipmentMods();
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
                    // ProcessNaturalEquipment(Actor?.Actor);
                }
            }
            return base.FireEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedBurrowingClaws mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;
            mutation.NaturalEquipmentMods = new();
            foreach ((string bodyPartType, ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, mutation));
            }
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, mutation);
            return mutation;
        }
    }
}
