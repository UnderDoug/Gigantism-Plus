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

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<CyberneticsManagedHandBones>> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT && (Wielder != null && Wielder.HasPart<GigantismPlus>()))
            {
                E.AddSecondaryElement(null, "suffering diminishing returns on increases to damage die count");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeApplyAdjustmentEvent E)
        {
            if (E.Adjustment.Source == typeof(ModGiganticNaturalWeapon) && E.Adjustment.GetType() == typeof(ChangeDetailColor))
            {
                Debug.Entry(4, $"Replaced {nameof(ModGiganticNaturalWeapon)} {nameof(ChangeDetailColor)} {nameof(E.Adjustment)}",
                    Indent: Debug.LastIndent + 1, Toggle: doDebug);
                Debug.LastIndent--;

                foreach (IAdjustment adjustment in Adjustments)
                {
                    if (adjustment.SameAs(E.Adjustment, false))
                    {
                        // E.Adjustment.Value = adjustment.Value;
                    }
                }
            }
            return base.HandleEvent(E);
        }
    }
}