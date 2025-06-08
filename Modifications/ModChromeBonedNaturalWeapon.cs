using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using UnityEngine.UIElements;

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
            return AssigningPart?.GetNaturalEquipmentColoredAdjective() ?? base.GetColoredAdjective();
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