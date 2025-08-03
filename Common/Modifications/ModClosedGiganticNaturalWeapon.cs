using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModClosedGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModClosedGiganticNaturalWeapon));

        public ModClosedGiganticNaturalWeapon()
        {
        }
        public ModClosedGiganticNaturalWeapon(NaturalEquipmentManager NewManager)
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

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<GigantismPlus>> E)
        {
            if (E.Adjective == GetColoredAdjective() && E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                E.BeforeEvent.ClearDescriptionElements();

                E.BeforeEvent.AddSecondaryElement(null, $"tightly clenched");
                E.BeforeEvent.AddSecondaryElement("", $"functions as a cudgel");
                if (ParentObject.HasPart<ModElongatedNaturalWeapon>())
                {
                    E.AddPrimaryElement("get", $"bonus penetration from Strength");
                }
            }
            return base.HandleEvent(E);
        }

    }
}