using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Rules;
using XRL.Language;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_ManagedCrystallinity : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedCrystallinity>
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod { get; set; }

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

        private bool _HasBurrowing = false;
        public bool HasBurrowing
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPartDescendedFrom<BurrowingClaws>();
                return _HasBurrowing;
            }
            set
            {
                _HasBurrowing = value;
            }
        }

        public UD_ManagedCrystallinity()
            : base()
        {
            NaturalEquipmentMods = new();

            NaturalEquipmentMod = new ModCrystallineNaturalWeapon()
            {
                AssigningPart = this,
                BodyPartType = "Hand",

                DamageDieSize = 1,

                ModPriority = 40,
                DescriptionPriority = 40,

                Adjective = "crystalline",
                AdjectiveColor = "crystallized",
                AdjectiveColorFallback = "M",

                Adjustments = new(),

                AddedParts = new()
                {
                    "Inorganic"
                },

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked" }
                },
            };
            NaturalEquipmentMod.AddAdjustment(GAMEOBJECT, "Skill", "ShortBlades");
            NaturalEquipmentMod.AddAdjustment(GAMEOBJECT, "Stat", "Strength");

            NaturalEquipmentMod.AddAdjustment(RENDER, "DisplayName", "point", true);

            NaturalEquipmentMod.AddAdjustment(RENDER, "Tile", "Creatures/natural-weapon-claw.bmp", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "ColorString", "&b", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "TileColor", "&b", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "DetailColor", "B", true);
        }
        public UD_ManagedCrystallinity(Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> naturalEquipmentMods, UD_ManagedCrystallinity NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod) in naturalEquipmentMods)
            {
                ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public UD_ManagedCrystallinity(ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod, UD_ManagedCrystallinity NewParent)
            : this()
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
        }

        public UD_ManagedCrystallinity(Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> naturalEquipmentMods, ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod, UD_ManagedCrystallinity NewParent)
            : this(naturalEquipmentMods, NewParent)
        {
            NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Parts)
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

        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalEquipmentMod.AddedStringProps = StringProps;
                NaturalEquipmentMod.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
                return 0;
            return 1;
        }

        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
            {
                return 1;
            }
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedIntProps;
        }

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level)
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
            foreach ((_, ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalEquipment(Body body)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedCrystallinity).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart bodyPart in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Type}]", Indent: 2);
                    ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod = null;
                    if (NaturalEquipmentMod != null && bodyPart.Type == NaturalEquipmentMod.BodyPartType)
                    {
                        naturalEquipmentMod = new(NaturalEquipmentMod);
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Property", Indent: 3);
                    }
                    else if (NaturalEquipmentMods.ContainsKey(bodyPart.Type))
                    {
                        naturalEquipmentMod = NaturalEquipmentMods[bodyPart.Type];
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Dictionary", Indent: 3);
                    }
                    else
                    {
                        Debug.Entry(4, $"No NaturalEquipmentMod for this BodyPart", Indent: 3);
                    }

                    if (naturalEquipmentMod == null) continue;

                    Debug.Entry(4, $"modNaturalWeapon: {naturalEquipmentMod?.Name}", Indent: 3);
                    GameObject equipment = bodyPart.DefaultBehavior ?? bodyPart.Equipped;
                    if (equipment != null && equipment.HasPart<NaturalEquipment>())
                    {
                        if (equipment.TryGetPart(out NaturalEquipmentManager manager))
                        {
                            Debug.Entry(4, $"Equipment: {equipment.ShortDisplayNameStripped}", Indent: 3);
                        }
                        else
                        {
                            Debug.Entry(4, $"WARN: {equipment.ShortDisplayNameStripped} is missing NaturalEquipmentManager", Indent: 3);
                            manager = equipment.AddPart<NaturalEquipmentManager>();
                        }
                        manager.WantToManage = true;
                        ModNaturalEquipment<UD_ManagedCrystallinity> newNaturalEquipmentMod = new(naturalEquipmentMod);
                        manager.AddNaturalEquipmentMod(newNaturalEquipmentMod);
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(UD_ManagedCrystallinity).Name}."
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
        public virtual void OnBodyPartsUpdated(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(UD_ManagedCrystallinity).Name}", $"{nameof(OnBodyPartsUpdated)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalEquipment(body);

            Debug.Footer(4,
                $"{typeof(UD_ManagedCrystallinity).Name}",
                $"{nameof(OnBodyPartsUpdated)}(body: {ParentObject.Blueprint})");
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
                    ProcessNaturalEquipment(Actor?.Body);
                }
            }
            return base.FireEvent(E);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == BodyPartsUpdatedEvent.ID;
        }

        public virtual bool HandEvent(BodyPartsUpdatedEvent E)
        {
            OnBodyPartsUpdated(E.Object.Body);
            return base.HandleEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedCrystallinity mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedCrystallinity;
            mutation.NaturalEquipmentMods = new();
            foreach ((string bodyPartType, ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, mutation));
            }
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, mutation);
            return mutation;
        }
    } //!-- public class UD_ManagedCrystallinity : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedCrystallinity>
}
