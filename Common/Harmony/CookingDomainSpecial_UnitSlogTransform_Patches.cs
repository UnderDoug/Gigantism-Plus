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
    public static class CookingDomainSpecial_UnitSlogTransform_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(CookingDomainSpecial_UnitSlogTransform_Patches));

        // Calls UpdateBodyParts on recently transformed slog-bods to ensure they get any natural equipment adjustments they might be due!
        [HarmonyPatch(
            declaringType: typeof(CookingDomainSpecial_UnitSlogTransform), 
            methodName: nameof(CookingDomainSpecial_UnitSlogTransform.ApplyTo),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void ApplyTo_CallUpdateBodyParts_Postfix(GameObject Object)
        {
            Object?.Body?.UpdateBodyParts();
        }
    }
 }
