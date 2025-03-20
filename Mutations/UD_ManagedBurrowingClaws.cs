using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using  static HNPS_GigantismPlus.Options;
using XRL.World.Parts;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_ManagedBurrowingClaws : BurrowingClaws, IManagedDefaultNaturalWeapon<UD_ManagedBurrowingClaws>
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

        // Dictionary holds a BodyPart.Type string as Key, and NaturalWeaponSubpart for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalWeaponSubpart.Type).
        public Dictionary<string, NaturalWeaponSubpart<UD_ManagedBurrowingClaws>> NaturalWeaponSubparts = new();
        public NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart { get; set; }

        public UD_ManagedBurrowingClaws()
            : base()
        {
            NaturalWeaponSubpart = new()
            {
                ParentPart = this,
                Level = Level,
                CosmeticOnly = false,
                Type = "Hand",
                ModPriority = 30,
                Skill = "ShortBlades",
                Adjective = "burrowing",
                AdjectiveColor = "W",
                AdjectiveColorFallback = "y",
                Noun = "claw",
                Tile = "Creatures/natural-weapon-claw.bmp",
                ColorString = "&w",
                DetailColor = "W",
                SecondColorString = "&w",
                SecondDetailColor = "W",
                SwingSound = "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing",
                BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked",
                AddedParts = new()
                {
                    "DiggingTool"
                }
            };
        }

        public UD_ManagedBurrowingClaws(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> naturalWeaponSubpart, UD_ManagedBurrowingClaws NewParent)
            : this()
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }
        public virtual NaturalWeaponSubpart<UD_ManagedBurrowingClaws> GetNaturalWeaponSubpart(
            string Type = "",
            GameObject Object = null,
            BodyPart BodyPart = null)
        {
            if (Type == "") goto CheckObject;

            if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
            if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];

            CheckObject:
            if (Object == null) goto CheckBodyPart;
            foreach (BodyPart part in Object?.Equipped.Body.LoopParts())
            {
                if (Object.IsDefaultEquipmentOf(part) || (part.Equipped == Object && Object.HasPart<NaturalEquipment>()))
                {
                    Type = part.Type;
                    if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
                    if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];
                }
            }

        CheckBodyPart:
            if (BodyPart == null) return null;
            Type = BodyPart.Type;
            if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
            if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];

            return null;
        }
        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<UD_ManagedBurrowingClaws> GetNaturalWeaponMod(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart)
        {
            ModNaturalWeaponBase<UD_ManagedBurrowingClaws> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod();
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = this;
            return NaturalWeaponMod;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, string Parts)
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

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1) 
        {
            NaturalWeaponSubpart.DamageDieCount = GetNaturalWeaponDamageDieCount(Level);
            return true; 
        }
        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            NaturalWeaponSubpart.DamageDieSize = GetNaturalWeaponDamageDieSize(Level);
            return true;
        }
        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1) 
        {
            NaturalWeaponSubpart.DamageBonus = GetNaturalWeaponDamageBonus(Level);
            return true; 
        }
        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1) 
        {
            NaturalWeaponSubpart.HitBonus = GetNaturalWeaponHitBonus(Level);
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

        public virtual int GetNaturalWeaponDamageDieCount(int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(int Level = 1)
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

        public virtual int GetNaturalWeaponDamageBonus(int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasCrystallinity))
            {
                return 1;
            }
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
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

        public override bool ChangeLevel(int NewLevel)
        {
            NaturalWeaponSubpart.Level = NewLevel;
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(UD_ManagedBurrowingClaws)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
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

                    part.DefaultBehavior.ApplyModification(NaturalWeaponSubpart.GetNaturalWeaponMod(), Actor: ParentObject);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);

            Exit:
            Debug.Footer(3, $"{nameof(UD_ManagedBurrowingClaws)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedBurrowingClaws burrowingClaws = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;
            burrowingClaws.NaturalWeaponSubpart = new(NaturalWeaponSubpart, burrowingClaws);
            return burrowingClaws;
        }
    }
}
