using HarmonyLib;
using XRL.World.Parts;

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
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.GetDetailedStats))]
        static bool GetDetailedStatsPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MeleeWeapon.AdjustBonusCap))]
        static bool AdjustBonusCapPrefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

    } //!-- public static class MaxStrengthBonus_Display_Patches

}
