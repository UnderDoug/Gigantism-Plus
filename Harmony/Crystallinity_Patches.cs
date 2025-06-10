using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Crystallinity_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Crystallinity_Patches));

        /*
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.Mutate))]
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
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.Mutate))]
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
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.GetLevelText))]
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
        */
    }//!-- public static class Crystallinity_Patches
}
