using HarmonyLib;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(Horns))]
    public static class Horns_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Horns.RegrowHorns))]
        static bool RegrowHorns_AlwaysForce_Prefix(ref Horns __instance, ref bool Force, ref GameObject ___HornsObject, ref string ___Variant)
        {
            Force = true;
            ___HornsObject = GameObject.Create(___Variant);
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Horns.RegrowHorns))]
        static void RegrowHorns_MaxStrengthBonus_Postfix(ref GameObject ___HornsObject)
        {
            MeleeWeapon HornsWeapon = ___HornsObject.GetPart<MeleeWeapon>();
            if (HornsWeapon != null)
            {
                HornsWeapon.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            }
        }
    }
}
