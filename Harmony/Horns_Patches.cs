using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
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
    }
}
