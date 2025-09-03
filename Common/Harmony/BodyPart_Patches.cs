using HarmonyLib;

using System;

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
    public static class BodyPart_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(BodyPart_Patches));

        [HarmonyPatch(
            declaringType: typeof(BodyPart),
            methodName: nameof(BodyPart.Implant),
            argumentTypes: new Type[] { typeof(GameObject), typeof(bool), typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void Implant_UpdateBodyParts_Postfix(BodyPart __instance)
        {
            BodyPart @this = __instance;

            Debug.Entry(4, $"{nameof(Body_Patches)}.{nameof(BodyPart.Implant)}(BodyPart __instance)", Indent: 0, Toggle: doDebug);

            Debug.Entry(4, $"Implantee is {@this.ParentBody?.ParentObject?.DebugName ?? NULL}", Indent: 1, Toggle: doDebug);

            @this?.ParentBody?.UpdateBodyParts();
        }

        [HarmonyPatch(
            declaringType: typeof(BodyPart),
            methodName: nameof(BodyPart.DefaultBehavior),
            methodType: MethodType.Setter)]
        [HarmonyPostfix]
        public static void set_DefaultBehavior_UpdateDefaultBehaviorBlueprint_Postfix(ref BodyPart __instance)
        {
            BodyPart @this = __instance;

            Debug.Entry(4, $"{nameof(Body_Patches)}.{nameof(BodyPart.DefaultBehavior)}(ref BodyPart __instance)", Indent: 0, Toggle: doDebug);

            Debug.Entry(4, $"Wielder is {@this.ParentBody?.ParentObject?.DebugName ?? NULL}", Indent: 1, Toggle: doDebug);

            __instance.DefaultBehaviorBlueprint = __instance.DefaultBehavior?.Blueprint;
        }
    }
}
