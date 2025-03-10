using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(BurrowingClaws))]
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)
        {
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(3, $"[HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]");
            Debug.Header(3, $"BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}");

            if (body == null) return true;

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(3, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(3, "Generating List<BodyPart> list", Indent: 1);
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(3, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(3, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(3, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(3, $"{part.Type} Found", Indent: 2);

                    UD_ManagedBurrowingClaws managedBurrowingClaws = __instance.ConvertToManaged();
                    managedBurrowingClaws.HasGigantism = actor.HasPart<GigantismPlus>();
                    managedBurrowingClaws.HasElongated = actor.HasPart<ElongatedPaws>();
                    managedBurrowingClaws.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
                    part.DefaultBehavior.ApplyModification(managedBurrowingClaws.GetNaturalWeaponMod(Managed: false), Actor: actor);

                    Debug.DiveOut(3, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//", Indent: 1);
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, "BurrowingClaws_Patches", $"OnRegenerateDefaultEquipment(body)");
            return false; // Skip the original method

        } //!-- static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)

    } //!-- public static class BurrowingClaws_Patches
}
