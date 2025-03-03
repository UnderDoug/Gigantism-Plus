using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.Tinkering;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [HarmonyPatch(typeof(GameObject))]
    public static class PseudoGiganticCreature_GameObject_Patches
    {
        // Goal is to simulate being Gigantic for the purposes of calculating body weight, if the GameObject in question is PseudoGigantic

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeightPrefix(ref GameObject __state, GameObject __instance)
        {
            // --instance gives you the instantiated object on which the original method call is happening,
            // __state lets you keep stuff between Pre- and Postfixes

            __state = __instance; // make the transferable object the current instance.
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature) 
            {
                // is the GameObject PseudoGigantic but not Gigantic
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
                Debug.Entry(3, "GameObject.GetBodyWeight() > PseudoGigantic not Gigantic");
                __state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)
                Debug.Entry(2, "Trying to be Heavy and PseudoGigantic");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeightPostfix(GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                Debug.Entry(3, "GameObject.GetBodyWeight() > PseudoGigantic and Gigantic");
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                Debug.Entry(2, "Should be Heavy and PseudoGigantic\n");
            }
        }
    } //!-- public static class PseudoGiganticCreature_GameObject_Patches


    // Why harmony for this one when it's an available event?
    // -- in the event that this hard-coded element is adjusted (such as the increase amount),
    //    this just ensures the "vanilla" behaviour is preserved by "masking" as Gigantic for the check.

    // Goal is to simulate being Gigantic for the purposes of calculating carry capacity, if the GameObject in question is PseudoGigantic

    [HarmonyPatch(typeof(GetMaxCarriedWeightEvent))]
    public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetMaxCarryWeightPrefix(ref GameObject Object, ref GameObject __state) 
        {
            // Object matches the paramater of the original,
            // __state lets you keep stuff between Pre- and Postfixes

            __state = Object;
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature)
            {
                // is the GameObject PseudoGigantic but not Gigantic
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
                Debug.Entry(3, "GetMaxCarriedWeightEvent.GetFor > PseudoGigantic not Gigantic");
                __state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)
                Debug.Entry(2, "Trying to have Carry Capacity and PseudoGigantic\n");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetMaxCarryWeightPostfix(GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                Debug.Entry(3, "GetMaxCarriedWeightEvent.GetFor() > PseudoGigantic and Gigantic");
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                Debug.Entry(2, "Should have Carry Capacity and PseudoGigantic");
            }
        }

    } //!-- public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches

    [HarmonyPatch(typeof(Body))]
    public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Body.RegenerateDefaultEquipment))]
        static void RegenerateDefaultEquipmentPrefix(ref GameObject __state,Body __instance)
        {
            // Object matches the paramater of the original,
            // __state lets you keep stuff between Pre- and Postfixes

            __state = __instance.ParentObject;
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature)
            {
                // is the GameObject PseudoGigantic but not Gigantic
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
                Debug.Entry(3, "Body.RegenerateDefaultEquipment() > PseudoGigantic not Gigantic");
                __state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)
                Debug.Entry(2, "Trying to generate gigantic natural equipment while PseudoGigantic\n");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Body.RegenerateDefaultEquipment))]
        static void RegenerateDefaultEquipmentPostfix(GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                Debug.Entry(3, "Body.RegenerateDefaultEquipment() > PseudoGigantic and Gigantic");
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                Debug.Entry(3, "Should have generated gigantic natural equipment while PseudoGigantic\n");
            }
        }

    } //!-- public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches

    // Why harmony for this one when it's an available event?
    // -- this keeps the behaviour consistent with vanilla but hijacks the value
    //    outside of a vanilla getting a significant rework, this should remain compatable.
    [HarmonyPatch]
    public static class ModGigantic_DisplayName_Shader
    {
        // goal display the SizeAdjective gigantic with its associated shader.

        static void GetDisplayNameEventOverride(GetDisplayNameEvent E, string Adjective, int Priority, bool IncludeProperName = false)
        {
            if (E.Object.HasProperName && !IncludeProperName) return; // skip for Proper Named items, unless including them.
            if (E.Object.HasTagOrProperty("ModGiganticNoDisplayName")) return; // skip for items that explicitly hide the adjective
            if (E.Object.HasTagOrProperty("ModGiganticNoUnknownDisplayName")) return; // skip for unknown items that explicitly hide the adjective
            if (!E.Understood()) return; // skip items not understood by the player

            if (E.DB.SizeAdjective == Adjective && E.DB.SizeAdjectivePriority == Priority)
            {
                // The base event runs every game tick for equipped range weapons.
                // possibly due to the item being displayed in the UI (bottom right)
                // Any form of output here will completely clog up the logs.

                Adjective = "{{gigantic|" + Adjective + "}}";

                E.DB.SizeAdjective = Adjective;
                
                // Debug.Entry(E.DB.GetDebugInfo());
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModGigantic), "HandleEvent", new Type[] { typeof(GetDisplayNameEvent) })]
        static void ModGiganticPatch(GetDisplayNameEvent E)
        {
            GetDisplayNameEventOverride(E, "gigantic", 30, true);
        }

    } //!--- public static class ModGiganticDisplayName_Shader

    [HarmonyPatch(typeof(MeleeWeapon))]
    public static class MaxStrengthBonus_Display_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.GetSimplifiedStats))]
        static bool GetSimplifiedStatsPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.GetDetailedStats))]
        static bool GetDetailedStatsPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.AdjustBonusCap))]
        static bool AdjustBonusCapPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

    } //!-- public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches

    [HarmonyPatch]
    public static class GiganticCreature_Implant_SmallCybernetics
    {
        // goal force gigantic creatures who are eligible for cybernetics to hunch over when using becomming nooks.
        // failure to do so really freaks out the cybernetic in question due to being "too small".
        // you end up with it installed, but the equipment copy ends up in your inventory.
        static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)
        {
            Debug.Entry(3, "* static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = false) called");

            Debug.Entry(3, "- Checking the Actor exists, is a True Kin, and has GigantismPlus");
            Debug.Entry(4, "* if (Actor != null && actor.IsTrueKin && Actor.TryGetPart<XRL.World.Parts.Mutation.GigantismPlus>(out var gigantism)");
            if (Actor != null && Actor.IsTrueKin() && Actor.TryGetPart(out GigantismPlus gigantism))
            {
                Debug.Entry(3, "-- Actor exists, is a True Kin, and has GigantismPlus");
                if (gigantism != null && (gigantism.UnHunchImmediately || IsStart))
                {
                    Debug.Entry(3, "-- GigantismPlus instantiated");
                    Debug.Entry(3, "-- Either UnHunchImmediately is set to true or this is the Start of the process.");
                    Debug.Entry(3, "-- Making Hunch Over free, Sending Command to Hunch Over");

                    gigantism.IsHunchFree = true;
                    gigantism.UnHunchImmediately = IsStart;
                    CommandEvent.Send(Actor, GigantismPlus.HUNCH_OVER_COMMAND_NAME);

                    if (IsStart)
                    {
                        Debug.Entry(3, "-- Popping up Popup");
                        Popup.Show("You peer down into the interface.");
                    }
                }
            } 
            else Debug.Entry(3, "x one or more of: actor doesn't exists, isn't a True Kin, or lacks GigantismPlus");
        } //!-- static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(InventoryActionEvent) })]
        static bool InventoryActionEventPrefix(InventoryActionEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E)");
            if (E.Command == "InterfaceWithBecomingNook")
            {
                Debug.Entry(4, $"E.Command is {E.Command}");

                CyberneticsTerminal2_ToggleHunched(E.Actor);

                Debug.Entry(3, "Deferring to patched method");
            }
            return true;
        } //!-- static bool InventoryActionEventPrefix(InventoryActionEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(InventoryActionEvent) })]
        static void InventoryActionEventPostfix(InventoryActionEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E)");
            if (E.Command == "InterfaceWithBecomingNook")
            {
                Debug.Entry(4, $"E.Command is {E.Command}");

                CyberneticsTerminal2_ToggleHunched(E.Actor, false);

                Debug.Entry(3, "x CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E) ]//");
            }
        } //!-- static void InventoryActionEventPostfix(InventoryActionEvent E)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2_ToggleHunched(E.Actor);

            Debug.Entry(3, "Deferring to patched method");
            return true;
        } //!-- static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2_ToggleHunched(E.Actor, false);

            Debug.Entry(3, "x CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E) ]//");
        } //!-- static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)

    } //!--- public static class ModGiganticDisplayName_Shader


    [HarmonyPatch(typeof(Tinkering_Disassemble))]
    public static class PreventCyberneticsBeingDisassembled
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Tinkering_Disassemble.CanBeConsideredScrap))]
        static void CanBeConsideredScrapPostfix(ref GameObject obj, ref bool __result)
        {
            if (obj != null && obj.HasPart<CyberneticsBaseItem>())
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(BurrowingClaws))]
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipmentPrefix(BurrowingClaws __instance, Body body)
        {
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3,  $"[HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3,  $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(3, $"Checking for this Mutation", Indent: 1);
            Debug.Entry(3, $"* if (actor.ParentObject.HasPart<BurrowingClaws>())", Indent: 1);
            if (!actor.HasPart<BurrowingClaws>())
            {
                Debug.Entry(3, $"- BurrowingClaws Mutation not present", Indent: 2);
                Debug.Footer(3, $"BurrowingClaws_Patches", "OnRegenerateDefaultEquipmentPrefix(body)");
                return false;
            }
            Debug.Entry(3, $"+ BurrowingClaws Mutation is present", Indent: 2);
            Debug.Entry(3, $"Proceeding", Indent: 1);

            if (body == null) return true;

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    ItemModding.ApplyModification(part.DefaultBehavior, "ModBurrowingNaturalWeapon", Actor: actor);

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method
        }
    } //!-- public static class BurrowingClaws_Patches

    [HarmonyPatch(typeof(Crystallinity))]
    public static class Crystallinity_Patches
    {
        [HarmonyPrefix]  
        [HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipmentPrefix(Crystallinity __instance, Body body)
        {
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3,  $"[HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3,  $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(3, $"Checking for this Mutation", Indent: 1);
            Debug.Entry(3, $"* if (__instance.ParentObject.HasPart<Crystallinity>())", Indent: 1);
            if (!actor.HasPart<Crystallinity>())
            {
                Debug.Entry(3, $"- Crystallinity Mutation not present", Indent: 2);
                Debug.Footer(3, $"Crystallinity_Patches", "OnRegenerateDefaultEquipmentPrefix(body)");
                return false;
            }
            Debug.Entry(3, $"+ Crystallinity Mutation is present", Indent: 2);
            Debug.Entry(3, $"Proceeding", Indent: 1);

            if (body == null) return true;

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    ItemModding.ApplyModification(part.DefaultBehavior, "ModCrystallineNaturalWeapon", Actor: actor);

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method
        }
    } //!-- public static class Crystallinity_Patches

    [HarmonyPatch(typeof(Crystallinity))]
    public static class Crystallinity_RefractChance_Patches
    {
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Crystallinity.Mutate))]
        static void MutatePrefix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())
            {
                // Wait for the original method to create the RefractLight part
                // Then in the postfix we'll modify its chance
                __instance.RefractAdded = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crystallinity.Mutate))]
        static void MutatePostfix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<GigantismPlus>() && __instance.RefractAdded)
            {
                var refractPart = GO.GetPart<RefractLight>();
                if (refractPart != null)
                {
                    refractPart.Chance = 35; // Increase from default 25 to 35
                }
            }
        }
    } //!-- public static class Crystallinity_RefractChance_Patches

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity))]
    public static class Crystallinity_LevelText_Patches
    {
        // Modify the Crystallinity mutation level text to include the GigantismPlus bonus
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crystallinity.GetLevelText))]
        static void GetLevelTextPostfix(Crystallinity __instance, ref string __result)
        {
            if (__instance.ParentObject.HasPart<GigantismPlus>())
            {
                // Replace the original 25% text with our modified version
                __result = __result.Replace(
                    "25% chance to refract light-based attacks",
                    "35% chance to refract light-based attacks (25% base chance {{rules|+}} 10% from {{gigantism|Gigantism}} ({{r|D}}))"
                );
            }
        }
    }//!-- public static class Crystallinity_LevelText_Patches

    [HarmonyPatch(typeof(GameObjectFactory))]
    public static class GiganticCreature_Inventory_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateObject", new Type[] { 
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

} //!-- namespace Mods.GigantismPlus.HarmonyPatches
