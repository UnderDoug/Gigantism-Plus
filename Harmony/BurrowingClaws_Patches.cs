using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(BurrowingClaws))]
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)
        {
            if (EnableManagedVanillaMutations) return true;
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                goto Exit;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "> foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    UD_ManagedBurrowingClaws managedBurrowingClaws = __instance.ConvertToManaged();
                    managedBurrowingClaws.HasGigantism = actor.HasPart<GigantismPlus>();
                    managedBurrowingClaws.HasElongated = actor.HasPart<ElongatedPaws>();
                    managedBurrowingClaws.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
                    part.DefaultBehavior.ApplyModification(managedBurrowingClaws.GetNaturalWeaponMod(Managed: false), Actor: actor);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);

            Exit:
            Debug.Entry(3, "Skipping patched Method", Indent: 1);
            Debug.Footer(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            return false; // Skip the original method

        } //!-- static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)

    } //!-- public static class BurrowingClaws_Patches
}
