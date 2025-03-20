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

        // Dictionary holds a BodyPart.Type string as Key, and NaturalWeaponSubpart for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalWeaponSubpart.Type).
        public Dictionary<string, NaturalWeaponSubpart<UD_ManagedCrystallinity>> NaturalWeaponSubparts = new();
        public NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart { get; set; }

        public UD_ManagedCrystallinity()
            : base()
        {
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

        public UD_ManagedCrystallinity(NaturalWeaponSubpart<UD_ManagedCrystallinity> naturalWeaponSubpart, UD_ManagedCrystallinity NewParent)
            : this()
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public virtual NaturalWeaponSubpart<UD_ManagedCrystallinity> GetNaturalWeaponSubpart(
            string Type = "",
            GameObject Object = null,
            BodyPart BodyPart = null)
        {
            if (Type != "")
            {
                if (Type == NaturalWeaponSubpart?.Type)
                    return NaturalWeaponSubpart;
                if (NaturalWeaponSubparts.ContainsKey(Type))
                    return NaturalWeaponSubparts[Type];
            }
            if (Object != null)
            {
                foreach (BodyPart part in Object?.Equipped.Body.LoopParts())
                {
                    if (Object.IsDefaultEquipmentOf(part) || (part.Equipped == Object && Object.HasPart<NaturalEquipment>()))
                    {
                        Type = part.Type;
                        if (Type == NaturalWeaponSubpart?.Type)
                            return NaturalWeaponSubpart;
                        if (NaturalWeaponSubparts.ContainsKey(Type))
                            return NaturalWeaponSubparts[Type];
                    }
                }
            }
            if (BodyPart != null)
            {
                Type = BodyPart.Type;
                if (Type == NaturalWeaponSubpart?.Type)
                    return NaturalWeaponSubpart;
                if (NaturalWeaponSubparts.ContainsKey(Type))
                    return NaturalWeaponSubparts[Type];
            }
            return null;
        }
        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<UD_ManagedCrystallinity> GetNaturalWeaponMod(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, bool Managed = true)
        {
            ModNaturalWeaponBase<UD_ManagedCrystallinity> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod(Managed);
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = this;
            return NaturalWeaponMod;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, string Parts)
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

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, string Props)
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

        public virtual int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
                return 0;
            return 1;
        }

        public virtual int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
            {
                return 1;
            }
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart<UD_ManagedCrystallinity> NaturalWeaponSubpart)
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

        public virtual bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<UD_ManagedCrystallinity> Subpart, int Level)
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
            foreach ((_, NaturalWeaponSubpart<UD_ManagedCrystallinity> Subpart) in NaturalWeaponSubparts)
            {
                UpdateNaturalWeaponSubpart(Subpart, NewLevel);
            }
            if (NaturalWeaponSubpart != null) UpdateNaturalWeaponSubpart(NaturalWeaponSubpart, NewLevel);
            return base.ChangeLevel(NewLevel);
        }
        public virtual bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false)
        {
            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    ModNaturalWeaponBase<UD_ManagedCrystallinity> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null && part.Type == NaturalWeaponSubpart.Type && NaturalWeaponSubpart.IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type) && NaturalWeaponSubparts[part.Type].IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                    }

                    if (modNaturalWeapon == null) continue;

                    if (part.DefaultBehavior != null)
                    {
                        part.DefaultBehavior.ApplyModification(modNaturalWeapon, Actor: ParentObject);
                    }
                    else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                    {
                        part.Equipped.ApplyModification(modNaturalWeapon, Actor: ParentObject);
                    }
                }
            }
            return true;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: false);

            /*
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(UD_ManagedCrystallinity)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                goto Exit;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "> foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod<UD_ManagedCrystallinity>(), Actor: ParentObject);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);

            Exit:
            Debug.Footer(3, $"{nameof(UD_ManagedCrystallinity)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            */
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: true);

            base.OnDecorateDefaultEquipment(body);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedCrystallinity managedCrystallinity = base.DeepCopy(Parent, MapInv) as UD_ManagedCrystallinity;
            managedCrystallinity.NaturalWeaponSubparts = new();
            foreach ((_, NaturalWeaponSubpart<UD_ManagedCrystallinity> subpart) in NaturalWeaponSubparts)
            {
                managedCrystallinity.NaturalWeaponSubparts.Add(subpart.Type, new(subpart, managedCrystallinity));
            }
            managedCrystallinity.NaturalWeaponSubpart = new(NaturalWeaponSubpart, managedCrystallinity);
            return managedCrystallinity;
        }
    }
}
