using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModChromeBonedNaturalWeapon : ModNaturalEquipment<CyberneticsManagedHandBones>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModChromeBonedNaturalWeapon));

        public ModChromeBonedNaturalWeapon()
        {
        }
        public ModChromeBonedNaturalWeapon(NaturalEquipmentManager NewManager)
            : base(NewManager)
        {
        }

        public override string GetColoredAdjective()
        {
            bool inorganic = ParentObject != null && !ParentObject.IsOrganic;
            bool metalic = ParentObject != null && ParentObject.HasPart<Metal>() || Operator.ParentLimb.Category.HasBit(BodyPartCategory.METAL);
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Inorganic: inorganic, Metalic: metalic) ?? base.GetColoredAdjective();
        }
        public override string GetAdjective()
        {
            bool inorganic = ParentObject != null && !ParentObject.IsOrganic;
            bool metalic = ParentObject != null && ParentObject.HasPart<Metal>() || Operator.ParentLimb.Category.HasBit(BodyPartCategory.METAL);
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Colorfulness: 1, Inorganic: inorganic, Metalic: metalic).Strip() ?? base.GetAdjective();
        }
    }
}