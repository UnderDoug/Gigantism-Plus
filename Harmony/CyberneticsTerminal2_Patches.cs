using HarmonyLib;
using System;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class GiganticCreature_Implant_SmallCybernetics
    {
        // goal force gigantic creatures who are eligible for cybernetics to hunch over when using becomming nooks.
        // failure to do so really freaks out the cybernetic in question due to being "too small".
        // you end up with it installed, but the equipment copy ends up in your inventory.
        static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)
        {
            Debug.Entry(3, "* static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = false) called");

            Debug.Entry(3, "- Checking the Actor exists, is a True Kin, and has GigantismPlus");
            Debug.Entry(4, "* if (Actor != null && actor.IsTrueKin && Actor.TryGetPart<GigantismPlus>(out var gigantism)");
            if (Actor != null && Actor.IsTrueKin() && Actor.TryGetPart(out GigantismPlus gigantism))
            {
                Debug.Entry(3, "-- Actor exists, is a True Kin, and has GigantismPlus");
                if (gigantism != null && (gigantism.UnHunchImmediately || IsStart))
                {
                    Debug.Entry(3, "-- GigantismPlus instantiated");
                    Debug.Entry(3, "-- Either UnHunchImmediately is set to true or this is the Start of the process.");
                    Debug.Entry(3, "-- Making Hunch Over free, Sending Command to Hunch Over");

                    gigantism.IsHunchFree = true;
                    gigantism.UnHunchImmediately = IsStart;
                    CommandEvent.Send(Actor, GigantismPlus.HUNCH_OVER_COMMAND_NAME);

                    if (IsStart)
                    {
                        Debug.Entry(3, "-- Popping up Popup");
                        Popup.Show("You peer down into the interface.");
                    }
                }
            }
            else Debug.Entry(3, "x one or more of: actor doesn't exists, isn't a True Kin, or lacks GigantismPlus");
        } //!-- static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), nameof(CyberneticsTerminal2.HandleEvent), new Type[] { typeof(InventoryActionEvent) })]
        static bool InventoryActionEventPrefix(InventoryActionEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E)");
            if (E.Command == "InterfaceWithBecomingNook")
            {
                Debug.Entry(4, $"E.Command is {E.Command}");

                CyberneticsTerminal2_ToggleHunched(E.Actor);

                Debug.Entry(3, "Deferring to patched method");
            }
            return true;
        } //!-- static bool InventoryActionEventPrefix(InventoryActionEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(InventoryActionEvent) })]
        static void InventoryActionEventPostfix(InventoryActionEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E)");
            if (E.Command == "InterfaceWithBecomingNook")
            {
                Debug.Entry(4, $"E.Command is {E.Command}");

                CyberneticsTerminal2_ToggleHunched(E.Actor, false);

                Debug.Entry(3, "x CyberneticsTerminal2 -> HandleEvent(InventoryActionEvent E) ]//");
            }
        } //!-- static void InventoryActionEventPostfix(InventoryActionEvent E)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPrefix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2_ToggleHunched(E.Actor);

            Debug.Entry(3, "Deferring to patched method");
            return true;
        } //!-- static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)
        {
            Debug.Entry(4, "HarmonyPatches.cs | [HarmonyPostfix]");
            Debug.Entry(3, "CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E)");

            CyberneticsTerminal2_ToggleHunched(E.Actor, false);

            Debug.Entry(3, "x CyberneticsTerminal2 -> HandleEvent(CommandSmartUseEvent E) ]//");
        } //!-- static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)

    } //!--- public static class ModGiganticDisplayName_Shader
}
