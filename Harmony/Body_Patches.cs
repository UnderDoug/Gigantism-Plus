using HarmonyLib;

using System.Collections.Generic;

using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Body))]
    public static class Body_Patches
    {
        private static bool doDebug => true;

        [HarmonyPrefix]
        [HarmonyPriority(800)]
        [HarmonyPatch((typeof(Body)), nameof(Body.FireEventOnBodyparts))]
        public static bool FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)
        {
            Body @this = __instance;
            GameObject ParentObject = @this?.ParentObject;
            Debug.Entry(4, 
                $"{typeof(Body_Patches).Name}." + 
                $"{nameof(FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix)}(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)", 
                Indent: 0, Toggle: doDebug);

            string objectDesc = @this.ParentObject != null 
                ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"Object is {objectDesc}", Indent: 1, Toggle: doDebug);

            Debug.Entry(4, $"? if (!@this.Is(null) and !ParentObject.Is(null) and ParentObject.HasPart<Inventory>())", Indent: 1, Toggle: doDebug);
            if (!@this.Is(null) && !ParentObject.Is(null) && ParentObject.HasPart<Inventory>())
            {
                Debug.Entry(4, $"? if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint))", Indent: 2, Toggle: doDebug);
                bool doReequip = false;
                if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint))
                {
                    if (Blueprint.Inventory.Is(null)) Blueprint.Inventory = new();

                    Debug.Entry(4, $"Getting list of blueprintItemBlueprints that are Natural", Indent: 3, Toggle: doDebug);
                    Dictionary<string, int> blueprintItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (InventoryObject inventoryObject in Blueprint.Inventory)", Indent: 3, Toggle: doDebug);
                    foreach (InventoryObject inventoryObject in Blueprint.Inventory)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                        Debug.LoopItem(4, $"inventoryObject.Blueprint", $"{inventoryObject.Blueprint}", Indent: 4, Toggle: doDebug);
                        if (TryGetGameObjectBlueprint(inventoryObject.Blueprint, out GameObjectBlueprint itemBlueprint))
                        {
                            if (itemBlueprint.IsNatural())
                            {
                                Debug.CheckYeh(4, $"Item is Natural", Indent: 4, Toggle: doDebug);
                                if (blueprintItemBlueprints.ContainsKey(inventoryObject.Blueprint))
                                {
                                    blueprintItemBlueprints[inventoryObject.Blueprint]++;
                                    continue;
                                }
                                if (!int.TryParse(inventoryObject.Number, out int number))
                                    number = 1;
                                blueprintItemBlueprints.Add(inventoryObject.Blueprint, number);
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Item is not Natural", Indent: 4, Toggle: doDebug);
                            }
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach (InventoryObject inventoryObject in Blueprint.Inventory) >//", Indent: 3, Toggle: doDebug);

                    Debug.Entry(4, $"Getting list of currentItemBlueprints that are Natural", Indent: 3, Toggle: doDebug);
                    Dictionary<string, int> currentItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (GameObject item in ParentObject.GetEquippedObjects())", Indent: 3, Toggle: doDebug);
                    foreach (GameObject item in ParentObject.GetEquippedObjects())
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                        Debug.LoopItem(4, $"item.Blueprint", $"{item.Blueprint}", Indent: 4, Toggle: doDebug);
                        if (item.IsNatural())
                        {
                            Debug.CheckYeh(4, $"Item is Natural", Indent: 4, Toggle: doDebug);
                            if (currentItemBlueprints.ContainsKey(item.Blueprint))
                            {
                                currentItemBlueprints[item.Blueprint]++;
                            }
                            else
                            {
                                currentItemBlueprints.Add(item.Blueprint, 1);
                            }
                            Debug.Entry(4, $"{item.Blueprint}, {currentItemBlueprints[item.Blueprint]}", Indent: 5, Toggle: doDebug);
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Item is not Natural", Indent: 4, Toggle: doDebug);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach (GameObject item in ParentObject.GetEquippedObjects()) >//", Indent: 3, Toggle: doDebug);

                    Debug.Entry(4, $"Reducing blueprintItemBlueprint counts by number of equivalent currentItemBlueprints", Indent: 3, Toggle: doDebug);
                    Debug.Entry(4, $"> foreach ((string blueprint, int number) in currentItemBlueprints)", Indent: 3, Toggle: doDebug);
                    foreach ((string blueprint, int number) in currentItemBlueprints)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                        if (blueprintItemBlueprints.ContainsKey(blueprint))
                        {
                            Debug.Entry(4, $"Before: {blueprint}, {number}", Indent: 4, Toggle: doDebug);
                            blueprintItemBlueprints[blueprint] -= number;
                            if (blueprintItemBlueprints[blueprint] < 1) blueprintItemBlueprints.Remove(blueprint);
                            Debug.Entry(4, $" After: {blueprint}, {number}", Indent: 4, Toggle: doDebug);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach ((string blueprint, int number) in currentItemBlueprints) >//", Indent: 3, Toggle: doDebug);

                    doReequip = !blueprintItemBlueprints.IsNullOrEmpty();

                    Debug.Entry(4, $"? if (doReequip)", Indent: 3, Toggle: doDebug);
                    Debug.LoopItem(4, $"doReequip", 
                        Indent: 4, Good: doReequip, Toggle: doDebug);
                    if (doReequip)
                    {
                        Debug.Entry(4, $"> foreach ((string blueprint, int number) in blueprintItemBlueprints)", Indent: 4, Toggle: doDebug);
                        foreach ((string blueprint, int number) in blueprintItemBlueprints)
                        {
                            Debug.Divider(4, HONLY, Count: 25, Indent: 4, Toggle: doDebug);
                            for (int i = 0; i < blueprintItemBlueprints.Count; i++)
                            {
                                Debug.CheckYeh(4, $"{blueprint} added to inventory", Indent: 5, Toggle: doDebug);
                                ParentObject.Inventory.AddObjectToInventory(GameObjectFactory.Factory.CreateObject(blueprint));
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 25, Indent: 4, Toggle: doDebug);
                        Debug.Entry(4, $"x foreach ((string blueprint, int number) in blueprintItemBlueprints) >//", Indent: 4, Toggle: doDebug);
                    }
                    Debug.Entry(4, $"x if (doReequip) ?//", Indent: 3, Toggle: doDebug);
                }
                else
                {
                    Debug.CheckNah(4, $"Object's Blueprint couldn't be retrieved", Indent: 2, Toggle: doDebug);
                }
                Debug.Entry(4, $"x if (ParentObject.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint)) ? //", Indent: 2, Toggle: doDebug);

                Debug.LoopItem(4, $"doReequip",
                    Indent: 2, Good: doReequip, Toggle: doDebug);
                if (doReequip)
                {
                    ParentObject.Brain.WantToReequip();
                }
            }
            else
            {
                Debug.CheckNah(4, $"Object null or lacks inventory", Indent: 1, Toggle: doDebug);
            }
            Debug.Entry(4, $"x if (!@this.Is(null) and !ParentObject.Is(null) and ParentObject.HasPart<Inventory>()) ?//", Indent: 1, Toggle: doDebug);

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
        [HarmonyPatch((typeof(Body)), nameof(Body.UpdateBodyParts))]
        public static void UpdateBodyParts_SendAfterUpdateBodyPartsEvent_Postfix(ref Body __instance)
        {
            Body @this = __instance;
            if (@this.built)
            {
                AfterBodyPartsUpdatedEvent.Send(@this.ParentObject);
            }
        }
    }
}
