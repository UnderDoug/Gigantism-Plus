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

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class GameObject_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(GameObject_Patches));

        [HarmonyPatch(
            declaringType: typeof(GameObject), 
            methodName: nameof(GameObject.CheckDefaultBehaviorGiganticness))]
        [HarmonyPrefix]
        public static bool CheckDefaultBehaviorGiganticness_ForceEquipped_BlockHideAdjective_Prefix(GameObject __instance, GameObject Equipper)
        {
            GameObject @this = __instance;
            if (GameObject.Validate(ref Equipper))
            {
                int indent = Debug.LastIndent;
                GameObject physicsEquipped = @this?.Physics?.Equipped;
                Debug.Entry(4, 
                    $"# {nameof(GameObject)}."
                    + $"{nameof(GameObject.CheckDefaultBehaviorGiganticness)}("
                    + $"{nameof(@this)}: {@this?.DebugName ?? NULL}, "
                    + $"{nameof(Equipper)}: {Equipper?.DebugName ?? NULL}, "
                    + $"{nameof(physicsEquipped)}: {physicsEquipped?.DebugName ?? NULL})", 
                    Indent: indent, Toggle: doDebug);

                // this is an extremely important line of code that guarantees that default equipment is considered equipped.
                // removing it completely breaks the NaturalEquipmentManager.
                if (@this?.Physics != null && @this.Physics.Equipped != Equipper)
                {
                    physicsEquipped = @this.Physics.Equipped = Equipper;
                    Debug.Entry(4, 
                        $"{@this?.DebugName ?? NULL} is equipped by {@this?.Physics?.Equipped?.DebugName ?? NULL}", 
                        Indent: indent + 1, Toggle: doDebug);
                }
                Debug.Entry(4,
                    $"x {nameof(GameObject)}."
                    + $"{nameof(GameObject.CheckDefaultBehaviorGiganticness)}("
                    + $"{nameof(@this)}: {@this?.DebugName ?? NULL}, "
                    + $"{nameof(Equipper)}: {Equipper?.DebugName ?? NULL}, "
                    + $"{nameof(physicsEquipped)}: {physicsEquipped?.DebugName ?? NULL})"
                    + $" #//",
                    Indent: indent, Toggle: doDebug);
            }
            return false;
        }

        [HarmonyPatch(
            declaringType: typeof(GameObject), 
            methodName: nameof(GameObject.FinalizeCopy))]
        [HarmonyPostfix]
        public static void FinalizeCopy_ReequipImprovedMutationMod_Postfix(GameObject __instance)
        {
            GameObject Object = __instance;
            if (Object?.Body == null) return;

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
                if (cybernetics != null) Debug.LoopItem(4, $" cybernetics", $"{cybernetics.ShortDisplayName}", Indent: 3, Toggle: doDebug);
                if (cybernetics != null && cybernetics.HasPartDescendedFrom<IModification>())
                {
                    List<IModification> modifications = cybernetics.GetPartsDescendedFrom<IModification>();
                    bool doImplantedEvent = false;
                    foreach (IModification modification in modifications)
                    {
                        if ($"{modification.GetType().BaseType}".Contains("ModImprovedMutationBase"))
                        {
                            doImplantedEvent = true;
                            break;
                        }
                    }
                    if (doImplantedEvent)
                    {
                        // ImplantedEvent.Send(Object, cybernetics, part, Object, true, true);
                        EffectAppliedEvent.Send(cybernetics, null, null, Object);
                        Debug.CheckYeh(4, $"{nameof(EffectAppliedEvent)}", "Sent", Indent: 4, Toggle: doDebug);
                        continue;
                    }
                    else
                    {
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4, Toggle: doDebug);
                    }
                }
                else if (cybernetics != null)
                {
                    Debug.CheckNah(4, $"No IModification", Indent: 4, Toggle: doDebug);
                }

                GameObject equipment = part.Equipped;
                if (equipment != null) Debug.LoopItem(4, $" equipment", $"{equipment.ShortDisplayName}", Indent: 3, Toggle: doDebug);
                if (equipment != null && equipment.HasPartDescendedFrom<IModification>() && !equipment.HasPartDescendedFrom<CyberneticsBaseItem>())
                {
                    List<IModification> modifications = equipment.GetPartsDescendedFrom<IModification>();
                    bool doEquippedEvent = false;
                    foreach (IModification modification in modifications)
                    {
                        if ($"{modification.GetType().BaseType}".Contains("ModImprovedMutationBase"))
                        {
                            doEquippedEvent = true;
                            break;
                        }
                    }
                    if (doEquippedEvent)
                    {
                        EquippedEvent.Send(Object, equipment, part);
                        EffectAppliedEvent.Send(equipment, null, null, Object);
                        Debug.CheckYeh(4, $"{nameof(EquippedEvent)}", "Sent", Indent: 4, Toggle: doDebug);
                    }
                    else
                    {
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4, Toggle: doDebug);
                    }
                }
                else if (equipment != null)
                {
                    Debug.CheckNah(4, $"No IModification or item is Cybernetics", Indent: 4, Toggle: doDebug);
                }
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
