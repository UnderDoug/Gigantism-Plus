using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;
using XRL.Wish;
using XRL;
using System;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Body body)
        {
            if ((bool)EnableManagedVanillaMutations) return true;
            /*GameObject actor = __instance.ParentObject;
            Zone InstanceObjectZone = actor.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");
            Debug.Entry(3, $"TARGET {actor.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);*/

            bool ShouldContinue = false;

            /*if (body != null)
            {
                UD_ManagedBurrowingClaws managed = new(__instance);
                int level = managed.Level;
                ModBurrowingNaturalWeaponUnmanaged naturalEquipmentMod =
                    new((ModBurrowingNaturalWeapon)managed.NaturalEquipmentMod);
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
                Debug.Entry(3, "No Actor. Aborting", Indent: 1);
            }

            Debug.Entry(3, $"Skipping patched Method: {!ShouldContinue}", Indent: 1);
            Debug.Footer(3, $"{nameof(BurrowingClaws_Patches)}", $"{nameof(OnRegenerateDefaultEquipment_Prefix)}(body)");*/
            return ShouldContinue; // Skip the the original method if we do anything.

        } //!-- static bool OnRegenerateDefaultEquipment_Prefix(BurrowingClaws __instance, Actor body)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.Register))]
        static bool Register_AdditionalEvents_Prefix(IEventRegistrar Registrar)
        {
            Debug.Entry(3,
                $"{nameof(BurrowingClaws_Patches)}." +
                $"{nameof(Register_AdditionalEvents_Prefix)}(IEventRegistrar Registrar)",
                Indent: 0);

            Registrar.Register(typeof(ManageDefaultEquipmentEvent).Name);
            Registrar.Register(typeof(UpdateNaturalEquipmentModsEvent).Name);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.FireEvent))]
        static bool FireEvent_AdditionalEvents_Prefix(ref BurrowingClaws __instance, Event E)
        {
            Debug.Entry(3,
                $"{nameof(BurrowingClaws_Patches)}." +
                $"{nameof(FireEvent_AdditionalEvents_Prefix)}(ref BurrowingClaws __instance, Event E)",
                Indent: 0);

            BurrowingClaws @this = __instance;
            if (E.ID == typeof(ManageDefaultEquipmentEvent).Name)
            {
                Debug.Entry(3, $"! E.ID == {typeof(ManageDefaultEquipmentEvent).Name}", Indent: 1);

                if (E.GetGameObjectParameter("Wielder") == @this.ParentObject)
                {
                    GameObject @object = E.GetGameObjectParameter("Object");
                    NaturalEquipmentManager manager = E.GetParameter("Manager") as NaturalEquipmentManager;
                    BodyPart bodyPart = E.GetParameter("BodyPart") as BodyPart;

                    GameObject actor = E.GetGameObjectParameter("Wielder");
                    UD_ManagedBurrowingClaws managed = new(@this);
                    ModBurrowingNaturalWeaponUnmanaged naturalEquipmentMod =
                        new((ModBurrowingNaturalWeapon)managed.NaturalEquipmentMod);
                    managed.HasGigantism = actor.HasPart<GigantismPlus>();
                    managed.HasElongated = actor.HasPart<ElongatedPaws>();
                    managed.HasCrystallinity = actor.HasPartDescendedFrom<Crystallinity>();
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
    } //!-- public static class BurrowingClaws_Patches
}
