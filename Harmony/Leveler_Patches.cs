using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Leveler_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(Leveler),
            methodName: nameof(Leveler.RapidAdvancement),
            argumentTypes: new Type[] { typeof(int), typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPriority(800)]
        [HarmonyPrefix]
        public static bool RapidAdvancement_SendBeforeEvent_Prefix(int Amount, GameObject ParentObject)
        {
            BeforeRapidAdvancementEvent.Send(Amount, ParentObject);
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(Leveler),
            methodName: nameof(Leveler.RapidAdvancement),
            argumentTypes: new Type[] { typeof(int), typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPriority(1)]
        [HarmonyPostfix]
        public static void RapidAdvancement_SendAfterEvent_Postfix(int Amount, GameObject ParentObject)
        {
            AfterRapidAdvancementEvent.Send(Amount, ParentObject);
        }
    }
}
