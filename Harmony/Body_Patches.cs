using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System.Collections.Generic;
using XRL.World.Anatomy;

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

                    Debug.Entry(4, $"Getting list of blueprintItemBlueprints that are Natural", Indent: 3);
                    Dictionary<string, int> blueprintItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (InventoryObject inventoryObject in Blueprint.Inventory)", Indent: 3);
                    foreach (InventoryObject inventoryObject in Blueprint.Inventory)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                        Debug.LoopItem(4, $"inventoryObject.Blueprint", $"{inventoryObject.Blueprint}", Indent: 4);
                        if (TryGetGameObjectBlueprint(inventoryObject.Blueprint, out GameObjectBlueprint itemBlueprint))
                        {
                            if (itemBlueprint.IsNatural())
                            {
                                Debug.CheckYeh(4, $"Item is Natural", Indent: 4);
                                if (blueprintItemBlueprints.ContainsKey(inventoryObject.Blueprint))
                                {
                                    blueprintItemBlueprints[inventoryObject.Blueprint]++;
                                    continue;
                                }
                                if (!int.TryParse(inventoryObject.Number, out int number))
                                    number = 1;
                                blueprintItemBlueprints.Add(inventoryObject.Blueprint, number);
                                /*
                                string targetType = itemBlueprint.GetPartParameter<string>(nameof(Armor), "WornOn") ?? itemBlueprint.GetPartParameter<string>(nameof(MeleeWeapon), "Slot");
                                bool freeSlot = false;
                                Debug.Entry(4, $"> foreach (BodyPart part in @this.LoopPart(targetType))", Indent: 4);
                                foreach (BodyPart part in @this.LoopPart(targetType))
                                {
                                    Debug.Divider(4, HONLY, Count: 25, Indent: 4);

                                    Debug.Entry(4, $"part", $"{part.DebugName()}", Indent: 5);
                                    Debug.Divider(4, HONLY, Count: 15, Indent: 5);
                                    bool naturalEquipped 
                                      = !part.Equipped.Is(null) 
                                     && !part.Equipped.IsNatural();
                                    Debug.LoopItem(4, $"naturalEquipped", Good: naturalEquipped, Indent: 5);

                                    bool defaultBehavior 
                                      = !part.DefaultBehavior.Is(null) 
                                     && part.DefaultBehavior.IsNatural();
                                    Debug.LoopItem(4, $"defaultBehavior", Good: defaultBehavior, Indent: 5);

                                    bool permanentCybernetic 
                                      = !part.Cybernetics.Is(null) 
                                     && part.Cybernetics.HasTag("CyberneticsUsesEqSlot") 
                                     && part.Cybernetics.HasTag("CyberneticsNoRemove");
                                    Debug.LoopItem(4, $"permanentCybernetic", Good: permanentCybernetic, Indent: 5);
                                    Debug.Divider(4, HONLY, Count: 15, Indent: 5);

                                    freeSlot = !naturalEquipped && !defaultBehavior && !permanentCybernetic;
                                    Debug.LoopItem(4, $"freeSlot", Good: freeSlot, Indent: 5);
                                    if (freeSlot) break;
                                }
                                Debug.Divider(4, HONLY, Count: 25, Indent: 4);
                                Debug.Entry(4, $"x foreach (BodyPart part in @this.LoopPart(targetType)) >//", Indent: 4);

                                if (freeSlot)
                                {
                                    Debug.Entry(4, 
                                        $"Have a Free Slot, creating {itemBlueprint.Name} and placing in inventory", 
                                        Indent: 4);
                                    ParentObject.Inventory.AddObjectToInventory(GameObjectFactory.Factory.CreateObject(itemBlueprint));
                                    doReequip = true;
                                    Debug.LoopItem(4, $"doReequip", Good: doReequip, Indent: 4);
                                }
                                */
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Item is not Natural", Indent: 4);
                            }
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.Entry(4, $"x foreach (InventoryObject inventoryObject in Blueprint.Inventory) >//", Indent: 3);

                    Debug.Entry(4, $"Getting list of currentItemBlueprints that are Natural", Indent: 3);
                    Dictionary<string, int> currentItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (GameObject item in ParentObject.GetEquippedObjects())", Indent: 3);
                    foreach (GameObject item in ParentObject.GetEquippedObjects())
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                        Debug.LoopItem(4, $"item.Blueprint", $"{item.Blueprint}", Indent: 4);
                        if (item.IsNatural())
                        {
                            Debug.CheckYeh(4, $"Item is Natural", Indent: 4);
                            if (currentItemBlueprints.ContainsKey(item.Blueprint))
                            {
                                currentItemBlueprints[item.Blueprint]++;
                            }
                            else
                            {
                                currentItemBlueprints.Add(item.Blueprint, 1);
                            }
                            Debug.Entry(4, $"{item.Blueprint}, {currentItemBlueprints[item.Blueprint]}", Indent: 5);
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Item is not Natural", Indent: 4);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.Entry(4, $"x foreach (GameObject item in ParentObject.GetEquippedObjects()) >//", Indent: 3);

                    Debug.Entry(4, $"Reducing blueprintItemBlueprint counts by number of equivalent currentItemBlueprints", Indent: 3);
                    Debug.Entry(4, $"> foreach ((string blueprint, int number) in currentItemBlueprints)", Indent: 3);
                    foreach ((string blueprint, int number) in currentItemBlueprints)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                        if (blueprintItemBlueprints.ContainsKey(blueprint))
                        {
                            Debug.Entry(4, $"Before: {blueprint}, {number}", Indent: 4);
                            blueprintItemBlueprints[blueprint] -= number;
                            if (blueprintItemBlueprints[blueprint] < 1) blueprintItemBlueprints.Remove(blueprint);
                            Debug.Entry(4, $" After: {blueprint}, {number}", Indent: 4);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.Entry(4, $"x foreach ((string blueprint, int number) in currentItemBlueprints) >//", Indent: 3);

                    doReequip = !blueprintItemBlueprints.IsNullOrEmpty();

                    Debug.Entry(4, $"? if (doReequip)", Indent: 3);
                    Debug.LoopItem(4, $"doReequip", Indent: 4,
                        Good: doReequip);
                    if (doReequip)
                    {
                        Debug.Entry(4, $"> foreach ((string blueprint, int number) in blueprintItemBlueprints)", Indent: 4);
                        foreach ((string blueprint, int number) in blueprintItemBlueprints)
                        {
                            Debug.Divider(4, HONLY, Count: 25, Indent: 4);
                            for (int i = 0; i < blueprintItemBlueprints.Count; i++)
                            {
                                Debug.CheckYeh(4, $"{blueprint} added to inventory", Indent: 5);
                                ParentObject.Inventory.AddObjectToInventory(GameObjectFactory.Factory.CreateObject(blueprint));
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 25, Indent: 4);
                        Debug.Entry(4, $"x foreach ((string blueprint, int number) in blueprintItemBlueprints) >//", Indent: 4);
                    }
                    Debug.Entry(4, $"x if (doReequip) ?//", Indent: 3);
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
