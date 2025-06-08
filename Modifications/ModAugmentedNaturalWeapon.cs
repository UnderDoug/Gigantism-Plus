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
    public class ModAugmentedNaturalWeapon : ModNaturalEquipment<CyberneticsGiganticExoframe>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModAugmentedNaturalWeapon));

        public ModAugmentedNaturalWeapon()
        {
        }

        public ModAugmentedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            if(ParentObject.TryGetPart(out NaturalEquipmentManager manager))
            {
                manager.DoDynamicTile = false;
            }
            base.ApplyModification(Object);
        }

        public override string GetColoredAdjective()
        {
            return AssigningPart?.GetNaturalEquipmentColoredAdjective() ?? base.GetColoredAdjective();
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<CyberneticsGiganticExoframe>> E)
        {
            string cyberneticsObject = AssigningPart?.ImplantObject?.ShortDisplayName;

            E.BeforeEvent.ClearDescriptionElements();
            E.BeforeEvent.AddGeneralElement("have", $"some of {E.Object.its} bonuses applied by an implanted {cyberneticsObject}");

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeApplyPartAdjustmentEvent E)
        {
            if (E.NaturalEquipmentMod == nameof(ModClosedGiganticNaturalWeapon) && E.Target == RENDER && E.Field == "Tile")
            {
                Debug.Entry(4, $"Replaced {nameof(ModClosedGiganticNaturalWeapon)} {E.Field} Adjustment", 
                    Indent: Debug.LastIndent + 1, Toggle: doDebug);
                Debug.LastIndent--;

                foreach (PartAdjustment adjustment in Adjustments)
                {
                    if (adjustment.Field == E.Field && adjustment.Target == E.Target)
                    {
                        E.Value = adjustment.Value;
                    }
                }
            }
            return base.HandleEvent(E);
        }
    } //!-- public class ModAugmentedNaturalWeapon : ModNaturalWeaponBase<CyberneticsGiganticExoframe>
}