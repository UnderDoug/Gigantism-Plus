using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.Rules;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_ManagedCrystallinity : Crystallinity, IManagedDefaultNaturalWeapon<UD_ManagedCrystallinity>
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, NaturalEquipmentSubpart<UD_ManagedCrystallinity>> NaturalWeaponSubparts { get; set; }
        public NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart { get; set; }

        public UD_ManagedCrystallinity NaturalWeaponManager { get; set; }

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
            NaturalWeaponSubparts = new();

            NaturalWeaponSubpart = new()
            {
                ParentPart = this,
                Level = 1,
                CosmeticOnly = false,
                Type = "Hand",

                DamageDieSize = 1,
                DamageBonus = -1, // this is to force the default "InorganicManipulator" to match the default fist.

                ModPriority = 40,
                Skill = "ShortBlades",
                Adjective = "crystalline",
                AdjectiveColor = "crystallized",
                AdjectiveColorFallback = "M",
                Noun = "point",

                Tile = "Creatures/natural-weapon-claw.bmp",
                ColorString = "&b",
                DetailColor = "B",
                SecondColorString = "&B",
                SecondDetailColor = "m",
                SwingSound = "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing",
                BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked",

                AddedParts = new()
                {
                    "Inorganic"
                }
            };
        }

        public UD_ManagedCrystallinity(NaturalEquipmentSubpart<UD_ManagedCrystallinity> naturalWeaponSubpart, UD_ManagedCrystallinity NewParent)
            : this()
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public virtual string GetNaturalWeaponModName(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<UD_ManagedCrystallinity> GetNaturalWeaponMod(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, bool Managed = true)
        {
            ModNaturalWeaponBase<UD_ManagedCrystallinity> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod(Managed);
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = this;
            NaturalWeaponMod.Wielder = ParentObject;
            return NaturalWeaponMod;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, string Parts)
        {
            if (Parts == null) return false;
            NaturalWeaponSubpart.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeaponSubpart.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(string Parts)
        {
            if (Parts == null) return false;
            NaturalWeaponSubpart.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeaponSubpart.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
                return 0;
            return 1;
        }

        public virtual int GetNaturalWeaponDamageBonus(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
            {
                return 1;
            }
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalEquipmentSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual List<string> GetNaturalWeaponAddedParts()
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps()
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps()
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual string GetNaturalWeaponEquipmentFrameColors()
        {
            return NaturalWeaponSubpart.EquipmentFrameColors;
        }

        public virtual bool UpdateNaturalWeaponSubpart(NaturalEquipmentSubpart<UD_ManagedCrystallinity> Subpart, int Level)
        {
            Subpart.Level = Level;
            Subpart.DamageDieCount = GetNaturalWeaponDamageDieCount(Subpart, Level);
            Subpart.DamageDieSize = GetNaturalWeaponDamageDieSize(Subpart, Level);
            Subpart.DamageBonus = GetNaturalWeaponDamageBonus(Subpart, Level);
            Subpart.HitBonus = GetNaturalWeaponHitBonus(Subpart, Level);
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            foreach ((_, NaturalEquipmentSubpart<UD_ManagedCrystallinity> Subpart) in NaturalWeaponSubparts)
            {
                UpdateNaturalWeaponSubpart(Subpart, NewLevel);
            }
            if (NaturalWeaponSubpart != null) UpdateNaturalWeaponSubpart(NaturalWeaponSubpart, NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedCrystallinity).Name}."
                + $"{nameof(ProcessNaturalWeaponSubparts)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{part.Description} [{part.ID}:{part.Type}]", Indent: 2);
                    ModNaturalWeaponBase<UD_ManagedCrystallinity> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null
                        && part.Type == NaturalWeaponSubpart.Type
                        && NaturalWeaponSubpart.IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                        Debug.Entry(4, $"NaturalEquipmentMod", Indent: 3);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type)
                        && NaturalWeaponSubparts[part.Type].IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                        Debug.Entry(4, $"NaturalEquipmentMods", Indent: 3);
                    }

                    Debug.Entry(4, $"modNaturalWeapon: {modNaturalWeapon?.Name}", Indent: 3);

                    if (modNaturalWeapon == null) continue;

                    if (part.DefaultBehavior != null)
                    {
                        part.DefaultBehavior.ApplyModification(modNaturalWeapon, Actor: body.ParentObject);
                    }
                    else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                    {
                        part.Equipped.ApplyModification(modNaturalWeapon, Actor: body.ParentObject);
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(ProcessNaturalWeaponSubparts)} @//",
                Indent: 1);
            return true;
        }
        public virtual bool UnprocessNaturalWeaponSubparts(Body body)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(UnprocessNaturalWeaponSubparts)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{part.Description} [{part.ID}:{part.Type}]", Indent: 2);
                    ModNaturalWeaponBase<UD_ManagedCrystallinity> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null
                        && part.Type == NaturalWeaponSubpart.Type)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                        Debug.Entry(4, $"NaturalEquipmentMod", Indent: 3);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type))
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                        Debug.Entry(4, $"NaturalEquipmentMods", Indent: 3);
                    }

                    Debug.Entry(4, $"modNaturalWeapon: {modNaturalWeapon?.Name}", Indent: 3);

                    if (modNaturalWeapon == null) continue;

                    if (part.DefaultBehavior != null)
                    {
                        part.DefaultBehavior.RemovePart(modNaturalWeapon);
                        if (part.DefaultBehavior.TryGetPart(out NaturalWeaponDescriber naturalWeaponDescriber))
                        {
                            naturalWeaponDescriber.ResetShortDescription();
                            naturalWeaponDescriber.CollectNaturalWeaponMods();
                            part.DefaultBehavior.Obliterate();
                        }
                    }
                    else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                    {
                        part.Equipped.RemovePart(modNaturalWeapon);
                        if (part.Equipped.TryGetPart(out NaturalWeaponDescriber naturalWeaponDescriber))
                        {
                            naturalWeaponDescriber.ResetShortDescription();
                            naturalWeaponDescriber.CollectNaturalWeaponMods();
                            part.Equipped.Obliterate();
                        }
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(UnprocessNaturalWeaponSubparts)} @//",
                Indent: 1);
            return true;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(UD_ManagedCrystallinity).Name}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: false);

            Debug.Entry(4, $"Skipping base.{nameof(OnRegenerateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(4,
                $"{typeof(UD_ManagedCrystallinity).Name}",
                $"{nameof(OnRegenerateDefaultEquipment)}(body: {ParentObject.Blueprint})");
            // skip base
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(UD_ManagedCrystallinity).Name}", $"{nameof(OnDecorateDefaultEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: true);

            Debug.Entry(4, $"* base.{nameof(OnRegenerateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(4,
                $"{typeof(UD_ManagedCrystallinity).Name}",
                $"{nameof(OnDecorateDefaultEquipment)}(body: {ParentObject.Blueprint})");
            base.OnDecorateDefaultEquipment(body);
        }
        public override void AfterUnmutate(GameObject GO)
        {
            UnprocessNaturalWeaponSubparts(GO.Body);
            GO.GetPart<Mutations>().ActiveMutationList.Vomit(4, GO.Blueprint, Indent: 3);
            foreach (BaseMutation mutation in GO.GetPart<Mutations>().ActiveMutationList)
            {
                if (mutation.GetMutationClass() == GetMutationClass()) continue;
                mutation.ChangeLevel(mutation.Level);
            }
            base.AfterUnmutate(GO);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("MutationAdded");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "MutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                ProcessNaturalWeaponSubparts(Actor?.Body);
            }
            return base.FireEvent(E);
        }


        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedCrystallinity managedCrystallinity = base.DeepCopy(Parent, MapInv) as UD_ManagedCrystallinity;
            managedCrystallinity.NaturalWeaponSubparts = new();
            foreach ((_, NaturalEquipmentSubpart<UD_ManagedCrystallinity> subpart) in NaturalWeaponSubparts)
            {
                managedCrystallinity.NaturalWeaponSubparts.Add(subpart.Type, new(subpart, managedCrystallinity));
            }
            managedCrystallinity.NaturalWeaponSubpart = new(NaturalWeaponSubpart, managedCrystallinity);
            return managedCrystallinity;
        }
    }
}
