using HarmonyLib;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{   
    [HarmonyPatch]
    public static class PseudoGiganticCreature_GameObject_Patches
    {
        // Goal is to simulate being Gigantic for the purposes of calculating body weight, if the GameObject in question is PseudoGigantic
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeightPrefix(ref GameObject __state, ref GameObject __instance)
        {
            __state = __instance; // make the transferable object the current instance.
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature)
            {
                // is the GameObject PseudoGigantic but not Gigantic
                // Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
                // Debug.Entry(3, "GameObject.GetBodyWeight() > PseudoGigantic not Gigantic");
                //__state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)
                // Debug.Entry(2, "Trying to be Heavy and PseudoGigantic");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeightPostfix(ref GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                // Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                // Debug.Entry(3, "GameObject.GetBodyWeight() > PseudoGigantic and Gigantic");
                //__state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                // Debug.Entry(2, "Should be Heavy and PseudoGigantic\n");
            }
        }
    } //!-- public static class PseudoGiganticCreature_GameObject_Patches

    [HarmonyPatch]
    public static class GigantismPlusControlledWeight_GameObject_Patches
    {
        // Goal is to simulate being Gigantic for the purposes of calculating body weight, if the GameObject in question is PseudoGigantic
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeight_GigantismPlus_Prefix(ref GameObject __state, ref GameObject __instance)
        {
            __state = __instance;
            
            if (__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic
                
                Debug.Entry(4,
                $"# {nameof(GigantismPlusControlledWeight_GameObject_Patches)}."
                + $"{nameof(GetBodyWeight_GigantismPlus_Prefix)}(ref GameObject __state, ref GameObject __instance)",
                Indent: 0, Toggle: Options.doDebug);
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic (we revert this as soon as the origianl method completes)

                Debug.Entry(4,
                $"Was gigantic, not now",
                Indent: 1, Toggle: Options.doDebug);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.GetBodyWeight))]
        static void GetBodyWeight_GigantismPlus_Postfix(ref GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            if (!__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic

                __state.IsGiganticCreature = true; // make the GameObject Gigantic

                Debug.Entry(4,
                $"Wasn't gigantic, are now",
                Indent: 1, Toggle: Options.doDebug);
                Debug.Entry(4,
                $"x {nameof(GigantismPlusControlledWeight_GameObject_Patches)}."
                + $"{nameof(GetBodyWeight_GigantismPlus_Postfix)}(ref GameObject __state, ref GameObject __instance) #//",
                Indent: 0, Toggle: Options.doDebug);
            }
        }
    } //!-- public static class PseudoGiganticCreature_GameObject_Patches

    [HarmonyPatch]
    public static class GigantismPlusControlledCarryCap_GetMaxCarriedWeightEvent_Patches
    {
        // Goal is to simulate being Gigantic for the purposes of calculating carry cap, if the GameObject in question is PseudoGigantic
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GetMaxCarriedWeightEvent), nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetFor_GigantismPlus_Prefix(ref GameObject __state, ref GameObject __instance)
        {
            __state = __instance;
            
            if (__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control carry cap based on the creature being Gigantic
                
                Debug.Entry(4,
                $"# {nameof(GigantismPlusControlledCarryCap_GetMaxCarriedWeightEvent_Patches)}."
                + $"{nameof(GetFor_GigantismPlus_Prefix)}(ref GameObject __state, ref GameObject __instance)",
                Indent: 0, Toggle: Options.doDebug);
                __state.IsGiganticCreature = false; // make the GameObject not Gigantic (we revert this as soon as the origianl method completes)

                Debug.Entry(4,
                $"Was gigantic, not now",
                Indent: 1, Toggle: Options.doDebug);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GetMaxCarriedWeightEvent), nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetFor_GigantismPlus_Postfix(ref GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            if (!__state.IsGiganticCreature && __state.HasPart<GigantismPlus>())
            {
                // GigantismPlus wants to control weight based on the creature being Gigantic

                __state.IsGiganticCreature = true; // make the GameObject Gigantic

                Debug.Entry(4,
                $"Wasn't gigantic, are now",
                Indent: 1, Toggle: Options.doDebug);
                Debug.Entry(4,
                $"x {nameof(GigantismPlusControlledCarryCap_GetMaxCarriedWeightEvent_Patches)}."
                + $"{nameof(GetFor_GigantismPlus_Prefix)}(ref GameObject __state, ref GameObject __instance) #//",
                Indent: 0, Toggle: Options.doDebug);
            }
        }
    } //!-- public static class PseudoGiganticCreature_GameObject_Patches

    // Goal is to simulate being Gigantic for the purposes of calculating carry capacity, if the GameObject in question is PseudoGigantic
    [HarmonyPatch(typeof(GetMaxCarriedWeightEvent))]
    public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetMaxCarryWeightPrefix(ref GameObject Object, ref GameObject __state)
        {
            __state = Object;
            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && !__state.IsGiganticCreature)
            {
                // is the GameObject PseudoGigantic but not Gigantic
                // Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
                // Debug.Entry(3, "GetMaxCarriedWeightEvent.GetFor > PseudoGigantic not Gigantic");
                //__state.IsGiganticCreature = true; // make the GameObject Gigantic (we revert this as soon as the origianl method completes)
                // Debug.Entry(2, "Trying to have Carry Capacity and PseudoGigantic\n");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GetMaxCarriedWeightEvent.GetFor))]
        static void GetMaxCarryWeightPostfix(GameObject __state)
        {
            // only need __state this time, since it holds the __instance anyway.

            bool IsPretendBig = __state.HasPart<PseudoGigantism>();
            if (IsPretendBig && __state.IsGiganticCreature)
            {
                // is the GameObject both PseudoGigantic and Gigantic (only supposed to be possible here)
                // Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
                // Debug.Entry(3, "GetMaxCarriedWeightEvent.GetFor() > PseudoGigantic and Gigantic");
                //__state.IsGiganticCreature = false; // make the GameObject not Gigantic 
                // Debug.Entry(2, "Should have Carry Capacity and PseudoGigantic");
            }
        }

    } //!-- public static class PseudoGiganticCreature_GetMaxCarriedWeightEvent_Patches
    

    // Goal is to ensure that NaturalEquipment generated while having Gigantism actually get the gigantic modifier
    // including when the creature is PsuedoGigantic
    [HarmonyPatch(typeof(Body))]
    public static class PseudoGiganticCreature_RegenerateDefaultEquipment_Patches
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Body.RegenerateDefaultEquipment))]
        static void RegenerateDefaultEquipment_Prefix(ref GameObject __state, Body __instance)
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
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Body.RegenerateDefaultEquipment))]
        static void RegenerateDefaultEquipmentPostfix(GameObject __state)
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
