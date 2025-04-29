using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System.Collections.Generic;
using XRL.UI;
using System.IO;
using XRL.EditorFormats.Screen;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{
    [Serializable]
    public class Vaultable : IScribedPart
    {
        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetInventoryActionsEvent.ID
                || ID == InventoryActionEvent.ID;
        }
        public override bool HandleEvent(GetInventoryActionsEvent E)
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

            return base.HandleEvent(E);
        }
        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Vault")
            {
                GameObject Vaulter = E.Actor;
                GameObject Vaultee = ParentObject;
                Cell originCell = Vaulter.CurrentCell;
                Cell destinationCell = null;
                bool directionIsCardianl = originCell.GetCardinalAdjacentCells().Contains(Vaultee.CurrentCell);
                
                string cellOppositeDirection = Vaultee.CurrentCell.GetDirectionFromCell(originCell);

                Debug.Entry(4, $"cellOppositeDirection", cellOppositeDirection, Indent: 2);
                foreach (Cell cell in Vaultee.CurrentCell.GetAdjacentCells())
                {
                    if (cell.GetDirectionFromCell(Vaultee.CurrentCell) != cellOppositeDirection)
                    {
                        Debug.Entry(4, $"cell.GetDirectionFromCell(Vaultee.CurrentCell)", cell.GetDirectionFromCell(Vaultee.CurrentCell), Indent: 2);
                        continue;
                    }
                    else
                    {
                        Debug.Entry(4, $"cell.GetDirectionFromCell(Vaultee.CurrentCell)", cell.GetDirectionFromCell(Vaultee.CurrentCell), Indent: 2);
                        destinationCell = cell;
                    }
                }
                List<Cell> possibleDestinations = new();
                if (destinationCell == null)
                {
                    foreach (Cell cell in Vaultee.CurrentCell.GetAdjacentCells())
                    {
                        bool cellIsADud =
                            originCell.GetAdjacentCells().Contains(cell)
                         || cell.HasWall()
                         || !cell.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true);
                        if (cellIsADud)
                        {
                            continue;
                        }
                        possibleDestinations.Add(cell);
                    }
                    if (possibleDestinations.IsNullOrEmpty())
                    {
                        E.RequestInterfaceExit();
                        if (Vaulter.IsPlayer())
                        {
                            Popup.Show($"There's no room on the other side of the {Vaultee.DisplayName} you're trying to vault over!");
                        }
                        return false;
                    }
                }
                SoundManager.PreloadClipSet("Sounds/Abilities/sfx_ability_jump");
                foreach (Cell cell in possibleDestinations)
                {
                    destinationCell ??= cell;
                    if (cell.CosmeticDistanceto(originCell.Location) > destinationCell.CosmeticDistanceto(originCell.Location))
                        destinationCell = cell;
                }

                GameObject Over = ParentObject;
                if (!Acrobatics_Jump.CheckPath(Vaulter, destinationCell, out Over, out List<Point> Path, Silent: true, CanJumpOverCreatures: true, CanLandOnCreature: false, "vault"))
                {
                    E.RequestInterfaceExit();
                    return false;
                }
                Vaulter?.PlayWorldSound("Sounds/Abilities/sfx_ability_jump");
                Vaulter.MovementModeChanged("Jumping");
                Vaulter.BodyPositionChanged("Jumping");
                Acrobatics_Jump.PlayAnimation(Vaulter, destinationCell);
                XDidYToZ(Vaulter, "vault", "over", ParentObject, null, ".");

                if (Vaulter.DirectMoveTo(destinationCell, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true))
                {
                    JumpedEvent.Send(Vaulter, originCell, destinationCell, Path, 2, "Vault");
                }
                Vaulter.Gravitate();
                Acrobatics_Jump.Land(originCell, destinationCell);
                E.RequestInterfaceExit();
            }
            return base.HandleEvent(E);
        }
    } //!-- public class Vaultable : IScribedPart
}
