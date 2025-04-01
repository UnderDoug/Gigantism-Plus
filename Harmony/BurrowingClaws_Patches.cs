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
                int level = managed.Level;
                ModBurrowingNaturalWeaponUnmanaged naturalEquipmentMod = managed.NaturalEquipmentMod as ModBurrowingNaturalWeaponUnmanaged;
                managed.HasGigantism = actor.HasPart<GigantismPlus>();
                managed.HasElongated = actor.HasPart<ElongatedPaws>();
                managed.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
                managed.NaturalEquipmentMod = naturalEquipmentMod;
                managed.NaturalEquipmentMod.AssigningPart = managed;
                managed.ChangeLevel(level);

                foreach (BodyPart bodyPart in body.LoopPart(managed.NaturalEquipmentMod.BodyPartType))
                {
                    NaturalEquipmentManager manager = bodyPart.DefaultBehavior?.GetPart<NaturalEquipmentManager>() ?? bodyPart.Equipped?.GetPart<NaturalEquipmentManager>();
                    if (manager != null) managed.OnManageNaturalEquipment(manager, bodyPart);
                }
            }
            else
            {
                ShouldContinue = true;
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
            }

            Debug.Entry(3, $"Skipping patched Method: {!ShouldContinue}", Indent: 1);
            Debug.Footer(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            return ShouldContinue; // Skip the the original method if we do anything.

        } //!-- static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)

    } //!-- public static class BurrowingClaws_Patches
}
