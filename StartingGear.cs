using XRL; 
using XRL.World;
using XRL.World.Parts;
using XRL.World.Tinkering;

namespace Mods.GigantismPlus
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
            Debug.Entry(4, "**if ((player.HasPart(\"GigantismPlus\") && Options.EnableGiganticStartingGear)");
            // Check for either mutation OR cybernetic as source of gigantism
            if (player.HasPart("GigantismPlus") && Options.EnableGiganticStartingGear)
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
}