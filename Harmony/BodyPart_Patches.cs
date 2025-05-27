using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class BodyPart_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(BodyPart),
            methodName: nameof(BodyPart.Implant),
            argumentTypes: new Type[] { typeof(GameObject), typeof(bool), typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void Implant_UpdateBodyParts_Postfix(BodyPart __instance)
        {
            BodyPart @this = __instance;

            Debug.Entry(4, $"{typeof(Body_Patches).Name}.{nameof(Implant_UpdateBodyParts_Postfix)}(ref Actor __instance)", Indent: 0);

            Debug.Entry(4, $"Object is {@this.ParentBody?.ParentObject?.DebugName ?? NULL}", Indent: 1);

            @this?.ParentBody?.UpdateBodyParts();
        }
    }
}
