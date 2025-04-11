using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

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
        static bool RapidAdvancement_SendBeforeEvent_Prefix(int Amount, GameObject ParentObject)
        {
            /*Debug.Entry(4, 
                $"{typeof(Leveler_Patches).Name}." + 
                $"{nameof(RapidAdvancement_SendBeforeEvent_Prefix)}(int Amount, GameObject ParentObject)", 
                Indent: 0);

            string objectDesc = ParentObject != null 
                ? $"{ParentObject?.ID}:{ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"ParentObject is {objectDesc}", Indent: 1);*/

            BeforeRapidAdvancementEvent.Send(Amount, ParentObject);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPriority(1)]
        [HarmonyPatch(nameof(Leveler.RapidAdvancement))]
        static void RapidAdvancement_SendAfterEvent_Postfix(int Amount, GameObject ParentObject)
        {
            /*Debug.Entry(4,
                $"{typeof(Leveler_Patches).Name}." +
                $"{nameof(RapidAdvancement_SendAfterEvent_Postfix)}(int Amount, GameObject ParentObject)", 
            Indent: 0);

            string objectDesc = ParentObject != null 
                ? $"{ParentObject?.ID}:{ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"ParentObject is {objectDesc}", Indent: 1);*/

            AfterRapidAdvancementEvent.Send(Amount, ParentObject);
        }
    }
}
