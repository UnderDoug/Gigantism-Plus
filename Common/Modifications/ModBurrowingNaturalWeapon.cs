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
            : base()
        {
            BodyPartType = "Hand";

            ModPriority = 80;
            DescriptionPriority = 80;

            ForceNoun = true;
            Noun = "claw";

            Adjective = "burrowing";
            AdjectiveColor = "W";
            AdjectiveColorFallback = "y";
        }
        public ModBurrowingNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }

    }
}