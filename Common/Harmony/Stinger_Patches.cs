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
    public static class Stinger_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(Stinger_Patches));

        [HarmonyPatch(
            declaringType: typeof(Stinger),
            methodName: nameof(Stinger.AddStingerTo),
            argumentTypes: new Type[] { typeof(BodyPart) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool AddStingerTo_NewStingerEveryTime_Prefix(ref Stinger __instance, ref BodyPart Limb, ref GameObject ___StingerObject)
        {
            if (Limb != null && ___StingerObject != null)
            {
                if (___StingerObject.EquipAsDefaultBehavior())
                {
                    Limb.DefaultBehavior = null;
                }
                ___StingerObject?.Obliterate();
                ___StingerObject = null;
            }
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(Stinger),
            methodName: nameof(Stinger.AddStingerTo),
            argumentTypes: new Type[] { typeof(BodyPart) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void AddStingerTo_NewStingerEveryTime_Postfix(ref Stinger __instance, ref BodyPart Limb, ref GameObject ___StingerObject)
        {
            if (Limb != null
                && ___StingerObject != null
                && ___StingerObject.EquipAsDefaultBehavior()
                && Limb.DefaultBehavior == ___StingerObject
                && Limb.DefaultBehaviorBlueprint != ___StingerObject.Blueprint)
            {
                Limb.DefaultBehaviorBlueprint = ___StingerObject.Blueprint;
            }
        }
    }
}
