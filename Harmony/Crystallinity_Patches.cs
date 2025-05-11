using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class Crystallinity_Patches
    {
        // Increase the chance to refract light-based attacks from 25% to 35% when GigantismPlus is present
        [HarmonyPrefix]
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.Mutate))]
        static void Mutate_Prefix(XRL.World.Parts.Mutation.Crystallinity __instance, GameObject GO)
        {
            if (GO.HasPart<GigantismPlus>())
            {
                // Wait for the original method to create the RefractLight part
                // Then in the postfix we'll modify its chance
                __instance.RefractAdded = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.Mutate))]
        static void Mutate_Postfix(XRL.World.Parts.Mutation.Crystallinity __instance, GameObject GO)
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
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.GetLevelText))]
        static void GetLevelText_Postfix(XRL.World.Parts.Mutation.Crystallinity __instance, ref string __result)
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

        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(XRL.World.Parts.Mutation.Crystallinity __instance, Body body)
        {
            return false; // Skip the the original method, we don't want it to run.
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.Register))]
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
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.Crystallinity), nameof(XRL.World.Parts.Mutation.Crystallinity.FireEvent))]
        static bool FireEvent_AdditionalEvents_Prefix(ref XRL.World.Parts.Mutation.Crystallinity __instance, Event E)
        {
            Debug.Entry(3,
                $"{nameof(Crystallinity_Patches)}." +
                $"{nameof(FireEvent_AdditionalEvents_Prefix)}(ref Crystallinity __instance, Event E)",
                Indent: 0);

            XRL.World.Parts.Mutation.Crystallinity @this = __instance;
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
        */
    }//!-- public static class Crystallinity_Patches
}
