using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;
using XRL;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Crystallinity_Patches
    {
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.Mutate))]
        static void Mutate_Prefix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<GigantismPlus>())
            {
                // Wait for the original method to create the RefractLight part
                // Then in the postfix we'll modify its chance
                __instance.RefractAdded = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.Mutate))]
        static void Mutate_Postfix(Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<GigantismPlus>() && __instance.RefractAdded)
            {
                var refractPart = GO.GetPart<RefractLight>();
                if (refractPart != null)
                {
                    refractPart.Chance = 35; // Increase from default 25 to 35
                }
            }
        }

        // Modify the Crystallinity mutation level text to include the GigantismPlus bonus
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.GetLevelText))]
        static void GetLevelText_Postfix(Crystallinity __instance, ref string __result)
        {
            if (__instance.ParentObject.HasPart<GigantismPlus>())
            {
                // Replace the original 25% text with our modified version
                __result = __result.Replace(
                    "25% chance to refract light-based attacks",
                    "35% chance to refract light-based attacks (25% base chance {{rules|+10%}} from {{gigantism|Gigantism}} ({{r|D}}))"
                );
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(Crystallinity __instance, Body body)
        {
            if ((bool)EnableManagedVanillaMutations) return true;
            /*GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(Crystallinity_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);*/

            bool ShouldContinue = false;

            /*if (body != null)
            {
                UD_ManagedCrystallinity managed = new(__instance);
                int level = managed.Level;
                ModCrystallineNaturalWeaponUnmanaged naturalEquipmentMod = 
                    new((ModCrystallineNaturalWeapon)managed.NaturalEquipmentMod);
                managed.HasGigantism = actor.HasPart<GigantismPlus>();
                managed.HasElongated = actor.HasPart<ElongatedPaws>();
                managed.HasBurrowing = actor.HasPartDescendedFrom<BurrowingClaws>();
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
                Debug.Entry(3, "No Actor. Aborting", Indent: 1);
            }
            Debug.Entry(3, $"Skipping patched Method: {!ShouldContinue}", Indent: 1);
            Debug.Footer(3, $"{nameof(Crystallinity_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");*/
            return ShouldContinue; // Skip the the original method if we do anything.
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.Register))]
        static bool Register_AdditionalEvents_Prefix(IEventRegistrar Registrar)
        {
            Debug.Entry(3, 
                $"{nameof(Crystallinity_Patches)}." + 
                $"{nameof(Register_AdditionalEvents_Prefix)}(IEventRegistrar Registrar)", 
                Indent: 0);

            Registrar.Register(typeof(ManageDefaultEquipmentEvent).Name);
            Registrar.Register(typeof(UpdateNaturalEquipmentModsEvent).Name);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Crystallinity), nameof(Crystallinity.FireEvent))]
        static bool FireEvent_AdditionalEvents_Prefix(ref Crystallinity __instance, Event E)
        {
            Debug.Entry(3,
                $"{nameof(Crystallinity_Patches)}." +
                $"{nameof(FireEvent_AdditionalEvents_Prefix)}(ref Crystallinity __instance, Event E)",
                Indent: 0);

            Crystallinity @this = __instance;
            if (E.ID == typeof(ManageDefaultEquipmentEvent).Name)
            {
                Debug.Entry(3, $"! E.ID == {typeof(ManageDefaultEquipmentEvent).Name}", Indent: 1);

                if (E.GetGameObjectParameter("Wielder") == @this.ParentObject)
                {
                    GameObject @object = E.GetGameObjectParameter("Object");
                    NaturalEquipmentManager manager = E.GetParameter("Manager") as NaturalEquipmentManager;
                    BodyPart bodyPart = E.GetParameter("BodyPart") as BodyPart;

                    GameObject actor = E.GetGameObjectParameter("Wielder");
                    UD_ManagedCrystallinity managed = new(@this);
                    ModCrystallineNaturalWeaponUnmanaged naturalEquipmentMod =
                        new((ModCrystallineNaturalWeapon)managed.NaturalEquipmentMod);
                    managed.HasGigantism = actor.HasPart<GigantismPlus>();
                    managed.HasElongated = actor.HasPart<ElongatedPaws>();
                    managed.HasBurrowing = actor.HasPartDescendedFrom<BurrowingClaws>();
                    managed.NaturalEquipmentMod = naturalEquipmentMod;
                    managed.NaturalEquipmentMod.AssigningPart = managed;

                    managed.OnUpdateNaturalEquipmentMods();

                    managed.OnManageNaturalEquipment(manager, bodyPart);
                }
            }
            if (E.ID == typeof(UpdateNaturalEquipmentModsEvent).Name)
            {
            }
            return true;
        }
    }//!-- public static class Crystallinity_Patches
}
