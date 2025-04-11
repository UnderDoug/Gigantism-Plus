using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System.Collections.Generic;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Body))]
    public static class Body_Patches
    {
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        [HarmonyPatch(nameof(Body.FireEventOnBodyparts))]
        static bool FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)
        {
            Body @this = __instance;
            GameObject ParentObject = @this?.ParentObject;
            Debug.Entry(4, 
                $"{typeof(Body_Patches).Name}." + 
                $"{nameof(FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix)}(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)", 
                Indent: 0);

            string objectDesc = @this.ParentObject != null 
                ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"Object is {objectDesc}", Indent: 1);

            Debug.Entry(4, $"? if (!@this.Is(null) and !ParentObject.Is(null) and ParentObject.HasPart<Inventory>())", Indent: 1);
            if (!@this.Is(null) && !ParentObject.Is(null) && ParentObject.HasPart<Inventory>())
            {
                Debug.Entry(4, $"? if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint))", Indent: 2);
                bool doReequip = false;
                if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint))
                {
                    if (Blueprint.Inventory.Is(null)) Blueprint.Inventory = new();
                    Debug.Entry(4, $"> foreach (InventoryObject inventoryObject in Blueprint.Inventory)", Indent: 3);
                    foreach (InventoryObject inventoryObject in Blueprint.Inventory)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                        Debug.LoopItem(4, $"inventoryObject.Blueprint", $"{inventoryObject.Blueprint}", Indent: 4);
                        if (TryGetGameObjectBlueprint(inventoryObject.Blueprint, out GameObjectBlueprint itemBlueprint))
                        {
                            if (itemBlueprint.HasPart("NaturalEquipment") || itemBlueprint.HasTagOrProperty("NaturalGear"))
                            {
                                Debug.CheckYeh(4, $"NaturalEquipment or NaturalGear", Indent: 4);
                                ParentObject.Inventory.AddObjectToInventory(GameObjectFactory.Factory.CreateObject(itemBlueprint));
                                doReequip = true;
                            }
                            else
                            {
                                Debug.CheckNah(4, $"NaturalEquipment or NaturalGear", Indent: 4);
                            }
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.Entry(4, $"x foreach (InventoryObject inventoryObject in Blueprint.Inventory) >//", Indent: 3);
                }
                else
                {
                    Debug.CheckNah(4, $"Object's Blueprint couldn't be retrieved", Indent: 2);
                }
                Debug.Entry(4, $"x if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint)) ? //", Indent: 2);

                Debug.LoopItem(4, $"doReequip", Indent: 2, 
                    Good: doReequip);
                if (doReequip) ParentObject.Brain.PerformReequip();
            }
            else
            {
                Debug.CheckNah(4, $"Object null or lacks inventory", Indent: 1);
            }
            Debug.Entry(4, $"x if (!@this.Is(null) and !ParentObject.Is(null) and ParentObject.HasPart<Inventory>()) ?//", Indent: 1);

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
        [HarmonyPriority(1)]
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
