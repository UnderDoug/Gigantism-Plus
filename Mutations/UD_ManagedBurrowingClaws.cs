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

        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<UD_ManagedBurrowingClaws>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod { get; set; }

        public UD_ManagedBurrowingClaws NaturalWeaponManager { get; set; }

        public bool HasGigantism =>  ParentObject != null && ParentObject.HasPart<GigantismPlus>();

        public bool HasElongated => ParentObject != null && ParentObject.HasPart<ElongatedPaws>();

        public bool HasCrystallinity => ParentObject != null && ParentObject.HasPartDescendedFrom<Crystallinity>();

        public UD_ManagedBurrowingClaws()
            : base()
        {
            NaturalEquipmentMods = new();

            NaturalEquipmentMod = NewBurrowingClawMod(this);
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

        public static ModBurrowingNaturalWeapon NewBurrowingClawMod(UD_ManagedBurrowingClaws assigningPart)
        {
            ModBurrowingNaturalWeapon burrowingClawsMod = new()
            {
                AssigningPart = assigningPart,
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
            burrowingClawsMod.AddAdjustment(MELEEWEAPON, "Skill", "ShortBlades");
            burrowingClawsMod.AddAdjustment(MELEEWEAPON, "Stat", "Strength");

            burrowingClawsMod.AddAdjustment(RENDER, "DisplayName", "claw", true);

            burrowingClawsMod.AddAdjustment(RENDER, "Tile", "Creatures/natural-weapon-claw.bmp", true);
            burrowingClawsMod.AddAdjustment(RENDER, "ColorString", "&w", true);
            burrowingClawsMod.AddAdjustment(RENDER, "TileColor", "&w", true);
            burrowingClawsMod.AddAdjustment(RENDER, "DetailColor", "W", true);
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
            // GO.RegisterEvent(this, ManageDefaultEquipmentEvent.ID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            NeedPartSupportEvent.Send(GO, "Digging");
            // GO.UnregisterEvent(this, ManageDefaultEquipmentEvent.ID);
            return base.Unmutate(GO);
        }
        public override void Remove()
        {
            NeedPartSupportEvent.Send(ParentObject, "Digging");
            base.Remove();
        }

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod, int Level)
        {
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}(ModNaturalEquipment<{nameof(UD_ManagedBurrowingClaws)}> NaturalEquipmentMod[{NaturalEquipmentMod.BodyPartType}], int Level: {Level})",
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
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1, Toggle: getDoDebug());

            string targetType = TargetBodyPart.Type;
            Debug.LoopItem(4, $" part", $"{TargetBodyPart.Description} [{TargetBodyPart.ID}:{TargetBodyPart.Type}]", Indent: 2, Toggle: getDoDebug());
            ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod = null;
            if (NaturalEquipmentMod != null && NaturalEquipmentMod.BodyPartType == targetType)
            {
                naturalEquipmentMod = NaturalEquipmentMod;
                Debug.CheckYeh(4, $"NaturalEquipmentMod Property contains entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"NaturalEquipmentMod Property does not contain entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
            }

            if (!NaturalEquipmentMods.IsNullOrEmpty() && NaturalEquipmentMods.ContainsKey(targetType))
            {
                naturalEquipmentMod = NaturalEquipmentMods[targetType];
                Debug.CheckYeh(4, $"NaturalEquipmentMod Dictionary contains entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
                Manager.AddNaturalEquipmentMod(naturalEquipmentMod);
                Debug.Entry(4, $"Added NaturalWeaponMod: {naturalEquipmentMod?.Name}", Indent: 2, Toggle: getDoDebug());
            }
            else
            {
                Debug.CheckNah(4, $"NaturalEquipmentMod Dictionary does not contain entry for this BodyPart", Indent: 2, Toggle: getDoDebug());
            }
            Debug.Entry(4,
                $"x {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1, Toggle: getDoDebug());
            return true;
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
        public virtual void OnManageNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject?.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{nameof(UD_ManagedBurrowingClaws)}", $"{nameof(OnManageNaturalEquipment)}(body)", Toggle: getDoDebug());
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", Indent: 0, Toggle: getDoDebug());

            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{nameof(UD_ManagedBurrowingClaws)}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {ParentObject?.Blueprint})", Toggle: getDoDebug());
        }
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
            foreach ((_, ModNaturalEquipment<UD_ManagedBurrowingClaws> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, Level);
            Debug.Entry(4, $"x foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: getDoDebug());
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
                || ID == UpdateNaturalEquipmentModsEvent.ID
                || ID == ManageDefaultEquipmentEvent.ID
                || ID == PartSupportEvent.ID;
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}({nameof(UpdateNaturalEquipmentModsEvent)} E)",
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
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
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
        public override bool HandleEvent(PartSupportEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(HandleEvent)}({typeof(PartSupportEvent).Name} E)",
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

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            NaturalEquipmentMods ??= new();
            Writer.Write(NaturalEquipmentMods.Count);
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((string bodyPartType, ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod) in NaturalEquipmentMods)
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
                NaturalEquipmentMods.Add(Reader.ReadOptimizedString(), (ModNaturalEquipment<UD_ManagedBurrowingClaws>)Reader.ReadObject());
            }

            NaturalEquipmentMod = (ModNaturalEquipment<UD_ManagedBurrowingClaws>)Reader.ReadObject();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedBurrowingClaws mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;

            mutation.NaturalEquipmentMods = new();
            NaturalEquipmentMods ??= new();
            if (NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((string bodyPartType, ModNaturalEquipment<UD_ManagedBurrowingClaws> naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, mutation));
                }
            }

            NaturalEquipmentMod ??= new();
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, mutation);

            return mutation;
        }
    } //!-- public class UD_ManagedBurrowingClaws : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedBurrowingClaws>
}
