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

        public ModClosedGiganticNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            if (ParentObject.TryGetPart(out NaturalEquipmentManager manager))
            {
                manager.DoDynamicTile = false;
            }
            base.ApplyModification(Object);
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<GigantismPlus>> E)
        {
            if (E.Adjective == GetColoredAdjective())
            {
                E.BeforeEvent.ClearDescriptionElements();

                E.BeforeEvent.AddGeneralElement(null, $"tightly clenched");
                E.BeforeEvent.AddGeneralElement("", $"functions as a cudgel");
                if (ParentObject.HasPart<ModElongatedNaturalWeapon>())
                {
                    E.AddWeaponElement("get", $"bonus penetration from Strength");
                }
            }
            return base.HandleEvent(E);
        }

    } //!-- public class ModClosedGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}