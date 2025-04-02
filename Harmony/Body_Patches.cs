using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Utils;

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
            Debug.Entry(4, $"{typeof(Body_Patches).Name}.{nameof(FireEventOnBodyparts_SendBodyPartsUpdatedEvent_Prefix)}(ref Body __instance)", Indent: 0);
            string objectDesc = @this.ParentObject != null 
                ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";
            Debug.Entry(4, $"Object is {objectDesc}", Indent: 1);
            AfterBodyPartsUpdatedEvent.Send(@this.ParentObject);
            return true;
        }
        /*
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Body.RegenerateDefaultEquipment))]
        static void RegenerateDefaultEquipment_SendBodyPartsUpdatedEvent_Prefix(ref Body __instance)
        {
            Body @this = __instance;
            Debug.Entry(4, $"{typeof(Body_Patches).Name}.{nameof(RegenerateDefaultEquipment_SendBodyPartsUpdatedEvent_Prefix)}(ref Body __instance)", Indent: 0);
            Debug.Entry(4, $"Object is {@this.ParentObject?.DebugName}", Indent: 1);
            AfterBodyPartsUpdatedEvent.Send(@this.ParentObject);
        }
        */
    }
}
