using HarmonyLib;
using System;
using XRL.World;
using XRL.World.Parts;
using HNPS_GigantismPlus;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(ModGigantic))]
    public static class ModGigantic_DisplayName_Shader
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModGigantic), nameof(ModGigantic.HandleEvent), new Type[] { typeof(GetDisplayNameEvent) })]
        static void ModGiganticPatch(GetDisplayNameEvent E)
        {
            // goal display the SizeAdjective gigantic with its associated shader.
            string Adjective = "gigantic";
            int Priority = 30;
            bool IncludeProperName = false;
            if (E.Object.HasProperName && !IncludeProperName) return; // skip for Proper Named items, unless including them.
            if (E.Object.HasTagOrProperty("ModGiganticNoDisplayName")) return; // skip for items that explicitly hide the adjective
            if (E.Object.HasTagOrProperty("ModGiganticNoUnknownDisplayName")) return; // skip for unknown items that explicitly hide the adjective
            if (!E.Understood()) return; // skip items not understood by the player

            if (E.DB.SizeAdjective == Adjective && E.DB.SizeAdjectivePriority == Priority)
            {
                // The base event runs every game tick for equipped range weapons.
                // possibly due to the item being displayed in the UI (bottom right)
                // Any form of output here will completely clog up the logs.

                E.DB.SizeAdjective = Adjective.MaybeColor("gigantic");
            }
        }


    } //!--- public static class ModGiganticDisplayName_Shader



    [HarmonyPatch(nameof(ModGigantic.ApplyModification))]
    public static class ModGigantic_LightSourcePatch
    {
        static void ApplyModification_LightSource_Postfix(ModGigantic __instance, GameObject Object)
        {
            LightSource part = Object.GetPart<LightSource>();
            if (part != null)
            {
                part.Radius *= 2;
            }
        }
    }

    [HarmonyPatch(nameof(ModGigantic.ApplyModification))]
    public static class ModGigantic_WeightWhenWornPatch
    {
        static void ApplyModification_WeightWhenWorn_Postfix(ModGigantic __instance, GameObject Object)
        {
            Backpack part = Object.GetPart<Backpack>();
            if (part != null)
            {
                part.WeightWhenWorn = (int)(part.WeightWhenWorn * 2.5f);
            }
        }
    }

    [HarmonyPatch(nameof(ModGigantic.ApplyModification))]
    public static class ModGigantic_CarryBonusPatch 
    {
        static void ApplyModification_CarryBonus_Postfix(ModGigantic __instance, GameObject Object)
        {
            Armor part = Object.GetPart<Armor>();
            if (part != null && part.CarryBonus > 0)
            {
                part.CarryBonus = (int)(part.CarryBonus * 1.25f);
            }
        }
    }
}
