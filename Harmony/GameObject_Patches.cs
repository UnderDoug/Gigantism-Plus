using HarmonyLib;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System.Collections.Generic;
using XRL.World.Anatomy;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(GameObject))]
    public static class GameObject_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.CheckDefaultBehaviorGiganticness))]
        public static bool CheckDefaultBehaviorGiganticness_ForceEquipped_BlockHideAdjective_Prefix(ref GameObject __instance, GameObject Equipper)
        {
            GameObject @this = __instance;
            if (GameObject.Validate(ref Equipper))
            {
                // this is an extremely important line of code that guarantees that default equipment is considered equipped.
                // removing it completely breaks the NaturalEquipmentManager.
                if (@this.Physics.Equipped != Equipper) @this.Physics.Equipped = Equipper;
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.FinalizeCopy))]
        public static void FinalizeCopy_ReequipImprovedMutationMod_Postfix(GameObject __instance)
        {
            GameObject Object = __instance;
            if (Object?.Body == null) return;

            Debug.Divider(4, HONLY, Count: 40, Indent: 0);
            Debug.Entry(4, 
                $"# {nameof(GameObject_Patches)}." + 
                $"{nameof(FinalizeCopy_ReequipImprovedMutationMod_Postfix)}(GameObject __instance: {Object.DebugName})", 
                Indent: 0);

            Debug.Entry(4, $"> foreach (BodyPart part in Object.Actor.LoopParts())", Indent: 1);
            foreach (BodyPart part in Object.Body.LoopParts())
            {
                Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                Debug.Entry(4, $"part", $"[{part.ID}:{part.Type}] {part.Description}", Indent: 2);

                GameObject cybernetics = part.Cybernetics;
                if (cybernetics != null) Debug.LoopItem(4, $" cybernetics", $"{cybernetics.ShortDisplayName}", Indent: 3);
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
                        Debug.CheckYeh(4, $"{nameof(ImplantedEvent)}", "Sent", Indent: 4);
                        continue;
                    }
                    else
                    {
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4);
                    }
                }
                else if (cybernetics != null)
                {
                    Debug.CheckNah(4, $"No IModification", Indent: 4);
                }

                GameObject equipment = part.Equipped;
                if (equipment != null) Debug.LoopItem(4, $" equipment", $"{equipment.ShortDisplayName}", Indent: 3);
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
                        // EquippedEvent.Send(Object, equipment, part);
                        EffectAppliedEvent.Send(equipment, null, null, Object);
                        Debug.CheckYeh(4, $"{nameof(EquippedEvent)}", "Sent", Indent: 4);
                    }
                    else
                    {
                        Debug.CheckNah(4, $"No ModImprovedMutation", Indent: 4);
                    }
                }
                else if (equipment != null)
                {
                    Debug.CheckNah(4, $"No IModification or item is Cybernetics", Indent: 4);
                }
            }
            Debug.Divider(4, HONLY, Count: 25, Indent: 2);
            Debug.Entry(4, $"x foreach (BodyPart part in Object.Actor.LoopParts()) >//", Indent: 1);
            Debug.Entry(4, $"Updating Body Parts: Object.Body.UpdateBodyParts()", Indent: 1);
            Object.Body.UpdateBodyParts();
            Debug.Entry(4, $"x {nameof(GameObject_Patches)}.{nameof(FinalizeCopy_ReequipImprovedMutationMod_Postfix)}(GameObject __instance: {Object.DebugName}) #//", Indent: 0);
            Debug.Divider(4, HONLY, Count: 40, Indent: 0);
        }
    }
}
