using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.NaturalEquipmentConditions;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModElongatedNaturalWeapon : ModNaturalEquipment<ElongatedPaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModElongatedNaturalWeapon));

        public ModElongatedNaturalWeapon()
        {
            BodyPartType = "Hand";

            ModPriority = 60;
            DescriptionPriority = 60;

            Noun = "paw";

            Adjective = "elongated";
            AdjectiveColor = "giant";
            AdjectiveColorFallback = "w";

            AddSkillAdjustment("ShortBlades", true);
            AddMeleeStatAdjustment("Agility", -120);

            AddNounAdjustment(true, Condition: IsOrganicFist);

            AddTileAdjustment("NaturalWeapons/ElongatedPaw.png", true, Condition: IsOrganicFist);
            AddColorStringAdjustment("&Z", true);
            AddTileColorAdjustment("&Z", true);
            AddDetailColorAdjustment("z", true);

            AnyConditions<GameObject> arbitraryAnyConditions = ThisModAdjustsMeleeCumulatively<ModElongatedNaturalWeapon>();

            string arbitraryEffect = $"=subject.possessive= bonus damage scale by half =subject.possessive= wielder's {ElongatedPaws.SCALE_STAT} Modifier";
            DescriptionElement emptyElement = DescriptionElement.Empty;
            ArbitraryDescription arbitraryCumulativeMeleeDescription = new(emptyElement, new("", arbitraryEffect))
            {
                Condition = arbitraryAnyConditions,
            };
            AddAdjustment(arbitraryCumulativeMeleeDescription, true);

            ArbitraryDescription arbitraryNoCumulativeMeleeDescription = new(emptyElement, new("have", arbitraryEffect))
            {
                Condition = new NotAnyConditions<GameObject>(arbitraryAnyConditions),
            };
            AddAdjustment(arbitraryNoCumulativeMeleeDescription, true);

            DiminishingReturns diminishingReturns = new("increases to damage die size")
            {
                Condition = new AnyConditions<GameObject>() { IsGigantic, IsBurrowing },
            };
            AddAdjustment(diminishingReturns, true);
            AddAdjustment(new SetSwingSound("Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing"), true, IsOrganicFist);
            AddAdjustment(new SetBlockedSound("Sounds/Melee/multiUseBlock/sfx_melee_longBlade_saltHopperMandible_blocked"), true, IsOrganicFist);
        }
        public ModElongatedNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }

        public override bool HandleEvent(BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            if (EnablePrereleaseContent && E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                string scalingStat = ElongatedPaws.SCALE_STAT;
                if (E.PrimaryDescriptions.IsNullOrEmpty())
                {
                    // E.AddWeaponElement("have", $"{E.Object.its} bonus damage scale by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                else
                {
                    // E.AddWeaponElement("", $"{E.Object.its} bonus damage scales by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                if (AssigningPart.HasGigantism || AssigningPart.HasBurrowing)
                {
                    // E.AddGeneralElement(null, "suffering diminishing returns on increases to damage die size");
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            if (!EnablePrereleaseContent && E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                string scalingStat = ElongatedPaws.SCALE_STAT;
                BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> D = E.BeforeEvent;
                D.ClearDescriptionElements();
                int dieSize = GetDamageDieSize();
                int damageBonus = GetDamageBonus();

                if (E.Object.TryGetPart(out MeleeWeapon meleeWeapon) && meleeWeapon.Stat == scalingStat)
                {
                    D.AddPrimaryElement("get", $"bonus penetration from {scalingStat}");
                }
                if (dieSize > 0 && (!AssigningPart.HasGigantism || !AssigningPart.HasBurrowing))
                {
                    D.AddPrimaryElement("gain", $"{dieSize.Signed()} damage die size");
                }
                if (damageBonus != 0)
                {
                    D.AddPrimaryElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (E.BeforeEvent.PrimaryDescriptions.IsNullOrEmpty())
                {
                    D.AddPrimaryElement("have", $"{E.Object.its} bonus damage scale by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                else
                {
                    D.AddPrimaryElement("", $"{E.Object.its} bonus damage scales by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                if (AssigningPart.HasGigantism || AssigningPart.HasBurrowing)
                {
                    D.AddSecondaryElement(null, "suffering diminishing returns on increases to damage die size");
                }
            }
            return base.HandleEvent(E);
        }

    }
}