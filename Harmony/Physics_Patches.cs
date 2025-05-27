using HarmonyLib;

using System;

using XRL.UI;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Physics_Patches
    {
        private static bool doDebug => true;

        [HarmonyPatch(
            declaringType: typeof(Physics), 
            methodName: nameof(Physics.HandleEvent),
            argumentTypes: new Type[] { typeof(EquippedEvent) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        static bool HandleEvent_EquippedEvent_Prefix(EquippedEvent E, ref Physics __instance)
        {
            // goal: reduce the amount of thrown warnings when the FinalizeCopy patch sends an EquippedEvent

            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"# [Prefix] {nameof(Physics)}."
                + $"{nameof(Physics.HandleEvent)}("
                + $"{nameof(EquippedEvent)} E, ref Physics __instance)",
                Indent: indent, Toggle: doDebug);

            if (__instance?._Equipped != null)
            {
                __instance._Equipped = null; 
                Debug.Entry(4, $"__instance._Equipped", __instance._Equipped?.DebugName ?? NULL, Indent: indent + 1, Toggle: doDebug);
            }
            return true;
        } //!-- static bool HandleEvent_EquippedEvent_Prefix(EquippedEvent E, ref Physics __instance)
    }
}
