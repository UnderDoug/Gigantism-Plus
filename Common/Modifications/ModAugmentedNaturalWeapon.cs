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
            : base()
        {
            BodyPartType = "Hand";

            ModPriority = -500;
            DescriptionPriority = -500;

            ForceNoun = true;
            Noun = "manipulator";

            Adjective = "augmented";
            AdjectiveColorFallback = "c";

            string cyberneticsObject = AssigningPart?.ImplantObject?.GetDisplayName(Short: true, AsIfKnown: true);
            AddSecondaryDescription(new("have", $"some of =subject.possessive= bonuses applied by an implanted {cyberneticsObject}"));
        }
        public ModAugmentedNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
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

        public override bool HandleEvent(BeforeApplyAdjustmentEvent E)
        {
            if (E.Adjustment.Source == typeof(ModClosedGiganticNaturalWeapon) 
                && E.Adjustment.GetType() == typeof(ChangeTile) 
                && E.Subject is GameObject equipment)
            {
                int indent = Debug.LastIndent;
                Debug.Entry(4, 
                    $"{nameof(ModAugmentedNaturalWeapon)}: " +
                    $"Replaced {nameof(ModClosedGiganticNaturalWeapon)} {nameof(ChangeTile)} {nameof(E.Adjustment)}",
                    Indent: indent + 1, Toggle: doDebug);

                foreach (IAdjustment adjustment in Adjustments)
                {
                    if (adjustment.GetType() == typeof(ChangeTile) && adjustment.Check(equipment))
                    {
                        Debug.Entry(4, "Appropriate Adjustment found, code execution skipped...", Indent:  indent + 2, Toggle: doDebug);
                        // E.Adjustment.Value = adjustment.Value;
                        break;
                    }
                }
                Debug.LastIndent = indent;
            }
            return base.HandleEvent(E);
        }
    } 
}