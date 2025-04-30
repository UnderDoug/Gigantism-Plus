using HarmonyLib;

using System.Collections.Generic;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Brain))]
    public static class Brain_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Brain.PreciseArmorScore))]
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
