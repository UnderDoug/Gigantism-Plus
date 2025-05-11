using HarmonyLib;

using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(MeleeWeapon))]
    public static class MeleeWeapon_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.GetSimplifiedStats))]
        public static bool GetSimplifiedStats_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.GetDetailedStats))]
        public static bool GetDetailedStats_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.AdjustBonusCap))]
        public static bool AdjustBonusCap_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }
    } //!-- public static class MaxStrengthBonus_Display_Patches

}
