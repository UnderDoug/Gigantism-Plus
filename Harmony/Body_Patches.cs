using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Body))]
    public static class Body_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Body.FireEventOnBodyparts))]
        static bool FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)
        {
            Body @this = __instance;
            /*Debug.Entry(4, 
                $"{typeof(Body_Patches).Name}." + 
                $"{nameof(FireEventOnBodyparts_SendCollectNaturalEquipmentModsEvent_Prefix)}(ref Actor __instance)", 
                Indent: 0);

            string objectDesc = @this.ParentObject != null 
                ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"Object is {objectDesc}", Indent: 1);*/

            if (E.Is(___eBodypartsUpdated))
            {
                // This one tells each NaturalEquipmentManager to reset its collected Modifications
                BeforeBodyPartsUpdatedEvent.Send(@this.ParentObject);

                // This one tells each ManagedNaturalEquipment source to update its stored NaturalEquipmentMods
                UpdateNaturalEquipmentModsEvent.Send(@this.ParentObject);
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Body.UpdateBodyParts))]
        static void UpdateBodyParts_SendAfterUpdateBodyPartsEvent_Postfix(ref Body __instance)
        {
            Body @this = __instance;
            if (@this.built)
            {
                /*Debug.Entry(4,
                    $"{typeof(Body_Patches).Name}." +
                    $"{nameof(UpdateBodyParts_SendAfterUpdateBodyPartsEvent_Postfix)}(ref Actor __instance)",
                    Indent: 0); 

                string objectDesc = @this.ParentObject != null
                    ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}"
                    : "[null]";

                Debug.Entry(4, $"Object is {objectDesc}", Indent: 1);*/

                AfterBodyPartsUpdatedEvent.Send(@this.ParentObject);
            }
        }
    }
}
