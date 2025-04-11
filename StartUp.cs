using System.Collections.Generic;

using XRL;
using XRL.Core;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [PlayerMutator]
    public class GigantifyStartingLoadout : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            Debug.Header(3, "GigantifyStartingLoadout", $"mutate(GameObject player: {player.DebugName})");

            if (player.HasPart<GigantismPlus>())
            {
                player.GigantifyInventory(EnableGiganticStartingGear, EnableGiganticStartingGear_Grenades);
            }

            Debug.Footer(3, "GigantifyStartingLoadout", $"mutate(GameObject player: {player.DebugName})");
        }
    }

    public class Gigantic_ModEntry_Adjustments
    {
        public static void AdjustGiganticModifier()
        {
            bool ShouldDerarify = SelectGiganticDerarification;
            bool ShouldGiganticTinkerable = SelectGiganticTinkering;
            Debug.Entry(3, "@ GiganticModifierAdjustments.AdjustGiganticModifier()", Indent: 1);

            Debug.Entry(4, "? if (!ShouldDerarify && !ShouldGiganticTinkerable)", Indent: 2);
            if (!ShouldDerarify && !ShouldGiganticTinkerable)
            {
                Debug.Entry(3, $"ShouldDerarify: {ShouldDerarify} | ShouldGiganticTinkerable: {ShouldGiganticTinkerable}", Indent: 3);
                Debug.Entry(3, "No action necessary, Aborting", Indent: 3);
                Debug.Entry(4, "x if (!ShouldDerarify && !ShouldGiganticTinkerable) ?//", Indent: 2);
                goto Exit;
            }
            Debug.Entry(3, $"+ ShouldDerarify: {ShouldDerarify} | ShouldGiganticTinkerable: {ShouldGiganticTinkerable}", Indent: 3);
            Debug.Entry(4, "x if (!ShouldDerarify && !ShouldGiganticTinkerable) ?//", Indent: 2);

            Debug.Entry(3, "Attempting ModList adjustment process", Indent: 2);
            Debug.Entry(4, "> foreach (ModEntry mod in ModificationFactory.ModList)", Indent: 2);
            Debug.Divider(4, "-", Count: 25, Indent: 2);
            // find the gigantic modifier ModEntry in the ModList
            foreach (ModEntry mod in ModificationFactory.ModList)
            {
                string ModPart = mod.Part;
                Debug.LoopItem(4, $"{ModPart}", Indent: 3);
                if (mod.Part == "ModGigantic")
                {
                    Debug.DiveIn(3, $"{ModPart}: Found", Indent: 3);
                    // should the rarity be adjusted? 
                    // - change the rarity from R2 (3) to R (2)
                    Debug.Entry(4, "? if (ShouldDerarify)", Indent: 4);
                    if (ShouldDerarify)
                    {
                        Debug.Entry(4, "+ Should", Indent: 5);
                        mod.Rarity = 2;
                        Debug.Entry(3, ModPart, "Rarity is R (decreased)", Indent: 5);
                    }
                    else
                    {
                        Debug.Entry(4, "- Shouldn't", Indent: 5);
                        mod.Rarity = 3;
                        Debug.Entry(3, ModPart, "Rarity is R2 (default)", Indent: 5);
                    }
                    Debug.Entry(4, "x if (ShouldDerarify) ?//", Indent: 4);

                    // should tinkering be allowed? 
                    // - change the tinkerability and add it to the list of recipes
                    Debug.Entry(4, "? if (ShouldGiganticTinkerable)", Indent: 4);
                    if (ShouldGiganticTinkerable)
                    {
                        Debug.Entry(4, "+ Should", Indent: 5);
                        mod.TinkerAllowed = true;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Enabled]", Indent: 5);

                        // Modifiers can actually be set to require an additional ingredient.
                        // mod.TinkerIngredient = "Torch";
                    }
                    else
                    {
                        Debug.Entry(4, "- Shouldn't", Indent: 5);
                        mod.TinkerAllowed = false;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Disabled] (default)", Indent: 5);
                    }
                    Debug.Entry(4, "x if (ShouldDerarify) ?//", Indent: 4);

                    Debug.Entry(3, "Reinitialising TinkerData._TinkerRecipes", Indent: 4);
                    // this is a workaround for what I'm sure is a more straightforward and simple solution
                    // - after adjusting the ModEntry to be tinkerable, it needs to be added to the list of recipes
                    // - flushing the list of recipes and then requesting the list uses an internal "get" function
                    //   that cycles all the TinkerData and ModEntries and adds them to the TinkerRecipes list
                    // - only works if you flush it first since the "get" function checks if the _list is empty first
                    //   and if it isn't just returns it.
                    // - it's probably NOT good, and could pose compatability issues with other mods if they do things
                    //   post HandsBlueprint pre-load, but I'm not nearly experienced enough to know what issues exactly.
                    TinkerData._TinkerRecipes.RemoveAll(r => r != null); 
                    Debug.Entry(2, "TinkerData._TinkerRecipes.RemoveAll(r => r != null);", Indent: 5);

                    List<TinkerData> reinitialise = TinkerData.TinkerRecipes; 
                    Debug.Entry(4, "List<TinkerData> reinitialise = TinkerData.TinkerRecipes;", Indent: 5);

                    reinitialise = null; 
                    Debug.Entry(4, "reinitialise = null;", Indent: 5);

                    Debug.Entry(4, "No Further Actions Required", "Exiting ModList", Indent: 4);
                    Debug.DiveOut(3, $"{ModPart}", Indent: 3);
                    break;
                }
            }
            Debug.Divider(4, "-", Count: 25, Indent: 2);
            Debug.Entry(4, "x foreach (ModEntry mod in ModificationFactory.ModList) >//", Indent: 2);
            Debug.Entry(3, "ModList exited, adjustment process finished", Indent: 2);
            Exit:
            Debug.Entry(3, "x GiganticModifierAdjustments.AdjustGiganticModifier() @//", Indent: 1);
        }
    } //!--- public class Gigantic_ModEntry_Adjustments

    [HasModSensitiveStaticCache]
    public static class GigantismPlusModBasedInitialiser
    {
        [ModSensitiveCacheInit]
        public static void AdditionalSetup()
        {
            // Called at game startup and whenever mod configuration changes
        }
    } //!-- public static class GigantismPlusModBasedInitialiser

    [HasGameBasedStaticCache]
    public static class GigantismPlusGameBasedInitialiser
    {
        public static bool GameLevelEventsRegistered = false;

        [GameBasedCacheInit]
        public static void AdditionalSetup()
        {
            // Called once when world is first generated.
            Debug.Header(3, 
                $"{nameof(GigantismPlusGameBasedInitialiser)}", 
                $"{nameof(AdditionalSetup)}()");

            Debug.Entry(4, $"Option EnableManagedVanillaMutationsCurrent", $"{EnableManagedVanillaMutationsCurrent}", Indent: 1);
            Debug.Entry(4, $"Before EnableManagedVanillaMutations", $"{EnableManagedVanillaMutations}", Indent: 1);
            EnableManagedVanillaMutations = 
                EnableManagedVanillaMutations != null 
                ? EnableManagedVanillaMutations 
                : EnableManagedVanillaMutationsCurrent;
            Debug.Entry(4, $"After  EnableManagedVanillaMutations", $"{EnableManagedVanillaMutations}", Indent: 1);
            ManagedVanillaMutationOptionHandler();

            if (!GameLevelEventsRegistered)
            {
                GameLevelEventsRegistered = RegisterGameLevelEventHandlers();
            }

            Debug.Footer(3, 
                $"{nameof(GigantismPlusGameBasedInitialiser)}", 
                $"{nameof(AdditionalSetup)}()");
        }
    } //!-- public static class GigantismPlusGameBasedInitialiser

    [PlayerMutator]
    public class GigantismPlusOnPlayerLoad : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            // Gets called once when the player is first generated
            Debug.Header(3, 
                $"{nameof(GigantismPlusOnPlayerLoad)}", 
                $"{nameof(mutate)}(GameObject player: {player.DebugName})");

            Debug.Entry(4, $"Option EnableManagedVanillaMutationsCurrent", $"{EnableManagedVanillaMutationsCurrent}", Indent: 1);
            Debug.Entry(4, $"Before EnableManagedVanillaMutations", $"{EnableManagedVanillaMutations}", Indent: 1);
            if (The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla"))
            {
                Debug.Entry(4, 
                    $"The.Game.HasStringGameState(\"Option_GigantismPlus_ManagedVanilla\")", 
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}", 
                    Indent: 2);
                EnableManagedVanillaMutations = The.Game.GetStringGameState("Option_GigantismPlus_ManagedVanilla").EqualsNoCase("Yes");
                Debug.Entry(4,
                    $"EnableManagedVanillaMutations set to",
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                    Indent: 2);
            }
            else
            {
                Debug.Entry(4,
                    $"The.Game.HasStringGameState(\"Option_GigantismPlus_ManagedVanilla\")",
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                    Indent: 2);
                The.Game.SetStringGameState("Option_GigantismPlus_ManagedVanilla", (bool)EnableManagedVanillaMutations ? "Yes" : "No");
                Debug.Entry(4,
                    $"GameState \"Option_GigantismPlus_ManagedVanilla\" set to",
                    $"{((bool)EnableManagedVanillaMutations ? "Yes" : "No")}",
                    Indent: 2);
            }
            Debug.Entry(4, 
                $"After  GameState \"Option_GigantismPlus_ManagedVanilla\"", 
                $"{The.Game.GetStringGameState("Option_GigantismPlus_ManagedVanilla")}", 
                Indent: 1);
            Debug.Entry(4, $"After  EnableManagedVanillaMutations", $"{EnableManagedVanillaMutations}", Indent: 1);

            ManagedVanillaMutationOptionHandler();

            Debug.Footer(3, 
                $"{nameof(GigantismPlusOnPlayerLoad)}",
                $"{nameof(mutate)}(GameObject player: {player.DebugName})");
        }
    } //!-- public class GigantismPlusOnPlayerLoad : IPlayerMutator

    [HasCallAfterGameLoaded]
    public class GigantismPlusOnLoadGameHandler
    {
        [CallAfterGameLoaded]
        public static void OnLoadGameCallback()
        {
            // Gets called every time the game is loaded but not during generation
            Debug.Header(3,
                $"{nameof(GigantismPlusOnLoadGameHandler)}",
                $"{nameof(OnLoadGameCallback)}()");

            if (!GigantismPlusGameBasedInitialiser.GameLevelEventsRegistered)
            {
                RegisterGameLevelEventHandlers();
            }

            Debug.Entry(4, $"Option EnableManagedVanillaMutationsCurrent", $"{EnableManagedVanillaMutationsCurrent}", Indent: 1);
            Debug.Entry(4, $"Before EnableManagedVanillaMutations", $"{EnableManagedVanillaMutations}", Indent: 1);
            if (The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla"))
            {
                Debug.Entry(4,
                    $"The.Game.HasStringGameState(\"Option_GigantismPlus_ManagedVanilla\")",
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                    Indent: 2);
                EnableManagedVanillaMutations = The.Game.GetStringGameState("Option_GigantismPlus_ManagedVanilla").EqualsNoCase("Yes");
                Debug.Entry(4,
                    $"EnableManagedVanillaMutations set to",
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                    Indent: 2);
            }
            else
            {
                Debug.Entry(4,
                    $"The.Game.HasStringGameState(\"Option_GigantismPlus_ManagedVanilla\")",
                    $"{The.Game.HasStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                    Indent: 2);
                The.Game.SetStringGameState("Option_GigantismPlus_ManagedVanilla", (bool)EnableManagedVanillaMutations ? "Yes" : "No");
                Debug.Entry(4,
                    $"GameState \"Option_GigantismPlus_ManagedVanilla\" set to",
                    $"{((bool)EnableManagedVanillaMutations ? "Yes" : "No")}",
                    Indent: 2);
            }
            Debug.Entry(4,
                $"After  GameState \"Option_GigantismPlus_ManagedVanilla\"",
                $"{The.Game.GetStringGameState("Option_GigantismPlus_ManagedVanilla")}",
                Indent: 1);

            ManagedVanillaMutationOptionHandler();

            Debug.Footer(3,
                $"{nameof(GigantismPlusOnLoadGameHandler)}",
                $"{nameof(OnLoadGameCallback)}()");
        }
    } //!-- public class GigantismPlusOnLoadGameHandler
}
