using System;
using System.Collections.Generic;
using System.Text;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.NaturalEquipmentConditions;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalEquipment<UD_ManagedBurrowingClaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModBurrowingNaturalWeapon));

        public static ConditionsAll<GameObject> ReturnsAreDiminished => new()
        {
            WielderHasGigantismPlus,
            new ConditionsNotAny<GameObject>()
            {
                WielderHasElongatedPaws,
                WielderHasCrystallinity,
            },
        };

        public ModBurrowingNaturalWeapon()
            : base()
        {
            BodyPartType = "Hand";

            ModPriority = 80;
            DescriptionPriority = 80;

            ForceNoun = true;
            Noun = "claw";

            Adjective = "burrowing";
            AdjectiveColor = "W";
            AdjectiveColorFallback = "y";

            AdjustNoun(true);

            AdjustMeleeSkill("ShortBlades", true);

            AdjustTile("Creatures/natural-weapon-claw.bmp", true);
            AdjustColorString("&w", true);
            AdjustTileColor("&w", true);
            AdjustDetailColor("W", true);

            AddPart<DiggingTool>(false);

            AddAdjustment(new SetSwingSound("Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing"), true);
            AddAdjustment(new SetBlockedSound("Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked"), true);

            AddDiminishingReturnsDescription("increases to damage die size", ReturnsAreDiminished);
        }
        public ModBurrowingNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }

    }
}