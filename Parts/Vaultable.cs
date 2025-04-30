using System;
using System.Collections.Generic;

using XRL.UI;
using XRL.World.Parts.Skill;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Linq;
using XRL.EditorFormats.Screen;

namespace XRL.World.Parts
{
    [Serializable]
    public class Vaultable : IScribedPart
    {
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

        public bool CanVault(GameObject Vaulter, GameObject Vaultee)
        {
            // BigEnough IF size doesn't matter OR vaulter is gigantic OR Vaultee isn't gigantic
            bool BigEnough = !SizeMatters || Vaulter.IsGiganticCreature || (Vaultee.HasPart<ModGigantic>() && !Vaultee.IsGiganticCreature);

            // HaveRequiredSkill IF no required skill OR vaulter has required skill
            bool HaveRequiredSkill = !RequiresSkill || Vaulter.HasSkill(nameof(Acrobatics_Jump));

            // HaveEnablingLimbs IF enabling limbs list is empty OR enabling limb list overlaps with vaulter limbs
            bool HaveEnablingLimbs = EnablingLimbs.IsNullOrEmpty() || EnablingLimbsList.OverlapsWith((from bp in Vaulter.Body.GetParts(EvenIfDismembered: false) select bp.Type).ToList());

            // HaveOverridingParts IF overriding parts list has entries AND overriding parts list overlaps with vaulter parts
            bool HaveOverridingParts = !OverridingParts.IsNullOrEmpty() && OverridingPartsList.OverlapsWith((from p in Vaulter.GetPartsDescendedFrom<IPart>() select p.Name).ToList());

            // AbleToMove IF vaulter can change to jump mode AND vaulter can change body to jump position
            bool AbleToMove = Vaulter.CanChangeMovementMode("Jumping") && Vaulter.CanChangeBodyPosition("Jumping");

            // OnTheGround IF vaulter is not flying
            bool OnTheGround = !Vaulter.IsFlying;

            // CanVault IF able to move AND on the groud AND one of: have overriding parts OR have enabling limbs OR have required skill
            return AbleToMove && OnTheGround && (HaveOverridingParts || HaveEnablingLimbs || HaveRequiredSkill);
        }

        public bool AttemptVault(GameObject Vaulter, GameObject Vaultee, IEvent FromEvent = null)
        {
            if (Vaulter.IsFlying)
            {
                Vaulter.Fail("You cannot vault while flying.");
                return false;
            }
            if (!Vaulter.CanChangeMovementMode("Jumping", ShowMessage: true) || !Vaulter.CanChangeBodyPosition("Jumping", ShowMessage: true))
            {
                return false;
            }

            Cell originCell = Vaulter.CurrentCell;
            Cell overCell = Vaultee.CurrentCell;
            Cell destinationCell = null;

            string TargetDirectionFromOverCell = originCell.GetDirectionFromCell(overCell);

            Debug.Entry(4, $"DirectionOverToTargetCell", TargetDirectionFromOverCell, Indent: 2);
            foreach (Cell cell in overCell.GetAdjacentCells())
            {
                if (overCell.GetDirectionFromCell(cell) != TargetDirectionFromOverCell)
                {
                    Debug.CheckNah(4, $"overCell.GetDirectionFromCell(cell)", overCell.GetDirectionFromCell(cell), Indent: 3);
                    continue;
                }
                Debug.CheckYeh(4, $"overCell.GetDirectionFromCell(cell)", overCell.GetDirectionFromCell(cell), Indent: 3);
                destinationCell = cell;
                break;
            }

            // do some checks about this if it's otherwise occupied.
            Cell preferedDestinationCell = destinationCell;


            List<Cell> possibleDestinations = new();
            if (destinationCell == null || !destinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true) || !destinationCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty())
            {
                Debug.Entry(4, $"Destination Cell Occupied, Finding Alternative", Indent: 2);
                foreach (Cell cell in overCell.GetAdjacentCells())
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2);
                    Debug.Entry(4, $"Checking cell [{cell.Location}] ({overCell.GetDirectionFromCell(cell)} of Vaultee)", Indent: 2);

                    bool CellIsAdjacentToOrigin = originCell.GetAdjacentCells().Contains(cell);
                    bool CellIsNotEmpty = !cell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true) || !destinationCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty();
                    bool DirectionIsUnaccptable = !IsDirectionAcceptable(originCell, overCell, cell);
;
                    bool cellIsADud =
                        CellIsAdjacentToOrigin
                     || CellIsNotEmpty
                     || DirectionIsUnaccptable;

                    Debug.LoopItem(4, $"cellIsADud", $"{cellIsADud}", Good: !cellIsADud, Indent: 3);
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.LoopItem(4, $"CellIsAdjacentToOrigin", $"{CellIsAdjacentToOrigin}", Good: !CellIsAdjacentToOrigin, Indent: 4);
                    Debug.LoopItem(4, $"CellIsNotEmpty", $"{CellIsNotEmpty}", Good: !CellIsNotEmpty, Indent: 4);
                    Debug.LoopItem(4, $"DirectionIsUnacceptable", $"{DirectionIsUnaccptable}", Good: !DirectionIsUnaccptable, Indent: 4);

                    if (cellIsADud)
                    {
                        continue;
                    }
                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);
                    Debug.Entry(4, $"Adding cell to possibleDestinations", Indent: 2);
                    possibleDestinations.Add(cell);
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2);

                if (possibleDestinations.IsNullOrEmpty())
                {
                    Debug.CheckNah(4, $"No possibleDestinations", Indent: 2);
                    FromEvent?.RequestInterfaceExit();
                    if (Vaulter.IsPlayer())
                    {
                        Popup.Show($"There's no room on the other side of the {Vaultee.DisplayName} you're trying to vault over!");
                    }
                    return false;
                }
            }
            SoundManager.PreloadClipSet("Sounds/Abilities/sfx_ability_jump");
            Debug.Entry(4, $"Getting furthest Cell from possibleDestinations", Indent: 2);
            foreach (Cell cell in possibleDestinations)
            {
                destinationCell ??= cell;
                if (originCell.CosmeticDistanceto(cell.Location) >= originCell.CosmeticDistanceto(destinationCell.Location))
                    destinationCell = cell;
            }
            Debug.CheckNah(4,
                $"destinationCell is [{destinationCell.Location}] " +
                $"which is {overCell.GetDirectionFromCell(destinationCell)} of {Vaultee.DisplayName} " +
                $"and {originCell.GetDirectionFromCell(destinationCell)} of {Vaulter.DisplayName}",
                Indent: 2);

            GameObject Over = ParentObject;
            if (!Acrobatics_Jump.CheckPath(Vaulter, destinationCell, out Over, out List<Point> Path, Silent: true, CanJumpOverCreatures: true, CanLandOnCreature: false, "vault"))
            {
                Debug.Entry(4, $"/!\\ WARN: CheckPath Failed, vault aborted", Indent: 2);
                FromEvent?.RequestInterfaceExit();
                return false;
            }

            if (!BeforeVaultEvent.CheckFor(Vaulter, originCell, Vaultee, destinationCell, out string Message))
            {
                Debug.CheckNah(4, $"Cancelled by VaultEvent.CheckFor", Message, Indent: 2);
                FromEvent?.RequestInterfaceExit();
                if (Vaulter.IsPlayer() && !Message.IsNullOrEmpty())
                {
                    Popup.Show(Message);
                }
                return false;
            }

            Vaulter?.PlayWorldSound("Sounds/Abilities/sfx_ability_jump");
            Vaulter.MovementModeChanged("Jumping");
            Vaulter.BodyPositionChanged("Jumping");

            Acrobatics_Jump.PlayAnimation(Vaulter, destinationCell);
            XDidYToZ(Vaulter, "vault", "over", ParentObject, null, ".");

            if (Vaulter.DirectMoveTo(destinationCell, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true))
            {
                JumpedEvent.Send(Vaulter, originCell, destinationCell, Path, Path.Count, "Vault");
                VaultedEvent.Send(Vaulter, originCell, Vaultee, destinationCell);
            }
            Vaulter.Gravitate();
            Acrobatics_Jump.Land(originCell, destinationCell);
            FromEvent?.RequestInterfaceExit();
            return true;
        }

        public static bool IsDirectionAcceptable(Cell Origin, Cell Over, Cell Target)
        {
            Debug.Divider(4, HONLY, Count: 15, Indent: 3);
            Debug.LoopItem(4, $"IsDirectionAcceptable: Target [{Target.Location}]", Indent: 3);
            string DirectionOriginToOverCell = Origin.GetDirectionFromCell(Over);

            string DirectionOverToTargetCell = Over.GetDirectionFromCell(Target);
            string DirectionOriginToTargetCell = Origin.GetDirectionFromCell(Target);

            string LongitudinalDirectionOverToTarget = (from s in DirectionOverToTargetCell where (s == 'N' || s == 'S') select s.ToString()).FirstOrDefault() ?? string.Empty;
            string LatitudinalDirectionOverToTarget = (from s in DirectionOverToTargetCell where (s == 'E' || s == 'W') select s.ToString()).FirstOrDefault() ?? string.Empty;

            Debug.LoopItem(4, $"LongitudinalDirectionOverToTarget", LongitudinalDirectionOverToTarget, Indent: 4);
            Debug.LoopItem(4, $"LatitudinalDirectionOverToTarget", LatitudinalDirectionOverToTarget, Indent: 4);

            bool directionIsLongitudinal = !LongitudinalDirectionOverToTarget.IsNullOrEmpty();
            bool directionIsLatitudinal = !LatitudinalDirectionOverToTarget.IsNullOrEmpty();

            bool directionIsCardianl = !(directionIsLongitudinal && directionIsLatitudinal);

            if(directionIsCardianl)
            {
                Debug.LoopItem(4, $"directionIsCardianl", $"{directionIsCardianl}", Good: directionIsCardianl, Indent: 4);
                if (directionIsLongitudinal)
                {
                    Debug.LoopItem(4, $"directionIsLongitudinal", $"{directionIsLongitudinal}", Indent: 4);
                    if (!DirectionOverToTargetCell.Contains(LongitudinalDirectionOverToTarget))
                    {
                        Debug.CheckNah(4, $"DirectionOverToTargetCell ({DirectionOverToTargetCell}), LongitudinalDirectionOverToTarget ({LongitudinalDirectionOverToTarget})", Indent: 5);
                        return false;
                    }
                    Debug.CheckYeh(4, $"DirectionOverToTargetCell ({DirectionOverToTargetCell}), LongitudinalDirectionOverToTarget ({LongitudinalDirectionOverToTarget})", Indent: 5);
                }
                if (directionIsLatitudinal)
                {
                    Debug.LoopItem(4, $"directionIsLatitudinal", $"{directionIsLatitudinal}", Indent: 4);
                    if (!DirectionOverToTargetCell.Contains(LatitudinalDirectionOverToTarget))
                    {
                        Debug.CheckNah(4, $"DirectionOverToTargetCell ({DirectionOverToTargetCell}), LatitudinalDirectionOverToTarget ({LatitudinalDirectionOverToTarget})", Indent: 5);
                        return false;
                    }
                    Debug.CheckYeh(4, $"DirectionOverToTargetCell ({DirectionOverToTargetCell}), LatitudinalDirectionOverToTarget ({LatitudinalDirectionOverToTarget})", Indent: 5);
                }
            }
            else
            {
                Debug.LoopItem(4, $"directionIsCardianl", $"{directionIsCardianl}", Good: directionIsCardianl, Indent: 4);
                if (!DirectionOriginToTargetCell.Contains(LongitudinalDirectionOverToTarget) && !Target.GetDirectionFromCell(Origin).Contains(LatitudinalDirectionOverToTarget))
                {
                    Debug.LoopItem(4,
                        $"DirectionOriginToTargetCell ({DirectionOriginToTargetCell}), LongitudinalDirectionOverToTarget ({LongitudinalDirectionOverToTarget})",
                        Good: !DirectionOriginToTargetCell.Contains(LongitudinalDirectionOverToTarget),
                        Indent: 5);
                    Debug.LoopItem(4, 
                        $"DirectionOriginToTargetCell ({DirectionOriginToTargetCell}), LatitudinalDirectionOverToTarget ({LatitudinalDirectionOverToTarget})",
                        Good: !DirectionOriginToTargetCell.Contains(LatitudinalDirectionOverToTarget),
                        Indent: 5);
                    return false;
                }
                Debug.LoopItem(4,
                    $"DirectionOriginToTargetCell ({DirectionOriginToTargetCell}), LongitudinalDirectionOverToTarget ({LongitudinalDirectionOverToTarget})",
                    Good: DirectionOriginToTargetCell.Contains(LongitudinalDirectionOverToTarget),
                    Indent: 5);
                Debug.LoopItem(4,
                    $"DirectionOriginToTargetCell ({DirectionOriginToTargetCell}), LatitudinalDirectionOverToTarget ({LatitudinalDirectionOverToTarget})",
                    Good: DirectionOriginToTargetCell.Contains(LatitudinalDirectionOverToTarget),
                    Indent: 5);
            }
            Debug.CheckYeh(4, $"IsDirectionAcceptable", Indent: 4);
            Debug.Divider(4, HONLY, Count: 15, Indent: 3);
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetInventoryActionsEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == CanSmartUseEvent.ID
                || ID == CommandSmartUseEvent.ID;
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
            if (!AttemptVault(E.Actor, ParentObject, E))
            {
                return false;
            }
            return base.HandleEvent(E);
        }
    } //!-- public class Vaultable : IScribedPart
}
