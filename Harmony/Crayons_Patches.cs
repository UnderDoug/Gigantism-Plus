using HarmonyLib;

using System;
using System.Linq;

using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Crayons))]
    public static class Crayons_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crayons.GetRandomColor))]
        public static void GetRandomColor_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Bright")["BrightColors"].GetRandomElement(R);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crayons.GetRandomColorAll))]
        public static void GetRandomColorAll_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "All")["AllColors"].GetRandomElement(R);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crayons.GetRandomColorExcept))]
        public static void GetRandomColorExcept_SendEvent_Postfix(ref string __result, Predicate<string> test, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Except")["AllColors"].Where((string c) => !test(c)).GetRandomElement(R);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crayons.GetRandomDarkColor))]
        public static void GetRandomDarkColor_SendEvent_Postfix(ref string __result, Random R = null)
        {
            __result = CrayonsGetColorsEvent.GetColors(Context: "Dark")["DarkColors"].GetRandomElement(R);
        }
    }
}