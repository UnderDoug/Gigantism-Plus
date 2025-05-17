using System;
using System.Collections.Generic;

using XRL;
using XRL.UI;
using XRL.Core;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;
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

    public static class Gigantic_ModEntry_Adjustments
    {
        public static void AdjustGiganticModifier()
        {
            bool ShouldDerarify = SelectGiganticDerarification;
            bool ShouldGiganticTinkerable = SelectGiganticTinkering;
            int indent = Debug.LastIndent;
            Debug.Entry(3,
                $"@ {nameof(Gigantic_ModEntry_Adjustments)}."
                + $"{nameof(AdjustGiganticModifier)}()", 
                Indent: indent++);

            ModEntry modGigantic = ModificationFactory.ModsByPart[nameof(ModGigantic)];

            bool derarified = Derarify(modGigantic, ShouldDerarify);
            Debug.LoopItem(2, $"{nameof(derarified)}", (ShouldDerarify && derarified) ? "derarification not necessary" : "",
                Good: ShouldDerarify && derarified, Indent: indent);

            bool tinkerabilityAdjusted = AdjustTinkerability(modGigantic, ShouldGiganticTinkerable);
            Debug.LoopItem(2, $"{nameof(tinkerabilityAdjusted)}", (ShouldGiganticTinkerable && tinkerabilityAdjusted) ? "tinkerability adjustment not necessary" : "",
                Good: ShouldGiganticTinkerable && tinkerabilityAdjusted, Indent: indent);

            ReinitializeTinkerData();

            Debug.Entry(3, $"x {nameof(Gigantic_ModEntry_Adjustments)}.{nameof(AdjustGiganticModifier)}() @//", Indent: --indent);
        }

        public static bool Derarify(ModEntry ModGiganticEntry, bool ShouldDerarify = true)
        {
            string modPart = ModGiganticEntry.Part;
            int was = ModGiganticEntry.Rarity;
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"* {nameof(Derarify)}"
                + $"({nameof(ModEntry)}: {ModGiganticEntry.Part}, "
                + $"{nameof(ShouldDerarify)}: {ShouldDerarify})",
                Indent: indent++);

            Debug.Entry(4, $"? if ({nameof(ShouldDerarify)})", Indent: indent++);
            if (ShouldDerarify)
            {
                Debug.CheckYeh(4, $"Should", Indent: indent);
                ModGiganticEntry.Rarity = 2;
                Debug.Entry(2, modPart, $"Rarity is R [decreased] (default)", Indent: indent);
            }
            else
            {
                Debug.CheckNah(4, $"Shouldn't", Indent: indent);
                ModGiganticEntry.Rarity = 3;
                Debug.Entry(2, modPart, $"Rarity is R2 [vanilla]", Indent: indent);

            }
            Debug.Entry(4, $"x if ({nameof(ShouldDerarify)}) ?//", Indent: --indent);

            Debug.Entry(4,
                $"x {nameof(Derarify)}"
                + $"(ModEntry: {ModGiganticEntry.Part}, "
                + $"{nameof(ShouldDerarify)}: {ShouldDerarify}) *//",
                Indent: --indent);
            return was != ModGiganticEntry.Rarity;
        }

        public static bool AdjustTinkerability(ModEntry ModGiganticEntry, bool ShouldGiganticTinkerable = true)
        {
            string modPart = ModGiganticEntry.Part;
            bool was = ModGiganticEntry.TinkerAllowed;
            int indent = Debug.LastIndent;

            Debug.Entry(4, 
                $"* {nameof(AdjustTinkerability)}"
                + $"(ModEntry: {ModGiganticEntry.Part}, " 
                + $"{nameof(ShouldGiganticTinkerable)}: {ShouldGiganticTinkerable})", 
                Indent: indent++);

            Debug.Entry(4, $"? if ({nameof(ShouldGiganticTinkerable)})", Indent: indent++);
            if (ShouldGiganticTinkerable)
            {
                Debug.CheckYeh(4, $"Should", Indent: indent);
                ModGiganticEntry.TinkerAllowed = true;
                Debug.Entry(2, modPart, $"Gigantic tinkering [Enabled] (default)", Indent: indent);

                // Modifiers can actually be set to require an additional ingredient.
                // mod.TinkerIngredient = "Torch";
            }
            else
            {
                Debug.CheckNah(4, $"Shouldn't", Indent: indent);
                ModGiganticEntry.TinkerAllowed = false;
                Debug.Entry(2, modPart, $"Gigantic tinkering [Disabled]", Indent: indent);

            }
            Debug.Entry(4, $"x if ({nameof(ShouldGiganticTinkerable)}) ?//", Indent: --indent);

            Debug.Entry(4,
                $"x {nameof(AdjustTinkerability)}"
                + $"(ModEntry: {ModGiganticEntry.Part}, "
                + $"{nameof(ShouldGiganticTinkerable)}: {ShouldGiganticTinkerable}) *//",
                Indent: --indent);
            return was != ModGiganticEntry.TinkerAllowed;
        }

        public static void ReinitializeTinkerData()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(ReinitializeTinkerData)}()",
                Indent: indent++);

            Debug.Entry(3, $"Reinitialising {nameof(TinkerData)}.{nameof(TinkerData._TinkerRecipes)}...", Indent: indent);
            // this is a workaround for what I'm sure is a more straightforward and simple solution
            // - after adjusting the ModEntry to be tinkerable, it needs to be added to the list of recipes
            // - flushing the list of recipes and then requesting the list uses an internal "get" function
            //   that cycles all the TinkerData and ModEntries and adds them to the TinkerRecipes list
            // - only works if you flush it first since the "get" function checks if the _list is empty first
            //   and if it isn't just returns it.
            // - it's probably NOT good, and could pose compatability issues with other mods if they do things
            //   post HandsBlueprint pre-load, but I'm not nearly experienced enough to know what issues exactly.
            TinkerData._TinkerRecipes.RemoveAll(r => r != null);
            Debug.Entry(2, $"{nameof(TinkerData)}.{nameof(TinkerData._TinkerRecipes)}.RemoveAll(r => r != null)", Indent: indent);

            List<TinkerData> reinitialise = TinkerData.TinkerRecipes;
            Debug.Entry(4, $"{nameof(List<TinkerData>)} {nameof(reinitialise)} = {nameof(TinkerData)}.{nameof(TinkerData._TinkerRecipes)}", Indent: indent);

            Debug.Entry(4,
                $"x {nameof(ReinitializeTinkerData)}() *//",
                Indent: --indent);
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
            
            // ManagedVanillaMutationOptionHandler();

            if (!GameLevelEventsRegistered)
            {
                GameLevelEventsRegistered = RegisterGameLevelEventHandlers();
            }

            Gigantic_ModEntry_Adjustments.AdjustGiganticModifier();

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

            // ManagedVanillaMutationOptionHandler();

            if (player.TryGetPart(out Wrassler wrassler) && !wrassler.KnowsChairs)
            {
                GameObject foldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (foldingChair.TryGetPart(out Examiner foldingChairEx) && !foldingChair.Understood())
                {
                    foldingChairEx.MakeUnderstood(ShowMessage: false);
                    // Popup.Show($"You're struck with a sudden, intimate understanding of {foldingChair.GetPluralName()}.");
                }
                wrassler.KnowsChairs = foldingChair.Understood();
                foldingChair.Obliterate();
            }
            
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

            // ManagedVanillaMutationOptionHandler();

            Gigantic_ModEntry_Adjustments.AdjustGiganticModifier();

            Debug.Footer(3,
                $"{nameof(GigantismPlusOnLoadGameHandler)}",
                $"{nameof(OnLoadGameCallback)}()");
        }
    } //!-- public class GigantismPlusOnLoadGameHandler
}
