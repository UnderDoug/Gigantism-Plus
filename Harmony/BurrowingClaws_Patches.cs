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
    public static class BurrowingClaws_Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BurrowingClaws), nameof(BurrowingClaws.OnRegenerateDefaultEquipment))]
        static bool OnRegenerateDefaultEquipment_DoNothing_Prefix(BurrowingClaws __instance, Body body)
        {
            return true; // We don't actually do anything.

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
        [HarmonyPatch(typeof(XRL.World.Parts.Mutation.BurrowingClaws), nameof(XRL.World.Parts.Mutation.BurrowingClaws.FireEvent))]
        static bool FireEvent_AdditionalEvents_Prefix(ref XRL.World.Parts.Mutation.BurrowingClaws __instance, Event E)
        {
            Debug.Entry(3,
                $"{nameof(BurrowingClaws_Patches)}." +
                $"{nameof(FireEvent_AdditionalEvents_Prefix)}(ref BurrowingClaws __instance, Event E)",
                Indent: 0);

            XRL.World.Parts.Mutation.BurrowingClaws @this = __instance;
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
