using HarmonyLib;
using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(GameObjectFactory))]
    public static class GiganticExoframeCreature_Inventory_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameObjectFactory.CreateObject), new Type[] {
            typeof(GameObjectBlueprint),
            typeof(int),
            typeof(int),
            typeof(string),
            typeof(Action<GameObject>),
            typeof(Action<GameObject>),
            typeof(string),
            typeof(List<GameObject>)
        })]
        static void CreateObject_Postfix(ref GameObject __result)
        {
            return;
            GameObject GO = __result;
            if (!Options.EnableGiganticNPCGear) return; // skip if Options.EnableGiganticNPCGear disabled
            if (GO == null) return;
            if (!GO.HasPart<CyberneticsGiganticExoframe>()) return; // skip creatures lacking GiganticExoframe
            if (!GO.HasPart<GigantismPlus>()) return; // skip non-gigantic creatures
            if (GO.Inventory == null) return; // skip objects without inventory
            if (GO.IsPlayer()) return; // skip players

            Debug.Entry(3, "Making inventory items gigantic for creature", GO.DebugName, Indent: 0);

            try
            {
                // Create a copy of the items list to avoid modifying during enumeration
                List<GameObject> itemsToProcess = new(GO.Inventory.Objects);

                // Keep track of items to equip after modification
                List<GameObject> itemsToEquip = new();

                bool gigantifyNPCGrenade = Options.EnableGiganticNPCGear_Grenades;

                // Process each item
                Debug.Entry(3, "> foreach (var item in itemsToProcess)", Indent: 1);
                Debug.Divider(4, "-", Count: 25, Indent: 1);
                foreach (var item in itemsToProcess)
                {
                    Debug.DiveIn(4, $"{item.DebugName}", Indent: 1);
                    // Skip items that are already gigantic or can't be made gigantic
                    if (item.HasPart<ModGigantic>() || !ItemModding.ModificationApplicable("ModGigantic", item))
                    {
                        Debug.LoopItem(3, "Skipping grenade due to disabled option", item.DebugName, Indent: 3);
                        goto Skip;
                    }

                    // Check if it's a grenade and if grenades should be gigantified
                    bool isGrenade = item.HasTag("Grenade");
                    if (isGrenade && !gigantifyNPCGrenade)
                    {
                        Debug.LoopItem(3, "Skipping grenade due to disabled option", item.DebugName, Indent: 3);
                        goto Skip;
                    }

                    Debug.Entry(3, "Applying ModGigantic to", item.DebugName, Indent: 2);

                    // Make the item gigantic
                    ItemModding.ApplyModification(item, "ModGigantic");

                    // If it's equippable, add it to our list to equip later
                    if (item.HasPart<MeleeWeapon>() || item.HasPart<Armor>() || item.HasPart<Shield>())
                    {
                        itemsToEquip.Add(item);
                        Debug.LoopItem(3, $"{item.Blueprint} Equipable", "Added to itemsToEquip", Indent: 3);
                    }
                    Skip:
                    Debug.DiveOut(4, $"{item.DebugName}", Indent: 1);
                }
                Debug.Divider(4, "-", Count: 25, Indent: 1);
                Debug.Entry(3, "x foreach (var item in itemsToProcess) >//", Indent: 1);

                // Now equip all items that should be equipped
                Debug.Entry(3, "> foreach (var item in itemsToEquip)", Indent: 1);
                Debug.Divider(4, "-", Count: 25, Indent: 1);
                foreach (var item in itemsToEquip)
                {
                    if (GO.Inventory.Objects.Contains(item))  // Make sure item is still in inventory
                    {
                        Debug.LoopItem(3, "Auto-equipping item", item.DebugName, Indent: 2);
                        GO.AutoEquip(item);
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 1);
                Debug.Entry(3, "x foreach (var item in itemsToEquip) >//", Indent: 1);
            }
            catch (Exception ex)
            {
                Debug.Entry(1, $"ERROR in {nameof(GiganticExoframeCreature_Inventory_Patches)}.{nameof(CreateObject_Postfix)}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
