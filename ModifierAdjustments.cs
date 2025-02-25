using System;
using System.Collections.Generic;
using XRL; 
using XRL.World;
using XRL.World.Tinkering;
using XRL.World.Parts;

namespace Mods.GigantismPlus
{
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