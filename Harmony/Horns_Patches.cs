using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Horns_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(Horns), 
            methodName: nameof(Horns.RegrowHorns),
            argumentTypes: new Type[] { typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        static bool RegrowHorns_AlwaysForce_Prefix(ref Horns __instance, ref bool Force, ref GameObject ___HornsObject, ref string ___Variant)
        {
            Force = true;
            ___HornsObject = GameObject.Create(___Variant);
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(Horns),
            methodName: nameof(Horns.RegrowHorns),
            argumentTypes: new Type[] { typeof(bool) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
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
