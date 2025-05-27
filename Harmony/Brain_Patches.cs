using HarmonyLib;

using System;
using System.Collections.Generic;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Brain_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(Brain),
            methodName: nameof(Brain.PreciseArmorScore),
            argumentTypes: new Type[] { typeof(GameObject), typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void PreciseArmorScore_SendEvent_Postfix(ref double __result, GameObject obj, GameObject who)
        {
            if (obj.HasRegisteredEvent("AdjustArmorScore"))
            {
                int score = (int)Math.Round(__result, MidpointRounding.AwayFromZero);
                Event @event = Event.New("AdjustArmorScore", "Score", score, "OriginalScore", __result, "User", who);
                obj.FireEvent(@event);
                int retrievedScore = @event.GetIntParameter("Score");
                if (score != retrievedScore)
                {
                    __result += retrievedScore - score;
                }
            }
        }
    }
}
