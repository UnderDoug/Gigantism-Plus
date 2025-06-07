using System;
using System.Collections.Generic;
using System.Text;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModCrystallineNaturalWeapon : ModNaturalEquipment<UD_ManagedCrystallinity>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModCrystallineNaturalWeapon));

        public ModCrystallineNaturalWeapon()
        {
        }

        public ModCrystallineNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<UD_ManagedCrystallinity>> E)
        {
            E.BeforeEvent.AddGeneralElement(null, $"inorganic");

            return base.HandleEvent(E);
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}