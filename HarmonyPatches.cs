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
using Mods.GigantismPlus;
using static Mods.GigantismPlus.HelperMethods;

namespace Mods.GigantismPlus.HarmonyPatches
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
            // __state lets you keep stuff between Pre- and Postfixes (might be redundant for this one)

            __state = __instance; // make the transferable object the current instance.
            bool IsPretendBig = __state.HasPart("CompactedExoframe") || __state.HasPart<PseudoGigantism>();
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

            bool IsPretendBig = __state.HasPart("CompactedExoframe") || __state.HasPart<PseudoGigantism>();
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
            // __state lets you keep stuff between Pre- and Postfixes (might be redundant for this one)

            __state = Object;
            bool IsPretendBig = __state.HasPart("CompactedExoframe") || __state.HasPart<PseudoGigantism>();
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

            bool IsPretendBig = __state.HasPart("CompactedExoframe") || __state.HasPart<PseudoGigantism>();
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
            // __state lets you keep stuff between Pre- and Postfixes (might be redundant for this one)

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

            bool IsPretendBig = __state.HasPart("CompactedExoframe") || __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                Debug.Entry(3, "Body.RegenerateDefaultEquipment() > PseudoGigantic and Gigantic");
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                Debug.Entry(2, "Should have generated gigantic natural equipment while PseudoGigantic\n");
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
            if (E.Object.HasTagOrProperty("ModGiganticNoUnknownDisplayName")) return; // skip for unknown items that explicitly hide the advective
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
            if (Actor != null && Actor.IsTrueKin() && Actor.TryGetPart<XRL.World.Parts.Mutation.GigantismPlus>(out var gigantism))
            {
                Debug.Entry(3, "-- Actor exists, is a True Kin, and has GigantismPlus");
                if (gigantism != null && (gigantism.UnHunchImmediately || IsStart))
                {
                    Debug.Entry(3, "-- GigantismPlus instantiated");
                    Debug.Entry(3, "-- Either UnHunchImmediately is set to true or this is the Start of the process.");
                    Debug.Entry(3, "-- Making Hunch Over free, Sending Command to Hunch Over");

                    gigantism.IsHunchFree = true;
                    gigantism.UnHunchImmediately = IsStart;
                    CommandEvent.Send(Actor, XRL.World.Parts.Mutation.GigantismPlus.HUNCH_OVER_COMMAND_NAME);

                    if (IsStart)
                    {
                        Debug.Entry(3, "-- Popping up Popup");
                        Popup.Show("You peer down into the interface.");
                    }
                }
            } 
            else Debug.Entry(3, "x one or more of: actor doesn't exists, isn't a True Kin, or lacks GigantismPlus");
        }

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
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2_ToggleHunched(E.Actor, false);

            Debug.Entry(3, "x CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E) ]//");
        }

    } //!--- public static class ModGiganticDisplayName_Shader

    //XRL.World.Parts.Skill.Tinkering_Disassemble
    //public static bool CanBeConsideredScrap(GameObject obj)
    //{

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

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.BurrowingClaws))]
    public static class BurrowingClaws_Patches
    {
        public static int GetBurrowingDieSize(int Level)
        {
            Debug.Entry(4, "* BurrowingClaws_Patches.GetBurrowingDieSize() called", Indent: 4);
            if (Level >= 19) return 12;     // 1d12
            if (Level >= 16) return 10;     // 1d10  
            if (Level >= 13) return  8;     // 1d8
            if (Level >= 10) return  6;     // 1d6
            if (Level >=  7) return  4;     // 1d4
            if (Level >=  4) return  3;     // 1d3
                             return  2;     // 1d2
        }

        public static int GetBurrowingBonusDamage(int Level)
        {
            Debug.Entry(4, "* BurrowingClaws_Patches.GetBurrowingBonusDamage() called", Indent: 4);
            if (Level >= 19) return 6;      // Going from 1d10 to 1d12
            if (Level >= 16) return 5;      // Going from 1d8 to 1d10
            if (Level >= 13) return 4;      // Going from 1d6 to 1d8
            if (Level >= 10) return 3;      // Going from 1d4 to 1d6
            if (Level >=  7) return 2;      // Going from 1d3 to 1d4
            if (Level >=  4) return 1;      // Going from 1d2 to 1d3
                             return 0;      // Base 1d2
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipmentPrefix(BurrowingClaws __instance, Body body)
        {
            Zone InstanceObjectZone = __instance.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3,  $"[HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3,  $"TARGET {__instance.ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(3, $"Checking for this Mutation", Indent: 1);
            Debug.Entry(3, $"* if (__instance.ParentObject.HasPart<BurrowingClaws>())", Indent: 1);
            if (!__instance.ParentObject.HasPart<BurrowingClaws>())
            {
                Debug.Entry(3, $"- BurrowingClaws Mutation not present", Indent: 2);
                Debug.Footer(3, $"BurrowingClaws_Patches", "OnRegenerateDefaultEquipmentPrefix(body)");
                return false;
            }
            Debug.Entry(3, $"+ BurrowingClaws Mutation is present", Indent: 2);
            Debug.Entry(3, $"Proceeding", Indent: 1);

            List<string> NaturalWeaponSupersedingMutations = new List<string>
            {
              //"CyberneticsGiganticExoframe",
                "Crystallinity"
            };

            Debug.Entry(4, $"Exposing superseding mutations loop:", Indent: 1);
            Debug.Entry(4, $"* foreach (string mutation in NaturalWeaponSupersedingMutations)", Indent: 1);
            int SupersededCount = 0;
            foreach (string mutation in NaturalWeaponSupersedingMutations)
            {
                Debug.LoopItem(4, $"Checking for {mutation}", Indent: 2);
                if (__instance.ParentObject.HasPart(mutation))
                {
                    Debug.Entry(4, $"+ Present, increasing SupersededCount: {SupersededCount} to {SupersededCount+1}", Indent: 3);
                    SupersededCount++;
                }
            }

            Debug.Entry(3, $"SupersededCount is {SupersededCount}", Indent: 1);
            bool IsNaturalWeaponSuperseded = SupersededCount > 0;

            Debug.Entry(3, $"Checking for incompatible Mutations or Parts", Indent: 1);
            Debug.Entry(3, $"* if (IsNaturalWeaponSuperseded)", Indent: 1);
            if (IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3,  $"A Superseding Mutation or Part is resent", Indent: 2);
                Debug.Footer(3, $"BurrowingClaws_Patches", "OnRegenerateDefaultEquipmentPrefix()");
                return false;
            }
            Debug.Entry(3, $"No Superseding Mutation or Part is present", Indent: 2);

            bool HasGigantism = __instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>();
            bool HasElongated = __instance.ParentObject.HasPart<ElongatedPaws>();

            int level = __instance.Level;
            string BurrowingClawBlueprintName = BurrowingClaws.OBJECT_BLUEPRINT_NAME;
            string GiganticBlueprintName = "Gigantic";
            string ElongatedBlueprintName = "Elongated";
            string BaseBlueprintName = "BurrowingClaw";
            string blueprintName = "";
            
            Debug.Entry(3, $"[] Generating Stats", Indent: 1);

            int dieCount = 1;
            int dieSize = GetBurrowingDieSize(level); // Base Burrowing from calculation, Gigantism overrides this to 1 in its section.
            int damageBonus = GetBurrowingBonusDamage(level);
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"|^ Starting Stats:", Indent: 2);
            Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 2);
            Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 2);
            Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 2);
            Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 2);
            Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 2);
            Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 2);
            Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 2);

            Debug.Entry(3, "[] Accumulating stats", Indent: 1);

            Debug.Entry(3, "* if (HasGigantism)", Indent: 1);
            if (HasGigantism)
            {
                Debug.DiveIn(3, "+ GigantismPlus Mutation is present", Indent: 2);
                var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();
                
                if (gigantism != null)
                {
                    // Add "Gigantic" Adjective
                    blueprintName += GiganticBlueprintName;

                    // Override dieCount from mutation
                    dieCount = XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieCount(gigantism.Level);

                    // Override dieSize from mutation, Gigantism dieSize + 1 for Burrowing.
                    dieSize = 1 + XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieSize(gigantism.Level);

                    // Add to damageBonus due to being Gigantic (simulates weapon having ModGigantic)
                    damageBonus += 3;

                    // Override maxStrBonus, redundant but good to exert.
                    maxStrBonus = gigantism.FistMaxStrengthBonus;

                    // Add to hitBonus from mutation (add in case something else is giving it too)
                    hitBonus += XRL.World.Parts.Mutation.GigantismPlus.GetFistHitBonus(gigantism.Level);

                    Debug.Entry(3, $"|? blueprintName: {blueprintName}", Indent: 3);
                    Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 3);
                    Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 3);
                    Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 3);
                    Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 3);
                    Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 3);
                    Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 3);
                    Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 3);
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate gigantism part", Indent: 3);
                }
                Debug.DiveOut(3, "x if (HasGigantism) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, "- GigantismPlus Mutation not present", Indent: 2);
            }

            Debug.Entry(4, "* if (HasElongated)", Indent: 2);
            if (HasElongated)
            {
                Debug.DiveIn(3, "+ ElongatedPaws Mutation is present", Indent: 2);
                var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();
                if (elongated != null)
                {
                    // Add "Elongated" Adjective
                    blueprintName += ElongatedBlueprintName;

                    // Add to dieSize if no Gigantism
                    dieSize += HasGigantism ? 0 : 1;

                    // add damage
                    damageBonus += elongated.ElongatedBonusDamage;

                    Debug.Entry(3, $"|? blueprintName: {blueprintName}", Indent: 3);
                    Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 3);
                    Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 3);
                    Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 3);
                    Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 3);
                    Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 3);
                    Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 3);
                    Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 3);
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate gigantism part", Indent: 3);
                }
                Debug.DiveOut(3, "x if (HasElongated) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, "- ElongatedPaws Mutation not present", Indent: 2);
            }

            Debug.Entry(3, "[] Finished accumulating stats", Indent: 1);

            blueprintName += BaseBlueprintName;

            if (!HasGigantism && !HasElongated) blueprintName = BurrowingClawBlueprintName;

            Debug.Divider(3, "_", 25, Indent: 2);
            Debug.Entry(3, $"|: blueprintName: {blueprintName}", 2);
            Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 2);
            Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 2);
            Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 2);
            Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 2);
            Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 2);
            Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 2);
            Debug.Divider(3, "V", 25, Indent: 2);

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList<BodyPart>();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    Debug.Entry(3, "Sending to assignment method", Indent: 3);
                    AddAccumulatedNaturalEquipmentTo(
                        Creature: __instance.ParentObject,
                        Part: part,
                        BlueprintName: blueprintName,
                        OldDefaultBehavior: part.DefaultBehavior,
                        BaseDamage: WeaponDamageString(dieCount, dieSize, damageBonus),
                        MaxStrBonus: maxStrBonus,
                        HitBonus: hitBonus,
                        AssigningMutation: "Burrowing Claws"
                        );

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method
        }
    } //!-- public static class BurrowingClaws_Patches

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity))]
    public static class Crystallinity_Patches
    {
        [HarmonyPrefix]  
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.Crystallinity.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipmentPrefix(Crystallinity __instance, Body body)
        {
            Zone InstanceObjectZone = __instance.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3,  $"[HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3,  $"TARGET {__instance.ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(3, $"Checking for this Mutation", Indent: 1);
            Debug.Entry(3, $"* if (__instance.ParentObject.HasPart<Crystallinity>())", Indent: 1);
            if (!__instance.ParentObject.HasPart<Crystallinity>())
            {
                Debug.Entry(3, $"- Crystallinity Mutation not present", Indent: 2);
                Debug.Footer(3, $"Crystallinity_Patches", "OnRegenerateDefaultEquipmentPrefix(body)");
                return false;
            }
            Debug.Entry(3, $"+ Crystallinity Mutation is present", Indent: 2);
            Debug.Entry(3, $"Proceeding", Indent: 1);

            List<string> NaturalWeaponSupersedingMutations = new List<string>
            {
              //"CyberneticsGiganticExoframe"
            };

            Debug.Entry(4, $"Exposing superseding mutations loop:", Indent: 1);
            Debug.Entry(4, $"* foreach (string mutation in NaturalWeaponSupersedingMutations)", Indent: 1);
            int SupersededCount = 0;
            foreach (string mutation in NaturalWeaponSupersedingMutations)
            {
                Debug.LoopItem(4, $"Checking for {mutation}", Indent: 2);
                if (__instance.ParentObject.HasPart(mutation))
                {
                    Debug.Entry(4, $"+ Present, increasing SupersededCount: {SupersededCount} to {SupersededCount + 1}", Indent: 3);
                    SupersededCount++;
                }
            }

            Debug.Entry(3, $"SupersededCount is {SupersededCount}", Indent: 1);
            bool IsNaturalWeaponSuperseded = SupersededCount > 0;

            Debug.Entry(3, $"Checking for incompatible Mutations or Parts", Indent: 1);
            Debug.Entry(3, $"* if (IsNaturalWeaponSuperseded)", Indent: 1);
            if (IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, $"A Superseding Mutation or Part is resent", Indent: 2);
                Debug.Footer(3, $"Crystallinity_Patches", "OnRegenerateDefaultEquipmentPrefix()");
                return false;
            }
            Debug.Entry(3, $"No Superseding Mutation or Part is present", Indent: 2);

            int level = __instance.Level;
            string CrystallinePointBlueprintName = "Crystalline Point";
            string BurrowingBlueprintName = "Burrowing";
            string GiganticBlueprintName = "Gigantic";
            string ElongatedBlueprintName = "Elongated";
            string BaseBlueprintName = "CrystallinePoint";
            string blueprintName = "";

            bool HasGigantism = __instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>();
            bool HasElongated = __instance.ParentObject.HasPart<ElongatedPaws>();
            bool HasBurrowing = __instance.ParentObject.HasPart<BurrowingClaws>();

            Debug.Entry(3, $"[] Generating Stats", Indent: 1);

            int dieCount = 1;
            int dieSize = 3; // Base Crystalline is 3, Gigantism overrides this to 1 in its section.
            int damageBonus = 0;
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"|^ Starting Stats:", Indent: 2);
            Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 2);
            Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 2);
            Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 2);
            Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 2);
            Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 2);
            Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 2);
            Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 2);

            Debug.Entry(3, "[] Accumulating stats", Indent: 1);

            Debug.Entry(3, "* if (HasGigantism)", Indent: 1);
            if (HasGigantism)
            {
                Debug.DiveIn(3, "+ GigantismPlus Mutation is present", Indent: 2);
                var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();

                if (gigantism != null)
                {
                    // Add "Gigantic" Adjective
                    blueprintName += GiganticBlueprintName;

                    // Override dieCount from mutation
                    dieCount = XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieCount(gigantism.Level);

                    // Override dieSize from mutation, Gigantism dieSize + 1 for Burrowing.
                    dieSize = 1 + XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieSize(gigantism.Level);

                    // Add to damageBonus due to being Gigantic (simulates weapon having ModGigantic)
                    damageBonus += 3;

                    // Override maxStrBonus, redundant but good to exert.
                    maxStrBonus = gigantism.FistMaxStrengthBonus;

                    // Add to hitBonus from mutation (add in case something else is giving it too)
                    hitBonus += XRL.World.Parts.Mutation.GigantismPlus.GetFistHitBonus(gigantism.Level);

                    Debug.Entry(3, $"|? blueprintName: {blueprintName}", Indent: 3);
                    Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 3);
                    Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 3);
                    Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 3);
                    Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 3);
                    Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 3);
                    Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 3);
                    Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 3);
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate gigantism part", Indent: 3);
                }
                Debug.DiveOut(3, "x if (HasGigantism) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, "- GigantismPlus Mutation not present", Indent: 2);
            }

            Debug.Entry(4, "* if (HasElongated)", Indent: 2);
            if (HasElongated)
            {
                Debug.DiveIn(3, "+ ElongatedPaws Mutation is present", Indent: 2);
                var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();
                if (elongated != null)
                {
                    // Add "Elongated" Adjective
                    blueprintName += ElongatedBlueprintName;

                    // Add to dieSize if no Gigantism
                    // Add to dieSize if no Burrowing
                    // (this adds 2 if both are missing, per old behavior)
                    dieSize += HasGigantism ? 0 : 1;
                    dieSize += HasBurrowing ? 0 : 1;

                    // add damage
                    damageBonus += elongated.ElongatedBonusDamage;

                    Debug.Entry(3, $"|? blueprintName: {blueprintName}", Indent: 3);
                    Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 3);
                    Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 3);
                    Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 3);
                    Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 3);
                    Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 3);
                    Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 3);
                    Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 3);
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate elongated part", Indent: 3);
                }
                Debug.DiveOut(3, "x if (HasElongated) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, "- ElongatedPaws Mutation not present", Indent: 2);
            }

            Debug.Entry(4, "* if (HasBurrowing)", Indent: 2);
            if (HasBurrowing)
            {
                Debug.DiveIn(3, "+ Burrowing Claws Mutation is present", Indent: 2);
                var burrowing = __instance.ParentObject.GetPart<BurrowingClaws>();
                if (burrowing != null)
                {
                    // Add "Burrowing" Adjective
                    blueprintName += BurrowingBlueprintName;

                    // Add to dieSize:
                    // 1 if HasGigantism, or
                    // from Burrowing Claws
                    dieSize += HasGigantism ? 1 : BurrowingClaws_Patches.GetBurrowingDieSize(burrowing.Level); ;

                    // Add to damageBonus from Burrowing Claws
                    damageBonus += BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowing.Level);

                    Debug.Entry(3, $"|? blueprintName: {blueprintName}", Indent: 3);
                    Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 3);
                    Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 3);
                    Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 3);
                    Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 3);
                    Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 3);
                    Debug.Entry(3, $"|[ full blueprintName: {blueprintName + BaseBlueprintName}", Indent: 3);
                    Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 3);
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate burrowing part", Indent: 3);
                }
                Debug.DiveOut(3, "x if (HasEburrowing) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, "- Burrowing Claws Mutation not present", Indent: 2);
            }

            Debug.Entry(3, "[] Finished accumulating stats");

            blueprintName += BaseBlueprintName;

            if (!HasGigantism && !HasElongated && !HasBurrowing) blueprintName = CrystallinePointBlueprintName;

            Debug.Divider(3, "_", 25, Indent: 2);
            Debug.Entry(3, $"|: blueprintName: {blueprintName}", Indent: 2);
            Debug.Entry(3, $"|> dieCount: {dieCount}", Indent: 2);
            Debug.Entry(3, $"|> dieSize: {dieSize}", Indent: 2);
            Debug.Entry(3, $"|> damageBonus: {damageBonus}", Indent: 2);
            Debug.Entry(3, $"|> maxStrBonus: {maxStrBonus}", Indent: 2);
            Debug.Entry(3, $"|> hitBonus: {hitBonus}", Indent: 2);
            Debug.Entry(3, $"|_ {WeaponDamageString(dieCount, dieSize, damageBonus)}", Indent: 2);
            Debug.Divider(3, "V", 25, Indent: 2);

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList<BodyPart>();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    Debug.Entry(3, "Sending to assignment method", Indent: 3);
                    AddAccumulatedNaturalEquipmentTo(
                        Creature: __instance.ParentObject,
                        Part: part,
                        BlueprintName: blueprintName,
                        OldDefaultBehavior: part.DefaultBehavior,
                        BaseDamage: WeaponDamageString(dieCount, dieSize, damageBonus),
                        MaxStrBonus: maxStrBonus,
                        HitBonus: hitBonus,
                        AssigningMutation: "Crystallinity"
                        );

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method
        }
    } //!-- public static class Crystallinity_Patches

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity))]
    public static class Crystallinity_RefractChance_Patches
    // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.Crystallinity.Mutate))]
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
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.Crystallinity.Mutate))]
        static void MutatePostfix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<XRL.World.Parts.Mutation.GigantismPlus>() && __instance.RefractAdded)
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
    // Modify the Crystallinity mutation level text to include the GigantismPlus bonus
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.Crystallinity.GetLevelText))]
        static void GetLevelTextPostfix(Crystallinity __instance, ref string __result)
        {
            if (__instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())
            {
                // Replace the original 25% text with our modified version
                __result = __result.Replace(
                    "25% chance to refract light-based attacks",
                    "35% chance to refract light-based attacks (25% base chance {{rules|+}} 10% from {{gigantism|Gigantism}} ({{r|D}}))"
                );
            }
        }
    }//!-- public static class Crystallinity_LevelText_Patches

} //!-- namespace Mods.GigantismPlus.HarmonyPatches
