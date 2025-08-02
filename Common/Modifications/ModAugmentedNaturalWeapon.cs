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
    public class ModAugmentedNaturalWeapon : ModNaturalEquipment<CyberneticsGiganticExoframe>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModAugmentedNaturalWeapon));

        public ModAugmentedNaturalWeapon()
        {
        }
        public ModAugmentedNaturalWeapon(NaturalEquipmentManager NewManager)
            : base(NewManager)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            if (Operator != null)
            {
                Operator.DoDynamicTile = false;
            }
            base.ApplyModification(Object);
        }

        public override string GetColoredAdjective()
        {
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Colorfulness) ?? base.GetColoredAdjective();
        }
        public override string GetAdjective()
        {
            return AssigningPart?.GetNaturalEquipmentColoredAdjective(Colorfulness: 1).Strip() ?? base.GetAdjective();
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<CyberneticsGiganticExoframe>> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                string cyberneticsObject = AssigningPart?.ImplantObject?.ShortDisplayName;

                E.BeforeEvent.ClearDescriptionElements();
                E.BeforeEvent.AddGeneralElement("have", $"some of {E.Object.its} bonuses applied by an implanted {cyberneticsObject}");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeApplyPartAdjustmentEvent E)
        {
            if (E.NaturalEquipmentMod == nameof(ModClosedGiganticNaturalWeapon) && E.Adjustment.Target == RENDER && E.Adjustment.Field == "Tile" && E.Subject is GameObject equipment)
            {
                Debug.Entry(4, $"Replaced {nameof(ModClosedGiganticNaturalWeapon)} {E.Adjustment.Field} PartAdjustment", 
                    Indent: Debug.LastIndent + 1, Toggle: doDebug);
                Debug.LastIndent--;

                foreach (PartAdjustment adjustment in PartAdjustments)
                {
                    if (adjustment.HasSameTargetAs(E.Adjustment))
                    {
                        if (adjustment.Check(equipment))
                        {
                            E.Adjustment.SetValue(adjustment.Value);
                        }
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(BeforeApplyAdjustmentEvent E)
        {
            if (E.Adjustment.Source == typeof(ModClosedGiganticNaturalWeapon) && E.Adjustment.GetType() == typeof(ChangeTile) && E.Subject is GameObject equipment)
            {
                Debug.Entry(4, $"Replaced {nameof(ModClosedGiganticNaturalWeapon)} {nameof(ChangeTile)} {nameof(E.Adjustment)}",
                    Indent: Debug.LastIndent + 1, Toggle: doDebug);
                Debug.LastIndent--;

                foreach (IAdjustment adjustment in Adjustments)
                {
                    if (adjustment.Check(equipment))
                    {
                        E.Adjustment.Value = adjustment.Value;
                    }
                }
            }
            return base.HandleEvent(E);
        }
    } 
}