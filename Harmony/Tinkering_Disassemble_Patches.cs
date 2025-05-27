using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    // Stops natural equipment and cybernetics from being able to be disassembled if they happen to have mods.
    [HarmonyPatch]
    public static class PreventCyberneticsBeingDisassembled
    {
        [HarmonyPatch(
            declaringType: typeof(Tinkering_Disassemble), 
            methodName: nameof(Tinkering_Disassemble.CanBeConsideredScrap),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        static void CanBeConsideredScrapPostfix(ref GameObject obj, ref bool __result)
        {
            if (obj != null && (obj.HasPart<CyberneticsBaseItem>() || obj.HasPart<NaturalEquipment>() || obj.IsNaturalEquipment()))
            {
                __result = false;
            }
        }
    }
}
