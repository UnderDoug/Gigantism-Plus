using HarmonyLib;

using XRL;
using XRL.World;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class AfterObjectCreatedEvent_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AfterObjectCreatedEvent), nameof(AfterObjectCreatedEvent.Process))]
        public static void Process_SendToTheGame_Postfix(ref AfterObjectCreatedEvent __result, GameObject Object, string Context, ref GameObject ReplacementObject)
        {
            if (The.Game.WantEvent(AfterObjectCreatedEvent.ID, MinEvent.CascadeLevel))
            {
                AfterObjectCreatedEvent afterObjectCreatedEvent = __result;
                afterObjectCreatedEvent.Object = Object;
                afterObjectCreatedEvent.Context = Context;
                afterObjectCreatedEvent.ReplacementObject = ReplacementObject;
                The.Game.HandleEvent(afterObjectCreatedEvent);
                ReplacementObject = afterObjectCreatedEvent.ReplacementObject;
            }
        }
    }
}