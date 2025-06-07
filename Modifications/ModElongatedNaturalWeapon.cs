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
    public class ModElongatedNaturalWeapon : ModNaturalEquipment<ElongatedPaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModElongatedNaturalWeapon));

        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            E.BeforeEvent.ClearDescriptionElements();

            int dieSize = GetDamageDieSize();
            int damageBonus = GetDamageBonus();

            if (dieSize > 0 && (!AssigningPart.HasGigantism || !AssigningPart.HasBurrowing))
            {
                E.AddWeaponElement("gain", $"{dieSize.Signed()} damage die size");
            }
            if (damageBonus != 0)
            {
                E.AddWeaponElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
            }
            E.AddWeaponElement("", $"{E.Object.its} bonus damage scales by half {E.Object.its} wielder's Strength Modifier");

            return base.HandleEvent(E);
        }

    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}