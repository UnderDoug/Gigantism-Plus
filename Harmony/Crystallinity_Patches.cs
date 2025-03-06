using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Crystallinity))]
    public static class Crystallinity_RefractChance_Patches
    {
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Crystallinity.Mutate))]
        static void MutatePrefix(Crystallinity __instance, GameObject GO)
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

    [HarmonyPatch(typeof(Crystallinity))]
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
}
