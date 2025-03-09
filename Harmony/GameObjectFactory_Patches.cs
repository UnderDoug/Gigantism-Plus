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
    public static class GiganticCreature_Inventory_Patches
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
        static void CreateObjectPostfix(ref GameObject __result)
        {
            // Skip players and objects without inventory
            if (__result == null || __result.IsPlayer() || __result.Inventory == null)
                return;

            // Check if the creature has GigantismPlus or appropriate exoframe
            if (!__result.HasPart<GigantismPlus>() && !__result.HasPart("CyberneticsGiganticExoframe"))
                return;

            // Check if the option to make NPC equipment gigantic is enabled
            if (!Options.EnableGiganticNPCGear)
                return;

            Debug.Entry(3, "Making inventory items gigantic for creature: " + __result.DebugName);

            try
            {
                // Create a copy of the items list to avoid modifying during enumeration
                List<GameObject> itemsToProcess = new(__result.Inventory.Objects);

                // Keep track of items to equip after modification
                List<GameObject> itemsToEquip = new();

                // Process each item
                foreach (var item in itemsToProcess)
                {
                    // Skip items that are already gigantic or can't be made gigantic
                    if (item.HasPart<ModGigantic>() || !ItemModding.ModificationApplicable("ModGigantic", item))
                        continue;

                    // Check if it's a grenade and if grenades should be gigantified
                    bool isGrenade = item.HasTag("Grenade");
                    if (isGrenade && !Options.EnableGiganticNPCGear_Grenades)
                    {
                        Debug.Entry(3, "  Skipping grenade due to disabled option: " + item.DebugName);
                        continue;
                    }

                    Debug.Entry(3, "  Applying ModGigantic to: " + item.DebugName);

                    // Make the item gigantic
                    ItemModding.ApplyModification(item, "ModGigantic");

                    // If it's equippable, add it to our list to equip later
                    if (item.HasPart<MeleeWeapon>() || item.HasPart<Armor>() || item.HasPart<XRL.World.Parts.Shield>())
                    {
                        itemsToEquip.Add(item);
                    }
                }

                // Now equip all items that should be equipped
                foreach (var item in itemsToEquip)
                {
                    if (__result.Inventory.Objects.Contains(item))  // Make sure item is still in inventory
                    {
                        Debug.Entry(3, "  Auto-equipping item: " + item.DebugName);
                        __result.AutoEquip(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Entry(1, $"ERROR in GiganticCreature_Inventory_Patches: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
