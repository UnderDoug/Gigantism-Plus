using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;

namespace HNPS_GigantismPlus
{
    [PlayerMutator]
    public class GigantifyStartingLoadout : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            Debug.Entry(2, "##################################################################");
            Debug.Entry(2, "public class GigantifyStartingLoadout : IPlayerMutator");
            Debug.Entry(3, "[{}] public void mutate(GameObject player)");

            Debug.Entry(2, "Checking if Gigantification of starting gear should occur");
            Debug.Entry(4, "**if ((player.HasPart(\"HNPS_GigantismPlus\") && Options.EnableGiganticStartingGear)");
            // Check for either mutation OR cybernetic as source of gigantism
            if (player.HasPart<GigantismPlus>() && Options.EnableGiganticStartingGear)
            {
                Debug.Entry(3, "- Player is Gigantic && Option is [Enabled]");

                Debug.Entry(3, "- Checking if Grenades should be included");
                Debug.Entry(4, "**if (Options.EnableGiganticStartingGear_Grenades)");
                if (Options.EnableGiganticStartingGear_Grenades)
                {
                    Debug.Entry(3, "-- Option is [Enabled] - Grenades will be Gigantified");
                }
                else
                {
                    Debug.Entry(3, "-- Option is [Disabled] - Grenades won't be Gigantified");
                }

                Debug.Entry(3, "- Performing Gigantification");
                Debug.Entry(3, "**foreach (GameObject item in player.GetInventoryAndEquipment())");
                // Cycle the player's inventory and equipped items.
                foreach (GameObject item in player.GetInventoryAndEquipment())
                {
                    Debug.Entry(3, "-------------------------------------------");
                    string ItemName = item.DebugName;
                    Debug.Entry(3, $"--@ Item Entry: {ItemName}");
                    ItemName = "--- " + item.Blueprint;
                    // Can the item have the gigantic modifier applied?
                    if (ItemModding.ModificationApplicable("ModGigantic", item))
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        // Is the item already gigantic? Don't attempt to apply it again.
                        if (item.HasPart<ModGigantic>())
                        {
                            Debug.Entry(3, ItemName, "is already gigantic");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not gigantic");

                        // Is the item a grenade, and is the option not set to include them?
                        if (!Options.EnableGiganticStartingGear_Grenades && item.HasTag("Grenade"))
                        {
                            Debug.Entry(3, ItemName, "is a grenade (excluded)");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        if (!item.HasTag("Grenade")) Debug.Entry(3, ItemName, "not a grenade");
                        if (item.HasTag("Grenade")) Debug.Entry(3, ItemName, "is a grenade");

                        // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                        if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                        {
                            Debug.Entry(3, ItemName, "is TradeGoods");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not TradeGoods");

                        // Is the item a tonic? Double doses are basically useless in the early game
                        if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                        {
                            Debug.Entry(3, ItemName, "is Tonics_NonRare");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not Tonics_NonRare");

                        // apply the gigantic mod to the item and attempt to auto-equip it
                        ItemModding.ApplyModification(item, "ModGigantic");
                        Debug.Entry(2, ItemName, "has been Gigantified");
                        // player.AutoEquip(item); Debug.Entry(2, ItemName, "AutoEquip Attempted");
                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                    }
                    else
                    {
                        Debug.Entry(2, ItemName, "cannot be made gigantic");
                        Debug.Entry(4, "--X Skipping");
                        Debug.Entry(3, $"{ItemName} ]//");
                        continue;
                    }

                    Debug.Entry(3, $"{ItemName} ]//");
                }
                Debug.Entry(3, "-------------------------------------------");
                Debug.Entry(3, "- Gigantification of starting gear finished");
                player.WantToReequip();
                Debug.Entry(3, "- Attempting to reequip items");
                Debug.Entry(2, "##################################################################");
            }
            else
            {
                Debug.Entry(4, "- Player not Gigantic || Option is [Disabled]");
                Debug.Entry(3, "- Check Failed");
                Debug.Entry(2, "##################################################################");
            }
        }
    }

    public class GiganticModifierAdjustments
    {
        public static void AdjustGiganticModifier()
        {
            bool ShouldDerarify = Options.SelectGiganticDerarification;
            bool ShouldGiganticTinkerable = Options.SelectGiganticTinkering;
            Debug.Entry(3, "[{}] AdjustGiganticModifier()");

            Debug.Entry(3, "Attempting ModList adjustment process");
            Debug.Entry(4, "**foreach (ModEntry mod in ModificationFactory.ModList)");
            // find the gigantic modifier ModEntry in the ModList
            foreach (ModEntry mod in ModificationFactory.ModList)
            {
                Debug.Entry(3, "-------------------------------------------");
                string ModPart = mod.Part;
                Debug.Entry(3, $"--@ Mod Entry: {ModPart}");
                ModPart = "--- " + ModPart;
                if (mod.Part == "ModGigantic")
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, ModPart, "Found");
                    // should the rarity be adjusted? 
                    // - change the rarity from R2 (3) to R (2) 
                    Debug.Entry(4, "**if (ShouldDerarify)");
                    if (ShouldDerarify)
                    {
                        Debug.Entry(4, "---- Should");
                        mod.Rarity = 2;
                        Debug.Entry(2, ModPart, "Rarity is R (decreased)");
                    }
                    else
                    {
                        Debug.Entry(4, "---- Shouldn't");
                        mod.Rarity = 3;
                        Debug.Entry(2, ModPart, "Rarity is R2 (default)");
                    }

                    // should tinkering be allowed? 
                    // - change the tinkerability and add it to the list of recipes
                    Debug.Entry(4, "**if (ShouldGiganticTinkerable)");
                    if (ShouldGiganticTinkerable)
                    {
                        Debug.Entry(4, "---- Should");
                        mod.TinkerAllowed = true;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Enabled]");

                        // Modifiers can actually be set to require an additional ingredient.
                        // mod.TinkerIngredient = "Torch";
                    }
                    else
                    {
                        Debug.Entry(4, "---- Shouldn't");
                        mod.TinkerAllowed = false;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Disabled] (default)");
                    }

                    // this is a workaround for what I'm sure is a more straightforward and simple solution
                    // - after adjusting the ModEntry to be tinkerable, it needs to be added to the list of recipes
                    // - flushing the list of recipes and then requesting the list uses an internal "get" function that cycles all the TinkerData and ModEntries and adds them to the TinkerRecipes list
                    // - only works if you flush it first since the "get" function checks if the _list is empty first and if it isn't just returns it
                    // it's probably NOT good, and could pose compatability issues with other mods if they do things post Blueprint pre-load, but I'm not nearly experienced enough to know what issues exactly

                    TinkerData._TinkerRecipes.RemoveAll(r => r != null); Debug.Entry(2, "--- Purged TinkerRecipes");
                    List<TinkerData> reinitialise = TinkerData.TinkerRecipes; Debug.Entry(2, "--- Reinitialised TinkerRecipes");
                    reinitialise = null; Debug.Entry(4, "--- Reinitialisation nulled");

                    Debug.Entry(4, "--- No Further Actions Required", "Exiting ModList");
                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    Debug.Entry(3, $"{ModPart} ]//");

                    break;

                }
                else Debug.Entry(2, ModPart, "Not ModGigantic");

                Debug.Entry(3, $"{ModPart} ]//");
            }
            Debug.Entry(3, "-------------------------------------------");
            Debug.Entry(1, "ModList exited, adjustment process finished");
        }
    } //!--- public class GiganticModifierAdjustments

    [PlayerMutator]
    public class OnPlayerLoad : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Entry(2, "public class OnPlayerLoad : IPlayerMutator");
            Debug.Entry(2, "public void mutate(GameObject player)");
            GiganticModifierAdjustments.AdjustGiganticModifier();
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    } //!--- public class OnPlayerLoad : IPlayerMutator

    [HasCallAfterGameLoadedAttribute]
    public class OnLoadGameHandler
    {
        [CallAfterGameLoadedAttribute]
        public static void OnLoadGameCallback()
        {
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Entry(2, "public class OnLoadGameHandler");
            Debug.Entry(2, "public static void OnLoadGameCallback()");
            GiganticModifierAdjustments.AdjustGiganticModifier();
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    } //!--- public class OnLoadGameHandler
}
