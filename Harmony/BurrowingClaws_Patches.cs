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
            if ((bool)EnableManagedVanillaMutations) return true;
            GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            bool ShouldContinue = false;

            if (body != null)
            {
                UD_ManagedBurrowingClaws managed = __instance.ConvertToManaged();
                managed.HasGigantism = actor.HasPart<GigantismPlus>();
                managed.HasElongated = actor.HasPart<ElongatedPaws>();
                managed.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
                managed.NaturalWeaponSubpart.Managed = false;
                managed.UpdateNaturalWeaponSubpart(managed.NaturalWeaponSubpart, managed.Level);
                managed.OnRegenerateDefaultEquipment(body);
                managed.OnDecorateDefaultEquipment(body);
            }
            else
            {
                ShouldContinue = true;
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
            }

            /*
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

                    UD_ManagedBurrowingClaws managed = __instance.ConvertToManaged();
                    managed.HasGigantism = actor.HasPart<GigantismPlus>();
                    managed.HasElongated = actor.HasPart<ElongatedPaws>();
                    managed.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
                    managed.UpdateNaturalWeaponSubpart(managed.NaturalEquipmentMod, managed.Level);
                    part.DefaultBehavior.ApplyModification(managed.NaturalEquipmentMod.GetNaturalEquipmentModName(Managed: false), Actor: actor);

                    Debug.DiveOut(4, $"{part.Type}", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) >//", Indent: 1);
            */

            Debug.Entry(3, $"Skipping patched Method: {!ShouldContinue}", Indent: 1);
            Debug.Footer(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            return ShouldContinue; // Skip the the original method if we do anything.

        } //!-- static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)

    } //!-- public static class BurrowingClaws_Patches
}
