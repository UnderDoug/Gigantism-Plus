using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    
    [HarmonyPatch(typeof(GameObject))]
    public static class ForceDefaultBehaviorToAssignEquipped
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameObject.CheckDefaultBehaviorGiganticness))]
        static void CheckDefaultBehaviorGiganticnessPostfix(ref GameObject __instance, ref GameObject Equipper)
        {
            GameObject @this = __instance;

            if (@this.Physics.Equipped != Equipper) @this.Physics.Equipped = Equipper;
        }
    }
}
