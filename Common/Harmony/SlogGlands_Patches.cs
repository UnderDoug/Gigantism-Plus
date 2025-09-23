using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class SlogGlands_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(SlogGlands_Patches));

        [HarmonyPatch(
            declaringType: typeof(SlogGlands),
            methodName: nameof(SlogGlands.AddSphincterTo),
            argumentTypes: new Type[] { typeof(BodyPart) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void AddSphincterTo_SetDefaultBehaviorBlueprint_Postfix(ref SlogGlands __instance, ref BodyPart Part)
        {
        }
    }
}
