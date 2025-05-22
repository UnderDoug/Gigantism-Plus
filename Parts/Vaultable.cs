using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using XRL.UI;
using XRL.World.Parts.Skill;
using XRL.World.Capabilities;
using XRL.World.Anatomy;
using XRL.World.AI.Pathfinding;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class Vaultable : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(Vaultable));

        public static readonly string COMMAND_VAULT_OVER_ME = "VaultOverMe";

        public bool SizeMatters;
        public bool RequiresJumpSkill;

        public string EnablingLimbs;
        public List<string> EnablingLimbsList;

        public string OverridingParts;
        public List<string> OverridingPartsList;

        public Vaultable() 
        {
            SizeMatters = false;
            RequiresJumpSkill = false;
            EnablingLimbs = null;
            EnablingLimbsList = EnablingLimbs?.CommaExpansion() ?? new();
            OverridingParts = null;
            OverridingPartsList = OverridingParts?.CommaExpansion() ?? new();
        }

        public override void Attach()
        {
            base.Attach();
            if (ParentObject.TryGetPart(out Vaultable vaultable) && vaultable != this)
            {
                SizeMatters = vaultable.SizeMatters;
                RequiresJumpSkill = vaultable.RequiresJumpSkill;
                EnablingLimbs = vaultable.EnablingLimbs;
                EnablingLimbsList = new(vaultable.EnablingLimbsList);
                OverridingParts = vaultable.OverridingParts;
                OverridingPartsList = new(vaultable.OverridingPartsList);
                ParentObject.RemovePart(vaultable);
            }
        }

        public Dictionary<Cell, Cell> GetVaultableCellPairs(GameObject For = null)
        {
            return GetVaultableCellPairs(ParentObject.CurrentCell, For);
        }
        public static Dictionary<Cell, Cell> GetVaultableCellPairs(Cell Pivot, GameObject For = null)
        {
            Dictionary<Cell, Cell> OriginDestinationPairs = new();
            foreach (Cell cell in Pivot.GetLocalAdjacentCells())
            {
                if (cell == Pivot || OriginDestinationPairs.ContainsKey(cell) || OriginDestinationPairs.ContainsValue(cell) || !IsValidDestination(cell, For))
                    continue;

                Cell cellOpposite = cell.GetCellOppositePivotCell(Pivot);
                if (cellOpposite != null && IsValidDestination(cellOpposite, For))
                {
                    OriginDestinationPairs.TryAdd(cell, cellOpposite);
                    OriginDestinationPairs.TryAdd(cellOpposite, cell);
                }

            }
            return OriginDestinationPairs;
        }

        public static bool IsValidDestination(Cell Destination, GameObject For = null)
        {
            if (Destination == null)
                return false;

            if (For != null && Destination == For.CurrentCell)
                return true;

            if (Destination.GetDangerousOpenLiquidVolume() != null)
                return false;

            if (Destination.HasSwimmingDepthLiquid())
                return false;

            if (!Destination.GetObjectsWithTagOrProperty("NoAutoWalk").IsNullOrEmpty())
                return false;

            if (Destination.HasCombatObject())
                return false;

            if (Destination.HasWall())
                return false;

            if (!Destination.IsEmptyOfSolid())
                return false;

            return true;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforePhysicsRejectObjectEntringCell");
            Registrar.Register(ParentObject, GetNavigationWeightEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(ParentObject, OkayToDamageEvent.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            bool zoneLoaded =
                ParentObject != null
             && ParentObject.CurrentZone == The.ActiveZone;

            return base.WantEvent(ID, cascade)
                || ID == GetItemElementsEvent.ID
                || ID == GetInventoryActionsEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == CanSmartUseEvent.ID
                || ID == CommandSmartUseEvent.ID
                || (zoneLoaded && ID == ObjectEnteringCellEvent.ID)
                || (DebugVaultDescriptions && zoneLoaded && ID == GetShortDescriptionEvent.ID);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if(The.Player != null && ParentObject.CurrentZone == The.ZoneManager.ActiveZone)
            {
                int navWeight = ParentObject.CurrentCell.GetNavigationWeightFor(The.Player, false);
                int navWeightAuto = ParentObject.CurrentCell.GetNavigationWeightFor(The.Player, true);

                int navWeightThreshold = 25;
                int navWeightAutoThreshold = 50;

                string navWeightColor =
                    navWeight <= navWeightThreshold
                    ? "G"
                    : navWeight == 100
                        ? "R"
                        : "W"
                        ;
                string navWeightAutoColor =
                    navWeightAuto <= navWeightAutoThreshold
                    ? "G"
                    : navWeightAuto == 100
                        ? "R"
                        : "W"
                        ;

                StringBuilder SB = Event.NewStringBuilder();

                Cell parentCell = ParentObject.CurrentCell;
                Dictionary<string, Cell> cellsByDirection = new()
                {
                    { "N", parentCell.GetCellFromDirection("N") },
                    { "NE", parentCell.GetCellFromDirection("NE") },
                    { "E", parentCell.GetCellFromDirection("E") },
                    { "SE", parentCell.GetCellFromDirection("SE") },
                    { "S", parentCell.GetCellFromDirection("S") },
                    { "SW", parentCell.GetCellFromDirection("SW") },
                    { "W", parentCell.GetCellFromDirection("W") },
                    { "NW", parentCell.GetCellFromDirection("NW") },
                };
                Dictionary<string, (string C, string YN)> DI = new();

                Dictionary<Cell, Cell> OriginDestinationPairs = GetVaultableCellPairs(The.Player);

                foreach ((string direction, Cell cell) in cellsByDirection)
                {
                    (string Color, string yehNah) entry = ("R", CROSS);
                    if (OriginDestinationPairs.ContainsKey(cell))
                    {
                        entry = ("G", TICK);
                    }
                    if (cell == The.Player.CurrentCell)
                    {
                        entry = ("O", SMLE2);
                    }
                    DI.TryAdd(direction, entry);
                }

                int validCellsCount = OriginDestinationPairs.Count / 2;
                string cellCountColor =
                    validCellsCount >= 2
                    ? "G"
                    : validCellsCount <= 0
                        ? "R"
                        : "W"
                        ;

                bool isDiggable = ParentObject.HasIntProperty(DIGGABLE);
                bool wasDiggable = ParentObject.HasIntProperty(WAS_DIGGABLE);

                SB.AppendColored("M", $"Vaultable").Append("[").AppendColored("g", $"{ParentObject?.CurrentCell?.Location}").Append("]").Append(": ")
                    .AppendLine()
                    .AppendColored("W", "Nav Weight")
                    .AppendLine()
                    .Append(VANDR).Append("(").AppendColored(navWeightColor, $"{navWeight}").Append($"){HONLY}NavigationWeight").AppendLine()
                    .Append(TANDR).Append("(").AppendColored(navWeightAutoColor, $"{navWeightAuto}".ToString()).Append($"){HONLY}NavigationWeight [").AppendColored("K", "AutoExplore").Append("]")
                    .AppendLine()
                    .AppendColored("W", "Vaultable Cells (").AppendColored(cellCountColor, $"{validCellsCount}").AppendColored("W", ")")
                    .AppendLine()
                    .Append(VANDR).Append("[").AppendColored(DI["NW"].C, DI["NW"].YN).Append($"]")
                                  .Append("[").AppendColored(DI["N"].C, DI["N"].YN).Append($"]")
                                  .Append("[").AppendColored(DI["NE"].C, DI["NE"].YN).Append($"]").AppendLine()
                    .Append(VANDR).Append("[").AppendColored(DI["W"].C, DI["W"].YN).Append($"]")
                                  .Append("[").AppendColored("y", STAR).Append($"]")
                                  .Append("[").AppendColored(DI["E"].C, DI["E"].YN).Append($"]").AppendLine()
                    .Append(TANDR).Append("[").AppendColored(DI["SW"].C, DI["SW"].YN).Append($"]")
                                  .Append("[").AppendColored(DI["S"].C, DI["S"].YN).Append($"]")
                                  .Append("[").AppendColored(DI["SE"].C, DI["SE"].YN).Append($"]")
                    .AppendLine()
                    //.AppendColored("W", "Diggable State")
                    //.AppendLine()
                    //.Append(VANDR).Append($"[{isDiggable.YehNah()}]{HONLY}isDiggable: ").AppendColored("B", $"{isDiggable}").AppendLine()
                    //.Append(TANDR).Append($"[{wasDiggable.YehNah()}]{HONLY}wasDiggable: ").AppendColored("B", $"{wasDiggable}").AppendLine()
                    ;

                E.Infix.AppendRules(Event.FinalizeString(SB));
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetItemElementsEvent E)
        {
            if (E.IsRelevantCreature(ParentObject))
            {
                E.Add("travel", 1);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            bool wantInventoryAction =
                E.Object == ParentObject
             && E.Actor.TryGetPart(out Tactics_Vault vaultSkill)
             && vaultSkill.CanNormallyVault(E.Object);
            if (wantInventoryAction)
            {
                int priority = 0;
                priority -= E.Object.IsCreature ? 5 : 0;
                int @default = 2;

                E.AddAction(
                Name: "Vault Over",
                Display: "vault over",
                Command: COMMAND_VAULT_OVER_ME,
                PreferToHighlight: null,
                Key: 'v',
                FireOnActor: false,
                Default: @default,
                Priority: priority,
                Override: false,
                WorksAtDistance: false,
                WorksTelekinetically: false);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == COMMAND_VAULT_OVER_ME && E.Item == ParentObject)
            {
                GameObject vaulter = E.Actor;
                GameObject vaultee = E.Item;
                Cell pivot = vaultee.CurrentCell;
                Cell targetCell = Tactics_Vault.GetValidDestinationCell(vaulter, pivot);

                Debug.Entry(4,
                    $"@ {nameof(Vaultable)}."
                    + $"{nameof(HandleEvent)}({nameof(InventoryActionEvent)}"
                    + $" E.Command: {E.Command.Quote()})",
                    Indent: 0, Toggle: doDebug);

                Debug.LoopItem(4, $"E.Item", $"{E.Item?.DebugName ?? NULL}", Good: E.Item != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"ParentObject", $"{ParentObject?.DebugName ?? NULL}", Good: ParentObject != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"vaultee", $"{vaultee?.DebugName ?? NULL}", Good: vaultee != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"E.Actor", $"{E.Actor?.DebugName ?? NULL}", Good: E.Actor != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"vaulter", $"{vaulter?.DebugName ?? NULL}", Good: vaulter != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"pivot", $"[{pivot?.Location}]", Good: pivot != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"targetCell", $"[{targetCell?.Location}]", Good: targetCell != null,
                    Indent: 1, Toggle: doDebug);

                if (!CommandEvent.Send(
                    Actor: vaulter,
                    Command: Tactics_Vault.COMMAND_NAME,
                    Target: vaultee,
                    TargetCell: targetCell,
                    Silent: false))
                {
                    return false;
                }
                E.RequestInterfaceExit();
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CanSmartUseEvent E)
        {
            bool canSmartUse =
                E.Item == ParentObject
             && !E.Item.IsCreature
             && !E.Item.HasPart<Container>()
             && !E.Item.HasPart<Pettable>()
             && E.Actor.TryGetPart(out Tactics_Vault vaultSkill) 
             && vaultSkill.CanNormallyVault(E.Item);

            if (canSmartUse)
            {
                return false; // not sure the logic, but this one is a "false means yes"
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CommandSmartUseEvent E)
        {
            bool shouldSmartUse =
                !E.Item.IsCreature
             && !E.Item.HasPart<Container>()
             && !E.Item.HasPart<Pettable>()
             && !E.Item.HasTagOrProperty("ForceSmartUse");

            if (shouldSmartUse && E.Actor.TryGetPart(out Tactics_Vault vaultSkill) && vaultSkill.CanVault(E.Item))
            {
                GameObject vaulter = E.Actor;
                Cell origin = vaulter.CurrentCell;
                GameObject vaultee = E.Item;
                Cell pivot = vaultee.CurrentCell;
                Cell targetCell = Tactics_Vault.GetValidDestinationCell(vaulter, pivot);

                Debug.Entry(4,
                    $"@ {nameof(Vaultable)}."
                    + $"{nameof(HandleEvent)}({nameof(CommandSmartUseEvent)} E.MinPriority: {E.MinPriority})",
                    Indent: 0, Toggle: doDebug);

                Debug.LoopItem(4, $"vaultee", $"{vaultee?.DebugName ?? NULL}", Good: vaultee != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"vaulter", $"{vaulter?.DebugName ?? NULL}", Good: vaulter != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"origin", $"[{origin?.Location}]", Good: origin != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"pivot", $"[{pivot?.Location}]", Good: pivot != null,
                    Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"targetCell", $"[{targetCell?.Location}]", Good: targetCell != null,
                    Indent: 1, Toggle: doDebug);

                CommandEvent.Send(
                    Actor: vaulter,
                    Command: Tactics_Vault.COMMAND_NAME,
                    Target: vaultee,
                    TargetCell: targetCell,
                    Silent: false);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetNavigationWeightEvent E)
        {
            Dictionary<Cell, Cell> originDestinationPairs = GetVaultableCellPairs(E.Actor);
            bool hasAnyValidVaultCells = !originDestinationPairs.IsNullOrEmpty();

            if (hasAnyValidVaultCells && E.Cell == ParentObject.CurrentCell && ParentObject.Physics.Solid && E.Actor != null)
            {
                if (Tactics_Vault.CanVault(E.Actor, ParentObject, out Tactics_Vault vaultSkill) && vaultSkill.WantToVault)
                {
                    int validPairs = originDestinationPairs.Count / 2;
                    int baseWeight = validPairs switch
                    {
                        4 => 1,
                        3 => 1,
                        2 => 1,
                        1 => 15,
                        _ => 100,
                    };

                    int weight = Math.Min(100, baseWeight * (E.Autoexploring ? 5 : 1));
                    int maxWeight = Math.Min(100, baseWeight * (E.Autoexploring ? 5 : 1));
                    if (maxWeight < 100)
                    {
                        E.Uncacheable = true;
                        E.MinWeight(weight, maxWeight);
                        return false;
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ObjectEnteringCellEvent E)
        {
            GameObject Vaulter = E.Object;
            GameObject Vaultee = ParentObject;

            Debug.Entry(4,
                $"@ {nameof(Vaultable)}."
                + $"{nameof(HandleEvent)}({nameof(ObjectEnteringCellEvent)} E)"
                + $" E.Cell: [{E?.Cell?.Location}],"
                + $" E.Actor: {Vaulter?.DebugName ?? NULL},"
                + $" E.Object: {Vaultee?.DebugName ?? NULL}",
                Indent: 0);

            Tactics_Vault vaultSkill = null;
            bool vaulterNotNull = Vaulter != null;
            bool cellExists = E.Cell != null;
            bool cellIsSolidForVaulter = cellExists && E.Cell.IsSolidFor(Vaulter);
            bool haveSkill = vaulterNotNull && Vaulter.TryGetPart(out vaultSkill);
            bool wantToVault = haveSkill && vaultSkill.WantToVault;
            bool midVault = haveSkill && vaultSkill.MidVault;
            bool notPlayer = vaulterNotNull && !Vaulter.IsPlayer();
            bool autoActActive = AutoAct.IsActive();
            bool actingAutomatically = notPlayer || autoActActive;

            bool shouldBlock =
                vaulterNotNull
             && cellIsSolidForVaulter
             && haveSkill
             && wantToVault
             && actingAutomatically;

            if (midVault)
            {
                Debug.LoopItem(4, $"{nameof(midVault)}", $"{midVault}",
                    Good: midVault, Indent: 1, Toggle: doDebug);

                Debug.Entry(4, $"Allowing Vaulter {Vaulter?.DebugName ?? NULL} to vault through cell [{E?.Cell?.Location}]", Indent: 1);
                vaultSkill.MidVault = false;
                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);
            }
            else
            {
                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}",
                    Good: vaulterNotNull, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(cellIsSolidForVaulter)}", $"{cellIsSolidForVaulter}",
                    Good: cellIsSolidForVaulter, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(haveSkill)}", $"{haveSkill}",
                    Good: haveSkill, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(wantToVault)}", $"{wantToVault}",
                    Good: wantToVault, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(midVault)}", $"{midVault}",
                    Good: midVault, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(actingAutomatically)}", $"{actingAutomatically}",
                    Good: actingAutomatically, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(notPlayer)}", $"{notPlayer}",
                    Good: notPlayer, Indent: 2, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(autoActActive)}", $"{autoActActive}",
                    Good: autoActActive, Indent: 2, Toggle: doDebug);

                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(shouldBlock)}", $"{shouldBlock}",
                    Good: shouldBlock, Indent: 1, Toggle: doDebug);

                if (shouldBlock && vaultSkill.CanVault(Vaultee, Silent: false))
                {
                    Debug.Entry(4, $"Preventing {Vaulter?.DebugName ?? NULL} from entering cell [{E?.Cell?.Location}]", Indent: 1);

                    return false;
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(OkayToDamageEvent E)
        {
            GameObject Vaulter = E.Actor;
            GameObject Vaultee = E.Object;

            if (Vaulter.CurrentCell.GetAdjacentCells().Contains(Vaultee.CurrentCell))
            {
                Debug.Entry(4,
                $"@ {nameof(Vaultable)}."
                + $"{nameof(HandleEvent)}({nameof(OkayToDamageEvent)} E)"
                + $" E.Actor: {Vaulter?.DebugName ?? NULL},"
                + $" E.Object: {Vaultee?.DebugName ?? NULL}",
                Indent: 0);

                Tactics_Vault vaultSkill = null;

                bool vaulterNotNull = Vaulter != null;

                Dictionary<Cell, Cell> originDestinationPairs = GetVaultableCellPairs(Vaulter);

                bool vaulteeHasValidCellPair = !originDestinationPairs.IsNullOrEmpty();

                bool vaultableCellPairsContainsVaulter =
                    vaulteeHasValidCellPair
                    && vaulterNotNull
                    && originDestinationPairs.ContainsKey(Vaulter.CurrentCell);

                bool vaulterHasSkill =
                    vaulterNotNull
                    && Vaulter.TryGetPart(out vaultSkill);

                bool vaulterCanVaultVaultee =
                    vaulterHasSkill
                    && vaultSkill.CanVault(Vaultee, Silent: true);

                bool vaulterIsBurrowerWantsToVault =
                    vaulterHasSkill
                    && vaultSkill.IsBurrowerWantsToVault;

                bool vaulterNotPlayer =
                    vaulterNotNull
                    && !Vaulter.IsPlayer();

                bool autoActActive = AutoAct.IsActive();

                bool actingAutomatically = vaulterNotPlayer || autoActActive;

                bool notOkayToDamage =
                    vaultableCellPairsContainsVaulter
                 && vaulterCanVaultVaultee
                 && vaulterIsBurrowerWantsToVault
                 && actingAutomatically;

                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}",
                    Good: vaulterNotNull, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulteeHasValidCellPair)}", $"{vaulteeHasValidCellPair}",
                    Good: vaulteeHasValidCellPair, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaultableCellPairsContainsVaulter)}", $"{vaultableCellPairsContainsVaulter}",
                    Good: vaultableCellPairsContainsVaulter, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulterHasSkill)}", $"{vaulterHasSkill}",
                    Good: vaulterHasSkill, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulterCanVaultVaultee)}", $"{vaulterCanVaultVaultee}",
                    Good: vaulterCanVaultVaultee, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulterIsBurrowerWantsToVault)}", $"{vaulterIsBurrowerWantsToVault}",
                    Good: vaulterIsBurrowerWantsToVault, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(actingAutomatically)}", $"{actingAutomatically}",
                    Good: actingAutomatically, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulterNotPlayer)}", $"{vaulterNotPlayer}",
                    Good: vaulterNotPlayer, Indent: 2, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(autoActActive)}", $"{autoActActive}",
                    Good: autoActActive, Indent: 2, Toggle: doDebug);

                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(notOkayToDamage)}", $"{notOkayToDamage}",
                    Good: notOkayToDamage, Indent: 1, Toggle: doDebug);

                if (notOkayToDamage)
                {
                    Debug.Entry(4, $"Dissallowing {Vaulter?.DebugName ?? NULL} from digging {Vaultee?.DebugName ?? NULL}", Indent: 1);

                    return false;
                }
            }
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforePhysicsRejectObjectEntringCell" && E.HasFlag("Actual"))
            {
                Debug.Entry(4,
                    $"@ {nameof(Vaultable)}."
                    + $"{nameof(FireEvent)}({nameof(Event)} E.Command: {E.ID.Quote()})",
                    Indent: 0, Toggle: doDebug);

                GameObject Vaulter = E.GetGameObjectParameter("Object");
                GameObject Vaultee = ParentObject;

                bool vaulterNotNull = Vaulter != null;
                bool haveSkill = Vaulter.TryGetPart(out Tactics_Vault vaultSkill);
                bool notMidVault = haveSkill && !vaultSkill.MidVault;
                bool vaulted = haveSkill && vaultSkill.Vaulted;
                bool wantToVault = haveSkill && vaultSkill.WantToVault;
                bool isPlayer = vaulterNotNull && Vaulter.IsPlayer();
                bool autoActActive = AutoAct.IsActive();
                bool actingAutomatically = isPlayer || autoActActive;

                bool shouldResumeAfterVault =
                    vaulterNotNull
                 && haveSkill
                 && notMidVault
                 && vaulted
                 && wantToVault
                 && actingAutomatically;

                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);
                Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}", 
                    Good: vaulterNotNull, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(haveSkill)}", $"{haveSkill}", 
                    Good: haveSkill, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(notMidVault)}", $"{notMidVault}", 
                    Good: notMidVault, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(vaulted)}", $"{vaulted}", 
                    Good: vaulted, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(wantToVault)}", $"{wantToVault}", 
                    Good: wantToVault, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(isPlayer)}", $"{isPlayer}", 
                    Good: isPlayer, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(autoActActive)}", $"{autoActActive}", 
                    Good: autoActActive, Indent: 1, Toggle: doDebug);

                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(shouldResumeAfterVault)}", $"{shouldResumeAfterVault}", 
                    Good: shouldResumeAfterVault, Indent: 1, Toggle: doDebug);

                if (shouldResumeAfterVault)
                {

                    if (vaultSkill.ResumeAfterVault())
                    {
                        Debug.CheckYeh(4, $"Resume Successful, allowing through", Indent: 1, Toggle: doDebug);

                        Debug.Entry(4,
                        $"x {nameof(Vaultable)}."
                        + $"{nameof(FireEvent)}({nameof(Event)} E.Command: {E.ID.Quote()}) @//",
                        Indent: 0, Toggle: doDebug);

                        return false; // don't Reject entering cell
                    }

                    Debug.CheckNah(4, $"Resume Failed, blocking", Indent: 1, Toggle: doDebug);

                }
                Debug.Entry(4,
                    $"x {nameof(Vaultable)}."
                    + $"{nameof(FireEvent)}({nameof(Event)} E.Command: {E.ID.Quote()}) @//",
                    Indent: 0, Toggle: doDebug);
            }
            return base.FireEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            Vaultable vaultable = base.DeepCopy(Parent, MapInv) as Vaultable;
            return vaultable;
        }

    } //!-- public class Vaultable : IScribedPart
}
