using HarmonyLib;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using XRL.World.Parts.Mutation;
using System;

namespace HNPS_GigantismPlus.Harmony
{   
    // Goal is to block the default weight-increasing behaviour if the GameObject in question is Gigantic(Plus)
    [HarmonyPatch]
    public static class GigantismPlus_ControlledWeight_GameObject_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.GetBodyWeight))]
        [HarmonyPrefix]
        static void GetBodyWeight_GigantismPlus_Prefix(ref GameObject __state, ref GameObject __instance)
        {
            __state = __instance;
            
            if (__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic
                
                Debug.Entry(4,
                $"# {nameof(GigantismPlus_ControlledWeight_GameObject_Patches)}."
                + $"{nameof(GetBodyWeight_GigantismPlus_Prefix)}(ref GameObject __state, ref GameObject __instance)",
                Indent: 0, Toggle: Options.doDebug);
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic (we revert this as soon as the origianl method completes)

                Debug.Entry(4,
                $"Was gigantic, not now",
                Indent: 1, Toggle: Options.doDebug);
            }
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject), 
            methodName: nameof(GameObject.GetBodyWeight))]
        [HarmonyPostfix]
        static void GetBodyWeight_GigantismPlus_Postfix(ref GameObject __state)
        {
            if (!__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic

                __state.IsGiganticCreature = true; // make the GameObject Gigantic

                Debug.Entry(4,
                $"Wasn't gigantic, are now",
                Indent: 1, Toggle: Options.doDebug);
                Debug.Entry(4,
                $"x {nameof(GigantismPlus_ControlledWeight_GameObject_Patches)}."
                + $"{nameof(GetBodyWeight_GigantismPlus_Postfix)}(ref GameObject __state, ref GameObject __instance) #//",
                Indent: 0, Toggle: Options.doDebug);
            }
        }
    } //!-- public static class GigantismPlus_ControlledWeight_GameObject_Patches

    // Goal is to block the default CarryCap-increasing behaviour if the GameObject in question is Gigantic(Plus)
    [HarmonyPatch]
    public static class GigantismPlus_ControlledCarryCap_GetMaxCarriedWeightEvent_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(GetMaxCarriedWeightEvent),
            methodName: nameof(GetMaxCarriedWeightEvent.GetFor),
            argumentTypes: new Type[] { typeof(GameObject), typeof(double) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPrefix]
        static void GetFor_GigantismPlus_Prefix(ref GameObject __state, GameObject Object)
        {
            __state = Object;
            
            if (__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control carry cap based on the creature being Gigantic
                
                Debug.Entry(4,
                $"# {nameof(GigantismPlus_ControlledCarryCap_GetMaxCarriedWeightEvent_Patches)}."
                + $"{nameof(GetFor_GigantismPlus_Prefix)}" 
                + $"(ref {nameof(GameObject)} {nameof(__state)},"
                + $" {nameof(GameObject)} {nameof(Object)})",
                Indent: 0, Toggle: Options.doDebug);

                __state.IsGiganticCreature = false; // make the GameObject not Gigantic (we revert this as soon as the origianl method completes)

                Debug.Entry(4,
                $"Was gigantic, not now",
                Indent: 1, Toggle: Options.doDebug);
            }
        }

        [HarmonyPatch(
            declaringType: typeof(GetMaxCarriedWeightEvent),
            methodName: nameof(GetMaxCarriedWeightEvent.GetFor),
            argumentTypes: new Type[] { typeof(GameObject), typeof(double) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        static void GetFor_GigantismPlus_Postfix(ref GameObject __state)
        {
            if (!__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic

                __state.IsGiganticCreature = true; // make the GameObject Gigantic

                Debug.Entry(4,
                $"Wasn't gigantic, are now",
                Indent: 1, Toggle: Options.doDebug);
                Debug.Entry(4,
                $"x {nameof(GigantismPlus_ControlledCarryCap_GetMaxCarriedWeightEvent_Patches)}."
                + $"{nameof(GetFor_GigantismPlus_Prefix)}" 
                + $"(ref {nameof(GameObject)} {nameof(__state)}) #//",
                Indent: 0, Toggle: Options.doDebug);
            }
        }
    } //!-- public static class GigantismPlus_ControlledCarryCap_GetMaxCarriedWeightEvent_Patches

    // Goal is to ensure that NaturalEquipment generated while having Gigantism actually get the gigantic modifier
    // including when the creature is PsuedoGigantic
    [HarmonyPatch]
    public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(Body),
            methodName: nameof(Body.RegenerateDefaultEquipment))]
        [HarmonyPrefix]
        public static bool RegenerateDefaultEquipment_Prefix(ref GameObject __state, ref Body __instance)
        {
            __state = __instance.ParentObject;
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature)
            {
                // is the GameObject PseudoGigantic but not Gigantic
                Debug.Entry(3, 
                    $"{nameof(Body)}." + 
                    $"{nameof(Body.RegenerateDefaultEquipment)}() " + 
                    $"-> PseudoGigantic not Gigantic",
                    Indent: 0);

                __state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)

                Debug.Entry(2, $"Trying to generate gigantic natural equipment while PseudoGigantic", Indent: 1);
            }
            return true;
        }

        [HarmonyPatch(
            declaringType: typeof(Body),
            methodName: nameof(Body.RegenerateDefaultEquipment))]
        [HarmonyPostfix]
        public static void RegenerateDefaultEquipmentPostfix(ref GameObject __state)
        {
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                Debug.Entry(3,
                    $"{nameof(Body)}." +
                    $"{nameof(Body.RegenerateDefaultEquipment)}() " +
                    $"-> PseudoGigantic not Gigantic",
                    Indent: 0);

                __state.IsGiganticCreature = false; // make the GameObject not Gigantic 

                Debug.Entry(3, "Should have generated gigantic natural equipment while PseudoGigantic", Indent: 1);
            }
        }

    } //!-- public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches
}
