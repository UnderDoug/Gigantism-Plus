using HarmonyLib;
using XRL.World.Parts;
using XRL.World;

namespace Mods.GigantismPlus
{
    [HarmonyPatch(typeof(ModGigantic))]
    [HarmonyPatch("ApplyModification")]
    public static class ModGigantic_LightSourcePatch
    {
        static void Postfix(ModGigantic __instance, GameObject Object)
        {
            LightSource part = Object.GetPart<LightSource>();
            if (part != null)
            {
                part.Radius *= 2;
            }
        }
    }

    [HarmonyPatch(typeof(ModGigantic))]
    [HarmonyPatch("ApplyModification")]
    public static class ModGigantic_WeightWhenWornPatch
    {
        static void Postfix(ModGigantic __instance, GameObject Object)
        {
            Backpack part = Object.GetPart<Backpack>();
            if (part != null)
            {
                part.WeightWhenWorn = (int)(part.WeightWhenWorn * 2.5f);
            }
        }
    }

    [HarmonyPatch(typeof(ModGigantic))]
    [HarmonyPatch("ApplyModification")]
    public static class ModGigantic_CarryBonusPatch 
    {
        static void Postfix(ModGigantic __instance, GameObject Object)
        {
            Armor part = Object.GetPart<Armor>();
            if (part != null && part.CarryBonus > 0)
            {
                part.CarryBonus = (int)(part.CarryBonus * 1.25f);
            }
        }
    }
}
