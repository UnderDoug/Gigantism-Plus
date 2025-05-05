using HarmonyLib;

using System;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Capabilities;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(AutoAct))]
    public static class AutoAct_Patches
    {
        [HarmonyPatch(
            declaringType: typeof(AutoAct),
            methodName: nameof(AutoAct.TryToMove),
            argumentTypes: new Type[] { 
                typeof(GameObject),             // Actor
                typeof(Cell),                   // FromCell
                typeof(GameObject),             // LastDoor
                typeof(Cell),                   // ToCell
                typeof(string),                 // Direction
                typeof(bool),                   // AllowDigging
                typeof(bool),                   // OpenDoors
                typeof(bool),                   // Peaceful
                typeof(bool),                   // PostMoveHostileCheck
                typeof(bool) },                 // PostMoveSidebarCheck
            argumentVariations: new ArgumentType[] { 
                ArgumentType.Normal,            // Actor
                ArgumentType.Normal,            // FromCell
                ArgumentType.Ref,               // LastDoor
                ArgumentType.Normal,            // ToCell
                ArgumentType.Normal,            // Direction
                ArgumentType.Normal,            // AllowDigging
                ArgumentType.Normal,            // OpenDoors
                ArgumentType.Normal,            // Peacefull
                ArgumentType.Normal,            // PostMoveHostileCheck
                ArgumentType.Normal })]         // PostMoveSidebarCheck
        [HarmonyPrefix]
        public static bool TryToMove_SendBeforeEvent_Prefix(ref bool __result, GameObject Actor, Cell FromCell, ref GameObject LastDoor, Cell ToCell = null, string Direction = null, bool AllowDigging = true, bool OpenDoors = true, bool Peaceful = true, bool PostMoveHostileCheck = true, bool PostMoveSidebarCheck = true)
        {
            bool Continue = AutoActTryToMoveEvent.GetFor(out bool? result, Actor, FromCell, ref LastDoor, ToCell, Direction, AllowDigging, OpenDoors, Peaceful, PostMoveHostileCheck, PostMoveSidebarCheck);

            if (result != null)
                __result = (bool)result;

            return Continue;
        }
    } //!-- public static class AutoAct_Patches
}
