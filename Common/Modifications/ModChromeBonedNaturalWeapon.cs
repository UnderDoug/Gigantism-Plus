using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using static HNPS_GigantismPlus.NaturalEquipmentConditions;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModChromeBonedNaturalWeapon : ModNaturalEquipment<CyberneticsManagedHandBones>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModChromeBonedNaturalWeapon));

        public const string ADJ = "boned";
        public const string ALT_COLOR = "r";

        public const string ADJ_INORGANIC = "reinforced";
        public const string ALT_COLOR_INORGANIC = "K";

        public const string ADJ_METAL = "infused";
        public const string ALT_COLOR_METAL = "c";

        public ModChromeBonedNaturalWeapon()
        {
            BodyPartType = "Hand";

            ModPriority = 490;
            DescriptionPriority = 490;

            Adjective = ADJ;

            AddDiminishingReturnsDescription("increases to damage die count", IsGigantic);
        }
        public ModChromeBonedNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }

        public override string GetColoredAdjective()
        {
            bool inorganic = ParentObject != null 
                && !ParentObject.IsOrganic;

            bool metalic = ParentObject != null 
                && ParentObject.HasPart<Metal>() 
                || (Operator != null && Operator.ParentLimb.Category.HasBit(BodyPartCategory.METAL));

            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Inorganic: inorganic, Metalic: metalic) 
                ?? base.GetColoredAdjective();
        }
        public override string GetAdjective()
        {
            bool inorganic = ParentObject != null 
                && !ParentObject.IsOrganic;

            bool metalic = ParentObject != null
                && ParentObject.HasPart<Metal>()
                || (Operator != null && Operator.ParentLimb.Category.HasBit(BodyPartCategory.METAL));

            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Colorfulness: 1, Inorganic: inorganic, Metalic: metalic).Strip() 
                ?? base.GetAdjective();
        }
    }
}