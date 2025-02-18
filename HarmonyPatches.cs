using HarmonyLib;
using System;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using Mods.GigantismPlus;

namespace Mods.GigantismPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.GameObject))]
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
            if (__state.HasPart<PseudoGigantism>() && !__state.IsGiganticCreature) 
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

            if (__state.HasPart<PseudoGigantism>() && __state.IsGiganticCreature)
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

    [HarmonyPatch(typeof(XRL.World.GetMaxCarriedWeightEvent))]
    public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetMaxCarryWeightPrefix(ref GameObject Object, ref GameObject __state) 
        {
            // Object matches the paramater of the original,
            // __state lets you keep stuff between Pre- and Postfixes (might be redundant for this one)

            __state = Object;
            if (__state.HasPart<PseudoGigantism>() && !__state.IsGiganticCreature)
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

            if (__state.HasPart<PseudoGigantism>() && __state.IsGiganticCreature)
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
        [HarmonyPatch(typeof(XRL.World.Parts.ModGigantic), "HandleEvent", new Type[] { typeof(GetDisplayNameEvent) })]
        static void ModGiganticPatch(GetDisplayNameEvent E)
        {
            GetDisplayNameEventOverride(E, "gigantic", 30, true);
        }

    } //!--- public static class ModGiganticDisplayName_Shader

    [HarmonyPatch(typeof(XRL.World.Parts.Mutation.BurrowingClaws))]
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(XRL.World.Parts.Mutation.BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipmentPrefix(XRL.World.Parts.Mutation.BurrowingClaws __instance, Body body)
        {
            foreach (BodyPart hand in body.GetParts())
            {
                if (hand.Type == "Hand")
                {
                    if (__instance.ParentObject.HasPart<XRL.World.Parts.Mutation.GigantismPlus>())
                    {
                        var gigantism = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.GigantismPlus>();
                        if (__instance.ParentObject.HasPart<XRL.World.Parts.Mutation.ElongatedPaws>())
                        {
                            if (gigantism.GiganticElongatedBurrowingClawObject == null)
                            {
                                gigantism.GiganticElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedBurrowingClaw");
                            }
                            hand.DefaultBehavior = gigantism.GiganticElongatedBurrowingClawObject;
                            var elongatedPaws = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.ElongatedPaws>();
                            var weapon = gigantism.GiganticElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                            weapon.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize}+{(elongatedPaws.StrengthModifier / 2) + 3}";
                            weapon.HitBonus = gigantism.FistHitBonus;
                            weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                        }//GiganticElongatedBurrowingClawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                        else
                        {
                            if (gigantism.GiganticBurrowingClawObject == null)
                            {
                                gigantism.GiganticBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticBurrowingClaw");
                            }
                            hand.DefaultBehavior = gigantism.GiganticBurrowingClawObject;
                            var weapon = gigantism.GiganticBurrowingClawObject.GetPart<MeleeWeapon>();
                            weapon.BaseDamage = XRL.World.Parts.Mutation.GigantismPlus.GetFistBaseDamage(__instance.Level);
                            weapon.HitBonus = gigantism.FistHitBonus;
                            weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                        }//GiganticBurrowingClawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                    }
                    else if (__instance.ParentObject.HasPart<XRL.World.Parts.Mutation.ElongatedPaws>())
                    {
                        var elongatedPaws = __instance.ParentObject.GetPart<XRL.World.Parts.Mutation.ElongatedPaws>();
                        if (elongatedPaws.ElongatedBurrowingClawObject == null)
                        {
                            elongatedPaws.ElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("ElongatedBurrowingClaw");
                        }
                        hand.DefaultBehavior = elongatedPaws.ElongatedBurrowingClawObject;
                        var weapon = elongatedPaws.ElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"1d5+{elongatedPaws.StrengthModifier}";
                    }//ElongatedBurrowingClawObject uses 1d5 + StrengthMod.
                    else
                    {
                        if (hand.DefaultBehavior == null || hand.DefaultBehavior.GetBlueprint(true).Name != "Burrowing Claws")
                        {
                            hand.DefaultBehavior = GameObjectFactory.Factory.CreateObject("Burrowing Claws");
                        }
                        var weapon = hand.DefaultBehavior.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = __instance.GetClawsDamage(__instance.Level);
                    }//Uses basic Burrowing Claws behavior, only on all hands.
                }
            }
            return false; // Skip the original method
        }
    }
}