using HarmonyLib;

using System;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Effects;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class CookingDomainSpecial_UnitCrystalTransform_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(CookingDomainSpecial_UnitCrystalTransform_Patches));

        // Calls UpdateBodyParts on recently transformed crytalline beings to ensure they get their funky new manipulators.
        [HarmonyPatch(
            declaringType: typeof(CookingDomainSpecial_UnitCrystalTransform), 
            methodName: nameof(CookingDomainSpecial_UnitCrystalTransform.ApplyTo),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        static void ApplyTo_CallUpdateBodyParts_Postfix(GameObject Object)
        {
            Object?.Body?.UpdateBodyParts();
        }
    } //!-- public static class CookingDomainSpecial_UnitCrystalTransform_Patches
 }
