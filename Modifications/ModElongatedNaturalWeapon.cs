using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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

            Adjustments = new();

            AddedStringProps = new()
            {
                { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_longBlade_saltHopperMandible_blocked" }
            };
            AddSkillAdjustment("ShortBlades", true);
            AddStatAdjustment("Agility", -120);

            static bool cosmeticCondition(GameObject Equipment)
            {
                return Equipment?.Blueprint != null
                    && Equipment.Blueprint == "DefaultFist";
            };
            AddNounAdjustment(true, Condition: cosmeticCondition);

            AddTileAdjustment("NaturalWeapons/ElongatedPaw.png", true, Condition: cosmeticCondition);
            AddColorStringAdjustment("&Z", true);
            AddTileColorAdjustment("&Z", true);
            AddDetailColorAdjustment("z", true);
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                E.BeforeEvent.ClearDescriptionElements();
                string scalingStat = ElongatedPaws.SCALE_STAT;
                int dieSize = GetDamageDieSize();
                int damageBonus = GetDamageBonus();

                if (E.Object.TryGetPart(out MeleeWeapon meleeWeapon) && meleeWeapon.Stat == scalingStat)
                {
                    E.AddWeaponElement("get", $"bonus penetration from {scalingStat}");
                }
                if (dieSize > 0 && (!AssigningPart.HasGigantism || !AssigningPart.HasBurrowing))
                {
                    E.AddWeaponElement("gain", $"{dieSize.Signed()} damage die size");
                }
                if (damageBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (E.WeaponDescriptions.IsNullOrEmpty())
                {
                    E.AddWeaponElement("have", $"{E.Object.its} bonus damage scale by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                else
                {
                    E.AddWeaponElement("", $"{E.Object.its} bonus damage scales by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                if (AssigningPart.HasGigantism || AssigningPart.HasBurrowing)
                {
                    E.AddGeneralElement(null, "suffering diminishing returns on increases to damage die size");
                }
            }
            return base.HandleEvent(E);
        }

    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}