using HarmonyLib;

using System;

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
    [HarmonyPatch]
    public static class Skulk_Tonic_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Skulk_Tonic_Patches));

        [HarmonyPatch(
            declaringType: typeof(Skulk_Tonic), 
            methodName: nameof(Skulk_Tonic.Apply),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        static void Apply_UseMutationEntry_Postfix(ref bool __result, ref Skulk_Tonic __instance, GameObject Object)
        {
            // Intercept the application of Burrowing Claws via Skulk_Tonic to apply the Mutations.xml version instead for compatibility.

            if (__result && __instance.BurrowingClawsID != Guid.Empty)
            {
                Mutations mutations = Object.RequirePart<Mutations>();
                mutations.RemoveMutationMod(__instance.BurrowingClawsID);
                MutationEntry burrowingClawsEntry = MutationFactory.GetMutationEntryByName("Burrowing Claws");
                int level = Object.HasPart(burrowingClawsEntry.Class) ? "2-3".RollCached() : 6;
                __instance.BurrowingClawsID = mutations.AddMutationMod(burrowingClawsEntry.Class, null, level, Mutations.MutationModifierTracker.SourceType.Tonic);
            }
        }
    } //!-- public static class Skulk_Tonic_Patches
}
