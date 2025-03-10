using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

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
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3, $"[HarmonyPatch(nameof(Crystallinity.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}");

            if (body == null) return true;

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    UD_ManagedCrystallinity managedCrystallinity = __instance.ConvertToManaged();
                    managedCrystallinity.HasGigantism = actor.HasPart<GigantismPlus>();
                    managedCrystallinity.HasElongated = actor.HasPart<ElongatedPaws>();
                    managedCrystallinity.HasBurrowing = actor.HasPartDescendedFrom<BurrowingClaws>();
                    part.DefaultBehavior.ApplyModification(managedCrystallinity.GetNaturalWeaponMod(Managed: false), Actor: actor);

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "Crystallinity_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method
        }
    }//!-- public static class Crystallinity_Patches
}
