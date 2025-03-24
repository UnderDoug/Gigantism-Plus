using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

namespace HNPS_GigantismPlus
{
    [HarmonyPatch(typeof(MeleeWeapon))]
    public static class MaxStrengthBonus_Display_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.GetSimplifiedStats))]
        static bool GetSimplifiedStatsPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.GetDetailedStats))]
        static bool GetDetailedStatsPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.AdjustBonusCap))]
        static bool AdjustBonusCapPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > 999) __instance.MaxStrengthBonus = 999;
            return true;
        }

    } //!-- public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches

    // Stops natural equipment and cybernetics from being able to be disassembled if they happen to have mods.
    [HarmonyPatch(typeof(Tinkering_Disassemble))]
    public static class PreventCyberneticsBeingDisassembled
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Tinkering_Disassemble.CanBeConsideredScrap))]
        static void CanBeConsideredScrapPostfix(ref GameObject obj, ref bool __result)
        {
            if (obj != null && (obj.HasPart<CyberneticsBaseItem>() || obj.HasPart<NaturalEquipment>()))
            {
                __result = false;
            }
        }
    }

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
