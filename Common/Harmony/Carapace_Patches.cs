using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Carapace_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Carapace_Patches));

        [HarmonyPatch(
            declaringType: typeof(Carapace), 
            methodName: nameof(Carapace.AddCarapaceTo),
            argumentTypes: new Type[] { typeof(BodyPart) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        static bool AddCarapaceTo_NewCarapaceEveryTime_Prefix(ref Carapace __instance, ref BodyPart body, ref GameObject ___CarapaceObject)
        {
            if (body != null && ___CarapaceObject != null)
            {
                GameObject.Release(ref ___CarapaceObject);
            }
            return true;
        }
    }
}
