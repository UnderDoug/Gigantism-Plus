using System;
using System.Collections.Generic;
using System.Text;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalEquipment<UD_ManagedBurrowingClaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModBurrowingNaturalWeapon));

        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4,
                $"\u2666 {typeof(ModBurrowingNaturalWeapon).Name}." +
                $"{nameof(ApplyModification)}" +
                $"(GameObject Object)",
                Indent: 3, Toggle: doDebug);
            base.ApplyModification(Object);

            Object.RequirePart<BurrowingClawsProperties>();
            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = UD_ManagedBurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            burrowingClawsProperties.WallBonusPercentage = UD_ManagedBurrowingClaws.GetWallBonusPercentage(AssigningPart.Level);
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<UD_ManagedBurrowingClaws>> E)
        {
            int wallBonusPenetration = UD_ManagedBurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            int wallHitsRequired = UD_ManagedBurrowingClaws.GetWallHitsRequired(AssigningPart.Level, Wielder);

            if (wallBonusPenetration != 0)
            {
                E.BeforeEvent.AddWeaponElement("get", $"{wallBonusPenetration.Signed()} penetration vs. walls");
            }
            if (wallHitsRequired != 0)
            {
                E.AddGeneralElement("destroy", $"walls after {wallHitsRequired} penetrating hits");
            }
            if (AssigningPart.HasGigantism && !(AssigningPart.HasElongated || AssigningPart.HasCrystallinity))
            {
                E.AddGeneralElement(null, "suffering diminishing returns on increases to damage die size");
            }

            return base.HandleEvent(E);
        }
    } //!-- public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
}