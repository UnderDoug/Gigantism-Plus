using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Crystallinity))]
    public static class Crystallinity_Patches
    {
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Crystallinity.Mutate))]
        static void Mutate_Prefix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<GigantismPlus>())
            {
                // Wait for the original method to create the RefractLight part
                // Then in the postfix we'll modify its chance
                __instance.RefractAdded = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crystallinity.Mutate))]
        static void Mutate_Postfix(Crystallinity __instance, GameObject GO)
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

        // Modify the Crystallinity mutation level text to include the GigantismPlus bonus
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Crystallinity.GetLevelText))]
        static void GetLevelText_Postfix(Crystallinity __instance, ref string __result)
        {
            if (__instance.ParentObject.HasPart<GigantismPlus>())
            {
                // Replace the original 25% text with our modified version
                __result = __result.Replace(
                    "25% chance to refract light-based attacks",
                    "35% chance to refract light-based attacks (25% base chance {{rules|+10%}} from {{gigantism|Gigantism}} ({{r|D}}))"
                );
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(Crystallinity __instance, Body body)
        {
            if ((bool)EnableManagedVanillaMutations) return true;
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(Crystallinity_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            bool ShouldContinue = false;

            if (body != null)
            {
                UD_ManagedCrystallinity managed = __instance.ConvertToManaged();
                managed.HasGigantism = actor.HasPart<GigantismPlus>();
                managed.HasElongated = actor.HasPart<ElongatedPaws>();
                managed.HasBurrowing = actor.HasPartDescendedFrom<BurrowingClaws>();
                managed.NaturalWeaponSubpart.Managed = false;
                managed.UpdateNaturalWeaponSubpart(managed.NaturalWeaponSubpart, managed.Level);
                managed.OnRegenerateDefaultEquipment(body);
                managed.OnDecorateDefaultEquipment(body);
            }
            else
            {
                ShouldContinue = true;
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
            }

            /*
            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "> foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    UD_ManagedCrystallinity managedCrystallinity = __instance.ConvertToManaged();
                    managedCrystallinity.HasGigantism = actor.HasPart<GigantismPlus>();
                    managedCrystallinity.HasElongated = actor.HasPart<ElongatedPaws>();
                    managedCrystallinity.HasBurrowing = actor.HasPartDescendedFrom<BurrowingClaws>();
                    part.DefaultBehavior.ApplyModification(managedCrystallinity.GetNaturalEquipmentModName(Managed: false), Actor: actor);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);

            Exit:
            */

            Debug.Entry(3, $"Skipping patched Method: {!ShouldContinue}", Indent: 1);
            Debug.Footer(3, $"{nameof(Crystallinity_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            return ShouldContinue; // Skip the the original method if we do anything.
        }
    }//!-- public static class Crystallinity_Patches
}
