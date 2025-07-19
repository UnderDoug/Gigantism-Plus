using HarmonyLib;

using System;

using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class MeleeWeapon_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(MeleeWeapon_Patches));

        [HarmonyPatch(
            declaringType: typeof(MeleeWeapon), 
            methodName: nameof(MeleeWeapon.GetSimplifiedStats),
            argumentTypes: new Type[] { typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool GetSimplifiedStats_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(MeleeWeapon), 
            methodName: nameof(MeleeWeapon.GetDetailedStats))]
        [HarmonyPrefix]
        public static bool GetDetailedStats_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(MeleeWeapon), 
            methodName: nameof(MeleeWeapon.AdjustBonusCap),
            argumentTypes: new Type[] { typeof(int) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool AdjustBonusCap_CapMaxStrBonus_Prefix(MeleeWeapon __instance)
        {
            // If the melee weapon's MaxStrengthBonus is greater than 999, cap it at that.
            if (__instance.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED) __instance.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            return true;
        }
    } //!-- public static class MaxStrengthBonus_Display_Patches

}
