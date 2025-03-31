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

            NaturalEquipmentMod = new ModElongatedNaturalWeapon()
            {
                AssigningPart = this,
                BodyPartType = "Hand",

                ModPriority = 20,
                DescriptionPriority = 20,

                Adjective = "elongated",
                AdjectiveColor = "giant",
                AdjectiveColorFallback = "w",

                Adjustments = new(),

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_longBlade_saltHopperMandible_blocked" }
                },
            };
            NaturalEquipmentMod.AddAdjustment(MELEEWEAPON, "Skill", "ShortBlades", true);
            NaturalEquipmentMod.AddAdjustment(MELEEWEAPON, "Stat", "Strength", true);

            NaturalEquipmentMod.AddAdjustment(RENDER, "DisplayName", "paw", true);

            NaturalEquipmentMod.AddAdjustment(RENDER, "Tile", "NaturalWeapons/ElongatedPaw.png", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "ColorString", "&x", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "TileColor", "&x", true);
            NaturalEquipmentMod.AddAdjustment(RENDER, "DetailColor", "z", true);
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

        public override int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod, int Level = 1)
        {
            int dieSize = 0;
            
            if (!HasGigantism) dieSize++;
            if (!HasBurrowing) dieSize++;

            return dieSize;
        }

        public override int GetNaturalWeaponPenBonus(ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod, int Level = 1)
        {
            return (int)Math.Floor(AgilityModifier / 2.0);
        }

        public override bool CanLevel() { return false; }

        public override bool AllowStaticRegistration() { return true; }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have " + "elongated paws".OptionalColorGiant(Colorfulness) + ", which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons " + "size bigger".OptionalColorGiant(Colorfulness) + " than you are as though they were your size."
                 + "\n\nYour " + "elongated paws".OptionalColorGiant(Colorfulness) + " count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|(StrMod/2)}}"
                 + "\n\n+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

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

                NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);

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
            elongatedPaws.NaturalEquipmentMods = new();
            foreach ((_, ModNaturalEquipment<ElongatedPaws> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                elongatedPaws.NaturalEquipmentMods.Add(naturalEquipmentMod.BodyPartType, new(naturalEquipmentMod, elongatedPaws));
            }
            elongatedPaws.NaturalEquipmentMod = new(NaturalEquipmentMod, elongatedPaws);
            return elongatedPaws;
        }

    } //!-- public class ElongatedPaws : BaseDefaultEquipmentMutation

} //!-- namespace XRL.World.Parts.Mutation
