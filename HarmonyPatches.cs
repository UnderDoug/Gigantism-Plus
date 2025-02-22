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

namespace Mods.GigantismPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(GameObject))]
    public static class PseudoGiganticCreature_GameObject_Patches
    {
        // Goal is to simulate being Gigantic for the purposes of calculating body weight, if the GameObject in question is PseudoGigantic

        /* 
         * This code breaks the rest of the patches. Harmony is really towards the limit of my coding ability.
         * 
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameObject.IsGiganticCreature), "get")]
        static bool IsGiganticCreatureGetter(GameObject __instance, ref bool __result)
        {
            // This is a skip. It's designed to make the only thing that counts towards gigantism whether the IntProperty is 1 or not.
            // --instance gives you the instantiated object on which the original method call is happening
            Debug.Entry(1,"We're in the Getter");
            __instance.ParentObject.RemovePart<Gigantism>();
            int intProperty = __instance.ParentObject.GetIntProperty("Gigantic");
            if (intProperty > 0)
            {
                intProperty = 1;
                __result = true;
                Debug.Entry(1, "Gigantic IntProperty is true");
                return false;
            }
            intProperty = 0;
            __result = false;
            Debug.Entry(1, "Gigantic IntProperty is false");
            return false;
        }
        */

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

        static void CyberneticsTerminal2Patch(GameObject Actor)
        {
            Debug.Entry(3, "**static void CyberneticsTerminal2Patch(GameObject Actor) called");

            Debug.Entry(3, "- Checking the Actor exists, is a True Kin, and has GigantismPlus");
            Debug.Entry(4, "**if (Actor != null && actor.IsTrueKin && actor.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())");
            if (Actor != null && Actor.IsTrueKin() && Actor.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())
            {
                Debug.Entry(3, "-- Actor exists, is a True Kin, and has GigantismPlus");
                var gigantism = Actor.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();
                if (gigantism != null)
                {
                    Debug.Entry(3, "-- GigantismPlus instantiated");
                    Debug.Entry(3, "-- Making Hunch Over free, Sending Command to Hunch Over");

                    gigantism.IsHunchFree = true;
                    gigantism.UnHunchNextTurn = true;
                    CommandEvent.Send(Actor, XRL.World.Parts.Mutation.GigantismPlus.HUNCH_OVER_COMMAND_NAME);

                    Debug.Entry(3, "-- Popping up Popup");
                    Popup.Show("You peer down into the interface.");
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

                CyberneticsTerminal2Patch(E.Actor);

                Debug.Entry(3, "Deferring to patched method");
            }
            return true;
        } //!-- static bool InventoryActionEventPrefix(InventoryActionEvent E)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2Patch(E.Actor);

            Debug.Entry(3, "Deferring to patched method");
            return true;
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

        public static void DoNaturalWeaponCreationAndAssign(BodyPart Part, string BlueprintName, GameObject OldDefaultBehaviour, int DieCount, int DieSize, int DamageBonus, int MaxStrBonus, int HitBonus)
        {
            Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);

            string baseDamage = $"{DieCount}d{DieSize}";
            if (DamageBonus > 0)
            {
                baseDamage += $"+{DamageBonus}";
            }

            if (Part.DefaultBehavior != null)
            {
                Debug.Entry(3, "---- Part.DefaultBehaviour not null, assigning stats");
                Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "BurrowingClaws", false);

                MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                weapon.BaseDamage = baseDamage;
                if (HitBonus != 0) weapon.HitBonus = HitBonus;
                weapon.MaxStrengthBonus = MaxStrBonus;

                Debug.Entry(4, $"---- hand.DefaultBehavior = {BlueprintName}");
                Debug.Entry(4, $"---- MaxStrBonus: {weapon.MaxStrengthBonus} | Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus}");
            }
            else
            {
                Debug.Entry(3, $"---- part.DefaultBehaviour was null, invalid blueprint name \"{BlueprintName}\"");
                Part.DefaultBehavior = OldDefaultBehaviour;
                Debug.Entry(3, $"---- OldDefaultBehaviour reassigned");
            }
        } //!-- public static void DoNaturalWeaponCreationAndAssign(BodyPart Part, string BlueprintName, GameObject OldDefaultBehaviour, int DieCount, int DieSize, int DamageBonus, int MaxStrBonus, int HitBonus = 0)

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
              //"MassiveExoframe",
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

            int level = __instance.Level;
            string BurrowingClawBlueprintName = BurrowingClaws.OBJECT_BLUEPRINT_NAME;
            string GiganticBlueprintName = "Gigantic";
            string ElongatedBlueprintName = "Elongated";
            string BaseBlueprintName = "BurrowingClaw";
            string blueprintName = "";
            GameObject OldDefaultBehaviour;
            
            Debug.Entry(3, "Generating Stats");

            int dieCount = 1;
            int dieSize = GetBurrowingDieSize(level); // Get the die size, add 1 for elongated variants
            int burrowingBonus = GetBurrowingBonusDamage(level);
            int damageBonus = burrowingBonus;
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                         + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

            bool HasGigantism = __instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>();
            bool HasElongated = __instance.ParentObject.HasPart<ElongatedPaws>();

            Debug.Entry(3, "Accumulating stats");

            Debug.Entry(4, "* if (HasGigantism)");
            if (HasGigantism)
            {
                Debug.Entry(3, "- GigantismPlus Mutation is present");
                var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();
                
                if (gigantism != null)
                {
                    blueprintName += GiganticBlueprintName;
                    int gigantismlevel = gigantism.Level;
                    Debug.Entry(4, $"> blueprintName: {blueprintName}");

                    dieCount = XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieCount(gigantism.Level);
                    dieSize = (int)Math.Floor((double)dieSize/2.0) + XRL.World.Parts.Mutation.GigantismPlus.GetFistDamageDieSize(gigantism.Level);
                    damageBonus += 3;
                    maxStrBonus = gigantism.FistMaxStrengthBonus;
                    hitBonus = XRL.World.Parts.Mutation.GigantismPlus.GetFistHitBonus(gigantism.Level);
                    Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                 + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                }
            }

            Debug.Entry(4, "* if (HasElongated)");
            if (HasElongated)
            {
                Debug.Entry(3, "- ElongatedPaws Mutation is present");
                var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();
                if (elongated != null)
                {
                    blueprintName += ElongatedBlueprintName;
                    Debug.Entry(4, $"> blueprintName: {blueprintName}");

                    damageBonus += elongated.ElongatedBonusDamage;
                    Debug.Entry(4, $"- damageBonus: {damageBonus}");

                    Debug.Entry(4, "* if (!HasGigantism)");
                    if (!HasGigantism)
                    {
                        Debug.Entry(3, "-- GigantismPlus Mutation is Absent");
                        dieSize += 1;
                        Debug.Entry(4, $"-- dieSize: {dieSize}");
                    }
                    Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                 + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                }
            }
            Debug.Entry(3, "Finished accumulating stats");

            Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                         + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

            blueprintName += BaseBlueprintName;

            if (!HasGigantism && !HasElongated) blueprintName = BurrowingClawBlueprintName;
            Debug.Entry(3, $"> blueprintName: {blueprintName}");

            Debug.Entry(3, "Performing application of behaviour to parts");

            Debug.Entry(3, "* foreach (BodyPart hand in body.GetParts())");
            Debug.Entry(4, "* if (hand.Type == \"Hand\")");
            foreach (BodyPart part in body.GetParts(EvenIfDismembered: true))
            {
                Debug.Entry(4, $"- Part is {part.Type}");
                if (part.Type == "Hand")
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, "-- Hand Found");

                    Debug.Entry(4, "-- Saving copy of current DefaultBehaviour in case creation fails");
                    OldDefaultBehaviour = part.DefaultBehavior;

                    DoNaturalWeaponCreationAndAssign(
                        Part: part, 
                        BlueprintName: blueprintName, 
                        OldDefaultBehaviour: OldDefaultBehaviour, 
                        DieCount: dieCount, 
                        DieSize: dieSize, 
                        DamageBonus: damageBonus, 
                        MaxStrBonus: maxStrBonus, 
                        HitBonus: hitBonus
                        );

                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    continue;

                    /* Old Code. Bypassing for now.
                     * 
                    Debug.Entry(3, "-- Checking for compatible Mutations");
                    Debug.Entry(4, "** if (HasGigantism)");
                    Debug.Entry(4, "** else if (HasElongated)");
                    if (HasGigantism)
                    {
                        Debug.Entry(3, "--- GigantismPlus Mutation is present");
                        Debug.Entry(4, "*** if (HasElongated)");
                        var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();

                        dieCount = gigantism.FistDamageDieCount;
                        dieSize = gigantism.FistDamageDieSize;
                        maxStrBonus = gigantism.FistMaxStrengthBonus;
                        hitBonus = gigantism.FistHitBonus;

                        if (HasElongated)
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation is present");
                            var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();

                            blueprintName = GiganticElongatedBurrowingClawBlueprintName;
                            damageBonus += (int)Math.Floor((double)elongated.StrengthModifier / 2.0);

                            DoNaturalWeaponCreationAndAssign(part, blueprintName, OldDefaultBehaviour, dieCount, dieSize, maxStrBonus, hitBonus);
                        }
                        else
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation not present");

                            blueprintName = GiganticBurrowingClawBlueprintName;
                            DoNaturalWeaponCreationAndAssign(part, blueprintName, OldDefaultBehaviour, dieCount, dieSize, maxStrBonus, hitBonus);
                        }
                    }
                    else if (HasElongated)
                    {
                        Debug.Entry(3, "--- GigantismPlus Mutation not present");
                        Debug.Entry(3, "--- ElongatedPaws Mutation is present");
                        var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();

                        if (elongatedPaws.ElongatedBurrowingClawObject == null)
                        {
                            Debug.Entry(4, "---- ElongatedBurrowingClawObject was null, init");
                            elongatedPaws.ElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject(ElongatedBurrowingClawBlueprintName);
                        }
                        part.DefaultBehavior = elongatedPaws.ElongatedBurrowingClawObject;
                        part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "BurrowingClaws", false);
                        var weapon = elongatedPaws.ElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                        // Use the increased die size for elongated paws (+1)
                        weapon.BaseDamage = $"1d{burrowingDieSize + 1}+{(elongatedPaws.StrengthModifier / 2) + burrowingBonus}";
                        Debug.Entry(4, "**hand.DefaultBehavior = elongatedPaws.ElongatedBurrowingClawObject");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                    }
                    else
                    {
                        Debug.Entry(3, "--- GigantismPlus Mutation not present");
                        Debug.Entry(3, "--- ElongatedPaws Mutation not present");
                        if (part.DefaultBehavior == null || part.DefaultBehavior.GetBlueprint(true).Name != "Burrowing Claws")
                        {
                            Debug.Entry(4, "---- hand.DefaultBehaviour was null or DefaultBehaviour was not \"Burrowing Claws\", init");
                            part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BurrowingClawBlueprintName);
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "BurrowingClaws", false);
                        }
                        var weapon = part.DefaultBehavior.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = __instance.GetClawsDamage(__instance.Level);
                        Debug.Entry(4, "**hand.DefaultBehavior = GameObjectFactory.Factory.CreateObject(\"Burrowing Claws\")");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage}");
                    }
                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    */
                }
            }
            Debug.Entry(3, "xxforeach (BodyPart hand in body.GetParts())");
            Debug.Entry(3, "END BurrowingClaws_Patches.OnRegenerateDefaultEquipmentPrefix()");
            Debug.Entry(2, "==================================================================");
            return false; // Skip the original method
        }
    } //!-- public static class BurrowingClaws_Patches

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity))]
    public static class Crystallinity_Patches
    {
        public static void DoNaturalWeaponCreationAndAssign(BodyPart Part, string BlueprintName, GameObject OldDefaultBehaviour, int DieCount, int DieSize, int DamageBonus, int MaxStrBonus, int HitBonus)
        {
            Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);

            string baseDamage = $"{DieCount}d{DieSize}";
            if (DamageBonus > 0)
            {
                baseDamage += $"+{DamageBonus}";
            }

            if (Part.DefaultBehavior != null)
            {
                Debug.Entry(3, "---- Part.DefaultBehaviour not null, assigning stats");
                Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);

                MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                weapon.BaseDamage = baseDamage;
                if (HitBonus != 0) weapon.HitBonus = HitBonus;
                weapon.MaxStrengthBonus = MaxStrBonus;

                Debug.Entry(4, $"---- hand.DefaultBehavior = {BlueprintName}");
                Debug.Entry(4, $"---- MaxStrBonus: {weapon.MaxStrengthBonus} | Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus}");
            }
            else
            {
                Debug.Entry(3, $"---- part.DefaultBehaviour was null, invalid blueprint name \"{BlueprintName}\"");
                Part.DefaultBehavior = OldDefaultBehaviour;
                Debug.Entry(3, $"---- OldDefaultBehaviour reassigned");
            }
        } //!-- public static void DoNaturalWeaponCreationAndAssign(BodyPart Part, string BlueprintName, GameObject OldDefaultBehaviour, int DieCount, int DieSize, int DamageBonus, int MaxStrBonus, int HitBonus = 0)

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
              //"MassiveExoframe"
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
            string CystallinePointBlueprintName = "Crystalline Point";
            string BurrowingBlueprintName = "Burrowing";
            string GiganticBlueprintName = "Gigantic";
            string ElongatedBlueprintName = "Elongated";
            string BaseBlueprintName = "CrystallinePoint";
            string blueprintName = "";
            GameObject OldDefaultBehaviour;

            Debug.Entry(3, "Generating Stats");

            int dieCount = 1;
            int dieSize = 3; // add 1 for elongated variants, add GetBurrowingDieSize for burrowing variants.
            int damageBonus = 0;
            int maxStrBonus = 999;
            int hitBonus = 0;

            Debug.Entry(3, $"dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                         + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

            bool HasGigantism = __instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>();
            bool HasElongated = __instance.ParentObject.HasPart<ElongatedPaws>();
            bool HasBurrowing = __instance.ParentObject.HasPart<BurrowingClaws>();

            Debug.Entry(3, "Accumulating stats");

            Debug.Entry(4, "* if (HasGigantism)");
            if (HasGigantism)
            {
                Debug.Entry(3, "- GigantismPlus Mutation is present");
                var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();

                if (gigantism != null)
                {
                    blueprintName += GiganticBlueprintName;
                    Debug.Entry(4, $"> blueprintName: {blueprintName}");

                    dieCount = gigantism.FistDamageDieCount;
                    dieSize += gigantism.FistDamageDieSize;
                    damageBonus += 3;
                    maxStrBonus = gigantism.FistMaxStrengthBonus;
                    hitBonus = gigantism.FistHitBonus;
                    Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                 + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                }
            }

            Debug.Entry(4, "* if (HasElongated)");
            if (HasElongated)
            {
                Debug.Entry(3, "- ElongatedPaws Mutation is present");
                var elongated = __instance.ParentObject.GetPart<ElongatedPaws>();
                if (elongated != null)
                {
                    blueprintName += ElongatedBlueprintName;
                    Debug.Entry(4, $"> blueprintName: {blueprintName}");

                    damageBonus += elongated.ElongatedBonusDamage;
                    Debug.Entry(4, $"- damageBonus: {damageBonus}");

                    Debug.Entry(4, "* if (!HasGigantism)");
                    if (!HasGigantism)
                    {
                        Debug.Entry(3, "-- GigantismPlus Mutation is Absent");
                        dieSize += 1;
                        Debug.Entry(4, $"-- dieSize: {dieSize}");
                    }
                    Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                 + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                }
            }

            if (HasBurrowing)
            {
                Debug.Entry(3, "- BurrowingPaws Mutation is present");
                var burrowing = __instance.ParentObject.GetPart<BurrowingClaws>();
                if (burrowing != null)
                {
                    blueprintName += BurrowingBlueprintName;
                    Debug.Entry(4, $"> blueprintName: {blueprintName}");

                    int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowing.Level);

                    if (HasGigantism) burrowingDieSize = (int)Math.Floor((double)burrowingDieSize / 2.0);
                    
                    dieSize += burrowingDieSize;
                    damageBonus += BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowing.Level);
                    Debug.Entry(4, $"- damageBonus: {damageBonus}");

                    Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                 + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                }
            }
            Debug.Entry(3, "Finished accumulating stats");

            Debug.Entry(3, $"dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                         + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

            blueprintName += BaseBlueprintName;
            if (!HasGigantism && !HasElongated && !HasBurrowing) blueprintName = CystallinePointBlueprintName;
            Debug.Entry(3, $"> blueprintName: {blueprintName}");

            Debug.Entry(3, "Performing application of behaviour to parts");
            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
            Debug.Entry(4, "Generating List<BodyPart> list");
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType  // Changed from VariantType to Type
                                   select p).ToList<BodyPart>();


            Debug.Entry(4, "Checking list of parts for expected entries");
            Debug.Entry(4, "* foreach (BodyPart hand in body.GetParts())");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"- Part is {part.Type}");
            }
            Debug.Entry(4, "x foreach (BodyPart hand in body.GetParts()) ]//");

            Debug.Entry(3, "Performing application of behaviour to parts");
            Debug.Entry(3, "* foreach (BodyPart hand in body.GetParts())");
            Debug.Entry(4, "* if (part.Type == targetPartType)");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"- Part is {part.Type}");
                if (part.Type == targetPartType) // Changed from "Hand" to "Quincunx"
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, $"-- {targetPartType} Found");

                    Debug.Entry(4, "-- Saving copy of current DefaultBehaviour in case creation fails");
                    OldDefaultBehaviour = part.DefaultBehavior;

                    DoNaturalWeaponCreationAndAssign(
                        Part: part,
                        BlueprintName: blueprintName,
                        OldDefaultBehaviour: OldDefaultBehaviour,
                        DieCount: dieCount,
                        DieSize: dieSize,
                        DamageBonus: damageBonus,
                        MaxStrBonus: maxStrBonus,
                        HitBonus: hitBonus
                        );

                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    continue;

                    /* Old Code. Bypassing for now.
                     * 
                    Debug.Entry(4, "-- Create the base crystalline point");
                    // Create the base crystalline point
                    if (part.DefaultBehavior == null || part.DefaultBehavior.GetBlueprint(true).Name != "Crystalline Point")
                    {
                        Debug.Entry(4, "-- hand.DefaultBehaviour was null or DefaultBehaviour was not \"Crystalline Point\", init");
                        part.DefaultBehavior = GameObjectFactory.Factory.CreateObject("Crystalline Point");
                        part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                    }

                    Debug.Entry(3, $"-- Apply the same weapon logic as before, just to the {targetPartType} part");
                    // Apply the same weapon logic as before, just to the Quincunx part
                    MeleeWeapon weaponPart = null;

                    Debug.Entry(3, "-- Checking for compatible Mutations");
                    Debug.Entry(4, "** if (__instance.ParentObject.HasPart<GigantismPlus>())");
                    Debug.Entry(4, "** else if (__instance.ParentObject.HasPart<ElongatedPaws>())");
                    if (__instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())
                    {
                        Debug.Entry(3, "--- GigantismPlus Mutation is present");
                        // Gigantism + Other combinations
                        var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();
                        // Gigantism + Elongated + Burrowing

                        Debug.Entry(4, "*** if ([..].HasPart<ElongatedPaws>() && [..].HasPart<BurrowingClaws>())");
                        Debug.Entry(4, "*** else if ([..].HasPart<ElongatedPaws>()");
                        Debug.Entry(4, "*** else if ([..].HasPart<BurrowingClaws>()");
                        if (__instance.ParentObject.HasPart<ElongatedPaws>() && __instance.ParentObject.HasPart<BurrowingClaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation is present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation is present");
                            var burrowingClaws = __instance.ParentObject.GetPart<BurrowingClaws>();
                            int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);
                            
                            if (gigantism.GiganticElongatedBurrowingClawObject == null || gigantism.GiganticElongatedBurrowingClawObject.Blueprint != "GiganticElongatedBurrowingCrystallinePoint")
                            {
                                Debug.Entry(4, "----- GiganticElongatedBurrowingClawObject was null, init as GiganticElongatedBurrowingCrystallinePoint");
                                gigantism.GiganticElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedBurrowingCrystallinePoint");
                            }
                            part.DefaultBehavior = gigantism.GiganticElongatedBurrowingClawObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            var elongatedPaws = __instance.ParentObject.GetPart<ElongatedPaws>();
                            weaponPart = gigantism.GiganticElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                            weaponPart.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize + 1}+{(elongatedPaws.StrengthModifier / 2) + 3 + burrowingBonus}";
                            weaponPart.HitBonus = gigantism.FistHitBonus;
                            weaponPart.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                            Debug.Entry(4, "**part.DefaultBehavior = gigantism.GiganticElongatedBurrowingClawObject (as GiganticElongatedBurrowingCrystallinePoint)");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage} | Hit: {weaponPart.HitBonus} | PenCap: {weaponPart.MaxStrengthBonus}");
                        }
                        // Gigantism + Elongated
                        else if (__instance.ParentObject.HasPart<ElongatedPaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation is present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation not present");
                            if (gigantism.GiganticElongatedPawObject == null || gigantism.GiganticElongatedPawObject.Blueprint != "GiganticElongatedCrystallinePoint")
                            {
                                Debug.Entry(4, "----- GiganticElongatedBurrowingClawObject was null, init as GiganticElongatedCrystallinePoint");
                                gigantism.GiganticElongatedPawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedCrystallinePoint");
                            }
                            part.DefaultBehavior = gigantism.GiganticElongatedPawObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            var elongatedPaws = __instance.ParentObject.GetPart<ElongatedPaws>();
                            weaponPart = gigantism.GiganticElongatedPawObject.GetPart<MeleeWeapon>();
                            weaponPart.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize + 1}+{(elongatedPaws.StrengthModifier / 2) + 3}";
                            weaponPart.HitBonus = gigantism.FistHitBonus;
                            weaponPart.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                            Debug.Entry(4, "**** part.DefaultBehavior = gigantism.GiganticElongatedPawObject (as GiganticElongatedCrystallinePoint)");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage} | Hit: {weaponPart.HitBonus} | PenCap: {weaponPart.MaxStrengthBonus}");
                        }
                        // Gigantism + Burrowing
                        else if (__instance.ParentObject.HasPart<BurrowingClaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation not present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation is present");
                            var burrowingClaws = __instance.ParentObject.GetPart<BurrowingClaws>();
                            int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);
                            
                            if (gigantism.GiganticBurrowingClawObject == null || gigantism.GiganticBurrowingClawObject.Blueprint != "GiganticBurrowingCrystallinePoint")
                            {
                                Debug.Entry(4, "----- GiganticBurrowingClawObject was null, init as GiganticBurrowingCrystallinePoint");
                                gigantism.GiganticBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticBurrowingCrystallinePoint");
                            }
                            part.DefaultBehavior = gigantism.GiganticBurrowingClawObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            weaponPart = gigantism.GiganticBurrowingClawObject.GetPart<MeleeWeapon>();
                            string baseDamage = XRL.World.Parts.Mutation.GigantismPlus.GetFistBaseDamage(__instance.Level);  
                            int dIndex = baseDamage.IndexOf('d');
                            int plusIndex = baseDamage.LastIndexOf('+');
                            if (dIndex != -1)
                            {
                                int dieCount = int.Parse(baseDamage.Substring(0, dIndex));
                                int dieSize = int.Parse(baseDamage.Substring(dIndex + 1, plusIndex - (dIndex + 1)));
                                weaponPart.BaseDamage = $"{dieCount}d{dieSize + 1}+{int.Parse(baseDamage.Substring(plusIndex + 1)) + burrowingBonus}";
                            }
                            weaponPart.HitBonus = gigantism.FistHitBonus;
                            weaponPart.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                            Debug.Entry(4, "**** part.DefaultBehavior = gigantism.GiganticBurrowingClawObject (as GiganticBurrowingCrystallinePoint)");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage} | Hit: {weaponPart.HitBonus} | PenCap: {weaponPart.MaxStrengthBonus}");
                        }
                        // Just Gigantism
                        else
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation not present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation not present");
                            if (gigantism.GiganticFistObject == null || gigantism.GiganticFistObject.Blueprint != "GiganticCrystallinePoint")
                            {
                                Debug.Entry(4, "----- GiganticFistObject was null, init as GiganticCrystallinePoint");
                                gigantism.GiganticFistObject = GameObjectFactory.Factory.CreateObject("GiganticCrystallinePoint");
                            }
                            part.DefaultBehavior = gigantism.GiganticFistObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            weaponPart = gigantism.GiganticFistObject.GetPart<MeleeWeapon>();
                            string baseDamage = XRL.World.Parts.Mutation.GigantismPlus.GetFistBaseDamage(__instance.Level);  
                            int dIndex = baseDamage.IndexOf('d');
                            int plusIndex = baseDamage.LastIndexOf('+');
                            if (dIndex != -1)
                            {
                                int dieCount = int.Parse(baseDamage.Substring(0, dIndex));
                                int dieSize = int.Parse(baseDamage.Substring(dIndex + 1, plusIndex - (dIndex + 1)));
                                weaponPart.BaseDamage = $"{dieCount}d{dieSize + 1}+{baseDamage.Substring(plusIndex + 1)}";
                            }
                            weaponPart.HitBonus = gigantism.FistHitBonus;
                            weaponPart.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                            Debug.Entry(4, "**** part.DefaultBehavior = gigantism.GiganticFistObject");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage} | Hit: {weaponPart.HitBonus} | PenCap: {weaponPart.MaxStrengthBonus}");
                        }
                    }
                    // Non-Gigantism combinations 
                    else 
                    {
                        Debug.Entry(3, "--- GigantismPlus Mutation not present");

                        // Elongated + Burrowing
                        Debug.Entry(4, "*** if ([..].HasPart<ElongatedPaws>() && [..].HasPart<BurrowingClaws>())");
                        Debug.Entry(4, "*** else if ([..].HasPart<ElongatedPaws>()");
                        Debug.Entry(4, "*** else if ([..].HasPart<BurrowingClaws>()");
                        if (__instance.ParentObject.HasPart<ElongatedPaws>() && __instance.ParentObject.HasPart<BurrowingClaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation is present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation is present");
                            var elongatedPaws = __instance.ParentObject.GetPart<ElongatedPaws>();
                            var burrowingClaws = __instance.ParentObject.GetPart<BurrowingClaws>();
                            int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                            int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);
                            
                            if (elongatedPaws.ElongatedBurrowingClawObject == null || elongatedPaws.ElongatedBurrowingClawObject.Blueprint != "ElongatedBurrowingCrystallinePoint")
                            {
                                Debug.Entry(4, "----- ElongatedBurrowingClawObject was null, init as ElongatedBurrowingCrystallinePoint");
                                elongatedPaws.ElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("ElongatedBurrowingCrystallinePoint");
                            }
                            part.DefaultBehavior = elongatedPaws.ElongatedBurrowingClawObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            weaponPart = elongatedPaws.ElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                            weaponPart.BaseDamage = $"1d{burrowingDieSize + 2 + 1}+{elongatedPaws.StrengthModifier / 2}";
                            Debug.Entry(4, "**** part.DefaultBehavior = elongatedPaws.ElongatedBurrowingClawObject (as BurrowingCrystallinePoint)");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage} | Hit: {weaponPart.HitBonus} | PenCap: {weaponPart.MaxStrengthBonus}");
                        }
                        // Just Elongated
                        else if (__instance.ParentObject.HasPart<ElongatedPaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation is present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation not present");
                            var elongatedPaws = __instance.ParentObject.GetPart<ElongatedPaws>();
                            if (elongatedPaws.ElongatedPawObject == null || elongatedPaws.ElongatedPawObject.Blueprint != "ElongatedCrystallinePoint")
                            {
                                Debug.Entry(4, "----- ElongatedPawObject was null, init as ElongatedCrystallinePoint");
                                elongatedPaws.ElongatedPawObject = GameObjectFactory.Factory.CreateObject("ElongatedCrystallinePoint");
                            }
                            part.DefaultBehavior = elongatedPaws.ElongatedPawObject;
                            part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            weaponPart = elongatedPaws.ElongatedPawObject.GetPart<MeleeWeapon>();
                            weaponPart.BaseDamage = $"1d5+{elongatedPaws.StrengthModifier / 2}"; // Base 1d4 + 1 for crystalline
                            Debug.Entry(4, "**** part.DefaultBehavior = elongatedPaws.ElongatedPawObject");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage}");
                        }
                        // Just Burrowing
                        else if (__instance.ParentObject.HasPart<BurrowingClaws>())
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation not present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation is present");
                            var burrowingClaws = __instance.ParentObject.GetPart<BurrowingClaws>();
                            int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                            
                            if (part.DefaultBehavior == null || part.DefaultBehavior.Blueprint != "BurrowingCrystallinePoint")
                            {
                                Debug.Entry(4, "---- hand.DefaultBehaviour was null or DefaultBehaviour was not \"BurrowingCrystallinePoint\", init");
                                part.DefaultBehavior = GameObjectFactory.Factory.CreateObject("BurrowingCrystallinePoint");
                                part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
                            }
                            weaponPart = part.DefaultBehavior.GetPart<MeleeWeapon>();
                            int dIndex = burrowingClaws.GetClawsDamage(burrowingClaws.Level).IndexOf('d');
                            int dieSize = int.Parse(burrowingClaws.GetClawsDamage(burrowingClaws.Level).Substring(dIndex + 1));
                            weaponPart.BaseDamage = $"1d{dieSize + 1}"; // Add 1 to die size for crystalline
                            Debug.Entry(4, "**part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(\"BurrowingCrystallinePoint\")");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage}");
                        }
                        // Default case - just Crystallinity
                        else
                        {
                            Debug.Entry(3, "---- ElongatedPaws Mutation not present");
                            Debug.Entry(3, "---- BurrowingClaws Mutation not present");
                            weaponPart = part.DefaultBehavior.GetPart<MeleeWeapon>();
                            weaponPart.BaseDamage = __instance.GetPointDamage(__instance.Level);
                            Debug.Entry(4, "**part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(\"Crystalline Point\")");
                            Debug.Entry(4, $"---- Base: {weaponPart.BaseDamage}");
                        }
                    }
                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                */
                }
            }
            Debug.Entry(3, "xxforeach (BodyPart part in body.GetParts())");
            Debug.Entry(3, "END Crystallinity_Patches.OnRegenerateDefaultEquipmentPrefix()");
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