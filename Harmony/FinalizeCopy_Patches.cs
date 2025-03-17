using HarmonyLib;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

namespace HNPS_GigantismPlus
{
    [HarmonyPatch(typeof(GameObject))]
    public static class GameObject_DeepCopy_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameObject.FinalizeCopy))]
        static void FinalizeCopy_Postfix(GameObject __instance)
        {
            GameObject Object = __instance;
            if (Object?.Body == null) return;
            Debug.Entry("Ding!");
            foreach (BodyPart part in Object.Body.LoopParts())
            {
                GameObject cybernetics = part.Cybernetics;
                if (cybernetics != null) ImplantedEvent.Send(Object, cybernetics, part, Object, false, true);
                GameObject equipment = part.Equipped;
                if (equipment != null) EquippedEvent.Send(Object, equipment, part);
            }
        }
    }
}
