using HarmonyLib;

using System;
using System.Collections.Generic;

using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;
using XRL;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class GameObject_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(GameObject_Patches));

        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.IsGiganticCreature),
            methodType: MethodType.Getter)]
        [HarmonyPrefix]
        public static bool IsGiganticCreature_getter_CheckEvent_Prefix(ref GameObject __instance, ref bool __result)
        {
            __result = false;
            if (GetGiganticCreatureEvent.CheckFor(__instance, out bool Strict))
            {
                __result = true;
                if (Strict)
                    return false;
            }

            if (MutationFactory.GetMutationEntryByName("Gigantism") is MutationEntry gigantismEntry
                && __instance.HasPart(gigantismEntry.Class))
                __result = true;

            return !__result;
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.IsGiganticCreature),
            methodType: MethodType.Setter)]
        [HarmonyPrefix]
        public static bool IsGiganticCreature_setter_CheckEvent_Prefix(ref GameObject __instance, ref bool value)
        {
            bool @override = false;
            bool isGigantic = __instance.IsGiganticCreature;
            if (value != isGigantic)
            {
                bool setValue = !SetGiganticCreatureEvent.CheckFor(__instance, out @override, isGigantic);
                value = @override ? setValue : value;
                if (value)
                {
                    if (MutationFactory.GetMutationEntryByName("Gigantism") is MutationEntry gigantismEntry)
                        __instance.RequirePart<Mutations>().AddMutation(gigantismEntry.Mutation);
                }
                else
                if (MutationFactory.GetMutationEntryByName("Gigantism") is MutationEntry gigantismEntry
                    && __instance.GetPart(gigantismEntry.Class) is BaseMutation gigantismMutation)
                    __instance.RequirePart<Mutations>().RemoveMutation(gigantismMutation);

                __instance.SetIntProperty("Gigantic", value ? 1 : (-1));
            }
            return !@override;
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.IsGiganticEquipment),
            methodType: MethodType.Getter)]
        [HarmonyPrefix]
        public static bool IsGiganticEquipment_getter_CheckEvent_Prefix(ref GameObject __instance, ref bool __result)
        {
            __result = false;
            if (GetGiganticEquipmentEvent.CheckFor(__instance, out bool Override))
            {
                __result = true;
                if (Override)
                    return false;
            }
            return !__result;
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.IsGiganticEquipment),
            methodType: MethodType.Setter)]
        [HarmonyPrefix]
        public static bool IsGiganticEquipment_setter_CheckEvent_Prefix(ref GameObject __instance, ref bool value)
        {
            bool @override = false;
            bool isGigantic = __instance.IsGiganticEquipment;
            if (value != isGigantic)
            {
                bool setValue = !SetGiganticEquipmentEvent.CheckFor(__instance, out @override, isGigantic);
                value = @override ? setValue : value;
            }
            return !@override;
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject),
            methodName: nameof(GameObject.FinalizeCopy),
            argumentTypes: new Type[] { typeof(GameObject), typeof(bool), typeof(bool), typeof(Func<GameObject, GameObject>) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void FinalizeCopy_ReequipImprovedMutationMod_Postfix(GameObject __instance)
        {
            GameObject Object = __instance;
            if (Object?.Body == null)
                return;

            Debug.Divider(4, HONLY, Count: 40, Indent: 0, Toggle: doDebug);
            Debug.Entry(4,
                $"# {nameof(GameObject_Patches)}." +
                $"{nameof(FinalizeCopy_ReequipImprovedMutationMod_Postfix)}(GameObject __instance: {Object.DebugName})",
                Indent: 0, Toggle: doDebug);

            Debug.Entry(4, $"> foreach (BodyPart part in Object.Actor.LoopParts())", Indent: 1, Toggle: doDebug);
            foreach (BodyPart part in Object.Body.LoopParts())
            {
                Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);
                Debug.Entry(4, $"part", $"[{part.ID}:{part.Type}] {part.Description}", Indent: 2, Toggle: doDebug);

                GameObject cybernetics = part.Cybernetics;
                if (cybernetics != null)
                    Debug.LoopItem(4, $" cybernetics", $"{cybernetics.ShortDisplayName}", Indent: 3, Toggle: doDebug);

                if (cybernetics != null
                    && cybernetics.HasPartDescendedFrom<IModification>())
                {
                    List<IModification> modifications = cybernetics.GetPartsDescendedFrom<IModification>();
                    bool doImplantedEvent = false;
                    foreach (IModification modification in modifications)
                        if ($"{modification.GetType().BaseType}".Contains("ModImprovedMutationBase"))
                        {
                            doImplantedEvent = true;
                            break;
                        }

                    if (doImplantedEvent)
                    {
                        if (cybernetics != null && Object != null)
                        {
                            // ImplantedEvent.Send(Object, cybernetics, part, Object, true, true);
                            EffectAppliedEvent.Send(cybernetics, "", new(), Object);
                            Debug.CheckYeh(4, $"{nameof(EffectAppliedEvent)}", "Sent", Indent: 4, Toggle: doDebug);
                            continue;
                        }
                    }
                    else
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4, Toggle: doDebug);
                }
                else
                if (cybernetics != null)
                    Debug.CheckNah(4, $"No ModPart", Indent: 4, Toggle: doDebug);

                GameObject equipment = part.Equipped;
                if (equipment != null)
                    Debug.LoopItem(4, $" equipment", $"{equipment.ShortDisplayName}", Indent: 3, Toggle: doDebug);

                if (equipment != null
                    && equipment.HasPartDescendedFrom<IModification>()
                    && !equipment.HasPartDescendedFrom<CyberneticsBaseItem>())
                {
                    List<IModification> modifications = equipment.GetPartsDescendedFrom<IModification>();
                    bool doEquippedEvent = false;
                    foreach (IModification modification in modifications)
                        if ($"{modification.GetType().BaseType}".Contains("ModImprovedMutationBase"))
                        {
                            doEquippedEvent = true;
                            break;
                        }

                    if (doEquippedEvent)
                    {
                        EquippedEvent.Send(Object, equipment, part);
                        EffectAppliedEvent.Send(equipment, "", new(), Object);
                        Debug.CheckYeh(4, $"{nameof(EquippedEvent)}", "Sent", Indent: 4, Toggle: doDebug);
                    }
                    else
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4, Toggle: doDebug);
                }
                else
                if (equipment != null)
                    Debug.CheckNah(4, $"No ModPart or item is Cybernetics", Indent: 4, Toggle: doDebug);
            }
            Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);
            Debug.Entry(4, $"x foreach (BodyPart part in Object.Actor.LoopParts()) >//", Indent: 1, Toggle: doDebug);

            Debug.Entry(4, $"Updating Body Parts: Object.Body.UpdateBodyParts()", Indent: 1, Toggle: doDebug);
            Object.Body.UpdateBodyParts();

            Debug.Entry(4,
                $"x {nameof(GameObject_Patches)}."
                + $"{nameof(FinalizeCopy_ReequipImprovedMutationMod_Postfix)}"
                + $"(GameObject __instance: {Object.DebugName}) #//",
                Indent: 0, Toggle: doDebug);

            Debug.Divider(4, HONLY, Count: 40, Indent: 0, Toggle: doDebug);
        }
    }
}
