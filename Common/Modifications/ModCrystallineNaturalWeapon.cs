using System;
using System.Collections.Generic;
using System.Text;

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
    public class ModCrystallineNaturalWeapon : ModNaturalEquipment<UD_ManagedCrystallinity>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModCrystallineNaturalWeapon));

        public static ConditionsAll<GameObject> ReturnsAreDiminished => new()
        {
            WielderHasGigantismPlus,
            new ConditionsAny<GameObject>()
            {
                WielderHasElongatedPaws,
                WielderHasBurrowingClaws,
            },
        };

        public ModCrystallineNaturalWeapon()
        {
            BodyPartType = "Hand";

            ModPriority = 100;
            DescriptionPriority = 100;

            ForceNoun = true;
            Noun = "point";

            Adjective = "crystalline";
            AdjectiveColor = "crystallized";
            AdjectiveColorFallback = "M";

            AdjustMeleeSkill("ShortBlades", true);

            AdjustNoun(true);
            AdjustTile("Creatures/natural-weapon-claw.bmp", true);

            AdjustColorString("&b", true);
            AdjustTileColor("&b", true);
            AdjustDetailColor("B", true);

            AddAdjustment(new AddPartInorganic(), false); // Use this instead of AddPart<Inorganic>() to get the DescriptionElement

            SetSwingSound("Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing", true);
            SetBlockedSound("Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked", true);

            AddDiminishingReturnsDescription("increases to damage die size and damage bonus", Condition: ReturnsAreDiminished);
        }
        public ModCrystallineNaturalWeapon(NaturalEquipmentManager NewManager)
            : base(NewManager)
        {
        }

    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}