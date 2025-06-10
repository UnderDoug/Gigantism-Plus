using HarmonyLib;

using System;

using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class MutationBGoneWishHandler_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(MutationBGoneWishHandler_Patches));

        [HarmonyPatch(
            declaringType: typeof(MutationBGoneWishHandler),
            methodName: nameof(MutationBGoneWishHandler.MutationBGone),
            argumentTypes: new Type[] { typeof(string) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool MutationBGone_WorkOnEntryName_Prefix(ref bool __result, string argument)
        {
            Debug.Entry(4,
                $"# {nameof(MutationBGoneWishHandler_Patches)}." +
                $"{nameof(MutationBGone_WorkOnEntryName_Prefix)}(string argument: {argument})",
                Indent: 0, Toggle: doDebug);

            Mutations mutations = MutationBGoneWishHandler.GetMutations();
            BaseMutation baseMutation = mutations.GetMutationByName(argument);
            if (baseMutation != null)
            {
                MutationBGoneWishHandler.RemoveMutation(mutations, baseMutation);
                __result = true;
                return false;
            }
            return true;
        }
    }
}
