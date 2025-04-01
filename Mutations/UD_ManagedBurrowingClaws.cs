using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.Rules;
using XRL.World.Parts;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

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
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> naturalEquipmentMods, 
            UD_ManagedBurrowingClaws NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod) in naturalEquipmentMods)
            {
                ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public UD_ManagedBurrowingClaws(
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod, 
            UD_ManagedBurrowingClaws NewParent)
            : this()
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
        }

        public UD_ManagedBurrowingClaws(
            Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> naturalEquipmentMods,
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod,
            UD_ManagedBurrowingClaws NewParent)
            : this(naturalEquipmentMods, NewParent)
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
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
        public override bool ChangeLevel(int NewLevel)
        {
            foreach ((_, ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
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
            if (NaturalEquipmentMod != null)
            {
                naturalEquipmentMod = NaturalEquipmentMod;
                Debug.CheckYeh(4, $"NaturalEquipmentMod for this BodyPart contained in Property", Indent: 3);
            }
            else if (NaturalEquipmentMods.ContainsKey(targetType))
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
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(UD_ManagedBurrowingClaws).Name}", $"{nameof(OnManageNaturalEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1);

            Debug.Footer(4,
                $"{typeof(UD_ManagedBurrowingClaws).Name}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {ParentObject.Blueprint})");
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ManageDefaultEquipmentEvent.ID;
        }
        public bool HandleEvent(ManageDefaultEquipmentEvent E)
        {
            if (E.Wielder == ParentObject)
            {
                Debug.Entry(4, $"E.target", E.target, Indent: 0);
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
