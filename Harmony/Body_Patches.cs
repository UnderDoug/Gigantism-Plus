using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Body))]
    public static class Body_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Body.FireEventOnBodyparts))]
        static bool FireEventOnBodyparts_SendBodyPartsUpdatedEvent_Prefix(ref Body __instance)
        {
            Body @this = __instance;
            BodyPartsUpdatedEvent.Send(@this.ParentObject);
            return true;
        }
    }
}
