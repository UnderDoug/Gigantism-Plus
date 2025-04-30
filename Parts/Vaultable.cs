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
using static UnityEngine.UI.Image;
using System.Drawing;
using PlayFab.DataModels;
using XRL.World.Capabilities;
using System.IO;

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
            if (Vaulter == null || Vaultee == null) return false;

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

            // PhaseMatches IF well, phase matches.
            bool PhaseMatches = Vaulter.PhaseMatches(Vaultee);

            // CanVault IF able to move AND on the ground AND phase matches AND any:
            //      have overriding parts OR have enabling limbs OR have required skill
            return AbleToMove && OnTheGround && PhaseMatches && (HaveOverridingParts || HaveEnablingLimbs || HaveRequiredSkill);
        }

        public bool AttemptVault(GameObject Vaulter, GameObject Vaultee, IEvent FromEvent = null, bool Silent = false)
        {
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
                Debug.CheckNah(4, $"No DestinationCell", Indent: 2);
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

        public static bool TryGetValidDestinationCell(GameObject Vaulter, GameObject Vaultee, out Cell DestinationCell)
        {
            Cell originCell = Vaulter.CurrentCell;
            Cell overCell = Vaultee.CurrentCell;
            DestinationCell = null;

            string DirectionOriginToOver = originCell.GetDirectionFromCell(overCell);
            bool DirectionOriginToOverIsCardinal = DirectionOriginToOver.Length == 1;

            Debug.Entry(4, $"DirectionOverToTargetCell ({DirectionOriginToOver}), IsCardinal:", $"{DirectionOriginToOverIsCardinal}", Indent: 2);
            foreach (Cell cell in overCell.GetAdjacentCells())
            {
                string DirectionOverToDestination = overCell.GetDirectionFromCell(cell);
                if (DirectionOverToDestination != DirectionOriginToOver)
                {
                    Debug.CheckNah(4, $"DirectionOverToDestination", DirectionOverToDestination, Indent: 3);
                    continue;
                }
                Debug.CheckYeh(4, $"DirectionOverToDestination", DirectionOverToDestination, Indent: 3);
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
                Indent: 3);

            Debug.LoopItem(4,
                $"DestinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true)",
                Good: DestinationCell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true),
                Indent: 3);

            Debug.LoopItem(4,
                $"DestinationCell.GetObjectsWithTagOrProperty(\"NoAutowalk\").IsNullOrEmpty()",
                Good: DestinationCell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty(),
                Indent: 3);

            Debug.LoopItem(4,
                $"destinationCellIsAcceptable",
                Good: destinationCellIsAcceptable,
                Indent: 2);

            if (!destinationCellIsAcceptable)
            {
                DestinationCell = null;
            }

            if (DestinationCell == null && !DirectionOriginToOverIsCardinal)
            {

                Debug.Entry(4, $"Destination cell unacceptable, finding alternative", Indent: 2);
                foreach (Cell cell in overCell.GetAdjacentCells())
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2);
                    Debug.Entry(4, $"Checking cell [{cell.Location}] ({overCell.GetDirectionFromCell(cell)} of Vaultee)", Indent: 3);

                    if (originCell.GetAdjacentCells().Contains(cell))
                    {
                        Debug.CheckNah(4, $"CellIsAdjacentToOrigin", Indent: 5);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsNotAdjacentToOrigin", Indent: 5);

                    if (!cell.GetAdjacentCells().Contains(preferedDestinationCell))
                    {
                        Debug.CheckNah(4, $"CellIsNotAdjacentToPreferredCell", Indent: 5);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsAdjacentToPreferredCell", Indent: 5);

                    if (!cell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true))
                    {
                        Debug.CheckNah(4, $"CellIsNotEmpty", Indent: 5);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsEmpty", Indent: 5);

                    if (!cell.GetObjectsWithTagOrProperty("NoAutowalk").IsNullOrEmpty())
                    {
                        Debug.CheckNah(4, $"CellIsNotSafe", Indent: 5);
                        continue;
                    }
                    Debug.CheckYeh(4, $"CellIsSafe", Indent: 5);

                    Debug.Divider(4, HONLY, Count: 25, Indent: 4);
                    Debug.Entry(4, $"DestinationCell set to [{cell.Location}]", Indent: 3);
                    DestinationCell = cell;
                    break;
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2);
            }

            return DestinationCell != null;
        }

        public bool PerformVault(GameObject Vaulter, GameObject Vaultee, Cell DestinationCell, bool Silent = false)
        {
            Cell originCell = Vaulter.CurrentCell;

            SoundManager.PreloadClipSet("Sounds/Abilities/sfx_ability_jump");

            Debug.CheckNah(4,
                $"DestinationCell is [{DestinationCell.Location}] " +
                $"which is {Vaultee.CurrentCell.GetDirectionFromCell(DestinationCell)} of {Vaultee.DisplayName}",
                Indent: 2);

            /*
            if (!Acrobatics_Jump.CheckPath(
                Vaulter, 
                DestinationCell, 
                out Vaultee, 
                out List<Point> Path, 
                Silent: true, 
                CanJumpOverCreatures: true, 
                CanLandOnCreature: false, "vault"))
            {
                Debug.Entry(4, $"/!\\ WARN: CheckPath Failed, vault aborted", Indent: 2);
                return false;
            }
            */

            if (!BeforeVaultEvent.CheckFor(Vaulter, originCell, Vaultee, DestinationCell, out string Message))
            {
                Debug.CheckNah(4, $"Cancelled by VaultEvent.CheckFor", Message, Indent: 2);
                if (Vaulter.IsPlayer() && !Message.IsNullOrEmpty() && !Silent)
                {
                    Popup.Show(Message);
                }
                return false;
            }

            Vaulter?.PlayWorldSound("Sounds/Abilities/sfx_ability_jump");
            Vaulter.MovementModeChanged("Jumping");
            Vaulter.BodyPositionChanged("Jumping");

            Acrobatics_Jump.PlayAnimation(Vaulter, DestinationCell);
            XDidYToZ(Vaulter, "vault", "over", ParentObject, null, ".");

            if (Vaulter.DirectMoveTo(DestinationCell, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true))
            {
                List<Point> Path = Zone.Line(originCell.X, originCell.Y, DestinationCell.X, DestinationCell.Y);
                JumpedEvent.Send(Vaulter, originCell, DestinationCell, Path, Path.Count, "Vault");
                VaultedEvent.Send(Vaulter, originCell, Vaultee, DestinationCell);
            }
            Vaulter.Gravitate();
            Acrobatics_Jump.Land(originCell, DestinationCell);
            return true;
        }

        public bool CanPathThrough(GameObject who)
        {
            if (CanVault(who, ParentObject))
            {
                return true;
            }
            return false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetInventoryActionsEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == CanSmartUseEvent.ID
                || ID == CommandSmartUseEvent.ID
                || ID == GetNavigationWeightEvent.ID
                || ID == GetMovementCapabilitiesEvent.ID;
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
        public override bool HandleEvent(GetNavigationWeightEvent E)
        {

            if (CanPathThrough(E.Actor))
            {
                E.Uncacheable = true;
                E.Weight = 0;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetMovementCapabilitiesEvent E)
        {
            E.Add("Vault over short objects", "Vault", 11250);
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforePhysicsRejectObjectEntringCell");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforePhysicsRejectObjectEntringCell" && E.HasFlag("Actual"))
            {
                GameObject Vaulter = E.GetGameObjectParameter("Object");
                GameObject Vaultee = ParentObject;
                if (CanVault(Vaulter, Vaultee) 
                    && TryGetValidDestinationCell(Vaulter, Vaultee, out Cell DestinationCell))
                {
                    return PerformVault(Vaulter, Vaultee, DestinationCell, Silent: true);
                }
            }
            return base.FireEvent(E);
        }
    } //!-- public class Vaultable : IScribedPart
}
