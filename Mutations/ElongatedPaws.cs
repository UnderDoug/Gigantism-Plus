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
    public class ElongatedPaws : BaseManagedDefaultEquipmentMutation
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };

        public int StrengthModifier => ParentObject.StatMod("Strength");
        public int AgilityModifier => ParentObject.StatMod("Agility");

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}"; //.OptionalColorGiant(Colorfulness);
            Type = "Physical";

            NaturalWeapon = new()
            {
                Level = 1,
                DamageDieCount = 1,
                DamageDieSize = 3,
                DamageBonus = 0,
                ModPriority = 20,
                Adjective = "elongated",
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

        public override int GetNaturalWeaponDamageDieSize(int Level = 1)
        {
            int dieSize = 0;
            
            if (!HasGigantism) dieSize++;
            if (!HasBurrowing) dieSize++;

            return dieSize;
        }

        public override int GetNaturalWeaponDamageBonus(int Level = 1)
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

                CalculateNaturalWeaponDamageBonus(Level);

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
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(ElongatedPaws)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
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

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod<ElongatedPaws>(), Actor: ParentObject);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);

            Exit:
            Debug.Entry(4, $"* base.{nameof(OnRegenerateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(3, $"{nameof(ElongatedPaws)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            base.OnRegenerateDefaultEquipment(body);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ElongatedPaws elongatedPaws = base.DeepCopy(Parent, MapInv) as ElongatedPaws;
            elongatedPaws.NaturalWeapon = new INaturalWeapon(NaturalWeapon);
            return elongatedPaws;
        }

    } //!-- public class ElongatedPaws : BaseDefaultEquipmentMutation

} //!-- namespace XRL.World.Parts.Mutation
