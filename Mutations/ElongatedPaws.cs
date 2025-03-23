using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws : BaseManagedDefaultEquipmentMutation<ElongatedPaws>
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };

        public int StrengthModifier => ParentObject.StatMod("Strength");
        public int AgilityModifier => ParentObject.StatMod("Agility");

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}"; //.OptionalColorGiant(Colorfulness);
            Type = "Physical";

            NaturalWeaponSubpart = new()
            {
                ParentPart = this,
                Level = Level,
                CosmeticOnly = false,
                Type = "Hand",
                DamageDieSize = 3,
                ModPriority = 20,
                Adjective = ("elongated", 20),
                AdjectiveColor = "giant",
                AdjectiveColorFallback = "w",
                Noun = "paw",
                Skill = "ShortBlades",
                Stat = "Strength",
                Tile = "NaturalWeapons/ElongatedPaw.png",
                ColorString = "&x",
                DetailColor = "z",
                SecondColorString = "&X",
                SecondDetailColor = "Z",
                SwingSound = "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing",
                BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_longBlade_saltHopperMandible_blocked"
            };
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

        public override int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<ElongatedPaws> NaturalWeaponSubpart, int Level = 1)
        {
            int dieSize = 0;
            
            if (!HasGigantism) dieSize++;
            if (!HasBurrowing) dieSize++;

            return dieSize;
        }

        public override int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<ElongatedPaws> NaturalWeaponSubpart, int Level = 1)
        {
            return (int)Math.Floor(StrengthModifier / 2.0);
        }

        public override bool CanLevel() { return false; }

        public override bool AllowStaticRegistration() { return true; }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have {{giant|elongated paws}}, which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons {{gigantic|a size bigger}} than you are as though they were your size."
                 + "\n\nYour {{giant|elongated paws}} count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|(StrMod/2)}}"
                 + "\n\n+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

        public override string GetLevelText(int Level) { return ""; }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == StatChangeEvent.ID;
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            if (Array.IndexOf(AffectedSlotTypes, E.SlotType) >= 0 && E.Actor == ParentObject)
            {
                E.Decreases++;
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Name == "Strength") // || E.Name == "Agility")
            {
                Body body = E.Object.Body;

                NaturalWeaponSubpart.DamageBonus = GetNaturalWeaponDamageBonus(NaturalWeaponSubpart, Level);

                body?.UpdateBodyParts();


                foreach (GameObject equipped in body.GetEquippedObjects())
                {
                    if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                    {
                        weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                    }
                }

            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            GO.CheckAffectedEquipmentSlots();
            
            return base.Mutate(GO, Level);
        }

        public override void AfterMutate()
        {
            GameObject GO = ParentObject;
            foreach (GameObject equipped in GO.Body.GetEquippedObjects())
            {
                if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                {
                    weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                }
            }
            base.AfterMutate();
        }

        public override bool Unmutate(GameObject GO)
        {
            foreach (GameObject equipped in GO.Body.GetEquippedObjects())
            {
                if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                {
                    weaponElongator.UnapplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                }
            }

            GO.CheckAffectedEquipmentSlots();

            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            base.OnRegenerateDefaultEquipment(body);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ElongatedPaws elongatedPaws = base.DeepCopy(Parent, MapInv) as ElongatedPaws;
            elongatedPaws.NaturalWeaponSubparts = new();
            foreach ((_, NaturalWeaponSubpart<ElongatedPaws> subpart) in NaturalWeaponSubparts)
            {
                elongatedPaws.NaturalWeaponSubparts.Add(subpart.Type, new(subpart, elongatedPaws));
            }
            elongatedPaws.NaturalWeaponSubpart = new(NaturalWeaponSubpart, elongatedPaws);
            return elongatedPaws;
        }

    } //!-- public class ElongatedPaws : BaseDefaultEquipmentMutation

} //!-- namespace XRL.World.Parts.Mutation
