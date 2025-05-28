using HarmonyLib;

using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.Language;
using XRL.World.ObjectBuilders;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class ModGigantic_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(ModGigantic_Patches));

        /// <summary>
        /// Goal is to display the size adjective "gigantic" with the shader added by this mod.
        /// </summary>
        /// <param name="E">The GetDisplayNameEvent the base game modification uses.</param>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.HandleEvent),
            argumentTypes: new Type[] { typeof(GetDisplayNameEvent) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        static void HandleEvent_GetDisplayName_Postfix(GetDisplayNameEvent E)
        {
            string Adjective = "gigantic";
            int Priority = 30;
            bool IncludeProperName = false;
            if (E.Object.HasProperName && !IncludeProperName) return; // skip for Proper Named items, unless including them.
            if (E.Object.HasTagOrProperty("ModGiganticNoDisplayName")) return; // skip for items that explicitly hide the adjective
            if (E.Object.HasTagOrProperty("ModGiganticNoUnknownDisplayName")) return; // skip for unknown items that explicitly hide the adjective

            if (E.DB.SizeAdjective == Adjective && E.DB.SizeAdjectivePriority == Priority)
            {
                // The base event runs every game tick for equipped range weapons.
                // possibly due to the item being displayed in the UI (bottom right)
                // Any form of output here will completely clog up the logs.

                E.DB.SizeAdjective = Adjective.OptionalColorGigantic(Colorfulness);
            }
        } //!-- public static class ModGigantic_HandleEventDisplayName_Patches

        /// <summary>
        /// Goal is to expose the modification's application process to allow other modders to easily add their own functionality when items are modified to be be gigantic.
        /// </summary>
        /// <param name="__instance">The ModGigantic instance being applied.</param>
        /// <param name="Object">The object to which the modification is being applied.</param>
        /// <returns>true, so that the base game functionality remains.</returns>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.ApplyModification),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        static bool ModificationApplied_AdditionalEffects_Prefix(ref ModGigantic __instance, GameObject Object)
        {
            BeforeModGiganticAppliedEvent.Send(Object, __instance);
            return true;
        }

        /// <summary>
        /// Goal is to expose the modification's application process to allow other modders to easily add their own functionality when items are modified to be be gigantic.
        /// </summary>
        /// <param name="__instance">The ModGigantic instance being applied.</param>
        /// <param name="Object">The object to which the modification is being applied.</param>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic), 
            methodName: nameof(ModGigantic.ApplyModification), 
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        static void ModificationApplied_AdditionalEffects_Postfix(ref ModGigantic __instance, GameObject Object)
        {
            AfterModGiganticAppliedEvent.Send(Object, __instance);
        }

        /// <summary>
        /// Overwrites the entire GetDescrption method (it's not super easy to target specific locations throughout) to include the additions from the above events.
        /// </summary>
        /// <param name="Object">The object to which the modification is being applied.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <returns>
        /// true, if there's no object to let the base game handle that case,<br></br>
        /// false, if we do anything sicne we're overriding the base game method.</returns>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.GetDescription),
            argumentTypes: new Type[] { typeof(int), typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool GetDescription_AdditionalEffects_Prefix(GameObject Object, ref string __result)
        {
            if (Object == null)
            {
                // send to default
                return true;
            }

            BeforeDescribeModGiganticEvent beforeEvent = BeforeDescribeModGiganticEvent.CollectFor(Object);
            __result = DescribeModGiganticEvent.Send(beforeEvent).Process();

            return false;
        }

        /// <summary>
        /// Goal is to make ModGigantic objects simulate being built with the Gigantified object builder.
        /// </summary>
        /// <param name="__instance">The ModGigantic instance upon which the event was fired.</param>
        /// <param name="E">The event which we're checking for an additional condition.</param>
        /// <returns>true, allowing the event to continue on its way being handled.</returns>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.FireEvent),
            argumentTypes: new Type[] { typeof(Event) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool FireEvent_ObjectAnimated_MakeGigantic_Prefix(ref ModGigantic __instance, Event E)
        {
            if (E.ID == "Animate")
            {
                ModGigantic @this = __instance;
                GameObject Object = E.GetGameObjectParameter(nameof(Object));

                List<IPart> PartsToRemove = E.GetParameter<List<IPart>>(nameof(PartsToRemove));
                PartsToRemove.TryAdd(@this);
                E.SetParameter(nameof(PartsToRemove), PartsToRemove);

                int objectTier = Gigantified.GetObjectTier(Object);

                int level = Gigantified.GetLevel(objectTier);

                int stews = Gigantified.GetStews();
                stews += 
                    Object.HasPart<LiquidVolume>() 
                    ? 6 
                    : 0
                    ;

                int tier = Gigantified.GetTier();

                string namePrefix = Gigantified.GetNamePrefix();

                Gigantified.Gigantify(Object, level, stews, tier, namePrefix, "Animate");
            }
            return true;
        }

        /// <summary>
        /// Goal is to make ModGigantic objects simulate being built with the Gigantified object builder.
        /// </summary>
        /// <param name="Registrar">The IEventRegistrar for the object ModGigantic is attached to.</param>
        /// <returns>true, allowing the event to continue on its way being handled.</returns>
        [HarmonyPatch(
            declaringType: typeof(ModGigantic),
            methodName: nameof(ModGigantic.Register),
            argumentTypes: new Type[] { typeof(GameObject), typeof(IEventRegistrar) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPrefix]
        public static bool Register_ObjectAnimated_MakeGigantic_Prefix(IEventRegistrar Registrar)
        {
            Registrar.Register("Animate");
            return true;
        }
    } //!-- public static class ModGigantic_GetDescription_Patches
}
