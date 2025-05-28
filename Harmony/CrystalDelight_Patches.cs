using HarmonyLib;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Effects;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    
    // Intercept the application of Crystallinity via Crystal Delight, apply the Mutations.xml version instead (much more compatible).
    [HarmonyPatch]
    public static class CookingDomainSpecial_UnitCrystalTransform_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(CookingDomainSpecial_UnitCrystalTransform_Patches));

        /*
        [HarmonyPatch(typeof(CookingDomainSpecial_UnitCrystalTransform), nameof(CookingDomainSpecial_UnitCrystalTransform.ApplyTo))]
        [HarmonyPostfix]
        static void ApplyTo_Postfix(GameObject Object)
        {
            if (!(bool)EnableManagedVanillaMutations) return; // Skip if Crystallinity isn't being merged into the extended class.

            if (Object.TryGetPart(out Crystallinity Crystallinity))
            {
                Object.RequirePart<Mutations>().RemoveMutation(Crystallinity);
                MutationEntry CrystallinityEntry = MutationFactory.GetMutationEntryByName("Crystallinity");
                Object.RequirePart<Mutations>().AddMutation(CrystallinityEntry);
            }
        }
        */
    } //!-- public static class CookingDomainSpecial_UnitCrystalTransform_Patches
 }
