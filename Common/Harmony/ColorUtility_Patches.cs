using ConsoleLib.Console;
using HarmonyLib;
using System;
using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class ColorUtility_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(ColorUtility_Patches));

        [HarmonyPatch(
            declaringType: typeof(ColorUtility),
            methodName: "GetPaths",
            argumentTypes: new Type[] { typeof(bool), typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void GetPaths_ForceDisplayTxt_Postfix(ref List<(string path, ModInfo mod)> __result, bool Mod)
        {
            if (Mod)
            {
                string path = $"{ThisMod.Path}/Display.txt";
                __result.Add((path, ThisMod));
            }
        }
    }
}
