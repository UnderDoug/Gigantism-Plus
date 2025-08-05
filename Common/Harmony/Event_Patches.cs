using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Event_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Leveler_Patches));

        [HarmonyPatch(
            declaringType: typeof(Event),
            methodName: nameof(Event.ResetPool),
            argumentTypes: new Type[] { typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void ResetPool_AlsoResetNaturalEquipModListPool_Postfix()
        {
            NaturalEquipmentManager.ResetNaturalEquipmentModListPool();
        }
    }
}
