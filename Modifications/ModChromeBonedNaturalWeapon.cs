using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using UnityEngine.UIElements;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModChromeBonedNaturalWeapon : ModNaturalEquipment<CyberneticsManagedHandBones>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModChromeBonedNaturalWeapon));

        public ModChromeBonedNaturalWeapon()
        {
        }

        public ModChromeBonedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override string GetColoredAdjective()
        {
            bool inorganic = ParentObject != null && !ParentObject.IsOrganic;
            bool metalic = ParentObject != null && ParentObject.HasPart<Metal>() || Manager.ParentLimb.Category.HasBit(BodyPartCategory.METAL);
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Inorganic: inorganic, Metalic: metalic) ?? base.GetColoredAdjective();
        }
        public override string GetAdjective()
        {
            bool inorganic = ParentObject != null && !ParentObject.IsOrganic;
            bool metalic = ParentObject != null && ParentObject.HasPart<Metal>() || Manager.ParentLimb.Category.HasBit(BodyPartCategory.METAL);
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Colorfulness: 1, Inorganic: inorganic, Metalic: metalic).Strip() ?? base.GetAdjective();
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<CyberneticsManagedHandBones>> E)
        {
            if (Wielder.HasPart<GigantismPlus>())
            {
                E.AddGeneralElement(null, "suffering diminishing returns on increases to damage die count");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeApplyPartAdjustmentEvent E)
        {
            if (E.NaturalEquipmentMod == nameof(ModGiganticNaturalWeapon) && E.Target == RENDER && E.Field == "DetailColor")
            {
                Debug.Entry(4, $"Replaced {nameof(ModGiganticNaturalWeapon)} {E.Field} Adjustment",
                    Indent: Debug.LastIndent + 1, Toggle: doDebug);
                Debug.LastIndent--;

                foreach (PartAdjustment adjustment in Adjustments)
                {
                    if (adjustment.Field == E.Field && adjustment.Target == E.Target)
                    {
                        // E.Value = adjustment.Value;
                    }
                }
            }
            return base.HandleEvent(E);
        }
    } //!-- public class ModChromeBonedNaturalWeapon : ModNaturalWeaponBase<CyberneticsManagedHandBones>
}