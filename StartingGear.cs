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
            Debug.Entry(1, "/ Checking if Gigantification of starting gear should occur.");
            // Check for either mutation OR cybernetic as source of gigantism
            if ((player.HasPart("GigantismPlus") || player.HasPart("MassiveExoframe")) && Options.EnableGiganticStartingGear)
            {
                if (Options.EnableGiganticStartingGear_Grenades) Debug.Entry(3, "| Checking", "grenades will be Gigantified");
                if (!Options.EnableGiganticStartingGear_Grenades) Debug.Entry(3, "| Checking", "grenades won't be Gigantified");
                Debug.Entry(1, "/ Spinning up InventoryAndEquipment");
                Debug.Entry(2, "________________________________________|");
                // Cycle the player's inventory and equipped items.
                foreach (GameObject item in player.GetInventoryAndEquipment())
                {
                    string ItemName = item.DebugName;
                    Debug.Entry(2, "BEGIN ENTRY", ItemName);
                    ItemName = "| " + item.Blueprint;
                    // Can the item have the gigantic modifier applied?
                    if (ItemModding.ModificationApplicable("ModGigantic", item))
                    {
                        // Is the item already gigantic? Don't attempt to apply it again.
                        if (item.HasPart<ModGigantic>())
                        {
                            Debug.Entry(3, ItemName, "is already gigantic");
                            Debug.Entry(3, "\\ Skipping");
                            Debug.Entry(2, "END ENTRY ------------------------------|");
                            Debug.Entry(2, "________________________________________|");
                            Debug.Entry(2, "");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not gigantic");

                        // Is the item a grenade, and is the option not set to include them?
                        if (!Options.EnableGiganticStartingGear_Grenades && item.HasTag("Grenade"))
                        {
                            Debug.Entry(3, ItemName, "is a grenade (excluded)");
                            Debug.Entry(3, "\\ Skipping");
                            Debug.Entry(2, "END ENTRY ------------------------------|");
                            Debug.Entry(2, "________________________________________|");
                            Debug.Entry(2, "");
                            continue;
                        }
                        if (!item.HasTag("Grenade")) Debug.Entry(3, ItemName, "not a grenade");
                        if (item.HasTag("Grenade")) Debug.Entry(3, ItemName, "is a grenade");

                        // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                        if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                        {
                            Debug.Entry(3, ItemName, "is TradeGoods");
                            Debug.Entry(3, "\\ Skipping");
                            Debug.Entry(2, "END ENTRY ------------------------------|");
                            Debug.Entry(2, "________________________________________|");
                            Debug.Entry(2, "");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not TradeGoods");

                        // Is the item a tonic? Double doses are basically useless in the early game
                        if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                        {
                            Debug.Entry(3, ItemName, "is Tonics_NonRare");
                            Debug.Entry(3, "\\ Skipping");
                            Debug.Entry(2, "END ENTRY ------------------------------|");
                            Debug.Entry(2, "________________________________________|");
                            Debug.Entry(2, "");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not Tonics_NonRare");

                        // apply the gigantic mod to the item and attempt to auto-equip it
                        ItemModding.ApplyModification(item, "ModGigantic");
                        Debug.Entry(2, ItemName, "has been Gigantified");
                        // player.AutoEquip(item); Debug.Entry(2, ItemName, "AutoEquip Attempted");

                    } 
                    else
                    {
                        Debug.Entry(2, ItemName, "cannot be made gigantic.");
                        Debug.Entry(3, "\\ Skipping");
                        Debug.Entry(2, "END ENTRY ------------------------------|");
                        Debug.Entry(2, "________________________________________|");
                        Debug.Entry(2, "");
                        continue;
                    }

                    Debug.Entry(2, "END ENTRY ------------------------------|");
                    Debug.Entry(2, "________________________________________|");
                    Debug.Entry(2, "");
                }
                Debug.Entry(1, "\\ Gigantification of starting gear finished.");
                Debug.Entry(1, "________________________________________");
                player.WantToReequip();
            }
            else
            {
                Debug.Entry(1, "\\Check failed.");
                Debug.Entry(1, "________________________________________");
            }

            /* The Debug.Entries appear to indicate that this code is redundant.
             * Keeping it here in case.
             * 
            // Check if player has the exoframe
            if (player.HasPart<MassiveExoframe>())
            {
                // Cycle the player's inventory and equipped items
                foreach (GameObject item in player.GetInventoryAndEquipment())
                {
                    if (ItemModding.ModificationApplicable("ModGigantic", item) 
                        && !item.HasPart<ModGigantic>()
                        && !item.HasTag("DynamicObjectsTable:TradeGoods")
                        && !item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        ItemModding.ApplyModification(item, "ModGigantic");
                        player.AutoEquip(item);
                    }
                }
            }
            */

        }
    }
}