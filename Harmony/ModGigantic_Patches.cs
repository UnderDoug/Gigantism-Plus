using HarmonyLib;

using System;
using System.Collections.Generic;

using XRL.World;
using XRL.World.Parts;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class ModGigantic_HandleEventDisplayName_Patch
    {
        // adds shader to ModGigantic adjective
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.HandleEvent),
            argumentTypes: new Type[] { typeof(GetDisplayNameEvent) })]
        [HarmonyPostfix]
        static void HandleEvent_GetDisplayName_Postfix(GetDisplayNameEvent E)
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

                E.DB.SizeAdjective = Adjective.OptionalColorGigantic(Colorfulness);
            }
        } //!-- public static class ModGigantic_HandleEventDisplayName_Patches
    }

    [HarmonyPatch]
    public static class ModGigantic_ModificationApplied_Patch
    {
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.ApplyModification),
            argumentTypes: new Type[] { typeof(GameObject) })]
        [HarmonyPrefix]
        static bool ModificationApplied_AdditionalEffects_Prefix(ref ModGigantic __instance, GameObject Object)
        {
            BeforeModGiganticAppliedEvent.Send(Object, __instance);
            return true;
        }

        [HarmonyPatch(typeof(ModGigantic), nameof(ModGigantic.ApplyModification), new Type[] { typeof(GameObject) })]
        [HarmonyPostfix]
        static void ModificationApplied_AdditionalEffects_Postfix(ref ModGigantic __instance, GameObject Object)
        {
            AfterModGiganticAppliedEvent.Send(Object, __instance);
        }
    } //!-- public static class ModGigantic_ModificationApplied_Patches

    [HarmonyPatch]
    public static class ModGigantic_GetDescription_Patches
    {
        // overwrites the entire GetDescrption method (it's not super easy to target specific locations throughout) to include the
        // additions from the above events.
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.GetDescription),
            argumentTypes: new Type[] { typeof(int), typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool GetDescription_AdditionalEffects_Prefix(int Tier, GameObject Object, ref string __result)
        {
            if (Object == null)
            {
                // send to default
                return true;
            }

            BeforeDescribeModGiganticEvent beforeEvent = new(Object, null);
            beforeEvent.Send();
            DescribeModGiganticEvent afterEvent = new(Object, null, beforeEvent);

            __result = afterEvent.Send().Process();

            BeforeDescribeModGiganticEvent.ResetTo(ref beforeEvent);
            DescribeModGiganticEvent.ResetTo(ref afterEvent);

            return false;
        }
    } //!-- public static class ModGigantic_GetDescription_Patches
}
