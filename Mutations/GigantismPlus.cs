using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using XRL.UI;
using XRL.Core;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Skill;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GigantismPlus 
        : BaseManagedDefaultEquipmentMutation<GigantismPlus>
        , IModEventHandler<BeforeVaultEvent>
        , IModEventHandler<VaultedEvent>
    {
        public static readonly int ICON_COLOR_PRIORITY = 81;
        public static readonly string ICON_COLOR = "&z";
        public static readonly string ICON_COLOR_FALLBACK = "&w";

        private bool MutationColor => UI.Options.MutationColor;

        public float WeightFactor = 5.0f;
        public float CarryCapFactor => 2.0f;
        public int CarryCapBonus;

        private static int MaxDamageDieIncrease => 7;
        private static int MinDamageBonusIncrease => 3;

        public bool IsVehicleCreature => ParentObject.HasPart(typeof(Vehicle));

        public bool IsGiganticCreature // basically a wrapper but forces you to not be PseudoGigantic at the same time 
        {
            get
            {
                return (ParentObject?.IsGiganticCreature) != null && ParentObject.IsGiganticCreature;
            }
            private set
            {
                if (ParentObject != null) ParentObject.IsGiganticCreature = value;
                if (IsPseudoGiganticCreature == value) IsPseudoGiganticCreature = !value;
            }
        }
        public bool IsPseudoGiganticCreature // ensures you aren't (typically) Gigantic and PseudoGigantic at the same time 
        {
            get
            {
                return ParentObject?.HasPart<PseudoGigantism>() != null && ParentObject.HasPart<PseudoGigantism>();
            }
            set
            {
                if (ParentObject != null && value) ParentObject.RequirePart<PseudoGigantism>();
                else ParentObject?.RemovePart<PseudoGigantism>();


                if (IsGiganticCreature == value) IsGiganticCreature = !value;
            }
        }

        public bool IsCyberGiant => GiganticExoframe != null;

        private CyberneticsGiganticExoframe _giganticExoframe;
        public CyberneticsGiganticExoframe GiganticExoframe
        {
            get => _giganticExoframe ??= ParentObject?.Body?.GetBody().Cybernetics?.GetPart<CyberneticsGiganticExoframe>();
            set => _giganticExoframe = value;
        }

        public Guid HunchOverActivatedAbilityID = Guid.Empty;
        public Guid GroundPoundActivatedAbilityID = Guid.Empty;
        public Guid CloseFistActivatedAbilityID = Guid.Empty;

        public static readonly string COMMAND_NAME_HUNCH_OVER = "CommandToggleGigantismPlusHunchOver";
        public static readonly string COMMAND_NAME_GROUND_POUND = "CommandToggleGigantismPlusGroundPound";
        public static readonly string COMMAND_NAME_CLOSE_FIST = "CommandToggleGigantismPlusCloseFist";

        public int HunchedOverAVModifier = 4;
        public int HunchedOverDVModifier = -6;
        public int HunchedOverQNModifier = -60;
        public int HunchedOverMSModifier = -60;
        
        private string HunchedOverAbilityHunched => !IsCyberGiant ? "Hunched Over" : "Compact Mode";
        // private string HunchedOverAbilityHunched => !IsCyberGiant ? "Hunched" : "Compact";
        // private string HunchedOverAbilityUpright => !IsCyberGiant ? "Upright" : "Regular";

        public bool UnHunchImmediately = false;

        public bool IsHunchFree = false;
        private int _hunchOverEnergyCost = 500;
        public int HunchOverEnergyCost
        {
            get
            {
                if (IsHunchFree)
                {
                    IsHunchFree = false;
                    return 0;
                }
                return _hunchOverEnergyCost;
            }
            private set
            {
                _hunchOverEnergyCost = value;
            }
        }

        [SerializeField]
        private int AppliedJumpRangeBonus = 0;

        private double _stunningForceLevelFactor = 0.5;

        public int StunningForceDistance = 3;

        public GigantismPlus()
        {
            DisplayName = "{{gigantic|Gigantism}} ({{r|D}})";
            Type = "Physical";

            ModGiganticNaturalWeapon GiganticFist = new()
            {
                AssigningPart = this,
                BodyPartType = "Hand",

                ModPriority = 10,
                DescriptionPriority = 10,

                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                AdjectiveColorFallback = "w",

                Adjustments = new(),

                AddedIntProps = new()
                {
                    { "ModGiganticNoShortDescription", 1 },
                    { "ModGiganticNoDisplayName", 1 }
                },
                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/cudgels/sfx_melee_cudgel_fistOfTheApeGod_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_cudgel_fistOfTheApeGod_block" }
                },
            };
            GiganticFist.AddAdjustment(MELEEWEAPON, "Skill", "Cudgel", true);
            GiganticFist.AddAdjustment(MELEEWEAPON, "Stat", "Strength", true);

            GiganticFist.AddAdjustment(RENDER, "DisplayName", "fist", true);

            GiganticFist.AddAdjustment(RENDER, "Tile", "NaturalWeapons/GiganticFist.png", true);
            GiganticFist.AddAdjustment(RENDER, "ColorString", "&x", true);
            GiganticFist.AddAdjustment(RENDER, "TileColor", "&x", true);
            GiganticFist.AddAdjustment(RENDER, "DetailColor", "z", true);

            NaturalEquipmentMods.Add(GiganticFist.BodyPartType, GiganticFist); 
            
            ModGiganticNaturalWeapon GiganticNoggin = new()
            {
                AssigningPart = this,
                BodyPartType = "Head",

                ModPriority = 10,
                DescriptionPriority = 10,

                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                AdjectiveColorFallback = "w",

                Adjustments = new(),

                AddedIntProps = new()
                {
                    { "ModGiganticNoShortDescription", 1 },
                    { "ModGiganticNoDisplayName", 1 }
                },
            };
            GiganticNoggin.AddAdjustment(MELEEWEAPON, "Stat", "Strength", true);

            GiganticNoggin.AddAdjustment(RENDER, "ColorString", "&x", true);
            GiganticNoggin.AddAdjustment(RENDER, "TileColor", "&x", true);
            GiganticNoggin.AddAdjustment(RENDER, "DetailColor", "z", true);

            NaturalEquipmentMods.Add(GiganticNoggin.BodyPartType, GiganticNoggin); 
            
            ModGiganticNaturalWeapon GiganticMug = new()
            {
                AssigningPart = this,
                BodyPartType = "Face",

                ModPriority = 10,
                DescriptionPriority = 10,

                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                AdjectiveColorFallback = "w",

                Adjustments = new(),

                AddedIntProps = new()
                {
                    { "ModGiganticNoShortDescription", 1 },
                    { "ModGiganticNoDisplayName", 1 }
                },
            };
            GiganticNoggin.AddAdjustment(MELEEWEAPON, "Stat", "Strength", true);

            GiganticNoggin.AddAdjustment(RENDER, "ColorString", "&x", true);
            GiganticNoggin.AddAdjustment(RENDER, "TileColor", "&x", true);
            GiganticNoggin.AddAdjustment(RENDER, "DetailColor", "z", true);

            NaturalEquipmentMods.Add(GiganticMug.BodyPartType, GiganticMug);

            ModNaturalEquipment<GigantismPlus> GiganticBod = new()
            {
                AssigningPart = this,
                BodyPartType = "Body",

                ModPriority = 10,
                DescriptionPriority = 10,

                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                AdjectiveColorFallback = "w",

                Adjustments = new(),

                AddedIntProps = new()
                {
                    { "ModGiganticNoShortDescription", 1 },
                    { "ModGiganticNoDisplayName", 1 }
                },
            };
            GiganticBod.AddAdjustment(RENDER, "ColorString", "&x", true);
            GiganticBod.AddAdjustment(RENDER, "TileColor", "&x", true);
            GiganticBod.AddAdjustment(RENDER, "DetailColor", "z", true);

            NaturalEquipmentMods.Add(GiganticBod.BodyPartType, GiganticBod);
        }

        public override bool CanLevel() { return true; }

        public override bool GeneratesEquipment() { return true; }

        public static float GetWeightFactor(int level)
        {
            return 4.75f + (0.25f * level);
        }
        public static int GetCarryCapBonus(int level)
        {
            return 8 * level;
        }

        public override int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<GigantismPlus> NaturalEquipmentMod, int Level = 1)
        {
            if (NaturalEquipmentMod.Adjective == "closed") return 0;
            if (NaturalEquipmentMod.BodyPartType == "Head") return 2;
            if (NaturalEquipmentMod.BodyPartType == "Hand") return (int)Math.Min(1 + Math.Floor(Level / 3.0), MaxDamageDieIncrease);
            return 0;
        }
        public override int GetNaturalWeaponDamageBonus(ModNaturalEquipment<GigantismPlus> NaturalEquipmentMod, int Level = 1)
        {
            if (NaturalEquipmentMod.Adjective == "closed") return 0;
            double perLevel = 3.0;
            int levelOffset = MinDamageBonusIncrease * (int)perLevel;
            if (NaturalEquipmentMod.BodyPartType == "Head") return 5;
            if (NaturalEquipmentMod.BodyPartType == "Hand") return (int)Math.Max(1, Math.Floor((Level - levelOffset) / perLevel));
            return 0;
        }
        public override int  GetNaturalWeaponHitBonus(ModNaturalEquipment<GigantismPlus> NaturalEquipmentMod, int Level = 1)
        {
            if (NaturalEquipmentMod.Adjective == "closed") return 0;
            if (NaturalEquipmentMod.BodyPartType == "Head") return 3;
            return -3 + (int)Math.Floor(Level / 2.0);
        }

        public static int GetHunchedOverAVModifier(int Level)
        {
            return 4;
        }
        public static int GetHunchedOverDVModifier(int Level)
        {
            return -6;
        }
        public static int GetHunchedOverQNModifier(int Level)
        {
            return Math.Min(-70 + (int)Math.Floor(Level * 10.0), -10);
        }
        public static int GetHunchedOverMSModifier(int Level)
        {
            return Math.Min(-70 + (int)Math.Floor(Level * 10.0), -10);
        }

        public int GetJumpRangeBonus(int Level)
        {
            return 1 + 
                GiganticExoframe?.JumpDistanceBonus != null 
                ? GiganticExoframe.JumpDistanceBonus
                : 0
                ;
        }
        public double GetStunningForceLevelFactor()
        {
            return _stunningForceLevelFactor =
                GiganticExoframe?.StunningForceLevelFactor == null
                ? _stunningForceLevelFactor
                : GiganticExoframe.StunningForceLevelFactor
                ;
        }
        public int GetStunningForceLevel(int Level)
        {
            return (int)Math.Max(Math.Floor(Level * GetStunningForceLevelFactor()), 1);
        }
        public int GetStunningForceDistance(int Level)
        {
            return StunningForceDistance;
        }

        public virtual bool ApplyStunningForceOnJump(GameObject GO = null, int Level = 0)
        {
            if (Level < 1)
                return false;

            GO ??= ParentObject;

            if (GO == null)
                return false;

            Debug.Entry(4, "Stunning Force", Indent: 1);
            // Stunning Force
            Debug.Entry(4, "? if (GO.TryGetPart(out StunningForceOnJump stunningForceOnJump))", Indent: 1);
            if (!GO.TryGetPart(out StunningForceOnJump stunning))
            {
                Debug.CheckNah(4, $"No StunningForceOnJump part", Indent: 2);
                GO.RequirePart<StunningForceOnJump>();
                Debug.Entry(4, $"GO.RequirePart<StunningForceOnJump>()", Indent: 3);
            }
            Debug.Entry(4, "x if (GO.TryGetPart(out StunningForceOnJump stunningForceOnJump)) ?//", Indent: 1);

            if (stunning != null)
            {
                Debug.CheckYeh(4, $"Have StunningForceOnJump part", Indent: 1);
                Debug.LoopItem(4, $"stunningForceOnJump.Level: {(stunning?.Level != null ? stunning?.Level : "null")}", Indent: 2);
                Debug.LoopItem(4, $"stunningForceOnJump.Distance: {(stunning?.Distance != null ? stunning?.Distance : "null")}", Indent: 2);
                stunning.Level = GetStunningForceLevel(Level); // Scale stunningForceOnJump force with mutation level
                stunning.Distance = StunningForceDistance;
                Debug.Entry(4, $"New values calculated and assigned", Indent: 1);
                Debug.LoopItem(4, $"stunningForceOnJump.Level: {stunning.Level}", Indent: 2);
                Debug.LoopItem(4, $"stunningForceOnJump.Distance: {stunning.Distance}", Indent: 2);
            }

            return true;
        }
        public virtual bool UnapplyStunningForceOnJump(GameObject GO = null)
        {
            GO ??= ParentObject;

            if (GO == null) 
                return false;

            Debug.LoopItem(4, "Removing StunningForceOnJump", Indent: 2);

            Debug.Entry(4, "? if (!GO.TryGetPart(out StunningForceOnJump stunningForceOnJump))", Indent: 2);
            if (!GO.TryGetPart(out StunningForceOnJump stunningForceOnJump))
            {
                Debug.CheckNah(4, "No StunningForceOnJump part", Indent: 3);
                Debug.Entry(4, "x if (GO.HasPart<StunningForceOnJump>()) ?//", Indent: 2);
                return false;
            }
            Debug.CheckYeh(4, "StunningForceOnJump part found", Indent: 3);
            Debug.LoopItem(4, $"Found StunningForceOnJump: [Level: {stunningForceOnJump.Level}, Distance: {stunningForceOnJump.Distance}]", Indent: 3);

            GO.RemovePart(stunningForceOnJump);
            Debug.LoopItem(4, $"StunningForceOnJump removed", Indent: 3);
            Debug.Entry(4, "x if (GO.HasPart<StunningForceOnJump>()) ?//", Indent: 2);

            return true;
        }

        public virtual bool ApplyJumpRangeBonus(GameObject GO = null, int Level = 0)
        {
            if (Level < 1) 
                return false;

            GO ??= ParentObject;

            if (GO == null)
                return false;

            Debug.Entry(4, "Jump Bonus", Indent: 1);
            // Jump Bonus
            Debug.Entry(4, "? if (AppliedJumpRangeBonus > 0 and UnapplyJumpRangeBonus())", Indent: 1);
            if (AppliedJumpRangeBonus > 0 && UnapplyJumpRangeBonus(GO))
            {
                Debug.Entry(4, $"JumpRangeModifier reduced to {GO.GetIntProperty(JUMP_RANGE_MODIFIER)}", Indent: 2);
            }
            else
            {
                Debug.CheckNah(4, $"AppliedJumpRangeBonus: {AppliedJumpRangeBonus}", Indent: 2);
            }
            Debug.Entry(4, "x if (AppliedJumpRangeBonus > 0 and UnapplyJumpRangeBonus()) ?//", Indent: 1);

            AppliedJumpRangeBonus = GetJumpRangeBonus(Level);

            Debug.Entry(4, $"Calculated new AppliedJumpRangeBonus: {AppliedJumpRangeBonus}", Indent: 1);
            Debug.Entry(4, $"JumpRangeModifier: {GO.GetIntProperty(JUMP_RANGE_MODIFIER)}", Indent: 1);

            GO.ModIntProperty(JUMP_RANGE_MODIFIER, AppliedJumpRangeBonus);

            Debug.Entry(4, $"JumpRangeModifier reduced to {GO.GetIntProperty(JUMP_RANGE_MODIFIER)}", Indent: 1);
            Acrobatics_Jump.SyncAbility(GO);

            return true;
        }
        public virtual bool UnapplyJumpRangeBonus(GameObject GO = null)
        {
            GO ??= ParentObject;

            if (GO == null)
                return false;

            Debug.CheckYeh(4, $"AppliedJumpRangeBonus: {AppliedJumpRangeBonus}", Indent: 2);
            Debug.Entry(4, $"JumpRangeModifier: {GO.GetIntProperty(JUMP_RANGE_MODIFIER)}", Indent: 2); 
            GO.ModIntProperty("JumpRangeModifier", -AppliedJumpRangeBonus, RemoveIfZero: true);
            AppliedJumpRangeBonus += -AppliedJumpRangeBonus;
            Acrobatics_Jump.SyncAbility(GO);
            Debug.LoopItem(4, "JumpRangeModifier reverted", Indent: 2);
            return AppliedJumpRangeBonus == 0;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            Debug.Header(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            // Straighten up if hunching.
            // Hunch over if hunched before level up.
            bool WasHunched = false;

            Debug.Entry(4, "? if (IsPseudoGiganticCreature and !IsVehicleCreature)", Indent: 1);
            if (IsPseudoGiganticCreature && !IsVehicleCreature)
            {
                Debug.CheckYeh(4, "Creature is PsuedoGigantic and not a Vehicle", Indent: 2);
                Debug.Entry(4, "Sending StraightenUp (silent)", Indent: 2);
                WasHunched = true;
                IsHunchFree = true;
                StraightenUp(Message: false);
            }
            else
            {
                Debug.LoopItem(4, $"IsPseudoGiganticCreature: {IsPseudoGiganticCreature}", Good: IsPseudoGiganticCreature, Indent: 2);
                Debug.LoopItem(4, $"!IsVehicleCreature: {!IsVehicleCreature}", Good: !IsVehicleCreature, Indent: 2);
            }
            Debug.Entry(4, "x if (IsPseudoGiganticCreature and !IsVehicleCreature) ?//", Indent: 1);

            Debug.Entry(4, "Start of Change Level updates", Indent: 1);
            // Start of Change Level updates.

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "Weight Factor and Carry Cap Bonus changes", Indent: 1);
            // Hunch Over Penalties
            Debug.Entry(4, $"Values Before", Indent: 2);
            Debug.Entry(4, $"WeightFactor: {WeightFactor}", Indent: 3);
            Debug.Entry(4, $"CarryCapBonus: {CarryCapBonus}", Indent: 3);
            WeightFactor = GetWeightFactor(NewLevel);
            CarryCapBonus = GetCarryCapBonus(NewLevel);
            Debug.Entry(4, $"Values After", Indent: 2);
            Debug.Entry(4, $"HunchedOverAVModifier: {HunchedOverAVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverDVModifier: {HunchedOverDVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverMSModifier: {HunchedOverMSModifier}", Indent: 3);

            Debug.Divider(4, "-", Count: 25, Indent: 1);

            ApplyJumpRangeBonus(ParentObject, NewLevel);

            Debug.Divider(4, "-", Count: 25, Indent: 1);

            if (IsMyActivatedAbilityToggledOn(GroundPoundActivatedAbilityID, ParentObject))
            {
                ApplyStunningForceOnJump(ParentObject, NewLevel);
            }

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "Hunch Over Penalties", Indent: 1);
            // Hunch Over Penalties
            Debug.Entry(4, $"Values Before", Indent: 2);
            Debug.Entry(4, $"HunchedOverAVModifier: {HunchedOverAVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverDVModifier: {HunchedOverDVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverMSModifier: {HunchedOverMSModifier}", Indent: 3);
            HunchedOverAVModifier = GetHunchedOverAVModifier(NewLevel);
            HunchedOverDVModifier = GetHunchedOverDVModifier(NewLevel);
            HunchedOverMSModifier = GetHunchedOverMSModifier(NewLevel);
            Debug.Entry(4, $"Values After", Indent: 2);
            Debug.Entry(4, $"HunchedOverAVModifier: {HunchedOverAVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverDVModifier: {HunchedOverDVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverMSModifier: {HunchedOverMSModifier}", Indent: 3);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "End of Change Level updates", Indent: 1);
            // End of Change Level updates
            Debug.Entry(4, "? if (WasHunched and !IsVehicleCreature)", Indent: 1);
            if (WasHunched && !IsVehicleCreature)
            {
                Debug.CheckYeh(4, "Creature was Hunched and not a Vehicle", Indent: 1);
                Debug.Entry(4, "Sending HunchOver (silent)", Indent: 1);
                IsHunchFree = true;
                HunchOver(Message: false);
            }
            else
            {
                Debug.LoopItem(4, $"WasHunched: {WasHunched}", Good: WasHunched, Indent: 2);
                Debug.LoopItem(4, $"!IsVehicleCreature: {!IsVehicleCreature}", Good: !IsVehicleCreature, Indent: 2);
            }
            Debug.Entry(4, "x if (WasHunched and !IsVehicleCreature) ?//", Indent: 1);

            Debug.Footer(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            return base.ChangeLevel(NewLevel);
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            // Currently unused but will comprise part of the stat-shifting for Hunch Over.
            string HunchedOverAV = GetHunchedOverAVModifier(Level).Signed();
            string HunchedOverDV = GetHunchedOverDVModifier(Level).Signed();
            string HunchedOverMS = GetHunchedOverMSModifier(Level).Signed();
            stats.Set("HunchedOverAV", HunchedOverAV);
            stats.Set("HunchedOverDV", HunchedOverDV);
            stats.Set("HunchedOverMS", HunchedOverMS);

            // Add stunning force on jump stats for the Ground Pound toggle.
        }

        public override string GetDescription()
        {
            string WeaponNoun = "fist";
            int stunningForceDistance = 3;
            double stunningForceLevelFactor = 0.5;
            if (ParentObject != null)
            {
                WeaponNoun = ParentObject?.Body?.GetFirstPart("Hand")?.DefaultBehavior?.GetObjectNoun() ?? WeaponNoun;
                stunningForceDistance = GetStunningForceDistance(Level);
                stunningForceLevelFactor = GetStunningForceLevelFactor();
            }

            string gigantismSource = (!IsCyberGiant) ? "unusually" : "cybernetically".Color("c");
            string exoframeName = string.Empty;
            if (IsCyberGiant) exoframeName = GiganticExoframe.ImplantObject.ShortDisplayName;

            string MaxDamageDie = $" (Max: {MaxDamageDieIncrease.Signed()})";
            string MinDamageBonus = $" (Min: {MinDamageBonusIncrease.Signed()})";

            StringBuilder SB = Event.NewStringBuilder();
            
            SB.Append($"You are {gigantismSource} large, ").Append("will ").AppendRule("struggle to enter small spaces")
                .Append(" without ").AppendColored("g", "hunching over").Append(", ").Append("and can typically ")
                .AppendRule("only").Append(" use ").AppendGigantic("gigantic").Append(" equipment.").AppendLine();

            SB.Append("You weigh ").AppendRule("5x").Append(" as much, ")
                .Append("can carry ").AppendRule("2x").Append(" as much weight, and ")
                .Append("all of your natural weapons are ").AppendGigantic("gigantic").Append(".")
                .AppendLine().AppendLine();

            SB.Append("You create a ").AppendRule("shockwave").Append(" where you land ")
                .Append("after jumping at least ").AppendRule($"{stunningForceDistance}").Append(" tiles.").AppendLine()
                .Append("Your shockwave's ").AppendRule("damage").Append(" and ").AppendRule("force")
                .Append(" increases every ").AppendRule($"{(int)(1 / stunningForceLevelFactor)} levels");
            if (IsCyberGiant) 
                SB.AppendLine().Append("This amount is being boosted by your ").Append(exoframeName);
            SB.AppendLine().AppendLine();

            SB.Append("Your ").AppendGigantic("gigantic").Append($" {WeaponNoun.Pluralize()} gain:").AppendLine()
                .AppendRule("1d").Append(" damage every ").AppendRule($"{3} levels").Append(MaxDamageDie).AppendLine()
                .AppendRule("+1").Append(" damage every ").AppendRule($"{3} levels").Append(MinDamageBonus).AppendLine()
                .AppendRule("+1").Append(" to hit every ").AppendRule($"{2} levels");
            

            return Event.FinalizeString(SB);
            
            /* Old mutation description kept for posterity.
             * 
               + "You are " + GigantismSource + " large, will {{rules|struggle to enter small spaces}} without {{g|hunching over}},"
               + "and can typically {{rules|only}} use {{gigantic|gigantic}} equipment.\n"
               + "You are {{rules|heavy}}, can carry {{rules|twice}} as much weight,"
               + "and all your natural weapons are {{gigantic|gigantic}}.\n\n"
               + "Your " + WeaponNoun + "s gain:\n"
               + "{{rules|+1}} To-Hit every {{rules|2 mutation levels}}\n"
               + "{{B|d1}} damage every {{B|3 mutation levels}}\n"
               + "{{W|1d}} damage every {{W|5 mutation levels}}\n"
               + "They have {{rules|uncapped penetration}}, but are harder {{rules|to hit}} with due to their size."
            */
        }

        public override string GetLevelText(int Level)
        {
            string MSPenalty;
            if (GetHunchedOverMSModifier(Level) >= 0)
            {
                MSPenalty = "No}} MS pentalty";
            }
            else
            {
                MSPenalty = GetHunchedOverMSModifier(Level) + "}} MS";
            }

            string WeaponNoun = "fist";
            int FistDamageDieCount = 1;
            int FistDamageBonus = 3;
            int FistHitBonus = -3;
            int StunningForceJumpLevel = 1;
            string stunningForceDamageIncrement = StunningForce.GetDamageIncrement(1);
            if (ParentObject != null)
            {
                ModNaturalEquipment<GigantismPlus> naturalEquipmentMod = NaturalEquipmentMods["Hand"];
                WeaponNoun = ParentObject?.Body?.GetFirstPart("Hand")?.DefaultBehavior?.Render?.DisplayName ?? WeaponNoun;
                FistDamageDieCount = GetNaturalWeaponDamageDieCount(naturalEquipmentMod, Level);
                FistDamageBonus = Math.Max(3, GetNaturalWeaponDamageBonus(naturalEquipmentMod, Level));
                FistHitBonus = GetNaturalWeaponHitBonus(naturalEquipmentMod, Level);
                StunningForceJumpLevel = GetStunningForceLevel(Level);
                stunningForceDamageIncrement = StunningForce.GetDamageIncrement(StunningForceJumpLevel);
            }
            StringBuilder SB = Event.NewStringBuilder();

            SB.Append($"Shockwave is Stunning Force (").AppendRule($"{StunningForceJumpLevel}").AppendLine(")")
                .Append($"Force damage increment: ").AppendRule($"{stunningForceDamageIncrement}").AppendLine();

            SB.AppendGigantic("Gigantic").Append($" modifier gives your natural {WeaponNoun} weapons:").AppendLine()
                .AppendRule($"{FistDamageDieCount.Signed()}").Append($" damage die count.").AppendLine()
                .AppendRule($"{FistDamageBonus.Signed()}").Append($" damage {FistDamageBonus.BonusOrPenalty()}.").AppendLine()
                .AppendRule($"{FistHitBonus.Signed()}").Append($" hit {FistHitBonus.BonusOrPenalty()}.");

            return Event.FinalizeString(SB);
            
            /* Hunch Over penalties.
             * 
               "When {{g|Hunched Over}}:\n"
               + "{{rules|" + GetHunchedOverQNModifier(Level) + "}} Quickness."
               + "{{rules|" + GetHunchedOverMSModifier(Level) + "}} Movespeed.";
            */
        }

        public virtual Guid AddActivatedAbilityGroundPound(GameObject GO, bool Force = false, bool Silent = false)
        {
            if (GO.HasSkill(nameof(Acrobatics_Jump)) || Force)
            {
                GroundPoundActivatedAbilityID = AddMyActivatedAbility(
                        Name: "Ground Pound",
                        Command: COMMAND_NAME_GROUND_POUND,
                        Class: "Physical Mutations",
                        Description: null,
                        Icon: "&#214",
                        DisabledMessage: null,
                        Toggleable: true,
                        DefaultToggleState: false,
                        ActiveToggle: true,
                        IsAttack: false,
                        IsRealityDistortionBased: false,
                        IsWorldMapUsable: false,
                        Silent: Silent
                        );
            }
            if (GroundPoundActivatedAbilityID != Guid.Empty)
            {
                AbilityToggledGroundPound(GO, ToggledOn: false);
            }
            return GroundPoundActivatedAbilityID;
        }
        public virtual bool RemoveActivatedAbilityGroundPound(GameObject GO, bool Force = false)
        {
            bool removed = false;
            if (!GO.HasSkill(nameof(Acrobatics_Jump)) || Force)
            {
                removed = RemoveMyActivatedAbility(ref GroundPoundActivatedAbilityID, GO);
            }
            return removed;
        }

        public virtual Guid AddActivatedAbilityCloseFist(GameObject GO, bool Force = false, bool Silent = false)
        {
            if ((GO.HasBodyPart("Hand", false) && CloseFistActivatedAbilityID == Guid.Empty) || Force)
            {
                CloseFistActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: "Close Fist",
                        Command: COMMAND_NAME_CLOSE_FIST,
                        Class: "Physical Mutations",
                        Description: null,
                        Icon: "&#214",
                        DisabledMessage: null,
                        Toggleable: true,
                        DefaultToggleState: false,
                        ActiveToggle: true,
                        IsAttack: false,
                        IsRealityDistortionBased: false,
                        IsWorldMapUsable: true,
                        Silent: Silent
                        );
            }
            return CloseFistActivatedAbilityID;
        }
        public virtual bool RemoveActivatedAbilityCloseFist(GameObject GO, bool Force = false)
        {
            bool removed = false;
            if ((!GO.HasBodyPart("Hand", false) && CloseFistActivatedAbilityID != Guid.Empty) || Force)
            {
                if (removed = RemoveMyActivatedAbility(ref CloseFistActivatedAbilityID, GO))
                {
                    CloseFistActivatedAbilityID = Guid.Empty;
                }
            }
            return removed;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            Body body = GO.Body;

            GO.RequirePart<Wrassler>();

            Debug.Entry(4, "? if (body != null)", Indent: 1);
            if (body != null)
            {
                Debug.CheckYeh(4, "Have Body", Indent: 2);
               
                GO.RemovePart<Gigantism>();
                Debug.LoopItem(4, "RemovePart<Gigantism>()", Indent: 2);
               
                IsGiganticCreature = true; // Enable the Gigantic flag
                Debug.LoopItem(4, "IsGiganticCreature = true", Indent: 2);
                
                GO.RequirePart<StunningForceOnJump>();
                Debug.LoopItem(4, "RequirePart<StunningForceOnJump>()", Indent: 2);
                
                if (!GO.TryGetPart(out StewBelly stewBelly))
                {
                    stewBelly = GO.RequirePart<StewBelly>();
                }
                if (!stewBelly.StartingStewsPocessed)
                {
                    stewBelly.StartingHankering = 1;
                    Debug.LoopItem(4, "stewBelly.StartingHankering = 1", Indent: 2);
                }
                Debug.LoopItem(4, "StewBelly already processed Stews", Indent: 2);
            }
            else
            {
                Debug.CheckNah(4, "Haven't Body", Indent: 2);
            }
            Debug.Entry(4, "x if (body != null) ?//", Indent: 1);

            AddActivatedAbilityGroundPound(GO);

            Debug.Entry(4, "? if (!GO.HasPart<Vehicle>())", Indent: 1);
            if (!GO.HasPart<Vehicle>())
            {
                Debug.CheckYeh(4, "Not Vehicle", Indent: 2);

                HunchOverActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: HunchedOverAbilityHunched,
                        // Name: "{{C|" + "{{W|[}}" + HunchedOverAbilityUpright + "{{W|]}}/" + HunchedOverAbilityHunched + "}}",
                        Command: COMMAND_NAME_HUNCH_OVER,
                        Class: "Physical Mutations",
                        Description: null,
                        Icon: "&#214",
                        DisabledMessage: null,
                        Toggleable: true,
                        DefaultToggleState: false,
                        ActiveToggle: true, 
                        IsAttack: false,
                        IsRealityDistortionBased: false,
                        IsWorldMapUsable: false
                        );

                Debug.LoopItem(4, "Activated Ability Assigned", Indent: 2);

                /*
                ActivatedAbilityEntry abilityEntry = GO.GetActivatedAbility(HunchOverActivatedAbilityID);
                abilityEntry.DisplayName = 
                    "{{C|" + 
                    "{{W|[}}" + HunchedOverAbilityUpright + "{{W|]}}\n" +
                                HunchedOverAbilityHunched + "\n" +
                       "}}";
                */

                // Debug.LoopItem(4, "Activated Ability DisplayName Changed", Indent: 2);
                /* This causes a village generation crash.
                 * 
                if (this.IsCyberGiant)
                {
                    abilityEntry.UITileDefault.ColorString = "b";
                    abilityEntry.UITileDefault.DetailColor = char.Parse("B");
                    abilityEntry.UITileToggleOn.ColorString = "b";
                    abilityEntry.UITileToggleOn.DetailColor = char.Parse("B");
                }
                */
            }
            else
            {
                Debug.CheckNah(4, "Is Vehicle", Indent: 2);
            }
            Debug.Entry(4, "x if (!GO.HasPart<Vehicle>()) ?//", Indent: 1);

            Debug.Entry(4, "deferring to base.Mutate(GO, Level)", Indent: 0);
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Debug.Header(4, $"GigantismPlus", $"Unmutate (GO: {GO.DebugName}, Level: {Level})");
            
            Debug.Entry(4, "? if (GO != null)", Indent: 1);
            if (GO != null)
            {
                Debug.CheckYeh(4, "GO not null", Indent: 2);

                Debug.Entry(4, "Attempting to StraightenUp()", Indent: 2);
                StraightenUp();
                GO.RemovePart<PseudoGigantism>();
                Debug.Entry(4, "RemovePart<PseudoGigantism>()", Indent: 2);
                GO.IsGiganticCreature = false; // Revert the Gigantic flag
                Debug.Entry(4, "IsGiganticCreature = false", Indent: 2);

                Debug.Entry(4, "? if (HunchOverActivatedAbilityID != Guid.Empty)", Indent: 2);
                if (HunchOverActivatedAbilityID != Guid.Empty)
                {
                    Debug.CheckYeh(4, "HunchOverActivatedAbilityID not Empty", Indent: 3);
                    RemoveMyActivatedAbility(ref HunchOverActivatedAbilityID);
                    Debug.LoopItem(4, "RemoveMyActivatedAbility(ref HunchOverActivatedAbilityID)", Indent: 3);
                }
                else
                {
                    Debug.CheckNah(4, "HunchOverActivatedAbilityID was Empty", Indent: 3);
                }
                Debug.Entry(4, "x if (HunchOverActivatedAbilityID != Guid.Empty) ?//", Indent: 2);

                ToggleMyActivatedAbility(CloseFistActivatedAbilityID, null, Silent: true, false);
                AbilityToggledCloseFist(GO, ToggledOn: false);
                RemoveActivatedAbilityCloseFist(GO, true);

                ToggleMyActivatedAbility(GroundPoundActivatedAbilityID, null, Silent: true, false);
                AbilityToggledGroundPound(GO, ToggledOn: false);
                RemoveActivatedAbilityGroundPound(GO, true);
                
                // Remove jumping properties
                UnapplyJumpRangeBonus(GO);

                UnapplyStunningForceOnJump(GO);

                Debug.LoopItem(4, "GO.WantToReequip()", Indent: 2);
                GO.WantToReequip();
                Debug.LoopItem(4, "GO.CheckEquipmentSlots()", Indent: 2);
                GO.CheckEquipmentSlots();
            }
            else
            {
                Debug.CheckNah(4, "GO is null", Indent: 2);
            }
            Debug.Entry(4, "x if (GO != null) ?//", Indent: 1);

            Debug.Entry(4, "deferring to base.Unmutate(GO, Level)", Indent: 0);
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            return base.Unmutate(GO);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            bool wantAddGroundPound = GroundPoundActivatedAbilityID == Guid.Empty;
            bool wantRemoveGroundPound = GroundPoundActivatedAbilityID != Guid.Empty;
            bool wantJumped = true || ParentObject.HasPart<StunningForceOnJump>();
            // Add once Hunch Over Stat-Shift is implemented: SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == GetIntrinsicWeightEvent.ID
                || ID == GetMaxCarriedWeightEvent.ID
                || ID == BeforeRapidAdvancementEvent.ID
                || ID == AfterRapidAdvancementEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == BeforeBodyPartsUpdatedEvent.ID
                || (wantAddGroundPound && ID == AfterAddSkillEvent.ID)
                || (wantRemoveGroundPound && ID == AfterRemoveSkillEvent.ID)
                || (wantJumped && ID == JumpedEvent.ID)
                || ID == BeforeVaultEvent.ID
                || ID == VaultedEvent.ID;
        }
        public override bool HandleEvent(GetIntrinsicWeightEvent E)
        {
            E.BaseWeight *= WeightFactor;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetMaxCarriedWeightEvent E)
        {
            E.BaseWeight *= CarryCapFactor;
            E.Weight += CarryCapBonus;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(BeforeRapidAdvancementEvent E)
        {
            if (!E.Amount.Is(0))
                SwapMutationCategory(nameof(GigantismPlus), "PhysicalDefects", "Physical");
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AfterRapidAdvancementEvent E)
        {
            if (!E.Amount.Is(0))
                SwapMutationCategory(nameof(GigantismPlus), "Physical", "PhysicalDefects");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AfterLevelGainedEvent E)
        {
            if (IsCyberGiant)
            {
                Body body = E.Actor.Body;
                body?.UpdateBodyParts();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(CanEnterInteriorEvent E)
        {
            Debug.Entry(1, "Checking CanEnterInteriorEvent");
            if (ParentObject == E.Object)
            {
                // This check is necessary because both the enterer and enteree handle this event.
                Debug.Entry(1, "Parent Object is the Target of Entry, Skip to base CanEnterInteriorEvent");
                return base.HandleEvent(E);
            }
            GameObject actor = E.Actor;
            if (actor != null && actor.IsGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(2, "We are big, gonna HunchOver");
                IsHunchFree = true;
                CommandEvent.Send(actor, COMMAND_NAME_HUNCH_OVER);
                Debug.Entry(3, "HunchOver Sent for CanEnterInteriorEvent");
                bool check = CanEnterInteriorEvent.Check(E.Actor, E.Object, E.Interior, ref E.Status, ref E.Action, ref E.ShowMessage);
                E.Status = check ? 0 : E.Status;
                string status = "";
                status += E.Status;
                Debug.Entry(3, "E.Status", status);

                Popup.Show("You try to squeeze into the space.");
            }
            else
            {
                Debug.Entry(2, "CanEnterInteriorEvent - We aren't big.");
            }
            Debug.Entry(1, "Sending to base CanEnterInteriorEvent");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
        {
            E.Features.Add("gigantic".OptionalColor("gianter", "w", Colorfulness) + " stature");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you equip non-gigantic equipment that is flagged as "GiganticEquippable" with half the slots it would normally take, provided it's not now too small.
            // exceptions are 
            if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment && E.Object.HasTagOrProperty("GiganticEquippable"))
            {
                E.Decreases++;
                if (E.SlotType != "Floating Nearby" && E.SlotType != "Thrown Weapon" && !E.Object.HasPart<CyberneticsBaseItem>())
                {
                    E.CanBeTooSmall = true;
                }
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            if (!E.Actor.HasBodyPart("Hand", false))
            {
                RemoveActivatedAbilityCloseFist(E.Actor, false);
            }
            else
            {
                AddActivatedAbilityCloseFist(E.Actor, false);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            // DescribeMyActivatedAbility(HunchOverActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AfterAddSkillEvent E)
        {
            bool includesJump = E.Skill.Name == nameof(Acrobatics_Jump);
            if (!E.Include.IsNullOrEmpty())
            {
                foreach (BaseSkill skill in E.Include)
                {
                    if (includesJump || skill.Name == nameof(Acrobatics_Jump))
                    {
                        includesJump = true;
                        break;
                    }
                }
            }
            AddActivatedAbilityGroundPound(E.Actor, includesJump);
            Acrobatics_Jump.SyncAbility(ParentObject);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AfterRemoveSkillEvent E)
        {
            RemoveActivatedAbilityGroundPound(E.Actor, E.Skill.Name == nameof(Acrobatics_Jump));
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(JumpedEvent E)
        {
            if (ParentObject != null && ParentObject == E.Actor)
            {
                float factor = 0.08f;
                float max = 1.2f;
                if (E.Actor.HasPart<StunningForceOnJump>())
                {
                    factor = 0.1f;
                    max = 1.5f;
                }
                Rumble(Level, factor, max);
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(BeforeVaultEvent E)
        {
            if (ParentObject != null && ParentObject == E.Vaulter && IsMyActivatedAbilityToggledOn(GroundPoundActivatedAbilityID, E.Vaulter))
            {
                CommandEvent.Send(E.Vaulter, COMMAND_NAME_GROUND_POUND);
                E.Vaulter.SetStringProperty("Flip_Ground_Pound", "Please");
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(VaultedEvent E)
        {
            if (ParentObject != null && ParentObject == E.Vaulter && !IsMyActivatedAbilityToggledOn(GroundPoundActivatedAbilityID, E.Vaulter))
            {
                if (E.Vaulter.HasStringProperty("Flip_Ground_Pound"))
                    CommandEvent.Send(E.Vaulter, COMMAND_NAME_GROUND_POUND);
            }
            return base.HandleEvent(E);
        }

        public override bool Render(RenderEvent E)
        {
            if (ParentObject.GetPropertyOrTag(GIGANTISMPLUS_COLORCHANGE_PROP, "true").Is("true"))
            {
                bool flag = true;
                if (ParentObject.IsPlayerControlled() && (XRLCore.FrameTimer.ElapsedMilliseconds & 0x7F) == 0L)
                {
                    flag = MutationColor;
                }
                if (flag && !IsCyberGiant)
                {
                    string newColor = Colorfulness > 2 ? ICON_COLOR : ICON_COLOR_FALLBACK;
                    E.ApplyColors(newColor, ICON_COLOR_PRIORITY);
                }
            }
            return base.Render(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(COMMAND_NAME_HUNCH_OVER);
            Registrar.Register(COMMAND_NAME_GROUND_POUND);
            Registrar.Register(COMMAND_NAME_CLOSE_FIST);
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == COMMAND_NAME_HUNCH_OVER)
            {
                GameObject actor = ParentObject;
                
                // Things that might stop you from taking this action
                if (actor.CurrentZone.ZoneWorld == "Interior" && !IsGiganticCreature)
                {
                    Popup.Show("This space is too small for you to stand upright!");
                    return base.FireEvent(E);
                }

                if (IsVehicleCreature)
                {
                    return base.FireEvent(E);
                }

                // Not prevented from taking action
                ToggleMyActivatedAbility(HunchOverActivatedAbilityID, null, Silent: true, null);
                Debug.Entry(3, "Hunch Ability Toggled");

                Debug.Entry(3, "Proceeding to Hunch Ability Effects");
                if (IsMyActivatedAbilityToggledOn(HunchOverActivatedAbilityID))
                    HunchOver(true); // Hunch
                else
                    StraightenUp(true); // Stand upright

                Debug.Entry(2, "IsPseudoGiganticCreature", IsPseudoGiganticCreature ? "true" : "false");
                Debug.Entry(2, "IsGiganticCreature", IsGiganticCreature ? "true" : "false");
            }

            if (E.ID == COMMAND_NAME_GROUND_POUND)
            {
                GameObject actor = ParentObject;

                ToggleMyActivatedAbility(GroundPoundActivatedAbilityID, null, Silent: true, null);
                Debug.Entry(3, "Ground Pound Toggled");

                Debug.Entry(3, "Proceeding to Ground Pound Ability Effects");
                AbilityToggledGroundPound(actor, IsMyActivatedAbilityToggledOn(GroundPoundActivatedAbilityID));
            }

            if (E.ID == COMMAND_NAME_CLOSE_FIST)
            {
                GameObject actor = ParentObject;

                if (ToggleMyActivatedAbility(CloseFistActivatedAbilityID, null, Silent: true, null))
                {
                    Debug.Entry(3, "Close Fist Toggled");
                }
                else
                {
                    Debug.Entry(3, "Close Fist failed to Toggle");
                }

                Debug.Entry(3, "Proceeding to Close Fist Ability Effects");
                if (IsMyActivatedAbilityToggledOn(CloseFistActivatedAbilityID) != AbilityToggledCloseFist(actor, IsMyActivatedAbilityToggledOn(CloseFistActivatedAbilityID)))
                {
                    Debug.CheckNah(3, "Something went wrong changing fist state (Open/Close)");
                }
            }

            The.Core.RenderBase();
            return base.FireEvent(E);
        }

        // Want to move the bulk of the Active Ability here.
        public void HunchOver(bool Message = false)
        {
            GameObject actor = ParentObject;
            if (IsPseudoGiganticCreature) // Already hunched over
            {
                Debug.Entry(1, "Tried to hunch, but was already PseudoGigantic");
                return;
            }

            IsPseudoGiganticCreature = true;

            if (!IsGiganticCreature && IsPseudoGiganticCreature)
            {
                // Action happened 
                UseEnergy(HunchOverEnergyCost, "Physical Defect Mutation Gigantism Hunch Over");
                //
                // Add the stat shifting code here.
                //

                actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_positiveVitality");
                if (Message)
                {
                    actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_positiveVitality");
                    Popup.Show("You hunch over, allowing you access to smaller spaces.");
                }

                // ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(HunchOverActivatedAbilityID);
                /*
                abilityEntry.DisplayName =
                    "{{C|" + 
                                HunchedOverAbilityUpright + "\n" +
                    "{{W|[}}" + HunchedOverAbilityHunched + "{{W|]}}\n" +
                       "}}";
                */

            }
            Debug.Entry(1, "Should be Hunched Over");
        } //!-- public void HunchOver(bool Message = false)

        // Want to move the bulk of the Active Ability here.
        public void StraightenUp(bool Message = false)
        {
            GameObject actor = ParentObject;
            if (!IsPseudoGiganticCreature) // Already Upright over
            {
                IsHunchFree = false;
                UnHunchImmediately = false;
                Debug.Entry(1, "Tried to straighten up, but wasn't PseudoGigantic");
                return;
            }

            IsPseudoGiganticCreature = false;

            if (IsGiganticCreature && !IsPseudoGiganticCreature)
            {
                // Action happened 
                UseEnergy(HunchOverEnergyCost, "Physical Defect Mutation Gigantism Hunch Over");
                
                //
                // Add the stat shifting code here.
                //

                if (Message)
                {
                    actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_negativeVitality");
                    Popup.Show("You stand tall, relaxing into your immense stature.");
                }

                // ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(HunchOverActivatedAbilityID);
                /* 
                abilityEntry.DisplayName = 
                    "{{C|" +
                    "{{W|[}}" + HunchedOverAbilityUpright + "{{W|]}}\n" +
                                HunchedOverAbilityHunched + "\n" +
                       "}}";
                */
            }
            Debug.Entry(1, "Should be Standing Tall");
        } //!-- public void StraightenUp(bool Message = false)

        public bool AbilityToggledGroundPound(GameObject GO = null, bool ToggledOn = false)
        {
            GO ??= ParentObject;
            if (GO == null)
                return false;

            if (ToggledOn)
            {
                ToggledOn = ApplyStunningForceOnJump(GO, Level);
            }
            else
            {
                ToggledOn = UnapplyStunningForceOnJump(GO);
            }
            GO.SetStringProperty("Flip_Ground_Pound", null, true);
            return ToggledOn;
        }

        public bool AbilityToggledCloseFist(GameObject GO = null, bool ToggledOn = false)
        {
            GO ??= ParentObject;

            if (GO == null)
                return false;

            bool OriginalToggledOn = ToggledOn;
            if (ToggledOn)
            {
                Debug.Entry(4, "AbilityToggledCloseFist ToggledOn", $"{ToggledOn}", Indent: 1);
                ModClosedGiganticNaturalWeapon ClosedGiganticFist = new()
                {
                    AssigningPart = this,
                    BodyPartType = "Hand",

                    ModPriority = -999999, // -999,999
                    DescriptionPriority = -999999, // -999,999

                    Adjective = "closed",
                    AdjectiveColor = "Y",
                    AdjectiveColorFallback = "y",

                    Adjustments = new(),

                    AddedIntProps = new()
                    {
                        { "ModGiganticNoShortDescription", 1 },
                        { "ModGiganticNoDisplayName", 1 }
                    },
                        AddedStringProps = new()
                    {
                        { "SwingSound", "Sounds/Melee/cudgels/sfx_melee_cudgel_fistOfTheApeGod_swing" },
                        { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_cudgel_fistOfTheApeGod_block" }
                    },
                };
                ClosedGiganticFist.AddAdjustment(MELEEWEAPON, "Skill", "Cudgel", false);
                ClosedGiganticFist.AddAdjustment(MELEEWEAPON, "Stat", "Strength", false);

                ClosedGiganticFist.AddAdjustment(RENDER, "DisplayName", "fist", false);

                ClosedGiganticFist.AddAdjustment(RENDER, "Tile", "NaturalWeapons/GiganticFist.png", false);

                NaturalEquipmentMod = ClosedGiganticFist;

                ToggledOn = true;

                if (NaturalEquipmentMod == null)
                {
                    Debug.Entry(4, "NaturalEquipmentMod Failed to instantiate", Indent: 1);
                    NaturalEquipmentMod = new();
                    ToggledOn = false;
                }
            }
            else
            {
                Debug.Entry(4, "AbilityToggledCloseFist ToggledOn", $"{ToggledOn}", Indent: 1);
                NaturalEquipmentMod = new();
                ToggledOn = false;
            }

            if (OriginalToggledOn == ToggledOn)
            {
                Debug.Entry(4, "Sending Bodyparts Update", Indent: 1);
                GO.Body.UpdateBodyParts();
            }

            return ToggledOn;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(HunchOverActivatedAbilityID);
            Writer.Write(GroundPoundActivatedAbilityID);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            HunchOverActivatedAbilityID = Reader.ReadGuid();
            GroundPoundActivatedAbilityID = Reader.ReadGuid();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            GigantismPlus gigantism = base.DeepCopy(Parent, MapInv) as GigantismPlus;
            gigantism.NaturalEquipmentMods = new();
            foreach ((_, ModNaturalEquipment<GigantismPlus> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                gigantism.NaturalEquipmentMods.Add(naturalEquipmentMod.BodyPartType, new(naturalEquipmentMod, gigantism));
            }
            gigantism.NaturalEquipmentMod = new(NaturalEquipmentMod, gigantism);
            gigantism.GiganticExoframe = null;
            return gigantism;
        }

    } //!-- public class GigantismPlus : BaseDefaultEquipmentMutation
}