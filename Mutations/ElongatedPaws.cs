using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws : BaseManagedDefaultEquipmentMutation
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };

        public int StrengthModifier => ParentObject.StatMod("Strength");

        public int ElongatedBonusDamage
        {
            get
            {
                Debug.Entry(4, $"@ ElongatedPaws.ElongatedBonusDamage", Indent: 4);
                Debug.Entry(4, $"Returning StrMod/2: {(int)Math.Floor(StrengthModifier / 2.0)}", Indent: 5);
                Debug.Entry(4, $"x ElongatedPaws.ElongatedBonusDamage >//", Indent: 4);
                return (int)Math.Floor(StrengthModifier / 2.0);
            }
        }

        public int ElongatedDieSizeBonus
        {
            get
            {
                Debug.Entry(4, $"@ ElongatedPaws.ElongatedDieSizeBonus", Indent: 4);
                int dieSize = 2;

                bool HasGigantism = ParentObject.HasPart<GigantismPlus>();
                bool HasBurrowing = ParentObject.HasPart<BurrowingClaws>();

                Debug.Entry(4, $"HasGigantism: {(HasGigantism ? "yeah" : "nah")}", Indent: 5);
                Debug.Entry(4, $"HasBurrowing: {(HasBurrowing ? "yeah" : "nah")}", Indent: 5);

                Debug.Entry(4, $"dieSize: {dieSize}", Indent: 4);

                dieSize += !HasGigantism ? 1 : 0;
                Debug.Entry(4, $"!HasGigantism? dieSize: {dieSize}", Indent: 4);
                dieSize += !HasBurrowing ? 1 : 0;
                Debug.Entry(4, $"!HasBurrowing? dieSize: {dieSize}", Indent: 4);

                Debug.Entry(4, $"Final dieSize: {dieSize}", Indent: 4);
                Debug.Entry(4, $"x ElongatedPaws.ElongatedDieSizeBonus >//", Indent: 4);
                return dieSize;
            }
        }

        public ElongatedPaws()
        {
            DisplayName = "Elongated Paws".Color("giant");
            Type = "Physical";

            NaturalWeapon = new()
            {
                DamageDieCount = 1,
                DamageDieSize = 3,
                DamageBonus = 0,
                ModPriority = 20,
                Adjective = "elongated",
                AdjectiveColor = "giant",
                Noun = "paw",
                Skill = "ShortBlades",
                Stat = "Strength",
                Tile = "NaturalWeapons/ElongatedPaw.png",
                RenderColorString = "&x",
                RenderDetailColor = "z",
                SecondRenderColorString = "&X",
                SecondRenderDetailColor = "Z"
            };
        }

        public override bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            NaturalWeapon.DamageBonus = ElongatedDieSizeBonus;
            return base.CalculateNaturalWeaponDamageDieSize(Level);
        }
        public override bool CalculateNaturalWeaponDamageBonus(int Level = 1)
        {
            NaturalWeapon.DamageBonus = ElongatedBonusDamage;
            return base.CalculateNaturalWeaponDamageBonus(Level);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ElongatedPaws elongatedPaws = base.DeepCopy(Parent, MapInv) as ElongatedPaws;
            elongatedPaws.NaturalWeapon = null;
            return elongatedPaws;
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
            if (E.Name == "Strength")
            {
                Body body = E.Object.Body;

                CalculateNaturalWeaponDamageBonus(Level);

                body?.UpdateBodyParts();


                foreach (GameObject equipped in body.GetEquippedObjects())
                {
                    if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                    {
                        weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>(), E.Object);
                    }
                }

            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {

            foreach (GameObject equipped in GO.Body.GetEquippedObjects())
            {
                if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                {
                    weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>(), GO);
                }
            }

            return base.Mutate(GO, Level);
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

            CheckAffected(GO, GO.Body);

            return base.Unmutate(GO);
        }

        public void CheckAffected(GameObject Actor, Body Body)
        {
            if (Actor == null || Body == null)
            {
                return;
            }
            List<GameObject> list = Event.NewGameObjectList();
            foreach (BodyPart item in Body.LoopParts())
            {
                if (Array.IndexOf(AffectedSlotTypes, item.Type) < 0)
                {
                    continue;
                }
                GameObject equipped = item.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    list.Add(equipped);
                    int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                    int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, item.Type);
                    if (partCountEquippedOn != slotsRequiredFor && item.TryUnequip(Silent: true, SemiForced: true) && partCountEquippedOn > slotsRequiredFor)
                    {
                        equipped.SplitFromStack();
                        item.Equip(equipped, 0, Silent: true, ForDeepCopy: false, Forced: false, SemiForced: true);
                    }
                }
            }
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, "ElongatedPaws", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
                Debug.Footer(3, "ElongatedPaws", $"OnRegenerateDefaultEquipment(body)");
                base.OnRegenerateDefaultEquipment(body);
                return;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod(), Actor: ParentObject);

                    Debug.DiveOut(4, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) ]//", Indent: 1);

            Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
            Debug.Footer(3, "ElongatedPaws", $"OnRegenerateDefaultEquipment(body)");
            base.OnRegenerateDefaultEquipment(body);
        }

    } //!-- public class ElongatedPaws : BaseDefaultEquipmentMutation

} //!-- namespace XRL.World.Parts.Mutation
