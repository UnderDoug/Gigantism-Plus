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

            AdjustMeleeSkill("ShortBlades", true);
            AdjustMeleeStat("Agility", -120);

            AdjustNoun(true, Condition: IsOrganicFist);

            AdjustTile("NaturalWeapons/ElongatedPaw.png", true, Condition: IsOrganicFist);
            AdjustColorString("&Z", true);
            AdjustTileColor("&Z", true);
            AdjustDetailColor("z", true);

            DescriptionElement itScales = new(
                Priority: DescriptionElement.ORDER_ADJUST_EXTREMELY_LATE,
                Verb: "have",
                Effect: $"=subject.possessive= bonus damage scale by half =subject.possessive= wielder's {ElongatedPaws.SCALE_STAT} Modifier");
            AddAdjustment(
                Adjustment: new ArbitraryDescription(DescriptionElement.Empty, itScales),
                FlipPriority: true,
                Condition: ThisModAdjustsMeleeCumulatively<ElongatedPaws>());

            DescriptionElement hasItScale = new(
                Priority: DescriptionElement.ORDER_ADJUST_EXTREMELY_LATE,
                Verb: "have",
                Effect: $"=subject.possessive= bonus damage scale by half =subject.possessive= wielder's {ElongatedPaws.SCALE_STAT} Modifier");
            AddAdjustment(
                Adjustment: new ArbitraryDescription(DescriptionElement.Empty, hasItScale),
                FlipPriority: true,
                Condition: new ConditionNot<GameObject>(ThisModAdjustsMeleeCumulatively<ElongatedPaws>()));

            AddAdjustment(new DiminishingReturns("increases to damage die size"), true, new ConditionsAny<GameObject>() { IsGigantic, IsBurrowing });
            AddAdjustment(new SetSwingSound("Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing"), true, IsOrganicFist);
            AddAdjustment(new SetBlockedSound("Sounds/Melee/multiUseBlock/sfx_melee_longBlade_saltHopperMandible_blocked"), true, IsOrganicFist);
        }
        public ModElongatedNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }
    }
}