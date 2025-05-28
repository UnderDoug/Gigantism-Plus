using System;
using System.Text;
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
    public class UD_ManagedCrystallinity 
        : Crystallinity
        , IManagedDefaultNaturalEquipment<UD_ManagedCrystallinity>
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_ManagedCrystallinity));
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
        public Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod { get; set; }

        public bool HasGigantism => ParentObject != null && ParentObject.HasPart<GigantismPlus>();

        public bool GiganticRefractAdded = false;

        public bool HasElongated => ParentObject != null && ParentObject.HasPart<ElongatedPaws>();

        public bool HasBurrowing => ParentObject != null && ParentObject.HasPartDescendedFrom<BurrowingClaws>();

        public UD_ManagedCrystallinity()
            : base()
        {
            NaturalEquipmentMods = new();

            NaturalEquipmentMod = NewCrystallinePointMod(this);
        }
        public UD_ManagedCrystallinity(
            Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods, 
            UD_ManagedCrystallinity NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            this.NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public UD_ManagedCrystallinity(
            ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, 
            UD_ManagedCrystallinity NewParent)
            : this()
        {
            this.NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public UD_ManagedCrystallinity(
            Dictionary<string, ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods,
            ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, 
            UD_ManagedCrystallinity NewParent)
            : this(NaturalEquipmentMods, NewParent)
        {
            this.NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public UD_ManagedCrystallinity(Crystallinity Crystallinity)
            : this()
        {
            Level = Crystallinity.Level;
            RefractAdded = Crystallinity.RefractAdded;
        }

        public static ModCrystallineNaturalWeapon NewCrystallinePointMod(UD_ManagedCrystallinity assigningPart)
        {
            ModCrystallineNaturalWeapon crystalinePointMod = new()
            {
                AssigningPart = assigningPart,
                BodyPartType = "Hand",

                ModPriority = 40,
                DescriptionPriority = 40,

                Adjective = "crystalline",
                AdjectiveColor = "crystallized",
                AdjectiveColorFallback = "M",

                Adjustments = new(),

                AddedParts = new()
                {
                    "Inorganic",
                },

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked" },
                },
            };
            crystalinePointMod.AddAdjustment(MELEEWEAPON, "Skill", "ShortBlades");
            crystalinePointMod.AddAdjustment(MELEEWEAPON, "Stat", "Strength");

            crystalinePointMod.AddAdjustment(RENDER, "DisplayName", "point", true);

            crystalinePointMod.AddAdjustment(RENDER, "Tile", "Creatures/natural-weapon-claw.bmp", true);
            crystalinePointMod.AddAdjustment(RENDER, "ColorString", "&b", true);
            crystalinePointMod.AddAdjustment(RENDER, "TileColor", "&b", true);
            crystalinePointMod.AddAdjustment(RENDER, "DetailColor", "B", true);
            return crystalinePointMod;
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Parts)
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
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            Props.ParseProps(out NaturalEquipmentMod.AddedStringProps, out NaturalEquipmentMod.AddedIntProps);
            return !NaturalEquipmentMod.AddedStringProps.IsNullOrEmpty() || !NaturalEquipmentMod.AddedIntProps.IsNullOrEmpty();
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
                return 1;
            return 2;
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

        public static int GetRefractChance(int Level)
        {
            return 25;
        }
        public int GetRefractChance()
        {
            return GetRefractChance(Level);
        }

        public static float GetGigantismRefractFactor(int Level)
        {
            return 0.4f;
        }
        public float GetGigantismRefractFactor()
        {
            return GetGigantismRefractFactor(Level);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            // GO.RegisterEvent(this, ManageDefaultEquipmentEvent.ID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            if (GO.TryGetPart(out RefractLight refractLight))
            {
                if (RefractAdded)
                {
                    refractLight.Chance -= GetRefractChance();
                    RefractAdded = false;
                }
                if (GiganticRefractAdded)
                {
                    refractLight.Chance -= (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = false;
                }
                if (refractLight.Chance < 1)
                {
                    GO.RemovePart(refractLight);
                }
            }
            return base.Unmutate(GO);
        }

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level)
        {
            Debug.Entry(4,
                $"* {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}(ModNaturalEquipment<{nameof(UD_ManagedCrystallinity)}> NaturalEquipmentMod[{NaturalEquipmentMod.BodyPartType}], int Level: {Level})",
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
        public override string GetLevelText(int Level)
        {
            string levelText = base.GetLevelText(Level);
            if (ParentObject != null && ParentObject.TryGetPart(out GigantismPlus gigantism))
            {
                int baseRefractChance = GetRefractChance(Level);
                int giganticBonusRefractChance = (int)(baseRefractChance * GetGigantismRefractFactor(Level));
                int totalRefractChance = baseRefractChance + giganticBonusRefractChance;

                StringBuilder SB = Event.NewStringBuilder();

                SB.Append(totalRefractChance).Append("% chance to refract light-based attacks ");
                SB.Append("(").Append(baseRefractChance).Append("% base chance ");
                SB.AppendRule($"{giganticBonusRefractChance.Signed()}%").Append(" from ");
                SB.Append(gigantism.GetDisplayName()).Append(")");

                return levelText.Replace("25% chance to refract light-based attacks", Event.FinalizeString(SB));
            }
            return levelText;
        }

        public virtual bool ProcessNaturalEquipment(NaturalEquipmentManager Manager, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1, Toggle: getDoDebug());

            string targetType = TargetBodyPart.Type;
            Debug.LoopItem(4, $" part", $"{TargetBodyPart.Description} [{TargetBodyPart.ID}:{TargetBodyPart.Type}]", Indent: 2, Toggle: getDoDebug());
            ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod = null;
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
                $"x {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1, Toggle: getDoDebug());
            return true;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if(body != null && ParentObject.Body == body)
            {
                if (!ParentObject.TryGetPart(out RefractLight refractLight))
                {
                    refractLight = ParentObject.RequirePart<RefractLight>();
                }
                if (!RefractAdded)
                {
                    refractLight.Chance += GetRefractChance();
                    RefractAdded = true;
                }
                if (HasGigantism && !GiganticRefractAdded)
                {
                    refractLight.Chance += (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = true;
                }
                if (!HasGigantism && GiganticRefractAdded)
                {
                    refractLight.Chance -= (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = false;
                }
            }
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
            Debug.Header(4, $"{nameof(UD_ManagedCrystallinity)}", $"{nameof(OnManageNaturalEquipment)}(body)", Toggle: getDoDebug());
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", Indent: 0, Toggle: getDoDebug());

            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());
            ProcessNaturalEquipment(Manager, TargetBodyPart);
            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{nameof(UD_ManagedCrystallinity)}",
                $"{nameof(OnManageNaturalEquipment)}(body of: {ParentObject?.Blueprint})", Toggle: getDoDebug());
        }
        public virtual void OnUpdateNaturalEquipmentMods()
        {
            Debug.Entry(4, $"> foreach ((_, ModNaturalEquipment<E> NaturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: getDoDebug());
            foreach ((_, ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod) in NaturalEquipmentMods)
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
                || ID == ManageDefaultEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(UpdateNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedCrystallinity)}."
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
                $"@ {nameof(UD_ManagedCrystallinity)}."
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
