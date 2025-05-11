using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    // Stops natural equipment and cybernetics from being able to be disassembled if they happen to have mods.
    [HarmonyPatch(typeof(Tinkering_Disassemble))]
    public static class PreventCyberneticsBeingDisassembled
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Tinkering_Disassemble.CanBeConsideredScrap))]
        static void CanBeConsideredScrapPostfix(ref GameObject obj, ref bool __result)
        {
            if (obj != null && (obj.HasPart<CyberneticsBaseItem>() || obj.HasPart<NaturalEquipment>() || obj.IsNaturalEquipment()))
            {
                __result = false;
            }
        }
    }
}
