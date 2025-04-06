using HarmonyLib;
using System;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class CyberneticsTerminal2_Patches
    {
        // goal force gigantic creatures who are eligible for cybernetics to hunch over when using becomming nooks.
        // failure to do so really freaks out the cybernetic in question due to being "too small".
        // you end up with it installed, but the equipment copy ends up in your inventory.
        static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)
        {
            Debug.Entry(3, 
                $"* {nameof(CyberneticsTerminal2_ToggleHunched)}(" + 
                $"GameObject Actor: {Actor.ShortDisplayName}, " + 
                $"bool IsStart: {IsStart})", 
                Indent:2);

            Debug.Entry(3, "Checking the Actor exists, is a True Kin, and has GigantismPlus", Indent: 3);
            Debug.Entry(4, 
                "? if (Actor != null and actor.IsTrueKin && Actor.TryGetPart<GigantismPlus>(out var gigantism)", 
                Indent: 3);
            if (Actor != null && Actor.IsTrueKin() && Actor.TryGetPart(out GigantismPlus gigantism))
            {
                Debug.CheckYeh(3, "Actor exists, is a True Kin, and has GigantismPlus", Indent: 4);
                if (!gigantism.Is(null) && (gigantism.UnHunchImmediately || IsStart))
                {
                    Debug.Entry(3, "Making Hunch Over free, Sending Command to Hunch Over", Indent: 5);

                    gigantism.IsHunchFree = true;
                    gigantism.UnHunchImmediately = IsStart;
                    CommandEvent.Send(Actor, GigantismPlus.HUNCH_OVER_COMMAND_NAME);

                    if (IsStart)
                    {
                        Popup.Show("You peer down into the interface.");
                    }
                }
            }
            else 
            { 
                Debug.CheckNah(3, "one or more of: actor doesn't exist, isn't a True Kin, or lacks GigantismPlus");
            }
            Debug.Entry(4, 
                "x if (Actor != null and actor.IsTrueKin && Actor.TryGetPart<GigantismPlus>(out var gigantism) ?//", 
                Indent: 3);

            Debug.Entry(3,
                $"x {nameof(CyberneticsTerminal2_ToggleHunched)}(" +
                $"GameObject Actor: {Actor.ShortDisplayName}, " +
                $"bool IsStart: {IsStart}) *//",
                Indent: 2);

        } //!-- static void CyberneticsTerminal2_ToggleHunched(GameObject Actor, bool IsStart = true)

        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(CyberneticsTerminal2), 
            nameof(CyberneticsTerminal2.HandleEvent), 
            new Type[] { typeof(InventoryActionEvent) })]
        static bool InventoryActionEvent_HunchOver_Prefix(InventoryActionEvent E)
        {
            if (E.Command.Is("InterfaceWithBecomingNook"))
            {
                Debug.Entry(3,
                    $"# {typeof(CyberneticsTerminal2).Name}."
                    + $"{nameof(CyberneticsTerminal2.HandleEvent)}({typeof(InventoryActionEvent).Name} E: \"{E.Command}\")",
                    Indent: 0);

                CyberneticsTerminal2_ToggleHunched(E.Actor);

                Debug.Entry(3, "Deferring to patched method", Indent: 1);
            }
            return true;
        } //!-- static bool InventoryActionEvent_HunchOver_Prefix(InventoryActionEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(
            typeof(CyberneticsTerminal2),
            nameof(CyberneticsTerminal2.HandleEvent),
            new Type[] { typeof(InventoryActionEvent) })]
        static void InventoryActionEvent_HunchOver_Postfix(InventoryActionEvent E)
        {
            if (E.Command == "InterfaceWithBecomingNook")
            {
                Debug.Entry(3, "Patched method run", Indent: 1);

                CyberneticsTerminal2_ToggleHunched(E.Actor, false);

                Debug.Entry(3,
                    $"x {typeof(CyberneticsTerminal2).Name}."
                    + $"{nameof(CyberneticsTerminal2.HandleEvent)}({typeof(InventoryActionEvent).Name} E: \"{E.Command}\") #//",
                    Indent: 0);
            }
        } //!-- static bool InventoryActionEvent_HunchOver_Postfix(InventoryActionEvent E)

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static bool CommandSmartUseEvent_HunchOver_Prefix(CommandSmartUseEvent E)
        {
            Debug.Entry(3,
                $"# {typeof(CyberneticsTerminal2).Name}."
                + $"{nameof(CyberneticsTerminal2.HandleEvent)}({typeof(CommandSmartUseEvent).Name} E)",
                Indent: 0);

            CyberneticsTerminal2_ToggleHunched(E.Actor);

            Debug.Entry(3, "Deferring to patched method", Indent: 1);
            return true;
        } //!-- static bool CommandSmartUseEventPrefix(CommandSmartUseEvent E)

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CyberneticsTerminal2), "HandleEvent", new Type[] { typeof(CommandSmartUseEvent) })]
        static void CommandSmartUseEvent_HunchOver_Postfix(CommandSmartUseEvent E)
        {
            Debug.Entry(3, "Patched method run", Indent: 1);

            CyberneticsTerminal2_ToggleHunched(E.Actor, false);

            Debug.Entry(3,
                $"x {typeof(CyberneticsTerminal2).Name}."
                + $"{nameof(CyberneticsTerminal2.HandleEvent)}({typeof(CommandSmartUseEvent).Name} E) #//",
                Indent: 0);
        } //!-- static void CommandSmartUseEventPostfix(CommandSmartUseEvent E)

    } //!--- public static class ModGiganticDisplayName_Shader
}
