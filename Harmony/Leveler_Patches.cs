using HarmonyLib;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Leveler))]
    public static class Leveler_Patches
    {
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        [HarmonyPatch(nameof(Leveler.RapidAdvancement))]
        public static bool RapidAdvancement_SendBeforeEvent_Prefix(int Amount, GameObject ParentObject)
        {
            BeforeRapidAdvancementEvent.Send(Amount, ParentObject);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPriority(1)]
        [HarmonyPatch(nameof(Leveler.RapidAdvancement))]
        public static void RapidAdvancement_SendAfterEvent_Postfix(int Amount, GameObject ParentObject)
        {
            AfterRapidAdvancementEvent.Send(Amount, ParentObject);
        }
    }
}
