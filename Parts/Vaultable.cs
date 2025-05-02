using System;
using System.Collections.Generic;
using System.Linq;

using XRL.UI;
using XRL.World.Parts.Skill;
using XRL.World.Anatomy;
using XRL.World.AI.Pathfinding;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using XRL.Rules;
using XRL.World.Conversations.Parts;
using XRL.World.Conversations;
using XRL.World.Capabilities;
using System.Data;
using XRL.EditorFormats.Screen;

namespace XRL.World.Parts
{
    [Serializable]
    public class Vaultable : IScribedPart
    {
        private static bool doDebug => false;

        public bool SizeMatters;
        public bool RequiresSkill;

        public string EnablingLimbs;
        public List<string> EnablingLimbsList;

        public string OverridingParts;
        public List<string> OverridingPartsList;

        public Vaultable() 
        {
            SizeMatters = false;
            RequiresSkill = false;
            EnablingLimbs = null;
            EnablingLimbsList = EnablingLimbs?.CommaExpansion() ?? new();
            OverridingParts = null;
            OverridingPartsList = OverridingParts?.CommaExpansion() ?? new();
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public bool CanVault(GameObject Vaulter)
        {
            return CanVault(Vaulter, ParentObject, SizeMatters, RequiresSkill, EnablingLimbsList, OverridingPartsList);
        }

        public static bool CanVault(GameObject Vaulter, GameObject Vaultee, bool SizeMatters = false, bool RequiresSkill = false, List<string> EnablingLimbsList = null, List<string> OverridingPartsList = null)
        {
            if (Vaulter == null || Vaultee == null) return false;

            // BigEnough IF size doesn't matter OR Vaulter is gigantic OR Vaultee isn't gigantic
            bool BigEnough = !SizeMatters || Vaulter.IsGiganticCreature || (Vaultee.HasPart<ModGigantic>() && !Vaultee.IsGiganticCreature);

            // HaveRequiredSkill IF no required skill OR Vaulter has required skill
            bool HaveRequiredSkill = !RequiresSkill || Vaulter.HasSkill(nameof(Acrobatics_Jump));

            // HaveEnablingLimbs IF enabling limbs list is empty OR enabling limb list overlaps with Vaulter limbs
            bool HaveEnablingLimbs = EnablingLimbsList.IsNullOrEmpty() || EnablingLimbsList.OverlapsWith((from bp in Vaulter.Body.GetParts(EvenIfDismembered: false) select bp.Type).ToList());

            // HaveOverridingParts IF overriding parts list has entries AND overriding parts list overlaps with Vaulter parts
            bool HaveOverridingParts = !OverridingPartsList.IsNullOrEmpty() && OverridingPartsList.OverlapsWith((from p in Vaulter.GetPartsDescendedFrom<IPart>() select p.Name).ToList());

            // AbleToMove IF Vaulter can change to jump mode AND Vaulter can change body to jump position
            bool AbleToMove = Vaulter.CanChangeMovementMode("Jumping") && Vaulter.CanChangeBodyPosition("Jumping");

            // OnTheGround IF Vaulter is not flying
            bool OnTheGround = !Vaulter.IsFlying;

            // PhaseMatches IF well, phase matches.
            bool PhaseMatches = Vaulter.PhaseMatches(Vaultee);

            // CanVault IF able to move AND on the ground AND phase matches AND any:
            //      have overriding parts OR have enabling limbs OR have required skill
            return AbleToMove && OnTheGround && PhaseMatches && (HaveOverridingParts || HaveEnablingLimbs || HaveRequiredSkill);
        }

        public static bool AttemptVault(GameObject Vaulter, GameObject Vaultee, IEvent FromEvent = null, bool Silent = false)
        {
            if (Vaulter == null || Vaultee == null) return false;

            Vaulter.SetStringProperty("Vaulting", null, true);

            if (Vaulter.IsFlying)
            {
                if (!Silent)
                    Vaulter.Fail("You cannot vault while flying.");
                return false;
            }
            if (!Vaulter.CanChangeMovementMode("Jumping", ShowMessage: !Silent) || !Vaulter.CanChangeBodyPosition("Jumping", ShowMessage: !Silent))
            {
                return false;
            }
            if (!Vaulter.PhaseMatches(Vaultee))
            {
                if (!Silent)
                    Vaulter.Fail("You cannot vault something you're out of phase with.");
                return false;
            }

            if (!TryGetValidDestinationCell(Vaulter, Vaultee, out Cell destinationCell))
            {
                Debug.CheckNah(4, $"No DestinationCell", Indent: 2, Toggle: doDebug);
                FromEvent?.RequestInterfaceExit();
                if (Vaulter.IsPlayer())
                {
                    Popup.Show($"There's no room on the other side of the {Vaultee.DisplayName} you're trying to vault over!");
                }
                return false;
            }

            if (!PerformVault(Vaulter, Vaultee, destinationCell))
                return false;

            FromEvent?.RequestInterfaceExit();
            return true;
        }

        public static bool TryGetValidDestinationCell(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, out Cell DestinationCell)
        {
            return TryGetValidDestinationCell(Vaulter, OriginCell, Vaultee, Vaultee.CurrentCell, out DestinationCell);
        }

        public static bool TryGetValidDestinationCell(GameObject Vaulter, GameObject Vaultee, out Cell DestinationCell)
        {
            return TryGetValidDestinationCell(Vaulter, Vaulter.CurrentCell, Vaultee, Vaultee.CurrentCell, out DestinationCell);
        }
        
        public static bool TryGetValidDestinationCell(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell OverCell, out Cell DestinationCell)
        {
            OriginCell ??= Vaulter.CurrentCell;
            OverCell ??= Vaultee.CurrentCell;
            DestinationCell = null;

            if (OriginCell == OverCell)
                return false;

            string DirectionOriginToOver = OriginCell.GetDirectionFromCell(OverCell);
            bool DirectionOriginToOverIsCardinal = DirectionOriginToOver.Length == 1;

            Debug.Entry(4, $"DirectionOverToTargetCell ({DirectionOriginToOver}), IsCardinal:", $"{DirectionOriginToOverIsCardinal}", Indent: 2, Toggle: doDebug);
            foreach (Cell cell in OverCell.GetAdjacentCells())
            {
                string DirectionOverToDestination = OverCell.GetDirectionFromCell(cell);
                if (DirectionOverToDestination != DirectionOriginToOver)
                {
                    Debug.CheckNah(4, $"DirectionOverToDestination", DirectionOverToDestination, Indent: 3, Toggle: doDebug);
                    continue;
                }
                Debug.CheckYeh(4, $"DirectionOverToDestination", DirectionOverToDestination, Indent: 3, Toggle: doDebug);
                DestinationCell = cell;
                break;
            }

            Cell preferedDestinationCell = DestinationCell;

            bool destinationCellIsAcceptable =
                DestinationCell != null
             && DestinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true)
             && DestinationCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty();

            Debug.LoopItem(4,
                $"DestinationCell != null",
                Good: DestinationCell != null,
                Indent: 3, Toggle: doDebug);

            Debug.LoopItem(4,
                $"DestinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true)",
                Good: DestinationCell != null && DestinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true),
                Indent: 3, Toggle: doDebug);

            Debug.LoopItem(4,
                $"DestinationCell.GetObjectsWithTagOrProperty(\"NoAutowalk\").IsNullOrEmpty()",
                Good: DestinationCell != null && DestinationCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty(),
                Indent: 3, Toggle: doDebug);

            Debug.LoopItem(4,
                $"destinationCellIs{(destinationCellIsAcceptable ? "" : "Not")}Acceptable",
                Good: destinationCellIsAcceptable,
                Indent: 2, Toggle: doDebug);

            if (!destinationCellIsAcceptable)
            {
                DestinationCell = null;
            }

            if (DestinationCell == null && !DirectionOriginToOverIsCardinal)
            {

                Debug.Entry(4, $"Destination cell unacceptable, finding alternative", Indent: 2, Toggle: doDebug);
                foreach (Cell cell in OverCell.GetAdjacentCells())
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: doDebug);
                    Debug.Entry(4, $"Checking cell [{cell.Location}] ({OverCell.GetDirectionFromCell(cell)} of Vaultee)", Indent: 3, Toggle: doDebug);

                    if (OriginCell.GetAdjacentCells().Contains(cell))
                    {
                        Debug.CheckNah(4, $"CellIsAdjacentToOrigin", Indent: 5, Toggle: doDebug);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsNotAdjacentToOrigin", Indent: 5, Toggle: doDebug);

                    if (!cell.GetAdjacentCells().Contains(preferedDestinationCell))
                    {
                        Debug.CheckNah(4, $"CellIsNotAdjacentToPreferredCell", Indent: 5, Toggle: doDebug);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsAdjacentToPreferredCell", Indent: 5, Toggle: doDebug);

                    if (!cell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true))
                    {
                        Debug.CheckNah(4, $"CellIsNotEmpty", Indent: 5, Toggle: doDebug);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsEmpty", Indent: 5, Toggle: doDebug);

                    if (!cell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty())
                    {
                        Debug.CheckNah(4, $"CellIsNotSafe", Indent: 5, Toggle: doDebug);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsSafe", Indent: 5, Toggle: doDebug);

                    Debug.Divider(4, HONLY, Count: 25, Indent: 4, Toggle: doDebug);
                    Debug.Entry(4, $"DestinationCell set to [{cell.Location}]", Indent: 3, Toggle: doDebug);
                    DestinationCell = cell;
                    break;
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: doDebug);
            }

            return DestinationCell != null;
        }

        public static bool PerformVault(GameObject Vaulter, GameObject Vaultee, Cell DestinationCell, bool Silent = false)
        {
            Cell originCell = Vaulter.CurrentCell;

            SoundManager.PreloadClipSet("Sounds/Abilities/sfx_ability_jump");

            Debug.Entry(4,
                $"DestinationCell is [{DestinationCell.Location}] " +
                $"which is {Vaultee.CurrentCell.GetDirectionFromCell(DestinationCell)} of {Vaultee.DisplayName}",
                Indent: 2, Toggle: doDebug);

            if (!BeforeVaultEvent.CheckFor(Vaulter, originCell, Vaultee, DestinationCell, out string Message))
            {
                Debug.CheckNah(3, $"Cancelled by VaultEvent.CheckFor", Message, Indent: 2);
                if (Vaulter.IsPlayer() && !Message.IsNullOrEmpty() && !Silent)
                {
                    Popup.Show(Message);
                }
                return false;
            }

            Vaulter?.PlayWorldSound("Sounds/Abilities/sfx_ability_jump");
            Vaulter.MovementModeChanged("Jumping");
            Vaulter.BodyPositionChanged("Jumping");

            Vaulter.SetStringProperty("Vaulting", "true");

            Acrobatics_Jump.PlayAnimation(Vaulter, DestinationCell);
            XDidYToZ(Vaulter, "vault", "over", Vaultee, null, ".");

            if (Vaulter.DirectMoveTo(DestinationCell, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true))
            {
                List<Point> Path = Zone.Line(originCell.X, originCell.Y, DestinationCell.X, DestinationCell.Y);
                JumpedEvent.Send(Vaulter, originCell, DestinationCell, Path, Path.Count, "Vault");
                VaultedEvent.Send(Vaulter, originCell, Vaultee, DestinationCell);
            }
            Vaulter.Gravitate();
            Acrobatics_Jump.Land(originCell, DestinationCell);

            Debug.Entry(4, $"AutoAct.Action Description: {AutoAct.Action?.GetDescription()}", Indent: 1);
            Debug.Entry(4, $"AutoAct.Setting: {AutoAct.Setting}", Indent: 2);
            Debug.Entry(4, $"PlayerWalking: {The.Core.PlayerWalking}", Indent: 2);

            Debug.Entry(4, $"AutoAct.ResumeAction Description: {AutoAct.ResumeAction?.GetDescription()}", Indent: 1);
            Debug.Entry(4, $"AutoAct.ResumeSetting: {AutoAct.ResumeSetting}", Indent: 2);
            if (AutoAct.IsMovement() && AutoAct.IsInterruptable())
            {
                AutoAct.ResumeAction = AutoAct.Action;
                AutoAct.ResumeSetting = AutoAct.Setting;
                // AutoAct.Resume();

                Debug.Entry(4, $"AutoAct.Action Description: {AutoAct.Action?.GetDescription()}", Indent: 1);
                Debug.Entry(4, $"AutoAct.Setting: {AutoAct.Setting}", Indent: 2);
                Debug.Entry(4, $"PlayerWalking: {The.Core.PlayerWalking}", Indent: 2);

                Debug.Entry(4, $"AutoAct.ResumeAction Description: {AutoAct.ResumeAction?.GetDescription()}", Indent: 1);
                Debug.Entry(4, $"AutoAct.ResumeSetting: {AutoAct.ResumeSetting}", Indent: 2);
            }
            return true;
        }

        public bool CanVaultThrough(GameObject Vaulter)
        {
            return CanVaultThrough(Vaulter, ParentObject, SizeMatters, RequiresSkill, EnablingLimbsList, OverridingPartsList);
        }

        public static bool CanVaultThrough(GameObject Vaulter, GameObject Vaultee, bool SizeMatters = false, bool RequiresSkill = false, List<string> EnablingLimbsList = null, List<string> OverridingPartsList = null)
        {
            if (!CanVault(Vaulter, Vaultee, SizeMatters, RequiresSkill, EnablingLimbsList, OverridingPartsList))
                return false;

            Cell vaulterCell = Vaulter.CurrentCell;
            Cell vaulteeCell = Vaultee.CurrentCell;

            Cell vaultTestCell = vaulteeCell.getClosestReachableCellFor(Vaulter);
            List<Cell> dudCells = new();
            FindPath path = new();

            bool canVaultThrough = false;

            if (!canVaultThrough && vaultTestCell != null && TryGetValidDestinationCell(Vaulter, vaultTestCell, Vaultee, out Cell destinationCell))
            {
                canVaultThrough = destinationCell != null;
            }
            else
            {
                dudCells.TryAdd(vaultTestCell);
            }

            if (!canVaultThrough)
            {
                foreach (Cell prospectiveCell in vaulteeCell.GetAdjacentCells())
                {
                    if (dudCells.Contains(prospectiveCell))
                        continue;

                    if (TryGetValidDestinationCell(Vaulter, prospectiveCell, Vaultee, out destinationCell))
                    {
                        if (canVaultThrough = destinationCell != null)
                            break;
                    }
                    dudCells.TryAdd(prospectiveCell);
                }
            }
            return canVaultThrough;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforePhysicsRejectObjectEntringCell");
            Registrar.Register(ParentObject, GetNavigationWeightEvent.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            bool wantAutoVault = 
                !ParentObject.CurrentCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty()
             || ParentObject.Physics.Solid;

            return base.WantEvent(ID, cascade)
                || ID == GetInventoryActionsEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == CanSmartUseEvent.ID
                || ID == CommandSmartUseEvent.ID
                || ID == GetNavigationWeightEvent.ID
                || (wantAutoVault && ID == ObjectEnteringCellEvent.ID)
                || ID == GetShortDescriptionEvent.ID;
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if(The.Player != null && ParentObject.CurrentZone == The.ZoneManager.ActiveZone)
            {
                int navWeight = ParentObject.CurrentCell.GetNavigationWeightFor(The.Player, false);
                int navWeightAuto = ParentObject.CurrentCell.GetNavigationWeightFor(The.Player, true);

                string navWeightColor =
                    navWeight == 0
                    ? "G"
                    : navWeight == 100
                        ? "R"
                        : "W"
                        ;
                string navWeightAutoColor =
                    navWeightAuto == 0
                    ? "G"
                    : navWeightAuto == 100
                        ? "R"
                        : "W"
                        ;

                E.Postfix.AppendRules(
                    $"Vaultable: " +
                    $"NavigationWeight ({navWeight.ToString().Color(navWeightColor)}) | " +
                    $"NavigationWeight[AutoExplore] ({navWeightAuto.ToString().Color(navWeightAutoColor)})");
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            if (CanVault(E.Actor, E.Object))
            {
                E.AddAction(
                Name: "Vault",
                Display: "vault",
                Command: "Vault",
                PreferToHighlight: null,
                Key: 'v',
                FireOnActor: false,
                Default: 12,
                Priority: 0,
                Override: false,
                WorksAtDistance: false,
                WorksTelekinetically: false);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Vault" && !AttemptVault(E.Actor, ParentObject, E))
            {
                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CanSmartUseEvent E)
        {
            if (CanVault(E.Actor, E.Item))
            {
                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CommandSmartUseEvent E)
        {
            if (!AttemptVault(E.Actor, ParentObject, E) && E.MinPriority > 0)
            {
                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetNavigationWeightEvent E)
        {
            if (E.Cell == ParentObject.CurrentCell)
            {
                Debug.Entry(4,
                    $"@ {nameof(Vaultable)}."
                    + $"{nameof(HandleEvent)}({nameof(GetNavigationWeightEvent)} E)"
                    + $" E.Cell[{E?.Cell?.Location}] / Cell[{ParentObject?.CurrentCell?.Location}],"
                    + $" E.Actor: {E?.Actor?.DebugName ?? NULL},"
                    + $" E.Object: {E?.Object.DebugName ?? NULL}",
                    Indent: 0, Toggle: doDebug);

                if (CanVaultThrough(E.Actor, E.Object))
                {
                    E.Uncacheable = true;
                    E.MinWeight(99);
                    return false;
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ObjectEnteringCellEvent E)
        {
            if (E.Cell == ParentObject.CurrentCell && E.Actor != null && E.Actor.HasStringProperty("Vaulting"))
            {
                GameObject Vaulter = E.Actor;
                GameObject Vaultee = ParentObject;

                Vaulter.SetStringProperty("Vaulting", null, true);

                Debug.Entry(4,
                    $"@ {nameof(Vaultable)}."
                    + $"{nameof(HandleEvent)}({nameof(ObjectEnteringCellEvent)} E)"
                    + $" E.Cell[{E?.Cell?.Location}] / Cell[{ParentObject?.CurrentCell?.Location}],"
                    + $" E.Actor: {Vaulter?.DebugName ?? NULL},"
                    + $" E.Object: {Vaultee?.DebugName ?? NULL}",
                    Indent: 0);

                Debug.Entry(4, $"Preventing {Vaulter?.DebugName ?? NULL} from entering cell [{E?.Cell?.Location}]", Indent: 1);

                Debug.Entry(4, $"AutoAct.Action Description: {AutoAct.Action?.GetDescription()}", Indent: 1);
                Debug.Entry(4, $"AutoAct.Setting: {AutoAct.Setting}", Indent: 2);
                Debug.Entry(4, $"PlayerWalking: {The.Core.PlayerWalking}", Indent: 2);

                Debug.Entry(4, $"AutoAct.ResumeAction Description: {AutoAct.ResumeAction?.GetDescription()}", Indent: 1);
                Debug.Entry(4, $"AutoAct.ResumeSetting: {AutoAct.ResumeSetting}", Indent: 2);
                if (AutoAct.IsMovement() && AutoAct.IsInterruptable())
                {
                    AutoAct.ResumeAction = AutoAct.Action;
                    AutoAct.ResumeSetting = AutoAct.Setting;
                    // AutoAct.Resume();

                    Debug.Entry(4, $"AutoAct.Action Description: {AutoAct.Action?.GetDescription()}", Indent: 1);
                    Debug.Entry(4, $"AutoAct.Setting: {AutoAct.Setting}", Indent: 2);
                    Debug.Entry(4, $"PlayerWalking: {The.Core.PlayerWalking}", Indent: 2);

                    Debug.Entry(4, $"AutoAct.ResumeAction Description: {AutoAct.ResumeAction?.GetDescription()}", Indent: 1);
                    Debug.Entry(4, $"AutoAct.ResumeSetting: {AutoAct.ResumeSetting}", Indent: 2);
                }
                return false;
            }
            E.Actor.SetStringProperty("Vaulting", null, true);
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            bool wantAutoVault = 
                !ParentObject.CurrentCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty()
             || ParentObject.Physics.Solid;

            if (wantAutoVault && E.ID == "BeforePhysicsRejectObjectEntringCell" && E.HasFlag("Actual"))
            {
                GameObject Vaulter = E.GetGameObjectParameter("Object");
                GameObject Vaultee = ParentObject;

                Debug.Entry(4, $"{E.ID} Attempting Vault ({Vaulter?.DebugName ?? NULL}, {Vaultee?.DebugName ?? NULL}", Indent: 1);
                if (SmartUse.PerformSmartUse(Vaultee, Vaulter, 1))
                {
                    return false;
                }
            }
            return base.FireEvent(E);
        }
    } //!-- public class Vaultable : IScribedPart
}
