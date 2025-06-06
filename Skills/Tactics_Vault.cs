using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Genkit;

using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.AI.Pathfinding;
using XRL.Wish;

using Mathf =  UnityEngine.Mathf;

using HNPS_GigantismPlus;
using Debug = HNPS_GigantismPlus.Debug;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Skill
{
    [Serializable]
    public class Tactics_Vault : BaseSkill
    {
        private static bool doDebug => getClassDoDebug(nameof(Tactics_Vault));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                "SD",   // ShortDescription
            };
            List<object> dontList = new()
            {
                'V',    // Vomit
                "AV",   // AttemptVault
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public static readonly string COMMAND_NAME = "CommandTacticsVault";
        public static readonly string COMMAND_TOGGLE = "CommandToggleTacticsVault";
        private static readonly string BEGIN_ATTACK_EVENT = "BeginAttack";

        public Guid ActivatedAbilityID = Guid.Empty;

        public string ActiveAbilityName = GetActivatedAbilityName();

        private bool _wantToVault = false;
        public bool WantToVault
        {
            get => _wantToVault = IsMyActivatedAbilityToggledOn(ActivatedAbilityID);
            set
            {
                if (value != _wantToVault && value != IsMyActivatedAbilityToggledOn(ActivatedAbilityID))
                {
                    ToggleMyActivatedAbility(ActivatedAbilityID, Silent: true, SetState: value);
                    _wantToVault = IsMyActivatedAbilityToggledOn(ActivatedAbilityID);
                }
            }
        }

        public bool MidVault = false;

        public bool Vaulted = false;

        public Cell Origin;

        public Cell Over;

        public Cell Destination;

        public bool WasAutoActing
        {
            get;
            private set;
        }

        public string AutoActSetting
        {
            get;
            private set;
        }

        public bool IsBurrowerWantsToVault =>
                ParentObject != null
             && WantToVault
             && !MidVault
             && !Vaulted
             && ParentObject.HasPart<Digging>();

        public Tactics_Vault ClearCells()
        {
            Origin = null;
            Over = null;
            Destination = null;
            return this;
        }
        public Tactics_Vault ClearAutoAct()
        {
            WasAutoActing = false;
            AutoActSetting = null;
            return this;
        }
        public Tactics_Vault Clear()
        {
            ClearCells();
            ClearAutoAct();
            Vaulted = false;
            return this;
        }

        public override bool AddSkill(GameObject GO)
        {
            ActivatedAbilityID = AddMyActivatedAbility(
                        Name: ActiveAbilityName,
                        Command: COMMAND_TOGGLE,
                        Class: "Skills",
                        Description: null,
                        Icon: "\xE3", // π
                        Toggleable: true,
                        DefaultToggleState: true,
                        AlwaysAllowToggleOff: true
                        );
            return true;
        }
        public override bool RemoveSkill(GameObject GO)
        {
            RemoveMyActivatedAbility(ref ActivatedAbilityID);
            return base.RemoveSkill(GO);
        }

        public static string GetActivatedAbilityName()
        {
            return $"Vault";
        }

        public static StringBuilder GetActivatedAbilityDescription(bool Enabled = true)
        {
            string onOffColor = Enabled ? "g" : "r";

            string enabledText = "You will vault over anything you're able to that would otherwise block you while auto-moving.";
            string disabledText = "You won't automatically vault while auto-moving, but you may still attempt to manually vault.";

            return Event.NewStringBuilder().AppendColored(onOffColor, Enabled ? enabledText : disabledText);
        }

        public void CollectStats(Templates.StatCollector stats)
        {
            stats.Set("EnableDisable", Event.FinalizeString(GetActivatedAbilityDescription(WantToVault)));
        }

        public static bool CanTryVault(GameObject Vaulter, GameObject Vaultee, out Tactics_Vault VaultSkill, bool Silent = true)
        {
            VaultSkill = null;

            if (Vaulter == null)
                return false;

            if (!Vaulter.TryGetPart(out VaultSkill))
                return false;

            if (VaultSkill.Vaulted)
            {
                Debug.Warn(2,
                    nameof(Tactics_Vault),
                    nameof(CanTryVault),
                    "Can't vault while already vaulting");
                return false;
            }

            if (Vaulter.IsFlying)
            {
                if (!Silent)
                    Vaulter.Fail("You cannot vault while flying.");
                return false;
            }

            if (!Vaulter.CanChangeMovementMode("Vaulting", ShowMessage: !Silent) || !Vaulter.CanChangeBodyPosition("Vaulting", ShowMessage: !Silent))
            {
                return false;
            }

            if (Vaultee != null && !Vaulter.PhaseMatches(Vaultee))
            {
                if (!Silent)
                    Vaulter.Fail("You cannot vault something you're out of phase with.");
                return false;
            }

            return true;
        }

        public static bool CanNormallyVault(GameObject Vaulter, GameObject Vaultee, bool? SizeMatters = false, bool? RequiresJumpSkill = false, List<string> EnablingLimbsList = null, List<string> OverridingPartsList = null)
        {
            if (Vaulter == null)
                return false;

            if (Vaultee != null && Vaultee.TryGetPart(out Vaultable vaultable))
            {
                SizeMatters ??= vaultable.SizeMatters;
                RequiresJumpSkill ??= vaultable.RequiresJumpSkill;
                EnablingLimbsList ??= vaultable.EnablingLimbsList;
                OverridingPartsList ??= vaultable.OverridingPartsList;
            }

            if (!OverridingPartsList.IsNullOrEmpty() 
             && OverridingPartsList.OverlapsWith((from p in Vaulter.GetPartsDescendedFrom<IPart>() select p.Name).ToList()))
                return true;

            if ((bool)SizeMatters && !Vaulter.IsGiganticCreature && Vaultee != null && (Vaultee.HasPart<ModGigantic>() || Vaultee.IsGiganticCreature))
                return false;

            if ((bool)RequiresJumpSkill && !Vaulter.HasSkill(nameof(Acrobatics_Jump)))
                return false;

            if (!EnablingLimbsList.IsNullOrEmpty() 
             && !EnablingLimbsList.OverlapsWith((from bp in Vaulter.Body.GetParts(EvenIfDismembered: false) select bp.Type).ToList()))
                return false;

            return true;
        }
        public bool CanNormallyVault(GameObject Vaultee)
        {
            return CanNormallyVault(ParentObject, Vaultee);
        }

        public static bool CanVault(GameObject Vaulter, GameObject Vaultee, out Tactics_Vault VaultSkill, bool? SizeMatters = false, bool? RequiresSkill = false, List<string> EnablingLimbsList = null, List<string> OverridingPartsList = null, bool Silent = false)
        {
            return CanTryVault(Vaulter, Vaultee, out VaultSkill, Silent) 
                && CanNormallyVault(Vaulter, Vaultee, SizeMatters, RequiresSkill, EnablingLimbsList, OverridingPartsList);
        }
        public static bool CanVault(GameObject Vaulter, GameObject Vaultee, bool? SizeMatters = false, bool? RequiresSkill = false, List<string> EnablingLimbsList = null, List<string> OverridingPartsList = null, bool Silent = false)
        {
            return CanVault(Vaulter, Vaultee, out _, SizeMatters, RequiresSkill, EnablingLimbsList, OverridingPartsList, Silent);
        }
        public bool CanVault(GameObject Vaultee, out Tactics_Vault VaultSkill, bool Silent = false)
        {
            return CanVault(ParentObject, Vaultee, out VaultSkill, Silent);
        }
        public bool CanVault(GameObject Vaultee, bool Silent = false)
        {
            return CanVault(ParentObject, Vaultee, out _, Silent);
        }

        /// <summary>
        /// Attempts to perform a vault based on the entered arguments. A vault is a pseudo-jump over an obstacle with a very restricted prospective destination which must be the cell directly opposite the obstacle, unless the vault is ordinal, in which case, if the preferred destination cell is otherwise invalid, the fallback cell is one of the two cells which is cardinally adjacent to both the obstacle and the preferred destination cell.
        /// </summary>
        /// <param name="Vaulter">The GameObject which will be moved from its origin cell to the destination of the vault.</param>
        /// <param name="OriginCell">The Cell from which the vault will be performed. Typically the cell the Vaulter is occupying.</param>
        /// <param name="Vaultee">The GameObject over which the vault will occur.</param>
        /// <param name="DestinationCell">The cell into which the Vaulter will land upon a successful vault.</param>
        /// <param name="FromEvent">The Event from which this method was called, allowing for the exiting of UI where appropriate.</param>
        /// <param name="Silent">Whether none or all of the pop-ups or player message queue messages should appear.</param>
        /// <returns>whether or not the vault was successful.</returns>
        public static bool AttemptVault(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell DestinationCell = null, IEvent FromEvent = null, bool Silent = false)
        {
            Debug.Entry(4,
                $"* {nameof(Tactics_Vault)}."
                + $"{nameof(AttemptVault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL},"
                + $" OriginCell: [{OriginCell?.Location}],"
                + $" Vaultee: {Vaultee?.DebugName ?? NULL},"
                + $" DestinationCell: [{DestinationCell?.Location}],"
                + $" FromEvent: {FromEvent?.GetType()?.Name ?? NULL},"
                + $" Silent: {Silent})",
                Indent: 0, Toggle: getDoDebug("AV"));

            if (!CanVault(Vaulter, Vaultee, out Tactics_Vault vaultSkill, Silent))
                return false;

            vaultSkill.Clear().Vomit(4, $"{nameof(AttemptVault)}", "Start", Indent: 1, Toggle: getDoDebug('V'));

            if ((DestinationCell != null && !IsTargetCellValidDestination(Vaulter, DestinationCell))
             || !TryGetValidDestinationCell(Vaulter, OriginCell, Vaultee, out DestinationCell))
            {
                Debug.CheckNah(4, $"No DestinationCell", Indent: 2, Toggle: getDoDebug("AV"));
                FromEvent?.RequestInterfaceExit();
                if (Vaulter.IsPlayer() && !Silent)
                {
                    Popup.Show($"There's no room on the other side of the {Vaultee.DisplayName} you're trying to vault over!");
                }
                return false;
            }

            vaultSkill.Origin = OriginCell ??= Vaulter.CurrentCell;
            vaultSkill.Over = Vaultee.CurrentCell;

            vaultSkill.Vomit(4, $"{nameof(AttemptVault)}", "Immediately Pre-Vault", Indent: 1, Toggle: getDoDebug('V'));

            if (!Vault(Vaulter, Vaultee, DestinationCell, Silent))
            {
                vaultSkill.Vomit(4, $"{nameof(AttemptVault)}", "Vault Failed", Indent: 1, Toggle: getDoDebug('V'));
                return false;
            }
            vaultSkill.Vomit(4, $"{nameof(AttemptVault)}", "Immediately Post-Vault", Indent: 1, Toggle: getDoDebug('V'));

            Debug.LoopItem(4, $" ] Requesting Interface Exit from event (if present)", Indent: 2, Toggle: getDoDebug("AV"));
            FromEvent?.RequestInterfaceExit();
            Debug.LoopItem(4, $"FromEvent not null", $"{FromEvent != null}", Good: FromEvent != null, Indent: 2, Toggle: getDoDebug("AV"));

            Debug.Entry(4,
                $"x {nameof(Tactics_Vault)}."
                + $"{nameof(AttemptVault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL},"
                + $" OriginCell: [{OriginCell?.Location}],"
                + $" Vaultee: {Vaultee?.DebugName ?? NULL},"
                + $" DestinationCell: [{DestinationCell?.Location}],"
                + $" FromEvent: {FromEvent?.GetType()?.Name ?? NULL},"
                + $" Silent: {Silent}) *//",
                Indent: 0, Toggle: getDoDebug("AV"));

            return true;
        }
        public static bool AttemptVault(GameObject Vaulter, GameObject Vaultee, Cell OriginCell = null, IEvent FromEvent = null, bool Silent = false)
        {
            return AttemptVault(Vaulter, OriginCell, Vaultee, null, FromEvent, Silent);
        }
        public bool AttemptVault(GameObject Vaultee, Cell DestinationCell, IEvent FromEvent = null, bool Silent = false)
        {
            return AttemptVault(ParentObject, ParentObject.CurrentCell, Vaultee, DestinationCell, FromEvent, Silent);
        }

        public static Cell GetValidDestinationCell(GameObject Vaulter, Cell Pivot)
        {
            if (Vaulter == null || Pivot == null)
                return null;

            Cell Origin = Vaulter.CurrentCell;
            Cell Destination = null;

            Debug.LoopItem(4, $"Origin", $"[{Origin?.Location}]", Good: Origin != null,
                Indent: 1, Toggle: doDebug);
            Debug.LoopItem(4, $"Pivot", $"[{Pivot?.Location}]", Good: Pivot != null,
                Indent: 1, Toggle: doDebug);
            Debug.LoopItem(4, $"Destination", $"[{Destination?.Location}]", Good: Destination == null,
                Indent: 1, Toggle: doDebug);

            if (Origin == Pivot)
                return null;

            string directionOfVault = Origin.GetDirectionFromCell(Pivot);
            bool directionOfVaultIsCardinal = Cell.DirectionListCardinalOnly.Contains(directionOfVault);

            Debug.Entry(4,
                $"directionOfVault ({directionOfVault}), " +
                $"IsCardinal:", $"{directionOfVaultIsCardinal}",
                Indent: 2, Toggle: doDebug);

            Destination = Origin.GetCellOppositePivotCell(Pivot);
            Cell preferedDestinationCell = Destination;

            if (!IsTargetCellValidDestination(Vaulter, Destination))
                Destination = null;

            if (Destination == null && !directionOfVaultIsCardinal && preferedDestinationCell != null)
            {
                Debug.Entry(4, $"Destination cell unacceptable, finding alternative", Indent: 2, Toggle: doDebug);
                foreach (Cell cell in Pivot.GetCardinalAdjacentCells())
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: doDebug);
                    Debug.Entry(4,
                        $"Checking cell [{cell.Location}] (" +
                        $"{Pivot.GetDirectionFromCell(cell)} of Vaultee)",
                        Indent: 3, Toggle: doDebug);

                    if (cell.GetAdjacentCells().Contains(preferedDestinationCell))
                    {
                        Debug.CheckNah(4, $"Cell is adjacent to preferredDestination", Indent: 5, Toggle: doDebug);
                        Destination = cell;
                        break;
                    }
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2, Toggle: doDebug);

                if (!IsTargetCellValidDestination(Vaulter, Destination))
                    Destination = null;
            }
            return Destination;
        }

        public static bool IsTargetCellValidDestination(GameObject Vaulter, Cell Target)
        {
            if (Vaulter == null)
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Vaulter == null", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            if (Target == null)
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target == null", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            if (!Target.IsEmptyOfSolidFor(Vaulter, IncludeCombatObjects: true))
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target is empty of solid for Vaulter ({Vaulter?.DebugName ?? NULL})", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            string noAutoWalk = "NoAutowalk";
            if (!Target.GetObjectsWithTagOrProperty(noAutoWalk).IsNullOrEmpty())
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target contains objects tagged {noAutoWalk.Quote()}", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            if (Target.GetDangerousOpenLiquidVolume() != null)
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target contains dangerous open liquid volume", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            if (Target.HasCombatObject())
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target contains combat object", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            if (Target.HasSwimmingDepthLiquid())
            {
                Debug.CheckNah(4, 
                    $"{nameof(IsTargetCellValidDestination)}", 
                    $"Target contains swimming depth liquid volume", 
                    Indent: 3, Toggle: doDebug);
                return false;
            }

            return true;
        }
        public bool IsTargetCellValidDestination(Cell Target)
        {
            return IsTargetCellValidDestination(ParentObject, Target);
        }

        public static bool TryGetValidDestinationCell(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell OverCell, out Cell DestinationCell)
        {
            OriginCell ??= Vaulter.CurrentCell;
            OverCell ??= Vaultee.CurrentCell;
            DestinationCell = null;

            if (OriginCell == OverCell)
                return false;

            return (DestinationCell = GetValidDestinationCell(Vaulter, OverCell)) != null;
        }
        public static bool TryGetValidDestinationCell(GameObject Vaulter, GameObject Vaultee, out Cell DestinationCell)
        {
            return TryGetValidDestinationCell(Vaulter, Vaulter.CurrentCell, Vaultee, Vaultee.CurrentCell, out DestinationCell);
        }
        public static bool TryGetValidDestinationCell(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, out Cell DestinationCell)
        {
            return TryGetValidDestinationCell(Vaulter, OriginCell, Vaultee, Vaultee.CurrentCell, out DestinationCell);
        }

        public static bool Vault(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell DestinationCell, out Cell Origin, out Cell Over, out Cell Destination, bool Silent = false)
        {
            Debug.Entry(4, 
                $"* {nameof(Tactics_Vault)}."
                + $"{nameof(Vault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL})", 
                Indent: 1, Toggle: getDoDebug());

            Origin = OriginCell ?? Vaulter.CurrentCell;
            Over = Vaultee.CurrentCell;
            Destination = DestinationCell;

            if (!Vaulter.TryGetPart(out Tactics_Vault vaultSkill))
            {
                Debug.LoopItem(4, $"!] Vaulter lacks {nameof(Tactics_Vault)} part", Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Clearing out Cells...", Indent: 3, Toggle: getDoDebug());
                Origin = null;
                Over = null;
                Destination = null;

                Debug.Entry(4,
                    $"x {nameof(Tactics_Vault)}."
                    + $"{nameof(Vault)}"
                    + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
                    Indent: 1, Toggle: getDoDebug());
                return false;
            }
            Debug.LoopItem(4, $"Storing cells in {nameof(vaultSkill)}...", Indent: 2, Toggle: getDoDebug());
            vaultSkill.Origin = Origin;
            vaultSkill.Over = Over;
            vaultSkill.Destination = Destination;
            vaultSkill.Vomit(4, nameof(Vault), $"Cells Stored", Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"Preloading Sound Clip...", Indent: 2, Toggle: getDoDebug());
            SoundManager.PreloadClipSet("Sounds/Abilities/sfx_ability_jump");

            Debug.Entry(4,
                $"DestinationCell is [{DestinationCell?.Location}] " +
                $"which is {Vaultee?.CurrentCell?.GetDirectionFromCell(DestinationCell)} of {Vaultee?.DisplayName}",
                Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"Sending BeforeVaultEvent...", Indent: 2, Toggle: getDoDebug());

            if (!BeforeVaultEvent.CheckFor(Vaulter, Origin, Over, Destination, out string Message))
            {
                Debug.CheckNah(3, $"Cancelled by BeforeVaultEvent.CheckFor", Message, Indent: 2);
                if (Vaulter.IsPlayer() && !Message.IsNullOrEmpty() && !Silent)
                {
                    Popup.Show(Message);
                }

                Debug.Entry(4,
                    $"x {nameof(Tactics_Vault)}."
                    + $"{nameof(Vault)}"
                    + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
                    Indent: 1, Toggle: getDoDebug());
                return vaultSkill.Vaulted = false;
            }
            Debug.CheckYeh(4, $"Vault not blocked by event", Indent: 2, Toggle: getDoDebug());

            bool isAutoActingPlayer = Vaulter.IsPlayer() && AutoAct.IsActive();

            Debug.LoopItem(4, $"isAutoActingPlayer", $"{isAutoActingPlayer}", Good: isAutoActingPlayer,
                Indent: 2, Toggle: getDoDebug());

            bool vaulted = vaultSkill.Vaulted = false;

            Debug.LoopItem(4, $"vaulted", $"{vaulted}", Good: !vaulted,
                Indent: 2, Toggle: getDoDebug());

            if (isAutoActingPlayer)
            {
                Debug.LoopItem(4, $"Checking AutoAct for Movement or Explore...", Indent: 2, Toggle: getDoDebug());
                if (AutoAct.IsAnyMovement() && (AutoAct.Setting.StartsWith("M") || AutoAct.ResumeSetting.StartsWith("M")))
                {
                    Debug.CheckYeh(4, $"AutoAct is Movement with \"M\" Setting", Indent: 2, Toggle: getDoDebug());
                    vaultSkill.AutoActSetting = AutoAct.Setting.StartsWith("M") ? AutoAct.Setting : AutoAct.ResumeSetting;
                    vaultSkill.WasAutoActing = vaultSkill.AutoActSetting.StartsWith("M");
                    Debug.CheckYeh(4, $"AutoActSetting", $"{vaultSkill.AutoActSetting}", Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"WasAutoActing", $"{vaultSkill.WasAutoActing}", Good: vaultSkill.WasAutoActing,
                        Indent: 2, Toggle: getDoDebug());
                }
                else if (AutoAct.IsAnyExploration() && (AutoAct.Setting.StartsWith("?") || AutoAct.ResumeSetting.StartsWith("?")))
                {
                    Debug.CheckYeh(4, $"AutoAct is Explore with \"?\" Setting", Indent: 2, Toggle: getDoDebug());
                    vaultSkill.AutoActSetting = AutoAct.Setting.StartsWith("?") ? AutoAct.Setting : AutoAct.ResumeSetting;
                    vaultSkill.WasAutoActing = vaultSkill.AutoActSetting.StartsWith("?");
                    Debug.CheckYeh(4, $"AutoActSetting", $"{vaultSkill.AutoActSetting}", Indent: 2, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"WasAutoActing", $"{vaultSkill.WasAutoActing}", Good: vaultSkill.WasAutoActing,
                        Indent: 2, Toggle: getDoDebug());
                }

                Debug.LoopItem(4, $"Interrupting AutoAct...", Indent: 2, Toggle: getDoDebug());
                AutoAct.Interrupt();

                Debug.LoopItem(4, $"Setting MidVault to true...", Indent: 2, Toggle: getDoDebug());
                vaultSkill.MidVault = true;
                Debug.LoopItem(4, $"MidVault", $"{vaultSkill.MidVault}", Good: vaultSkill.MidVault,
                    Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"DirectMove Vaulter to Vaultee Cell...", Indent: 2, Toggle: getDoDebug());
                bool directMove = Vaulter.DirectMoveTo(Over, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true, Ignore: Vaultee);
                Debug.LoopItem(4, $"directMove", $"{directMove}", Good: directMove,
                    Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"MidVault", $"{vaultSkill.MidVault}", Good: vaultSkill.MidVault,
                    Indent: 2, Toggle: getDoDebug());
                if (vaultSkill.MidVault)
                {
                    Debug.Warn(4, 
                        $"{nameof(Tactics_Vault)}", 
                        $"{nameof(Vault)}",
                        $"MidVault is still \"true\" following DirectMove",
                        Indent: 0);
                    vaultSkill.MidVault = false;
                }

                Debug.LoopItem(4, $"AutoAct.TryToMove Vaulter to Destination Cell...", Indent: 2, Toggle: getDoDebug());
                vaultSkill.Vaulted = vaulted = AutoAct.TryToMove(Vaulter, Over, Destination);
                Debug.LoopItem(4, $"vaulted", $"{vaulted}", Good: vaulted,
                    Indent: 2, Toggle: getDoDebug());
            }
            // Possibly swap this to be "else" instead of a different "if"
            if (!vaulted)
            {
                Debug.LoopItem(4, $"DirectMove Vaulter to Destination Cell...", Indent: 2, Toggle: getDoDebug());
                vaulted = Vaulter.DirectMoveTo(Destination, 0, Forced: false, IgnoreCombat: true, IgnoreGravity: true);
                Debug.LoopItem(4, $"vaulted", $"{vaulted}", Good: vaulted,
                    Indent: 2, Toggle: getDoDebug());
            }

            if (vaulted)
            {
                Debug.LoopItem(4, $"Playing World Sound...", Indent: 2, Toggle: getDoDebug());
                Vaulter?.PlayWorldSound("Sounds/Abilities/sfx_ability_jump");

                Debug.LoopItem(4, $"Changing Movement Mode...", Indent: 2, Toggle: getDoDebug());
                Vaulter.MovementModeChanged("Jumping");

                Debug.LoopItem(4, $"Changing Body Position...", Indent: 2, Toggle: getDoDebug());
                Vaulter.BodyPositionChanged("Jumping");

                Debug.LoopItem(4, $"Playing Animation...", Indent: 2, Toggle: getDoDebug());
                PlayAnimation(Vaulter, Origin, Destination);

                Debug.LoopItem(4, $"Sending Message Queue Text ({nameof(XDidYToZ)})...", Indent: 2, Toggle: getDoDebug());
                XDidYToZ(Vaulter, "vault", "over", Vaultee, null, ".");

                Debug.LoopItem(4, $"Sending Vaulted Event...", Indent: 2, Toggle: getDoDebug());
                VaultedEvent.Send(Vaulter, Origin, Over, DestinationCell);

                Debug.LoopItem(4, $"Gravitating Vaulter...", Indent: 2, Toggle: getDoDebug());
                Vaulter.Gravitate();

                Debug.LoopItem(4, $"Landing Vaulter from Origin [{Origin?.Location}] to Destination [{Destination?.Location}]...", 
                    Indent: 2, Toggle: getDoDebug());
                Land(Origin, Destination);
            }
            if (!vaulted && isAutoActingPlayer)
            {
                Debug.LoopItem(4, $"vaulted", $"{vaulted}", Good: !vaulted,
                    Indent: 2, Toggle: getDoDebug());
                Debug.LoopItem(4, $"isAutoActingPlayer", $"{isAutoActingPlayer}", Good: isAutoActingPlayer,
                    Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"!] Vault has gone wrong somewhere", Indent: 2, Toggle: getDoDebug());
                Debug.LoopItem(4, $"Clearing out Cells...", Indent: 3, Toggle: getDoDebug());
                Origin = null;
                Over = null;
                Destination = null;

                Debug.LoopItem(4, $"Clearing Stored Cells...", Indent: 3, Toggle: getDoDebug());

                vaultSkill.Clear().Vomit(4, nameof(Vault), $"Cleared after Vault erroneously failed", Indent: 3, Toggle: getDoDebug());

                Debug.Entry(4,
                    $"x {nameof(Tactics_Vault)}."
                    + $"{nameof(Vault)}"
                    + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
                    Indent: 1, Toggle: getDoDebug());
                return vaultSkill.Vaulted = false;
            }
            if (!isAutoActingPlayer)
            {
                Debug.LoopItem(4, $"isAutoActingPlayer", $"{isAutoActingPlayer}", Good: isAutoActingPlayer,
                    Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Clearing Stored Cells...", Indent: 3, Toggle: getDoDebug());
                vaultSkill.Clear().Vomit(4, nameof(Vault), $"Cleared after Manual Vault", Indent: 3, Toggle: getDoDebug());
                Debug.LoopItem(4, $"Setting Vaulted to false...", Indent: 3, Toggle: getDoDebug());
                vaultSkill.Vaulted = false;
            }

            Debug.Entry(4,
                $"x {nameof(Tactics_Vault)}."
                + $"{nameof(Vault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
                Indent: 1, Toggle: getDoDebug());
            return vaulted;
        }
        public static bool Vault(GameObject Vaulter, GameObject Vaultee, Cell DestinationCell, out Cell Origin, out Cell Over, out Cell Destination, bool Silent = false)
        {
            return Vault(Vaulter, null, Vaultee, DestinationCell, out Origin, out Over, out Destination, Silent);
        }
        public static bool Vault(GameObject Vaulter, GameObject Vaultee, Cell DestinationCell, bool Silent = false)
        {
            return Vault(Vaulter, Vaultee, DestinationCell, out _, out _, out _, Silent);
        }
        public bool Vault(GameObject Vaultee, Cell DestinationCell, bool Silent = false)
        {
            return Vaulted = Vault(ParentObject, null, Vaultee, DestinationCell, out Origin, out Over, out Destination, Silent);
        }
        public bool Vault(GameObject Vaultee, Cell DestinationCell, out Cell Origin, out Cell Over, out Cell Destination, bool Silent = false)
        {
            Vaulted = Vault(ParentObject, null, Vaultee, DestinationCell, out this.Origin, out this.Over, out this.Destination, Silent);
            Origin = this.Origin;
            Over = this.Over;
            Destination = this.Destination;
            return Vaulted;
        }

        public static void Land(Cell From, Cell To, int Count = 3, int Life = 6)
        {
            if (To.IsVisible())
            {
                float angle = (float)Math.Atan2(To.X - From.X, To.Y - From.Y);
                Land(To.X, To.Y, angle, Count, Life);
            }
        }
        public static void Land(int X, int Y, float Angle, int Count = 3, int Life = 6)
        {
            for (int i = 0; i < Count; i++)
            {
                float f = Stat.RandomCosmetic(-75, 75) * (MathF.PI / 180f) + Angle;
                float xDel = Mathf.Sin(f) / (Life / 2f);
                float yDel = Mathf.Cos(f) / (Life / 2f);
                string particle = ((Stat.RandomCosmetic(1, 4) <= 3) ? "&y." : "&y±");
                XRLCore.ParticleManager.Add(particle, X, Y, xDel, yDel, Life, 0f, 0f, 0L);
            }
        }

        public static void PlayAnimation(GameObject Actor, Cell OriginCell, Cell TargetCell)
        {
            OriginCell ??= Actor.CurrentCell;
            Location2D originLocation = OriginCell.Location;
            Location2D targetLocation = TargetCell.Location;

            if (UI.Options.UseOverlayCombatEffects && TargetCell.IsVisible())
            {
                CombatJuice.BlockUntilFinished(
                    CombatJuice.Jump(
                        Actor: Actor,
                        Location: originLocation,
                        Target: targetLocation,
                        Duration: Stat.Random(0.2f, 0.3f),
                        Arc: Stat.Random(0.3f, 0.4f),
                        Scale: 0.65f,
                        Focus: Actor.IsPlayer()),
                    new GameObject[1] { Actor },
                    Interruptible: false);
            }
        }
        public static void PlayAnimation(GameObject Actor, Cell TargetCell)
        {
            PlayAnimation(Actor, Actor.CurrentCell, TargetCell);
        }

        public static bool ResumeAfterVault(GameObject Vaulter)
        {
            Debug.Entry(4,
                $"* {nameof(Tactics_Vault)} "
                + $"{nameof(ResumeAfterVault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL})",
                Indent: 0, Toggle: getDoDebug());

            Tactics_Vault vaultSkill = null;
            bool vaulterNotNull = Vaulter != null;
            bool isPlayer = vaulterNotNull && Vaulter.IsPlayer();
            bool haveSkill = vaulterNotNull && Vaulter.TryGetPart(out vaultSkill);
            bool wasAutoActing = haveSkill && vaultSkill.WasAutoActing;
            bool haveAutoActSetting = haveSkill && !vaultSkill.AutoActSetting.IsNullOrEmpty();
            bool autoActSettingIsMovement = haveAutoActSetting && vaultSkill.AutoActSetting.StartsWith("M");
            bool autoActSettingIsExplore = haveAutoActSetting && vaultSkill.AutoActSetting.StartsWith("?");
            bool autoActSettingIsMovementOrExplore = autoActSettingIsMovement || autoActSettingIsExplore;

            bool shouldResume =
                vaulterNotNull
             && isPlayer
             && haveSkill
             && wasAutoActing
             && haveAutoActSetting
             && autoActSettingIsMovementOrExplore;

            Debug.Entry(4, $"Determining whether to resume AutoAct", Indent: 1, Toggle: getDoDebug());

            Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}",
                Good: vaulterNotNull, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(isPlayer)}", $"{isPlayer}",
                Good: isPlayer, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(haveSkill)}", $"{haveSkill}",
                Good: haveSkill, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(wasAutoActing)}", $"{wasAutoActing}",
                Good: wasAutoActing, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(haveAutoActSetting)}", $"{haveAutoActSetting}",
                Good: haveAutoActSetting, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(autoActSettingIsMovementOrExplore)}", $"{autoActSettingIsMovementOrExplore}",
                Good: autoActSettingIsMovementOrExplore, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(autoActSettingIsMovement)}", $"{autoActSettingIsMovement}",
                Good: autoActSettingIsMovement, Indent: 3, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(autoActSettingIsExplore)}", $"{autoActSettingIsExplore}",
                Good: autoActSettingIsExplore, Indent: 3, Toggle: getDoDebug());

            Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(shouldResume)}", $"{shouldResume}",
                Good: shouldResume, Indent: 1, Toggle: getDoDebug());

            if (shouldResume)
            {
                Debug.CheckYeh(4, $"vaultSkill.AutoActSetting", vaultSkill.AutoActSetting, Indent: 1, Toggle: getDoDebug());
                AutoAct.ResumeSetting = vaultSkill.AutoActSetting;
                Debug.CheckYeh(4, $"AutoAct.ResumeSetting", AutoAct.ResumeSetting, Indent: 1, Toggle: getDoDebug());
                Debug.Entry(4, $"Resuming...", Indent: 1, Toggle: getDoDebug());
                AutoAct.Resume();
                Debug.CheckYeh(4, $"AutoAct.Setting", AutoAct.Setting, Indent: 1, Toggle: getDoDebug());
                The.ActionManager.SkipPlayerTurn = true;
                Debug.LoopItem(4, $"The.ActionManager.SkipPlayerTurn", $"{The.ActionManager.SkipPlayerTurn}", 
                    Good: The.ActionManager.SkipPlayerTurn, Indent: 1, Toggle: getDoDebug());
                try
                {
                    Debug.Entry(4, $"The.ActionManager.RunSegment()", Indent: 1, Toggle: getDoDebug());
                    The.ActionManager.RunSegment();
                }
                catch (NullReferenceException)
                {
                    Debug.Warn(4,
                        $"{nameof(Tactics_Vault)}",
                        $"{nameof(ResumeAfterVault)}()",
                        $"Call to The.ActionManager.RunSegment() failed with {nameof(NullReferenceException)}", 
                        Indent: 0);
                }
                vaultSkill.Clear().Vomit(4, nameof(ResumeAfterVault), $"End of Method (Success)", Indent: 1, Toggle: getDoDebug());

                Debug.Entry(4,
                $"x {nameof(Tactics_Vault)} "
                + $"{nameof(ResumeAfterVault)}"
                + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
                Indent: 0, Toggle: getDoDebug());

                return true;
            }
            vaultSkill.Clear().Vomit(4, nameof(ResumeAfterVault), $"End of Method (Failure)", Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4,
            $"x {nameof(Tactics_Vault)} "
            + $"{nameof(ResumeAfterVault)}"
            + $"(Vaulter: {Vaulter?.DebugName ?? NULL}) *//",
            Indent: 0, Toggle: getDoDebug());
            return false;
        }
        public bool ResumeAfterVault()
        {
            return ResumeAfterVault(ParentObject);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(COMMAND_TOGGLE);
            Registrar.Register(BEGIN_ATTACK_EVENT);
            Registrar.Register(ObjectLeavingCellEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(EnteredCellEvent.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (DebugVaultDescriptions && ID == GetShortDescriptionEvent.ID)
                || ID == BeforeAbilityManagerOpenEvent.ID
                || ID == GetMovementCapabilitiesEvent.ID
                || ID == CommandEvent.ID
                || ID == GetItemElementsEvent.ID
                || ID == PooledEvent<ShouldAttackToReachTargetEvent>.ID
                || ID == PooledEvent<PathAsBurrowerEvent>.ID;
        }
        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            DescribeMyActivatedAbility(ActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (The.Player != null && ParentObject.CurrentZone == The.ZoneManager.ActiveZone)
            {
                bool haveOrigin = Origin != null;
                bool haveOver = Over != null;
                bool haveDestination = Destination != null;
                bool haveAutoActSetting = !AutoActSetting.IsNullOrEmpty();

                StringBuilder SB = Event.NewStringBuilder();
                SB.AppendColored("M", $"Vaulter").Append("[").AppendColored("g", $"{ParentObject?.CurrentCell?.Location}").Append("]").Append(": ")
                    .AppendLine()
                    .AppendColored("W", $"Cells and Vault State").AppendLine()
                    .Append(VANDR).Append($"[{MidVault.YehNah(true)}]{HONLY}MidVault: ").AppendColored("B", $"{MidVault}").AppendLine()
                    .Append(VANDR).Append($"[{haveOrigin.YehNah(!MidVault)}]{HONLY}Origin: [").AppendColored("g", $"{Origin?.Location}").Append($"]").AppendLine()
                    .Append(VANDR).Append($"[{haveOver.YehNah(!MidVault)}]{HONLY}Over: [").AppendColored("g", $"{Over?.Location}").Append($"]").AppendLine()
                    .Append(VANDR).Append($"[{haveDestination.YehNah(!MidVault)}]{HONLY}Destination: [").AppendColored("g", $"{Destination?.Location}").Append($"]").AppendLine()
                    .Append(TANDR).Append($"[{Vaulted.YehNah(!MidVault)}]{HONLY}Vaulted: ").AppendColored("B", $"{Vaulted}").AppendLine()
                    .AppendColored("W", $"AutoAct State").AppendLine()
                    .Append(VANDR).Append($"[{WantToVault.YehNah()}]{HONLY}WantToVault: ").AppendColored("B", $"{WantToVault}").AppendLine()
                    .Append(VANDR).Append($"[{WasAutoActing.YehNah(WantToVault)}]{HONLY}WasAutoActing: ").AppendColored("B", $"{WasAutoActing}").AppendLine()
                    .Append(VANDR).Append($"[{haveAutoActSetting.YehNah(WantToVault)}]{HONLY}AutoActSetting: ").AppendColored("o", $"{AutoActSetting?.Quote() ?? "null".Color("B")}").AppendLine()
                    .Append(TANDR).Append($"[{IsBurrowerWantsToVault.YehNah(!WantToVault)}]{HONLY}IsBurrowerWantsToVault: ").AppendColored("B", $"{IsBurrowerWantsToVault}").AppendLine();
                    
                E.Infix.AppendLine().AppendRules(Event.FinalizeString(SB));
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetMovementCapabilitiesEvent E)
        {
            E.Add("Vault over square", COMMAND_NAME, 7000);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CommandEvent E)
        {
            if (E.Command == COMMAND_NAME && E.Actor == ParentObject)
            {
                if (AttemptVault(E.Target, E.TargetCell, E, E.Silent))
                {
                    ParentObject.UseEnergy(1000, "Movement Vault", null, ParentObject.GetStat("MoveSpeed").BaseValue);
                }
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
        public override bool HandleEvent(ObjectLeavingCellEvent E)
        {
            if (WantToVault && ParentObject != null && E.Actor == ParentObject && E.Cell != null && E.Cell == ParentObject.CurrentCell)
            {
                GameObject Vaulter = ParentObject;
                GameObject Vaultee = null;

                Cell Origin = E.Cell;
                Cell Over = Origin.GetCellFromDirection(E.Direction);
                Cell Destination = null;

                Vaultee = Over.GetFirstObjectWithPart(nameof(Vaultable));

                if (Vaultee != null)
                {
                    Destination = GetValidDestinationCell(Vaulter, Over);

                    Debug.Entry(4,
                    $"@ {nameof(Tactics_Vault)}."
                    + $"{nameof(HandleEvent)}({nameof(ObjectLeavingCellEvent)} E)"
                    + $" E.Cell: [{Origin?.Location}],"
                    + $" E.Actor: {Vaulter?.DebugName ?? NULL}",
                    Indent: 0, Toggle: getDoDebug());

                    bool cellIsSolidForVaulter = Over.IsSolidFor(Vaulter);
                    bool notPlayer = !Vaulter.IsPlayer();
                    bool autoActActive = AutoAct.IsActive();
                    bool actingAutomatically = notPlayer || autoActActive;

                    bool shouldVault =
                        cellIsSolidForVaulter
                     && !Vaulted
                     && WantToVault
                     && actingAutomatically;

                    Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(cellIsSolidForVaulter)}", $"{cellIsSolidForVaulter}",
                        Good: cellIsSolidForVaulter, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"not {nameof(Vaulted)}", $"{!Vaulted}",
                        Good: !Vaulted, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(WantToVault)}", $"{WantToVault}",
                        Good: WantToVault, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(actingAutomatically)}", $"{actingAutomatically}",
                        Good: actingAutomatically, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(notPlayer)}", $"{notPlayer}",
                        Good: notPlayer, Indent: 2, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(autoActActive)}", $"{autoActActive}",
                        Good: autoActActive, Indent: 2, Toggle: getDoDebug());

                    Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(shouldVault)}", $"{shouldVault}",
                        Good: shouldVault, Indent: 1, Toggle: getDoDebug());

                    if (shouldVault && CanVault(Vaultee, Silent: false))
                    {
                        Debug.Entry(4, $"Attempted to enter cell [{Over?.Location}] with vaultee {Vaultee?.DebugName ?? NULL}", 
                            Indent: 1, Toggle: getDoDebug());

                        Debug.Entry(4, $"Attempting Vault into Destination cell [{Destination?.Location}]", 
                            Indent: 1, Toggle: getDoDebug());

                        Debug.LoopItem(4, $"Vaulter", $"{Vaulter?.DebugName ?? NULL}", Good: Vaulter != null,
                            Indent: 2, Toggle: getDoDebug());
                        Debug.LoopItem(4, $"Vaultee", $"{Vaultee?.DebugName ?? NULL}", Good: Vaultee != null,
                            Indent: 2, Toggle: getDoDebug());
                        Debug.LoopItem(4, $"Origin", $"[{Origin?.Location}]", Good: Origin != null,
                            Indent: 2, Toggle: getDoDebug());
                        Debug.LoopItem(4, $"Over", $"[{Over?.Location}]", Good: Over != null,
                            Indent: 2, Toggle: getDoDebug());
                        Debug.LoopItem(4, $"Destination", $"[{Destination?.Location}]", Good: Destination != null,
                            Indent: 2, Toggle: getDoDebug());

                        if (AttemptVault(Vaulter, Origin, Vaultee, Destination, E, Silent: false))
                        {
                            Debug.Entry(4, $"Vault successful", Indent: 1, Toggle: getDoDebug());
                            Debug.Entry(4,
                                $"x {nameof(Tactics_Vault)}."
                                + $"{nameof(HandleEvent)}({nameof(ObjectLeavingCellEvent)} E)"
                                + $" E.Cell: [{Origin?.Location}],"
                                + $" E.Actor: {Vaulter?.DebugName ?? NULL} @//",
                                Indent: 0, Toggle: getDoDebug());
                            return true;
                        }
                    }
                    Debug.Entry(4, $"Shouldn't Vault or Can't Vault", Indent: 1, Toggle: getDoDebug());
                    Debug.Entry(4,
                        $"x {nameof(Tactics_Vault)}."
                        + $"{nameof(HandleEvent)}({nameof(ObjectLeavingCellEvent)} E)"
                        + $" E.Cell: [{Origin?.Location}],"
                        + $" E.Actor: {Vaulter?.DebugName ?? NULL} @//",
                        Indent: 0, Toggle: getDoDebug());

                } // if (Vaultee != null)
            } // if (E.Actor == ParentObject && ParentObject != null && WantToVault)
            if (false && E.Cell.InActiveZone && IsBurrowerWantsToVault)
            {
                GameObject Vaulter = ParentObject;

                Debug.Entry(4,
                $"@ {nameof(Tactics_Vault)}."
                + $"{nameof(HandleEvent)}({nameof(ObjectLeavingCellEvent)} E)"
                + $" E.Cell: [{E.Cell?.Location}],"
                + $" E.Actor: {Vaulter?.DebugName ?? NULL}",
                Indent: 0, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(IsBurrowerWantsToVault)}", $"{IsBurrowerWantsToVault}",
                    Good: IsBurrowerWantsToVault, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Checking adjacent cells for wasDiggable vaultables...", Indent: 1, Toggle: getDoDebug());
                foreach (Cell cell in E.Cell.GetAdjacentCells())
                {
                    if (cell != null && cell.HasDiggableVaultableObject())
                    {
                        Debug.CheckYeh(4, $"Cell [{cell.Location}] contains a wasDiggable vaultable", Indent: 1, Toggle: getDoDebug());
                        foreach (GameObject vaultable in cell.GetObjectsWithPart(nameof(Vaultable)))
                        {
                            bool vaultableWasDiggable = vaultable.HasIntProperty(WAS_DIGGABLE);
                            bool vaulterCanVaultVaultable = CanVault(vaultable);
                            bool vaultableCellPairsContainsVaulter = vaultable.GetPart<Vaultable>().GetVaultableCellPairs(Vaulter).ContainsKey(E.Cell);
                            bool swapDiggable =
                                vaultableWasDiggable && vaulterCanVaultVaultable && vaultableCellPairsContainsVaulter;

                            Debug.LoopItem(4, $"vaultableWasDiggable", $"{vaultableWasDiggable}",
                                Good: vaultableWasDiggable, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"vaulterCanVaultVaultable", $"{vaulterCanVaultVaultable}",
                                Good: vaulterCanVaultVaultable, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"vaultableCellPairsContainsVaulter", $"{vaultableCellPairsContainsVaulter}",
                                Good: vaultableCellPairsContainsVaulter, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"swapDiggable", $"{swapDiggable}",
                                Good: swapDiggable, Indent: 1, Toggle: getDoDebug());
                            Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

                            if (swapDiggable)
                            {
                                vaultable.SetIntProperty(DIGGABLE, 1);
                                vaultable.SetIntProperty(WAS_DIGGABLE, 0, true);

                                Debug.CheckYeh(4, $"vaultable {vaultable?.DebugName ?? NULL}:", Indent: 2, Toggle: getDoDebug());

                                Debug.LoopItem(4, $"Diggable".Quote(), $"{vaultable.HasIntProperty(DIGGABLE)}",
                                    Good: vaultable.HasIntProperty(DIGGABLE), Indent: 3, Toggle: getDoDebug());

                                Debug.LoopItem(4, $"WasDiggable".Quote(), $"{vaultable.HasIntProperty(WAS_DIGGABLE)}",
                                    Good: !vaultable.HasIntProperty(WAS_DIGGABLE), Indent: 3, Toggle: getDoDebug());

                                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());
                            }
                        }
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Cell [{cell.Location}] contains no wasDiggable vaultables", Indent: 1, Toggle: getDoDebug());
                    }
                }
            }
            if (E.Blocking != null)
            {
                Debug.Entry(4, $"BLOCKED: E.Cell: [{E.Cell.Location}], E.Blocking in [{E.Blocking.CurrentCell.Location}]", 
                    Indent: 1, Toggle: getDoDebug());
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EnteredCellEvent E)
        {
            if (false && E.Cell.InActiveZone && E.Actor == ParentObject && IsBurrowerWantsToVault && (!ParentObject.IsPlayer() || AutoAct.IsAnyMovement()))
            {
                GameObject Vaulter = E.Actor;

                Debug.Entry(4,
                $"@ {nameof(Tactics_Vault)}."
                + $"{nameof(HandleEvent)}({nameof(EnteredCellEvent)} E)"
                + $" E.Cell: [{E.Cell?.Location}],"
                + $" E.Actor: {Vaulter?.DebugName ?? NULL}",
                Indent: 0, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(IsBurrowerWantsToVault)}", $"{IsBurrowerWantsToVault}",
                    Good: IsBurrowerWantsToVault, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"NotPlayer", $"{!ParentObject.IsPlayer()}",
                    Good: !ParentObject.IsPlayer(), Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"AutoAct.IsAnyMovement", $"{AutoAct.IsAnyMovement()}",
                    Good: AutoAct.IsAnyMovement(), Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"Checking adjacent cells for diggable vaultables...", Indent: 1, Toggle: getDoDebug());
                foreach (Cell cell in E.Cell.GetAdjacentCells())
                {
                    if (cell != null && cell.HasDiggableVaultableObject())
                    {
                        Debug.CheckYeh(4, $"Cell [{cell.Location}] contains a diggable vaultable", Indent: 1, Toggle: getDoDebug());
                        foreach (GameObject vaultable in cell.GetObjectsWithPart(nameof(Vaultable)))
                        {
                            bool vaultableIsDiggable = vaultable.HasIntProperty(DIGGABLE);
                            bool vaulterCanVaultVaultable = CanVault(vaultable);
                            bool vaultableCellPairsContainsVaulter = vaultable.GetPart<Vaultable>().GetVaultableCellPairs(Vaulter).ContainsKey(E.Cell);
                            bool swapDiggable =
                                vaultableIsDiggable && vaulterCanVaultVaultable && vaultableCellPairsContainsVaulter;

                            Debug.LoopItem(4, $"vaultableIsDiggable", $"{vaultableIsDiggable}",
                                Good: vaultableIsDiggable, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"vaulterCanVaultVaultable", $"{vaulterCanVaultVaultable}",
                                Good: vaulterCanVaultVaultable, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"vaultableCellPairsContainsVaulter", $"{vaultableCellPairsContainsVaulter}",
                                Good: vaultableCellPairsContainsVaulter, Indent: 2, Toggle: getDoDebug());

                            Debug.LoopItem(4, $"swapDiggable", $"{swapDiggable}",
                                Good: swapDiggable, Indent: 1, Toggle: getDoDebug());
                            Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());

                            if (swapDiggable)
                            {
                                vaultable.SetIntProperty(WAS_DIGGABLE, 1);
                                vaultable.SetIntProperty(DIGGABLE, 0, true);

                                Debug.CheckYeh(4, $"vaultable {vaultable?.DebugName ?? NULL}:", Indent: 2, Toggle: getDoDebug());

                                Debug.LoopItem(4, $"Diggable".Quote(), $"{vaultable.HasIntProperty(DIGGABLE)}",
                                    Good: !vaultable.HasIntProperty(DIGGABLE), Indent: 3, Toggle: getDoDebug());

                                Debug.LoopItem(4, $"WasDiggable".Quote(), $"{vaultable.HasIntProperty(WAS_DIGGABLE)}",
                                    Good: vaultable.HasIntProperty(WAS_DIGGABLE), Indent: 3, Toggle: getDoDebug());

                                Debug.Divider(4, HONLY, Count: 30, Indent: 1, Toggle: getDoDebug());
                            }
                        }
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Cell [{cell.Location}] contains no diggable vaultables", Indent: 1, Toggle: getDoDebug());
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(PathAsBurrowerEvent E)
        {
            if (E.Object != null && E.Object == ParentObject && IsBurrowerWantsToVault && (!ParentObject.IsPlayer() || AutoAct.IsAnyMovement()))
            {
                GameObject Vaulter = E.Object;
                Cell vaulterCell = Vaulter.CurrentCell;

                Debug.Entry(4,
                $"@ {nameof(Tactics_Vault)}."
                + $"{nameof(HandleEvent)}({nameof(PathAsBurrowerEvent)} E)"
                + $" E.Actor: {Vaulter?.DebugName ?? NULL}",
                Indent: 0, Toggle: getDoDebug());

                bool vaulterNotNull = Vaulter != null;

                bool vaulterNotPlayer =
                    vaulterNotNull
                    && !Vaulter.IsPlayer();

                bool autoActActive = AutoAct.IsActive();

                bool actingAutomatically = vaulterNotPlayer || autoActActive;

                GameObject Vaultee = null;

                foreach (Cell cell in Vaulter.CurrentCell.GetAdjacentCells())
                {
                    if (cell != null && cell.HasDiggableVaultableObject())
                    {
                        foreach (GameObject vaultee in cell.GetObjectsWithPart(nameof(Vaultable)))
                        {
                            if (vaultee.HasPropertyOrTag(DIGGABLE))
                            {
                                Vaultee = vaultee;
                                break;
                            }
                        }
                    }
                }

                bool vaulteeNotNull = Vaultee != null;

                Vaultable vaultable = null;

                bool vaulteeHasVaultable = vaulteeNotNull && Vaultee.TryGetPart(out vaultable);

                Dictionary<Cell, Cell> originDestinationPairs = vaultable?.GetVaultableCellPairs(Vaulter);

                bool vaulteeHasValidCellPair = !originDestinationPairs.IsNullOrEmpty();

                bool vaultableCellPairsContainsVaulter =
                    vaulteeHasValidCellPair
                    && vaulterNotNull
                    && originDestinationPairs.ContainsKey(Vaulter.CurrentCell);

                bool vaulterCanVaultVaultee = CanVault(Vaultee, Silent: true);

                bool shouldNotPathAsBurrower =
                    vaulterNotNull
                 && IsBurrowerWantsToVault
                 && actingAutomatically
                 && vaulteeNotNull
                 && vaultableCellPairsContainsVaulter
                 && vaulterCanVaultVaultee;

                Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}",
                    Good: vaulterNotNull, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(IsBurrowerWantsToVault)}", $"{IsBurrowerWantsToVault}",
                    Good: IsBurrowerWantsToVault, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(actingAutomatically)}", $"{actingAutomatically}",
                    Good: actingAutomatically, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaulterNotPlayer)}", $"{vaulterNotPlayer}",
                    Good: vaulterNotPlayer, Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(autoActActive)}", $"{autoActActive}",
                    Good: autoActActive, Indent: 2, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaulteeNotNull)}", $"{vaulteeNotNull}",
                    Good: vaulteeNotNull, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaulteeHasVaultable)}", $"{vaulteeHasVaultable}",
                    Good: vaulteeHasVaultable, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaulteeHasValidCellPair)}", $"{vaulteeHasValidCellPair}",
                    Good: vaulteeHasValidCellPair, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaultableCellPairsContainsVaulter)}", $"{vaultableCellPairsContainsVaulter}",
                    Good: vaultableCellPairsContainsVaulter, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(vaulterCanVaultVaultee)}", $"{vaulterCanVaultVaultee}",
                    Good: vaulterCanVaultVaultee, Indent: 1, Toggle: getDoDebug());

                Debug.LoopItem(4, $"{nameof(shouldNotPathAsBurrower)}", $"{shouldNotPathAsBurrower}",
                    Good: shouldNotPathAsBurrower, Indent: 1, Toggle: getDoDebug());

                if (shouldNotPathAsBurrower)
                {
                    Debug.Entry(4, $"Stopping {Vaulter?.DebugName ?? NULL} from pathing as burrower",
                        Indent: 1, Toggle: getDoDebug());

                    return true; // this is a return !flag one so true gives a !true result.
                }
            }

            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ShouldAttackToReachTargetEvent E)
        {
            GameObject Vaulter = E.Actor;
            GameObject Vaultee = E.Object;

            Debug.Entry(4,
                $"@ {nameof(Tactics_Vault)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ShouldAttackToReachTargetEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            Debug.Entry(4, $"E.Actor", $"{E.Actor?.DebugName ?? NULL}",
                Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"E.Object", $"{E.Object?.DebugName ?? NULL}",
                Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, $"E.Target", $"{E.Target?.DebugName ?? NULL}",
                Indent: 1, Toggle: getDoDebug());

            bool vaulterNotNull = Vaulter != null && ParentObject == E.Actor;

            bool vaulteeNotNull = Vaultee != null;

            bool vaulterMovingAutomatically = (!Vaulter.IsPlayer() || AutoAct.IsAnyMovement());

            bool targetNotVaultee = vaulteeNotNull && E.Target != Vaultee;

            Vaultable vaultable = null;
            bool vaulteeIsVautable = vaulteeNotNull && Vaultee.TryGetPart(out vaultable);

            Dictionary<Cell, Cell> vaultableCellPairs = new();

            if (vaulteeNotNull)
            {
                vaultableCellPairs = vaultable?.GetVaultableCellPairs();
            }

            bool vaulteeHasValidVaultLocations = vaulteeNotNull && !vaultableCellPairs.IsNullOrEmpty();

            bool vaulterIsInValidVaultLocation =
                vaulteeHasValidVaultLocations
             && (vaultableCellPairs.ContainsKey(Vaulter.CurrentCell)
                || vaultableCellPairs.ContainsValue(Vaulter.CurrentCell)
                || GetValidDestinationCell(Vaulter, Vaultee.CurrentCell) != null);

            bool vaulterCanVaultVaultee =
                vaulterNotNull
             && CanVault(Vaultee, Silent: true);

            bool shouldNotAttack =
                vaulterNotNull
             && vaulterMovingAutomatically
             && targetNotVaultee
             && vaulteeIsVautable
             && vaulteeHasValidVaultLocations
             && vaulterIsInValidVaultLocation
             && vaulterCanVaultVaultee
             && IsBurrowerWantsToVault;

            Debug.LoopItem(4, $"Determining whether should not attack...",
                Indent: 1, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulterNotNull)}", $"{vaulterNotNull}",
                Good: vaulterNotNull, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulterMovingAutomatically)}", $"{vaulterMovingAutomatically}",
                Good: vaulterMovingAutomatically, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(targetNotVaultee)}", $"{targetNotVaultee}",
                Good: targetNotVaultee, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulteeIsVautable)}", $"{vaulteeIsVautable}",
                Good: vaulteeIsVautable, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulteeHasValidVaultLocations)}", $"{vaulteeHasValidVaultLocations}",
                Good: vaulteeHasValidVaultLocations, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulterIsInValidVaultLocation)}", $"{vaulterIsInValidVaultLocation}",
                Good: vaulterIsInValidVaultLocation, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(vaulterCanVaultVaultee)}", $"{vaulterCanVaultVaultee}",
                Good: vaulterCanVaultVaultee, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(IsBurrowerWantsToVault)}", $"{IsBurrowerWantsToVault}",
                Good: IsBurrowerWantsToVault, Indent: 2, Toggle: getDoDebug());

            Debug.LoopItem(4, $"{nameof(shouldNotAttack)}", $"{shouldNotAttack}",
                Good: shouldNotAttack, Indent: 1, Toggle: getDoDebug());

            if (shouldNotAttack)
            {
                E.ShouldAttack = false;

                Debug.LoopItem(4, $"E.ShouldAttack", $"{E.ShouldAttack}",
                    Good: !E.ShouldAttack, Indent: 1, Toggle: getDoDebug());

                Debug.Entry(4,
                    $"x {nameof(Tactics_Vault)}."
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(ShouldAttackToReachTargetEvent)} E) @//",
                    Indent: 0, Toggle: getDoDebug());

                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == COMMAND_TOGGLE)
            {
                ToggleMyActivatedAbility(ActivatedAbilityID, null, Silent: true, null);
                Debug.CheckYeh(3, 
                    $"{nameof(Tactics_Vault)} Toggled", 
                    $"{WantToVault}", 
                    Indent: 0, Toggle: getDoDebug());
            }
            if (false && E.ID == BEGIN_ATTACK_EVENT && WantToVault && ParentObject != null && ParentObject.IsPlayer() && AutoAct.IsAnyMovement())
            {
                Debug.Entry(4,
                    $"@ {nameof(Tactics_Vault)}."
                    + $"{nameof(FireEvent)}({nameof(Event)} E.ID: {BEGIN_ATTACK_EVENT})",
                    Indent: 0, Toggle: getDoDebug());

                Cell targetCell = E.GetParameter<Cell>("TargetCell");
                if (!AutoAct.TryToMove(ParentObject, ParentObject.CurrentCell, targetCell, AllowDigging: false))
                {
                    return false;
                }
            }
            return base.FireEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            Tactics_Vault tactics_Vault = base.DeepCopy(Parent, MapInv) as Tactics_Vault;
            tactics_Vault.Clear();
            return tactics_Vault;
        }

    } //!-- public class Tactics_Vault : BaseSkill
}
