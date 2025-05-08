using HarmonyLib;

using System;

using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class MutationBGoneWishHandler_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(
            declaringType: typeof(MutationBGoneWishHandler),
            methodName: nameof(MutationBGoneWishHandler.MutationBGone),
            argumentTypes: new Type[] { typeof(string) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        public static bool MutationBGone_WorkOnEntryName_Prefix(string argument)
        {
            Debug.Entry(4,
                $"# {nameof(MutationBGoneWishHandler_Patches)}." +
                $"{nameof(MutationBGone_WorkOnEntryName_Prefix)}(string argument: {argument})",
                Indent: 0);

            Mutations mutations = The.Player.GetPart<Mutations>();
            BaseMutation mutation = mutations.GetMutationByName(argument);
            if (mutation != null)
            {
                argument = mutation.Name;
            }
            return true;
        }
    }
}
