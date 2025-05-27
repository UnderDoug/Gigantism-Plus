using HarmonyLib;

using System.Collections.Generic;

using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Body_Patches
    {
        private static bool doDebug => true;

        [HarmonyPatch(
            declaringType: typeof(Body),
            methodName: nameof(Body.FireEventOnBodyparts),
            argumentTypes: new Type[] { typeof(Event) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPriority(800)]
        [HarmonyPrefix]
        public static bool FireEventOnBodyparts_SendNaturalEquipmentEvent_Prefix(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)
        {
            Body @this = __instance;
            GameObject ParentObject = @this?.ParentObject;
            Debug.Divider(4, HONLY, Count: 60, Indent: 0, Toggle: doDebug);
            Debug.Entry(4, 
                $"# [Prefix] {nameof(Body)}." 
                + $"{nameof(Body.FireEventOnBodyparts)}"
                + $"(ref Body __instance, Event E, ref Event ___eBodypartsUpdated)", 
                Indent: 0, Toggle: doDebug);

            string objectDesc = @this.ParentObject != null 
                ? $"{@this.ParentObject?.ID}:{@this.ParentObject?.ShortDisplayNameStripped}" 
                : "[null]";

            Debug.Entry(4, $"Object is {objectDesc}", Indent: 1, Toggle: doDebug);

            if (ParentObject != null)
            {
                RegenerateNonDefaultNaturalEquipment(ParentObject);
            }

            if (E.Is(___eBodypartsUpdated) && @this.ParentObject != null)
            {
                // This one tells each NaturalEquipmentManager to reset its collected Modifications
                BeforeBodyPartsUpdatedEvent.Send(@this.ParentObject);

                // This one tells each ManagedNaturalEquipment source to update its stored NaturalEquipmentMods
                UpdateNaturalEquipmentModsEvent.Send(@this.ParentObject);
            }

            Debug.Entry(4,
                $"x [Prefix] {nameof(Body)}."
                + $"{nameof(Body.FireEventOnBodyparts)}"
                + $"(ref Body __instance, Event E, ref Event ___eBodypartsUpdated) #//",
                Indent: 0, Toggle: doDebug);
            return true;
        }

        public static void RegenerateNonDefaultNaturalEquipment(GameObject Creature)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, 
                $"* {nameof(Body_Patches)}."
                + $"{nameof(RegenerateNonDefaultNaturalEquipment)}("
                + $"{nameof(Creature)}: {Creature?.ShortDisplayNameStripped ?? NULL})", 
                Indent: indent, Toggle: doDebug);

            Debug.Entry(4, $"? if (!Creature.Is(null) and Creature.HasPart<Inventory>())", Indent: indent + 1, Toggle: doDebug);
            if (!Creature.Is(null) && Creature.HasPart<Inventory>())
            {
                Debug.Entry(4, $"? if (Creature.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint) and Blueprint.Inventory != null)", Indent: indent + 2, Toggle: doDebug);
                bool doReequip = false;
                if (Creature.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint) && Blueprint.Inventory != null)
                {
                    Debug.Entry(4, $"Getting list of blueprintItemBlueprints that are Natural", Indent: indent + 3, Toggle: doDebug);
                    Dictionary<string, int> blueprintItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (InventoryObject inventoryObject in Blueprint.Inventory)", Indent: indent + 3, Toggle: doDebug);
                    foreach (InventoryObject inventoryObject in Blueprint.Inventory)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                        Debug.LoopItem(4, $"inventoryObject.Blueprint", $"{inventoryObject.Blueprint}", Indent: indent +4, Toggle: doDebug);
                        if (TryGetGameObjectBlueprint(inventoryObject.Blueprint, out GameObjectBlueprint itemBlueprint))
                        {
                            if (itemBlueprint.IsNatural())
                            {
                                Debug.CheckYeh(4, $"Item is Natural", Indent: indent + 4, Toggle: doDebug);
                                if (blueprintItemBlueprints.ContainsKey(inventoryObject.Blueprint))
                                {
                                    blueprintItemBlueprints[inventoryObject.Blueprint]++;
                                    continue;
                                }
                                if (!int.TryParse(inventoryObject.Number, out int number))
                                {
                                    number = 1;
                                }
                                blueprintItemBlueprints.Add(inventoryObject.Blueprint, number);
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Item is not Natural", Indent: indent + 4, Toggle: doDebug);
                            }
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach (InventoryObject inventoryObject in Blueprint.Inventory) >//", Indent: indent + 3, Toggle: doDebug);

                    Debug.Entry(4, $"Getting list of currentItemBlueprints that are Natural", Indent: indent + 3, Toggle: doDebug);
                    Dictionary<string, int> currentItemBlueprints = new();
                    Debug.Entry(4, $"> foreach (GameObject item in ParentObject.GetEquippedObjects())", Indent: indent + 3, Toggle: doDebug);
                    foreach (GameObject item in Creature.GetEquippedObjects())
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                        Debug.LoopItem(4, $"item.Blueprint", $"{item.Blueprint}", Indent: indent + 4, Toggle: doDebug);
                        if (item.IsNatural())
                        {
                            Debug.CheckYeh(4, $"Item is Natural", Indent: indent + 4, Toggle: doDebug);
                            if (currentItemBlueprints.ContainsKey(item.Blueprint))
                            {
                                currentItemBlueprints[item.Blueprint]++;
                            }
                            else
                            {
                                currentItemBlueprints.Add(item.Blueprint, 1);
                            }
                            Debug.Entry(4, $"{item.Blueprint}, {currentItemBlueprints[item.Blueprint]}", Indent: indent + 4, Toggle: doDebug);
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Item is not Natural", Indent: indent + 4, Toggle: doDebug);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach (GameObject item in ParentObject.GetEquippedObjects()) >//", 
                        Indent: indent + 3, Toggle: doDebug);

                    Debug.Entry(4, $"Reducing blueprintItemBlueprint counts by number of equivalent currentItemBlueprints", 
                        Indent: indent + 3, Toggle: doDebug);
                    Debug.Entry(4, $"> foreach ((string blueprint, int number) in currentItemBlueprints)", Indent: indent + 3, Toggle: doDebug);
                    foreach ((string blueprint, int number) in currentItemBlueprints)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                        if (blueprintItemBlueprints.ContainsKey(blueprint))
                        {
                            Debug.Entry(4, $"Before: {blueprint}, {number}", Indent: indent + 4, Toggle: doDebug);
                            blueprintItemBlueprints[blueprint] -= number;
                            if (blueprintItemBlueprints[blueprint] < 1) blueprintItemBlueprints.Remove(blueprint);
                            Debug.Entry(4, $" After: {blueprint}, {number}", Indent: indent + 4, Toggle: doDebug);
                        }
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: indent + 4, Toggle: doDebug);
                    Debug.Entry(4, $"x foreach ((string blueprint, int number) in currentItemBlueprints) >//", Indent: 4, Toggle: doDebug);

                    doReequip = !blueprintItemBlueprints.IsNullOrEmpty();

                    Debug.Entry(4, $"? if ({nameof(doReequip)})", Indent: indent + 3, Toggle: doDebug);
                    Debug.LoopItem(4, $"{nameof(doReequip)}", $"{doReequip}",
                        Good: doReequip, Indent: indent + 4, Toggle: doDebug);
                    if (doReequip)
                    {
                        Debug.Entry(4, $"> foreach ((string blueprint, int number) in blueprintItemBlueprints)", Indent: indent + 4, Toggle: doDebug);
                        foreach ((string blueprint, int number) in blueprintItemBlueprints)
                        {
                            Debug.Divider(4, HONLY, Count: 25, Indent: indent + 5, Toggle: doDebug);
                            for (int i = 0; i < blueprintItemBlueprints.Count; i++)
                            {
                                Debug.CheckYeh(4, $"{blueprint} added to inventory", Indent: indent + 5, Toggle: doDebug);
                                Creature.Inventory.AddObjectToInventory(GameObjectFactory.Factory.CreateObject(blueprint));
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 25, Indent: indent + 5, Toggle: doDebug);
                        Debug.Entry(4, $"x foreach ((string blueprint, int number) in blueprintItemBlueprints) >//", Indent: indent + 4, Toggle: doDebug);
                    }
                    Debug.Entry(4, $"x if (doReequip) ?//", Indent: indent + 3, Toggle: doDebug);
                }
                else
                {
                    Debug.CheckNah(4, $"Creature's Blueprint couldn't be retrieved", Indent: indent + 3, Toggle: doDebug);
                }
                Debug.Entry(4, $"x if (Creature.TryGetGameObjectBlueprint(out GameObjectBlueprint Blueprint) and Blueprint.Inventory != null)) ? //", Indent: indent + 2, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(doReequip)}", $"{doReequip}",
                    Good: doReequip, Indent: indent + 2, Toggle: doDebug);
                if (doReequip)
                {
                    Creature.Brain.WantToReequip();
                }
            }
            else
            {
                Debug.CheckNah(4, $"Creature null or lacks inventory", Indent: indent + 2, Toggle: doDebug);
            }
            Debug.Entry(4, $"x if (!Creature.Is(null) and Creature.HasPart<Inventory>()) ?//", Indent: indent + 1, Toggle: doDebug);

            Debug.Entry(4,
                $"x {nameof(Body_Patches)}."
                + $"{nameof(RegenerateNonDefaultNaturalEquipment)}("
                + $"{nameof(Creature)}: {Creature?.ShortDisplayNameStripped ?? NULL}) *//",
                Indent: indent, Toggle: doDebug);
        }

        [HarmonyPatch(
            declaringType: typeof(Body),
            methodName: nameof(Body.UpdateBodyParts),
            argumentTypes: new Type[] { typeof(int) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPriority(1)]
        [HarmonyPostfix]
        public static void UpdateBodyParts_SendAfterUpdateBodyPartsEvent_Postfix(ref Body __instance)
        {
            Body @this = __instance;
            if (@this.built)
            {
                GameObject parentObject = @this?.ParentObject;
                if (parentObject != null)
                {
                    Debug.Entry(4,
                        $"# [Postfix] {nameof(Body)}."
                        + $"{nameof(Body.UpdateBodyParts)}"
                        + $"(ref Body __instance)",
                        Indent: 0, Toggle: doDebug);

                    Debug.Entry(4, $"{nameof(parentObject)}", $"{parentObject?.DebugName ?? NULL}", Indent: 1, Toggle: doDebug);

                    AfterBodyPartsUpdatedEvent.Send(@this.ParentObject);

                    Debug.Entry(4,
                        $"x [Postfix] {nameof(Body)}."
                        + $"{nameof(Body.UpdateBodyParts)}"
                        + $"(ref Body __instance) #//",
                        Indent: 0, Toggle: doDebug);

                    Debug.Divider(4, HONLY, Count: 60, Indent: 0, Toggle: doDebug);
                }
            }
        }
    }
}
