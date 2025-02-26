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
    } //!--- public static class PseudoGiganticCreature_GameObject_Patches


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

    } //!--- public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches


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
            Debug.Entry(4, "* BurrowingClaws_Patches.GetBurrowingDieSize() called");
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
            Debug.Entry(4, "* BurrowingClaws_Patches.GetBurrowingBonusDamage() called");
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
            Debug.Entry(2, "==================================================================");
            Zone InstanceObjectZone = __instance.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2, "[HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]");
            Debug.Entry(3, "@START BurrowingClaws_Patches.OnRegenerateDefaultEquipmentPrefix() called");
            Debug.Entry(2, $"TARGET {__instance.ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(3, "Checking for this Mutation");
            Debug.Entry(4, "* if (__instance.ParentObject.HasPart<BurrowingClaws>())");
            if (!__instance.ParentObject.HasPart<BurrowingClaws>())
            {
                Debug.Entry(3, "- BurrowingClaws Mutation not present");
                Debug.Entry(3, "x BurrowingClaws_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
                return false;
            }
            Debug.Entry(3, "- BurrowingClaws Mutation is present");

            List<string> NaturalWeaponSupersedingMutations = new List<string>
            {
              //"CyberneticsGiganticExoframe",
                "Crystallinity"
            };

            int SupersededCount = 0;
            foreach (string mutation in NaturalWeaponSupersedingMutations)
            {
                Debug.Entry(4, $"- Checking for {mutation}");
                if (__instance.ParentObject.HasPart(mutation))
                {
                    Debug.Entry(4, "-- Present, increasing SupersededCount");
                    SupersededCount++;
                }
            }

            Debug.Entry(4, $"- SupersededCount is {SupersededCount}");
            bool IsNaturalWeaponSuperseded = SupersededCount > 0;

            Debug.Entry(3, "Checking for incompatible Mutations or Parts");
            Debug.Entry(4, "* if (IsNaturalWeaponSuperseded)");
            if (IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, "- A Superseding Mutation or Part is resent");
                Debug.Entry(3, "x BurrowingClaws_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
                return false;
            }
            Debug.Entry(3, "- No Superseding Mutation or Part is present");

            bool HasGigantism = __instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>();
            bool HasElongated = __instance.ParentObject.HasPart<ElongatedPaws>();

            int level = __instance.Level;
            string BurrowingClawBlueprintName = BurrowingClaws.OBJECT_BLUEPRINT_NAME;
            string GiganticBlueprintName = "Gigantic";
            string ElongatedBlueprintName = "Elongated";
            string BaseBlueprintName = "BurrowingClaw";
            string blueprintName = "";
            
            Debug.Entry(3, "[] Generating Stats");

            int dieCount = 1;
            int dieSize = GetBurrowingDieSize(level); // Base Burrowing from calculation, Gigantism overrides this to 1 in its section.
            int damageBonus = GetBurrowingBonusDamage(level);
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"|^ Starting Stats");
            Debug.Entry(3, $"|> dieCount: {dieCount} \n"
                         + $"|> dieSize: {dieSize} \n"
                         + $"|> damageBonus: {damageBonus} \n"
                         + $"|> maxStrBonus: {maxStrBonus} \n"
                         + $"|> hitBonus: {hitBonus}\n"
                         + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");

            Debug.Entry(3, "[] Accumulating stats");

            Debug.Entry(4, "* if (HasGigantism)");
            if (HasGigantism)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                Debug.Entry(3, "+ GigantismPlus Mutation is present");
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

                    Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                    Debug.Entry(4, $"|> dieCount: {dieCount} \n"
                                 + $"|> dieSize: {dieSize} \n"
                                 + $"|> damageBonus: {damageBonus} \n"
                                 + $"|> maxStrBonus: {maxStrBonus} \n"
                                 + $"|> hitBonus: {hitBonus}\n"
                                 + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate gigantism part");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Entry(3, "- GigantismPlus Mutation not present");
            }

            Debug.Entry(4, "* if (HasElongated)");
            if (HasElongated)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                Debug.Entry(3, "+ ElongatedPaws Mutation is present");
                var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();
                if (elongated != null)
                {
                    // Add "Elongated" Adjective
                    blueprintName += ElongatedBlueprintName;

                    // Add to dieSize if no Gigantism
                    dieSize += HasGigantism ? 0 : 1;

                    // add damage
                    damageBonus += elongated.ElongatedBonusDamage;

                    Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                    Debug.Entry(4, $"|> dieCount: {dieCount}\n"
                                 + $"|> dieSize: {dieSize}\n"
                                 + $"|> damageBonus: {damageBonus}\n"
                                 + $"|> maxStrBonus: {maxStrBonus}\n"
                                 + $"|> hitBonus: {hitBonus}\n"
                                 + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate elongated part");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Entry(3, "- ElongatedPaws Mutation not present");
            }

            Debug.Entry(3, "[] Finished accumulating stats");

            blueprintName += BaseBlueprintName;

            if (!HasGigantism && !HasElongated) blueprintName = BurrowingClawBlueprintName;

            Debug.Entry(4, $"|: blueprintName: {blueprintName}");
            Debug.Entry(4, $"|> dieCount: {dieCount} \n"
                         + $"|> dieSize: {dieSize} \n"
                         + $"|> damageBonus: {damageBonus} \n"
                         + $"|> maxStrBonus: {maxStrBonus} \n"
                         + $"|> hitBonus: {hitBonus}\n"
                         + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
            Debug.Entry(3, "vvvvvvvvvvvvvvvvvvvvvvv");

            Debug.Entry(3, "Performing application of behavior to parts");

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
            Debug.Entry(4, "Generating List<BodyPart> list");
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList<BodyPart>();

            Debug.Entry(4, "Checking list of parts for expected entries");
            Debug.Entry(4, "* foreach (BodyPart part in list)");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"- Part is {part.Type}");
                if (part.Type == "Hand")
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, "-- Hand Found");

                    Debug.Entry(4, "-- Saving copy of current DefaultBehavior in case creation fails");

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

                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    continue;
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//");
            Debug.Entry(3, "END BurrowingClaws_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
            Debug.Entry(2, "==================================================================");
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
            Debug.Entry(2, "==================================================================");
            Zone InstanceObjectZone = __instance.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2, "[HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]");
            Debug.Entry(3, "@START Crystallinity_Patches.OnRegenerateDefaultEquipmentPrefix() called");
            Debug.Entry(2, $"TARGET {__instance.ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            Debug.Entry(4, "Checking for this Mutation");
            Debug.Entry(4, "* if (__instance.ParentObject.HasPart<Crystallinity>())");
            if (!__instance.ParentObject.HasPart<Crystallinity>())
            {
                Debug.Entry(3, "- Crystallinity Mutation not present");
                Debug.Entry(3, "x Crystallinity_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
                return false;
            }
            Debug.Entry(3, "- Crystallinity Mutation is present");

            List<string> NaturalWeaponSupersedingMutations = new List<string>
            {
              //"CyberneticsGiganticExoframe"
            };
            
            int SupersededCount = 0;
            foreach (string mutation in NaturalWeaponSupersedingMutations)
            {
                Debug.Entry(4, $"- Checking for {mutation}");
                if (__instance.ParentObject.HasPart(mutation))
                {
                    Debug.Entry(4, "-- Present, increasing SupersededCount");
                    SupersededCount++;
                }
            }
            Debug.Entry(4, $"- SupersededCount is {SupersededCount}");
            bool IsNaturalWeaponSuperseded = SupersededCount > 0;

            Debug.Entry(3, "Checking for incompatible Mutations or Parts");
            Debug.Entry(4, "* if (IsNaturalWeaponSuperseded)");
            if (IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, "- A Superseding Mutation or Part is resent");
                Debug.Entry(3, "x Crystallinity_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
                return false;
            }
            Debug.Entry(3, "- No Superseding Mutation or Part is present");

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

            Debug.Entry(3, "[] Generating Stats");

            int dieCount = 1;
            int dieSize = 3; // Base Crystalline is 3, Gigantism overrides this to 1 in its section.
            int damageBonus = 0;
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"|^ Starting Stats");
            Debug.Entry(3, $"|> dieCount: {dieCount} \n"
                         + $"|> dieSize: {dieSize} \n"
                         + $"|> damageBonus: {damageBonus} \n"
                         + $"|> maxStrBonus: {maxStrBonus} \n"
                         + $"|> hitBonus: {hitBonus}\n"
                         + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");

            Debug.Entry(3, "[] Accumulating stats");

            Debug.Entry(4, "* if (HasGigantism)");
            if (HasGigantism)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                Debug.Entry(3, "+ GigantismPlus Mutation is present");
                var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();

                if (gigantism != null)
                {
                    // Add "Gigantic" Adjective
                    blueprintName += GiganticBlueprintName;

                    // Override dieCount from mutation
                    dieCount = XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieCount(gigantism.Level);

                    // Override dieSize from mutation, Gigantism dieSize + 1 for Crystal.
                    dieSize = 1 + XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieSize(gigantism.Level);

                    // Add to damageBonus due to being Gigantic (simulates weapon having ModGigantic)
                    damageBonus += 3;

                    // Override maxStrBonus, redundant but good to exert.
                    maxStrBonus = gigantism.FistMaxStrengthBonus;

                    // Add to hitBonus from mutation (add in case something else is giving it too)
                    hitBonus += XRL.World.Parts.Mutation.GigantismPlus.GetFistHitBonus(gigantism.Level);

                    Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                    Debug.Entry(4, $"|> dieCount: {dieCount} \n"
                                 + $"|> dieSize: {dieSize} \n"
                                 + $"|> damageBonus: {damageBonus} \n"
                                 + $"|> maxStrBonus: {maxStrBonus} \n"
                                 + $"|> hitBonus: {hitBonus}\n"
                                 + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate gigantism part");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Entry(3, "- GigantismPlus Mutation not present");
            }

            Debug.Entry(4, "* if (HasElongated)");
            if (HasElongated)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                Debug.Entry(3, "+ ElongatedPaws Mutation is present");
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

                    Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                    Debug.Entry(4, $"|> dieCount: {dieCount}\n"
                                 + $"|> dieSize: {dieSize}\n"
                                 + $"|> damageBonus: {damageBonus}\n"
                                 + $"|> maxStrBonus: {maxStrBonus}\n"
                                 + $"|> hitBonus: {hitBonus}\n"
                                 + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate elongated part");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Entry(3, "- ElongatedPaws Mutation not present");
            }

            Debug.Entry(4, "* if (HasBurrowing)");
            if (HasBurrowing)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                Debug.Entry(3, "+ Burrowing Claws Mutation is present");
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

                    Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                    Debug.Entry(4, $"|> dieCount: {dieCount}\n"
                                 + $"|> dieSize: {dieSize}\n"
                                 + $"|> damageBonus: {damageBonus}\n"
                                 + $"|> maxStrBonus: {maxStrBonus}\n"
                                 + $"|> hitBonus: {hitBonus}\n"
                                 + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                }
                else
                {
                    Debug.Entry(3, "! Failed to instantiate burrowing part");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Entry(3, "- Burrowing Claws Mutation not present");
            }

            Debug.Entry(3, "[] Finished accumulating stats");

            blueprintName += BaseBlueprintName;

            if (!HasGigantism && !HasElongated && !HasBurrowing) blueprintName = CrystallinePointBlueprintName;

            Debug.Entry(4, $"|: blueprintName: {blueprintName}");
            Debug.Entry(4, $"|> dieCount: {dieCount} \n"
                         + $"|> dieSize: {dieSize} \n"
                         + $"|> damageBonus: {damageBonus} \n"
                         + $"|> maxStrBonus: {maxStrBonus} \n"
                         + $"|> hitBonus: {hitBonus}\n"
                         + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
            Debug.Entry(3, "vvvvvvvvvvvvvvvvvvvvvvv");

            Debug.Entry(3, "Performing application of behavior to parts");

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
            Debug.Entry(4, "Generating List<BodyPart> list");
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList<BodyPart>();

            Debug.Entry(4, "Checking list of parts for expected entries");
            Debug.Entry(4, "* foreach (BodyPart part in list)");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"- Part is {part.Type}");
            }
            Debug.Entry(4, "x foreach (BodyPart hand in body.GetParts()) ]//");

            Debug.Entry(3, "Performing application of behavior to parts");
            Debug.Entry(3, "* foreach (BodyPart hand in body.GetParts())");
            Debug.Entry(4, "* if (part.Type == targetPartType)");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"- Part is {part.Type}");
                if (part.Type == targetPartType) // Changed from "Hand" to "Quincunx"
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, $"-- {targetPartType} Found");

                    Debug.Entry(4, "-- Saving copy of current DefaultBehavior in case creation fails");

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

                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    continue;
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//");
            Debug.Entry(3, "END Crystallinity_Patches.OnRegenerateDefaultEquipmentPrefix() ]//");
            Debug.Entry(2, "==================================================================");
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
