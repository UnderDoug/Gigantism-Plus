using System;
using System.Collections.Generic;
using System.Linq;

using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws 
        : BaseManagedDefaultEquipmentMutation<ElongatedPaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ElongatedPaws));
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

        private static string[] AffectedSlotTypes => new string[3] { "Hand", "Hands", "Missile Weapon" };

        public static string SCALE_STAT => "Agility";
        public int ScaleStatModifier => ParentObject.StatMod(SCALE_STAT);
        public int StrengthModifier => ParentObject.StatMod("Strength");
        public int AgilityModifier => ParentObject.StatMod("Agility");

        private int StatChangeEventAttempts = 0; // This is in case UpdateBodyParts results in a stat change that causes a loop.

        public ElongatedPaws()
        {
        }

        public static ModElongatedNaturalWeapon NewElongatedWeaponMod(ElongatedPaws assigningPart)
        {
            ModElongatedNaturalWeapon elongatedNaturalWeaponMod = new()
            {
                AssigningPart = assigningPart,
                BodyPartType = "Hand",

                ModPriority = 60,
                DescriptionPriority = 60,

                Noun = "paw",

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
            elongatedNaturalWeaponMod.AddSkillAdjustment("ShortBlades", true);
            elongatedNaturalWeaponMod.AddStatAdjustment("Agility", -120);

            static bool cosmeticCondition(GameObject Equipment)
            {
                return Equipment?.Blueprint != null
                    && Equipment.Blueprint == "DefaultFist";
            };
            elongatedNaturalWeaponMod.AddNounAdjustment(true, Condition: cosmeticCondition);

            elongatedNaturalWeaponMod.AddTileAdjustment("NaturalWeapons/ElongatedPaw.png", true, Condition: cosmeticCondition);
            elongatedNaturalWeaponMod.AddColorStringAdjustment("&Z", true);
            elongatedNaturalWeaponMod.AddTileColorAdjustment("&Z", true);
            elongatedNaturalWeaponMod.AddDetailColorAdjustment("z", true);
            return elongatedNaturalWeaponMod;
        }
        public override ModNaturalEquipment<ElongatedPaws> GetNaturalEquipmentMod(Predicate<ModNaturalEquipment<ElongatedPaws>> Filter = null, ElongatedPaws NewAssigner = null)
        {
            ModNaturalEquipment<ElongatedPaws> naturalEquipmentMod = NewElongatedWeaponMod(NewAssigner ?? this);
            return Filter == null || Filter(naturalEquipmentMod) ? naturalEquipmentMod : base.GetNaturalEquipmentMod(Filter, NewAssigner);
        }

        public bool HasGigantism => ParentObject != null && ParentObject.HasPart<GigantismPlus>();
        public bool HasBurrowing => ParentObject != null && ParentObject.HasPartDescendedFrom<BurrowingClaws>();
        public bool HasCrystallinity => ParentObject != null && ParentObject.HasPartDescendedFrom<Crystallinity>();

        public override int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod, int Level = 1)
        {
            int dieSize = 0;
            
            if (!HasGigantism) dieSize++;
            if (!HasBurrowing) dieSize++;

            return dieSize;
        }

        public override int GetNaturalWeaponDamageBonus(ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod, int Level = 1)
        {
            return (int)Math.Floor(ScaleStatModifier / 2.0);
        }

        public override bool CanLevel() { return false; }

        public override bool AllowStaticRegistration() { return true; }

        protected override string GetBaseDisplayName()
        {
            return base.GetBaseDisplayName().OptionalColorGiant(Colorfulness);
        }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have " + "elongated paws".OptionalColorGiant(Colorfulness) + ", which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons " + "size bigger".OptionalColorGiant(Colorfulness) + " than you are as though they were your size."
                 + "\n\nYour " + "elongated paws".OptionalColorGiant(Colorfulness) + " count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|(StrMod/2)}}"
                 + "\n\n+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

        public override string GetLevelText(int Level)
        {
            return "";
        }

        public override void TurnTick(long TimeTick, int Amount)
        {
            StatChangeEventAttempts = 0;

            base.TurnTick(TimeTick, Amount);
        }
        public override bool WantTurnTick()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || (StatChangeEventAttempts < 3 && ID == StatChangeEvent.ID);
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
            if (E.Name == SCALE_STAT && StatChangeEventAttempts < 3)
            {
                Body body = E.Object?.Body;

                if (body != null && NaturalEquipmentMod != null)
                {
                    StatChangeEventAttempts++;
                    body.UpdateBodyParts();

                    foreach (GameObject equipped in body.GetEquippedObjects())
                    {
                        if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                        {
                            weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                        }
                    }
                }
            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            GO.CheckEquipmentSlots();
            
            return base.Mutate(GO, Level);
        }

        public override void AfterMutate()
        {
            GameObject GO = ParentObject;
            if (GO != null && GO.Body != null && !GO.Body.GetEquippedObjects().IsNullOrEmpty())
            {
                foreach (GameObject equipped in GO.Body.GetEquippedObjects())
                {
                    if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                    {
                        weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                    }
                }
            }
            base.AfterMutate();
        }

        public override bool Unmutate(GameObject GO)
        {
            if (GO != null && GO.Body != null && !GO.Body.GetEquippedObjects().IsNullOrEmpty())
            {
                foreach (GameObject equipped in GO.Body.GetEquippedObjects())
                {
                    if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                    {
                        weaponElongator.UnapplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                    }
                }

                GO.CheckEquipmentSlots();
            }

            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            base.OnRegenerateDefaultEquipment(body);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ElongatedPaws elongatedPaws = base.DeepCopy(Parent, MapInv) as ElongatedPaws;

            return elongatedPaws;
        }

    } //!-- public class ElongatedPaws : BaseManagedDefaultEquipmentMutation<ElongatedPaws>
}
