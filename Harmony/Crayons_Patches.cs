using HarmonyLib;

using System;
using System.Linq;

using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Crayons_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Crayons_Patches));

        [HarmonyPatch(
            declaringType: typeof(Crayons),
            methodName: nameof(Crayons.GetRandomColor),
            argumentTypes: new Type[] { typeof(Random) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void GetRandomColor_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Bright")["BrightColors"].GetRandomElement(R);
        }

        [HarmonyPatch(
            declaringType: typeof(Crayons),
            methodName: nameof(Crayons.GetRandomColorAll),
            argumentTypes: new Type[] { typeof(Random) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void GetRandomColorAll_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "All")["AllColors"].GetRandomElement(R);
        }

        [HarmonyPatch(
            declaringType: typeof(Crayons),
            methodName: nameof(Crayons.GetRandomColorExcept),
            argumentTypes: new Type[] { typeof(Predicate<string>), typeof(Random) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void GetRandomColorExcept_SendEvent_Postfix(ref string __result, Predicate<string> test, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Except")["AllColors"].Where((string c) => !test(c)).GetRandomElement(R);
        }

        [HarmonyPatch(
            declaringType: typeof(Crayons),
            methodName: nameof(Crayons.GetRandomDarkColor),
            argumentTypes: new Type[] { typeof(Random) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void GetRandomDarkColor_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Dark")["DarkColors"].GetRandomElement(R);
        }
    }
}