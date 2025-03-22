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
        // Dictionary holds a BodyPart.Type string as Key, and NaturalWeaponSubpart for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalWeaponSubpart.Type).
        public Dictionary<string, NaturalWeaponSubpart<UD_ManagedBurrowingClaws>> NaturalWeaponSubparts { get; set; }
        public NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart { get; set; }

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
            NaturalWeaponSubparts = new();

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

        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<UD_ManagedBurrowingClaws> GetNaturalWeaponMod(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, bool Managed = true)
        {
            ModNaturalWeaponBase<UD_ManagedBurrowingClaws> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod(Managed);
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = this;
            NaturalWeaponMod.Wielder = ParentObject;
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

        public virtual int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
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

        public virtual int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasCrystallinity))
            {
                return 1;
            }
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<UD_ManagedBurrowingClaws> Subpart, int Level)
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
            foreach ((_, NaturalWeaponSubpart<UD_ManagedBurrowingClaws> Subpart) in NaturalWeaponSubparts)
            {
                UpdateNaturalWeaponSubpart(Subpart, NewLevel);
            }
            if (NaturalWeaponSubpart != null) UpdateNaturalWeaponSubpart(NaturalWeaponSubpart, NewLevel);
            return base.ChangeLevel(NewLevel);
        }
        public virtual bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false)
        {
            Debug.Entry(4,
                $"@ {typeof(UD_ManagedBurrowingClaws).Name}."
                + $"{nameof(ProcessNaturalWeaponSubparts)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{part.Description} [{part.ID}:{part.Type}]", Indent: 2);
                    ModNaturalWeaponBase<UD_ManagedBurrowingClaws> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null
                        && part.Type == NaturalWeaponSubpart.Type
                        && NaturalWeaponSubpart.IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                        Debug.Entry(4, $"NaturalWeaponSubpart", Indent: 3);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type)
                        && NaturalWeaponSubparts[part.Type].IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                        Debug.Entry(4, $"NaturalWeaponSubparts", Indent: 3);
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

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (body != null) 
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: false);
            // skip base
        }

        public override void OnDecorateDefaultEquipment(Body body)
        {
            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: true);

            base.OnDecorateDefaultEquipment(body);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedBurrowingClaws managedBurrowingClaws = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;
            managedBurrowingClaws.NaturalWeaponSubparts = new();
            foreach ((_, NaturalWeaponSubpart<UD_ManagedBurrowingClaws> subpart) in NaturalWeaponSubparts)
            {
                managedBurrowingClaws.NaturalWeaponSubparts.Add(subpart.Type, new(subpart, managedBurrowingClaws));
            }
            managedBurrowingClaws.NaturalWeaponSubpart = new(NaturalWeaponSubpart, managedBurrowingClaws);
            return managedBurrowingClaws;
        }
    }
}
