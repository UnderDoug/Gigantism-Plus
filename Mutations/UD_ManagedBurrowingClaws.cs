using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using  static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    public class UD_ManagedBurrowingClaws : BurrowingClaws, IManagedDefaultNaturalWeapon
    {
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {
            public override string GetColoredAdjective()
            {
                return GetAdjective().OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
            }
        }

        public INaturalWeapon NaturalWeapon = new()
        {
            Level = 1,
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

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
        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        public virtual string GetNaturalWeaponMod(bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
        }

        public virtual bool CalculateNaturalWeaponLevel(int Level = 1)
        {
            NaturalWeapon.Level = Level;
            return true;
        }

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

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1) 
        {
            NaturalWeapon.DamageDieCount = GetNaturalWeaponDamageDieCount(Level);
            return true; 
        }
        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            NaturalWeapon.DamageDieSize = GetNaturalWeaponDamageDieSize(Level);
            return true;
        }
        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1) 
        {
            NaturalWeapon.DamageBonus = GetNaturalWeaponDamageBonus(Level);
            return true; 
        }
        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1) 
        {
            NaturalWeapon.HitBonus = GetNaturalWeaponHitBonus(Level);
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(string Parts)
        {
            if (Parts == null) return false;
            NaturalWeapon.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeapon.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeapon.AddedStringProps = StringProps;
                NaturalWeapon.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(int Level = 1)
        {
            return NaturalWeapon.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(int Level = 1)
        {
            if (HasGigantism)
            {
                return 3;
            }
            DieRoll baseDamage = new(GetClawsDamage(Level));
            return baseDamage.RightValue;
        }

        public virtual int GetNaturalWeaponDamageBonus(int Level = 1)
        {
            return NaturalWeapon.DamageBonus;
        }

        public virtual int GetNaturalWeaponHitBonus(int Level = 1)
        {
            return NaturalWeapon.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts()
        {
            return NaturalWeapon.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps()
        {
            return NaturalWeapon.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps()
        {
            return NaturalWeapon.AddedIntProps;
        }

        public virtual string GetNaturalWeaponEquipmentFrameColors()
        {
            return NaturalWeapon.EquipmentFrameColors;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponLevel(NewLevel);
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

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod(), Actor: ParentObject);

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
            burrowingClaws.NaturalWeapon = null;
            return burrowingClaws;
        }
    }
}
