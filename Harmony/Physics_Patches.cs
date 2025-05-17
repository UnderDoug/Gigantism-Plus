using HarmonyLib;
using System;
using System.Diagnostics;
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
        [HarmonyPatch(
            declaringType: typeof(Physics), 
            methodName: nameof(Physics.HandleEvent),
            argumentTypes: new Type[] { typeof(EquippedEvent) })]
        [HarmonyPrefix]
        static bool HandleEvent_EquippedEvent_Prefix(EquippedEvent E, ref Physics __instance)
        {
            // goal: reduce the amount of thrown warnings when the FinalizeCopy patch sends an EquippedEvent
            if (__instance._Equipped != null)
            {
                __instance._Equipped = null;
            }
            return true;
        } //!-- static bool HandleEvent_EquippedEvent_Prefix(EquippedEvent E)
    }
}
