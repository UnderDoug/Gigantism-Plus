using System;
using System.Collections.Generic;
using System.Linq;
using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Skill;
using XRL.World.Tinkering;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Secrets;
using UnityEngine.Tilemaps;
using JetBrains.Annotations;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GigantismPlus : BaseManagedDefaultEquipmentMutation
    {
        public int FistDamageDieCount;
        public int FistDamageDieSize;
        private string FistBaseDamage;
        public int FistHitBonus;
        public int FistMaxStrengthBonus = 999;

        public int appliedJumpBonus = 0;
        public double StunningForceLevelFactor = 0.5;
        public int StunningForceDistance = 3;

        public static readonly string HUNCH_OVER_COMMAND_NAME = "CommandToggleGigantismPlusHunchOver";

        public Guid EnableActivatedAbilityID = Guid.Empty;

        public int HunchedOverAVModifier;
        public int HunchedOverDVModifier;
        public int HunchedOverQNModifier;
        public int HunchedOverMSModifier;

        private bool _IsVehicleCreature = false;
        public bool IsVehicleCreature
        {
            get
            {
                if (ParentObject.HasPart(typeof(Vehicle)))
                {
                    _IsVehicleCreature = true;
                }
                else
                {
                    _IsVehicleCreature = false;
                }
                return _IsVehicleCreature;
            }
        }

        private string HunchedOverAbilityHunched
        {
            get
            {
                if (IsCyberGiant)
                    return "Compact";
                return "Hunched";
            }
        }
        private string HunchedOverAbilityUpright
        {
            get
            {
                if (IsCyberGiant)
                    return "Regular"; // was "Standard" but it's one too many characters
                return "Upright";
            }
        }

        public static int GetFistDamageDieCount(int Level)
        {
            return Math.Min(1 + (int)Math.Floor(Level / 3.0), 8);
        }
        public static int GetFistDamageDieSize(int Level)
        {
            return 3 + (int)Math.Floor(Level / 3.0);
        }
        public static int GetFistDamageBonus(int Level)
        {
            return (int)Math.Max(0, Math.Floor((Level - 9) / 3.0) - 3);
        }
        public static string GetFistBaseDamage(int Level)
        {
            return $"{GetFistDamageDieCount(Level)}d{GetFistDamageDieSize(Level)}+3";
        }
        public static int GetFistHitBonus(int Level)
        {
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

        public int GetJumpDistanceBonus(int Level)
        {
            int baseBonus = 1;
            var cybernetics = ParentObject.Body.GetBody().Cybernetics;
            if (cybernetics != null && cybernetics.TryGetPart(out CyberneticsGiganticExoframe exoframe))
            {
                return baseBonus + exoframe.JumpDistanceBonus;
            }
            return baseBonus;
        }
        public int GetStunningForceLevelFactor(int Level)
        {
            double factor = StunningForceLevelFactor;
            var cybernetics = ParentObject.Body.GetBody().Cybernetics;
            if (cybernetics != null && cybernetics.TryGetPart(out CyberneticsGiganticExoframe exoframe))
            {
                factor = exoframe.StunningForceLevelFactor;
            }
            return (int)Math.Floor(Level * factor);
        }
        public int GetStunningForceDistance(int Level)
        {
            return StunningForceDistance;
        }

        public bool IsGiganticCreature // basically a wrapper but forces you to not be PseudoGigantic at the same time 
        {
            get
            {
                return ParentObject.IsGiganticCreature;
            }
            private set
            {
                ParentObject.IsGiganticCreature = value;
                if (IsPseudoGiganticCreature == value)
                {
                    IsPseudoGiganticCreature = !value;
                }
            }
        }
        public bool IsPseudoGiganticCreature // designed to ensure you aren't (typically) Gigantic and PseudoGigantic at the same time 
        {
            get
            {
                return ParentObject.HasPart<PseudoGigantism>();
            }
            set
            {
                if (value) ParentObject.RequirePart<PseudoGigantism>();
                else ParentObject.RemovePart<PseudoGigantism>();

                if (IsGiganticCreature == value)
                {
                    IsGiganticCreature = !value;
                }

            }
        }

        public bool IsCyberGiant
        {
            get
            {
                Debug.Entry(3, "IsCyberGiant, get");
                if (ParentObject != null)
                {
                    Debug.Entry(3, "- ParentObject not null");
                    GameObject cybernetics = ParentObject.Body.GetPartByName("body").Cybernetics;
                    if (cybernetics != null)
                    {
                        Debug.Entry(3, "- cybernetics not null");
                        Debug.Entry(3, "CyberGiant: true");
                        return cybernetics.GetBlueprint().Inherits == "BaseGiganticExoframe";
                    }
                    Debug.Entry(3, "- cybernetics is null");
                    Debug.Entry(3, "CyberGiant: false");
                    return false;
                }
                Debug.Entry(3, "- ParentObject is null");
                Debug.Entry(3, "CyberGiant: false");
                return false;
            }
        }

        public bool UnHunchImmediately = false;

        public bool IsHunchFree = false;
        private int _hunchOverEnergyCost = 500;
        public int HunchOverEnergyCost
        {
            get
            {
                Debug.Entry(4, "HunchEnergyCost requested");
                if (IsHunchFree)
                {
                    Debug.Entry(3, "Hunch Is Free");
                    IsHunchFree = false;
                    return 0;
                }
                Debug.Entry(4, "Hunch Cost given", _hunchOverEnergyCost.ToString());
                return _hunchOverEnergyCost;
            }
            private set
            {
                Debug.Entry(3, "attempt to set HunchEnergyCost");
                _hunchOverEnergyCost = value;
                Debug.Entry(4, "new HunchEnergyCost", _hunchOverEnergyCost.ToString());
            }
        }

        private string _NaturalWeaponBlueprintName = "GiganticFist";
        public string NaturalWeaponBlueprintName
        {
            get
            {
                if (IsCyberGiant)
                {
                    return ParentObject.Body.GetPartByName("body").Cybernetics.GetPart<CyberneticsGiganticExoframe>().ManipulatorBlueprintName;
                }
                return _NaturalWeaponBlueprintName;
            }
            private set
            {
                _NaturalWeaponBlueprintName = value;
            }
        }

        [NonSerialized]
        protected GameObjectBlueprint _NaturalWeaponBlueprint;
        public GameObjectBlueprint NaturalWeaponBlueprint
        {
            get
            {
                _NaturalWeaponBlueprint = GameObjectFactory.Factory.GetBlueprint(NaturalWeaponBlueprintName);
                return _NaturalWeaponBlueprint;
            }
            set
            {
                _NaturalWeaponBlueprint = value;
            }
        }

        private static readonly List<string> NaturalWeaponSupersedingMutations = new List<string>
        {
          //"CyberneticsGiganticExoframe",
            "BurrowingClaws",
            "Crystallinity"
        };
        public bool IsNaturalWeaponSuperseded
        {
            get
            {
                int count = 0;
                foreach (string mutation in NaturalWeaponSupersedingMutations)
                {
                    if (ParentObject.HasPart(mutation))
                    {
                        count++;
                    }
                }
                Debug.Entry(4, $"Superseding Count is {count}", Indent: 4);
                return count > 0;
            }
        }

        public GigantismPlus()
        {
            DisplayName = "{{gigantism|Gigantism}} ({{r|D}})";
            Type = "Physical";

            NaturalWeapon = new()
            {
                DamageDieCount = 1,
                DamageDieSize = 2,
                DamageBonus = 0,
                ModPriority = 10,
                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                Noun = "fist",
                Skill = "Cudgel",
                Stat = "Strength",
                Tile = "GiganticFist.png",
                RenderColorString = "&x",
                RenderDetailColor = "z",
                SecondRenderColorString = "&X",
                SecondRenderDetailColor = "Z"
            };
        }

        public override bool CalculateNaturalWeaponDamageDieCount(int Level = 1)
        {
            Debug.Entry(4, "GigantismPlus", "CalculateNaturalWeaponDamageDieCount", Indent: 7);
            NaturalWeapon.DamageDieCount = Math.Min(1 + (int)Math.Floor(Level / 3.0), 8);
            return base.CalculateNaturalWeaponDamageDieCount(Level);
        }
        public override bool CalculateNaturalWeaponDamageBonus(int Level = 1)
        {
            NaturalWeapon.DamageBonus = (int)Math.Max(0, Math.Floor((Level - 9) / 3.0) - 3);
            return base.CalculateNaturalWeaponDamageBonus(Level);
        }
        public override bool CalculateNaturalWeaponHitBonus(int Level = 1)
        {
            NaturalWeapon.HitBonus = (int)Math.Max(0, Math.Floor((Level - 9) / 3.0) - 3);
            return base.CalculateNaturalWeaponHitBonus(Level);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            GigantismPlus gigantism = base.DeepCopy(Parent, MapInv) as GigantismPlus;
            //gigantism.GiganticFistObject = null;
            return gigantism;
        }

        public override bool CanLevel() { return true; } // Enable leveling

        public override bool GeneratesEquipment() { return true; }

        public override bool ChangeLevel(int NewLevel)
        {
            Debug.Header(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            // Straighten up if hunching.
            // Hunch over if hunched before level up.
            bool WasHunched = false;
            if (IsPseudoGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(4, "Creature is PsuedoGigantic && not a Vehicle", Indent: 1);
                Debug.Entry(4, "Sending StraightenUp", Indent: 1);
                WasHunched = true;
                IsHunchFree = true;
                StraightenUp(Message: false);
            }
            // Start of Change Level updates.

            // Jump Bonus
            if (appliedJumpBonus > 0)
            {
                ParentObject.ModIntProperty("JumpRangeModifier", -appliedJumpBonus);
            }
            appliedJumpBonus = GetJumpDistanceBonus(NewLevel);
            ParentObject.ModIntProperty("JumpRangeModifier", appliedJumpBonus);
            Acrobatics_Jump.SyncAbility(ParentObject);

            // Stunning Force
            if (ParentObject.TryGetPart(out StunningForceOnJump stunning))
            {
                stunning.Level = GetStunningForceLevelFactor(NewLevel); // Scale stunning force with mutation level
                stunning.Distance = StunningForceDistance;
            }

            HunchedOverAVModifier = GetHunchedOverAVModifier(NewLevel);
            HunchedOverDVModifier = GetHunchedOverDVModifier(NewLevel);
            HunchedOverMSModifier = GetHunchedOverMSModifier(NewLevel);
            

            // End of Change Level updates
            if (WasHunched && !IsVehicleCreature)
            {
                Debug.Entry(4, "Creature was Hunched && not a Vehicle", Indent: 1);
                Debug.Entry(4, "Sending HunchOver", Indent: 1);
                IsHunchFree = true;
                HunchOver(Message: false);
            }
            Debug.Footer(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            return base.ChangeLevel(NewLevel);
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            // Currently unused but will comprise part of the stat-shifting for Hunch Over.
            int HunchedOverAV = GetHunchedOverAVModifier(Level);
            int HunchedOverDV = GetHunchedOverDVModifier(Level);
            int HunchedOverMS = GetHunchedOverMSModifier(Level);
            stats.Set("HunchedOverAV", "+" + HunchedOverAV);
            stats.Set("HunchedOverDV", HunchedOverDV);
            stats.Set("HunchedOverMS", HunchedOverMS);
        }

        // method to swap Gigantism mutation category between Physical and PhysicalDefects
        // - Rapid advancement checks the Physical MutationCategory Entries.
        private void SwapMutationCategory(bool Before = true)
        {
            string state = Before ? "true" : "false";
            Debug.Header(3, "GigantismPlus",$"SwapMutationCategory(Before: {state})");

            // prefer this for repeated uses of strings.
            string Physical = "Physical";
            string PhysicalDefects = "PhysicalDefects";

            // direction of swap depends on whether before or after LevelGain
            string IntoCategory = Before ? Physical : PhysicalDefects;
            string OutOfCategory = Before ? PhysicalDefects : Physical;

            Debug.Entry(3, $"Into Category:   \"{IntoCategory}\"", Indent: 1);
            Debug.Entry(3, $"Out Of Category: \"{OutOfCategory}\"", Indent: 1);

            MutationEntry GigantismEntry = MutationFactory.GetMutationEntryByName(this.Name);

            Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 1);
            foreach (MutationCategory category in MutationFactory.GetCategories())
            {
                Debug.LoopItem(4, category.Name, Indent: 2);
                if (category.Name == IntoCategory)
                {
                    Debug.DiveIn(4, $"Found Category: \"{IntoCategory}\"", Indent: 2);

                    Debug.Entry(3, $"Adding \"{GigantismEntry.DisplayName}\" Mutation to \"{IntoCategory}\" Category", Indent: 3);
                    category.Add(GigantismEntry);
                    category.Entries.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

                    Debug.Entry(4, $"Displaying all entries in \"{IntoCategory}\" Category", Indent: 3);
                    Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 3);
                    foreach (MutationEntry entry in category.Entries)
                    {
                        Debug.LoopItem(4, entry.DisplayName, Indent: 4);
                    }
                    Debug.DiveOut(3, $"x {IntoCategory} >//", Indent: 2);
                }
                if (category.Name == OutOfCategory)
                {
                    Debug.DiveIn(3, $"Found Category: \"{OutOfCategory}\"", Indent: 2);
                    Debug.Entry(3, $"Removing \"{GigantismEntry.DisplayName}\" from \"{OutOfCategory}\" Category", Indent: 3);
                    category.Entries.RemoveAll(r => r == GigantismEntry);
                    Debug.DiveOut(3, $"x {OutOfCategory} >//", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (MutationCategory category in MutationFactory.GetCategories()) ]//", Indent: 1);
            Debug.Footer(3, "GigantismPlus", $"SwapMutationCategory(Before: {state})");
        } //!--- private void SwapMutationCategory(bool Before = true)

        private bool ShouldRapidAdvance(int Level, GameObject Actor)
        {
            bool IsMutant = Actor.IsMutant();
            bool RapidAdvancement = IsMutant
                                 && (Level + 5) % 10 == 0
                                 && !Actor.IsEsper()
                                 && HNPS_GigantismPlus.Options.EnableGigantismRapidAdvance;

            return RapidAdvancement;
        } //!--- private bool ShouldRapidAdvance(int Level, GameObject Actor)

        public override bool WantEvent(int ID, int cascade)
        {
            // Add once Hunch Over Stat-Shift is implemented: SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == BeforeLevelGainedEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == GetMaxCarriedWeightEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(BeforeLevelGainedEvent E)
        {
            if (ShouldRapidAdvance(E.Level, E.Actor))
            {
                SwapMutationCategory(true);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AfterLevelGainedEvent E)
        {
            if (ShouldRapidAdvance(E.Level, E.Actor))
            {
                SwapMutationCategory(false);
            }
            if (IsCyberGiant)
            {
                Body body = E.Actor.Body;
                body?.UpdateBodyParts();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
        {
            E.Features.Add("{{gianter|gigantic}} stature");
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

        public override bool HandleEvent(CanEnterInteriorEvent E)
        {
            Debug.Entry(1,"Checking CanEnterInteriorEvent");
            if (ParentObject == E.Object)
            {
                Debug.Entry(1,"Parent Object is the Target of Entry, Skip to base CanEnterInteriorEvent");
                return base.HandleEvent(E);
            }
            GameObject actor = E.Actor;
            if (actor != null && actor.IsGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(2,"We are big, gonna HunchOver");
                IsHunchFree = true;
                CommandEvent.Send(actor, HUNCH_OVER_COMMAND_NAME);
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

        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            // DescribeMyActivatedAbility(EnableActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }

        public override string GetDescription()
        {
            string GigantismSource = (!this.IsCyberGiant) ? "unusually" : "{{c|cybernetically}}";
            string WeaponName = (ParentObject == null) ? "gigantic fists" : ParentObject.Body.GetFirstPart("Hand").DefaultBehavior.ShortDisplayName;
            
            return "You are " + GigantismSource + " large, will {{rules|struggle to enter small spaces}} without {{g|hunching over}}, and can typically {{rules|only}} use {{gigantic|gigantic}} equipment.\n"
                 + "You are {{rules|heavy}}, can carry {{rules|twice}} as much weight, and all your natural weapons are {{gigantic|gigantic}}.\n\n"
                 + "Your " + WeaponName + "s gain:\n"
                 + "{{rules|+1}} To-Hit every {{rules|2 mutation levels}}\n"
                 + "{{B|d1}} damage every {{B|3 mutation levels}}\n"
                 + "{{W|1d}} damage every {{W|5 mutation levels}}\n"
                 + "They have {{rules|uncapped penetration}}, but are harder {{rules|to hit}} with due to their size.";
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

            string intSign = GetFistHitBonus(Level) >= 0 ? "+" : "";
            string toHitString = "";
            string penaltyBonus = GetFistHitBonus(Level) >= 0 ? "bonus" : "penalty";
            if (GetFistHitBonus(Level) != 0)
            {
                toHitString = "and {{rules|" + intSign + GetFistHitBonus(Level) + "}} To Hit " + penaltyBonus + "\n";
            }

            string WeaponName = (ParentObject == null) ? "gigantic fists" : ParentObject.Body.GetFirstPart("Hand").DefaultBehavior.ShortDisplayName;
            return WeaponName + " {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{W|" + GetFistDamageDieCount(Level) + "}}{{rules|d}}{{B|" + GetFistDamageDieSize(Level) + "}}{{rules|+3}}\n"
                 + toHitString; /*+ "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}";
                 + "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}"; */
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Debug.Entry(2, $"GigantismPlus -> Mutate {GO.DebugName}");
            Body body = GO.Body;
            if (body != null)
            {
                Debug.Entry(2, "- Mutating: Have Body");
                GO.RemovePart<Gigantism>();
                Debug.Entry(2, "- Mutating: RemovePart<Gigantism>()");
                IsGiganticCreature = true; // Enable the Gigantic flag
                Debug.Entry(2, "- Mutating: IsGiganticCreature = true");
                GO.RequirePart<StunningForceOnJump>();
            }
            if (!GO.HasPart<Vehicle>())
            {
                Debug.Entry(2, "- Mutating: Not Vehicle");
                                /* AddActivatedAbility() - Full Method Arguments.
                 * AddActivatedAbility(Name, Command, Class, Description, Icon, DisabledMessage, Toggleable, DefaultToggleState, ActiveToggle, IsAttack, IsRealityDistortionBased, IsWorldMapUsable, Silent, AIDisable, AlwaysAllowToggleOff, AffectedByWillpower, TickPerTurn, Distinct: false, Cooldown, CommandForDescription, UITileDefault, UITileToggleOn, UITileDisabled, UITileCoolingDown); */
                EnableActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: "{{C|" + "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}/" + this.HunchedOverAbilityHunched + "}}",
                        Command: HUNCH_OVER_COMMAND_NAME,
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

                Debug.Entry(2, "- Mutating: Activeated Ability Assigned");
                ActivatedAbilityEntry abilityEntry = GO.GetActivatedAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = 
                    "{{C|" + 
                    "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}\n" +
                                this.HunchedOverAbilityHunched + "\n" +
                       "}}";

                Debug.Entry(2, "- Mutating: Activeated Ability DisplayName Changed");
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

            Debug.Entry(2, $"GigantismPlus -> base.Mutate {GO.DebugName}");
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Debug.Entry(2, $"GigantismPlus -> Unmutate {GO.DebugName}");
            if (GO != null)
            {
                // Remove jumping properties
                GO.ModIntProperty("JumpRangeModifier", -appliedJumpBonus, RemoveIfZero: true);
                appliedJumpBonus = 0;
                Acrobatics_Jump.SyncAbility(GO);

                Debug.Entry(2, "- Removing StunningForceOnJump");
                if (GO.HasPart<StunningForceOnJump>())
                {
                    var stunning = GO.GetPart<StunningForceOnJump>();
                    Debug.Entry(2, $"- Found StunningForceOnJump: Level={stunning.Level}, Distance={stunning.Distance}");
                    GO.RemovePart<StunningForceOnJump>();
                    Debug.Entry(2, "- StunningForceOnJump removed");
                }
                else
                {
                    Debug.Entry(2, "- No StunningForceOnJump part found to remove");
                }

                StraightenUp();
                GO.RemovePart<PseudoGigantism>();
                GO.IsGiganticCreature = false; // Revert the Gigantic flag

                if (EnableActivatedAbilityID != Guid.Empty)
                {
                    RemoveMyActivatedAbility(ref EnableActivatedAbilityID);
                }

                GO.WantToReequip();
                CheckAffected(GO, GO.Body);
            }

            Debug.Entry(2, $"GigantismPlus -> base.Unmutate {GO.DebugName}");
            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, "GigantismPlus", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
                Debug.Footer(3, "GignatismPlus", $"OnRegenerateDefaultEquipment(body)");
                base.OnRegenerateDefaultEquipment(body);
                return;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);
            
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                    where p.Type == targetPartType
                                    select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod(), Actor: ParentObject);

                    Debug.DiveOut(4, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) ]//", Indent: 1);
            
            Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
            Debug.Footer(3, "GigantismPlus", $"OnRegenerateDefaultEquipment(body)");
            base.OnRegenerateDefaultEquipment(body);
        } //!--- public override void OnRegenerateDefaultEquipment(Body body)

        public void CheckAffected(GameObject Actor, Body Body)
        {
            if (Actor == null || Body == null)
            {
                return;
            }
            List<GameObject> list = Event.NewGameObjectList();
            foreach (BodyPart item in Body.LoopParts())
            {
                GameObject equipped = item.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    list.Add(equipped);
                    int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                    int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, item.Type);
                    if (partCountEquippedOn != slotsRequiredFor && item.TryUnequip(Silent: true, SemiForced: true) && partCountEquippedOn > slotsRequiredFor)
                    {
                        equipped.SplitFromStack();
                        item.Equip(equipped, 0, Silent: true, ForDeepCopy: false, Forced: false, SemiForced: true);
                    }
                }
            }
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(HUNCH_OVER_COMMAND_NAME);
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == HUNCH_OVER_COMMAND_NAME)
            {
                GameObject actor = this.ParentObject;
                
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
                ToggleMyActivatedAbility(EnableActivatedAbilityID, null, Silent: true, null);
                Debug.Entry(3, "Hunch Ability Toggled");

                Debug.Entry(3, "Proceeding to Hunch Ability Effects");
                if (IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID))
                    HunchOver(true); // Hunch
                else
                    StraightenUp(true); // Stand upright

                Debug.Entry(2, "IsPseudoGiganticCreature", IsPseudoGiganticCreature ? "true" : "false");
                Debug.Entry(2, "IsGiganticCreature", IsGiganticCreature ? "true" : "false");

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

                ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName =
                    "{{C|" + 
                                HunchedOverAbilityUpright + "\n" +
                    "{{W|[}}" + HunchedOverAbilityHunched + "{{W|]}}\n" +
                       "}}";

            }
            Debug.Entry(1, "Should be Hunched Over");
        } //!--- public void HunchOver(bool Message = false)

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

                ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName =
                    "{{C|" +
                    "{{W|[}}" + HunchedOverAbilityUpright + "{{W|]}}\n" +
                                HunchedOverAbilityHunched + "\n" +
                       "}}";
            }

            CheckAffected(actor, actor.Body);
            Debug.Entry(1, "Should be Standing Tall");
        } //!-- public void StraightenUp(bool Message = false)

    } //!-- public class GigantismPlus : BaseDefaultEquipmentMutation

}

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class PseudoGigantism : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID;
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            if (!E.Actor.IsGiganticCreature)
            {
                E.Decreases++;
                if (!E.Object.IsGiganticEquipment && E.SlotType != "Floating Nearby" && !E.Object.HasPart<CyberneticsBaseItem>() && !E.Object.HasTagOrProperty("GiganticEquippable"))
                {
                    E.CanBeTooSmall = true;
                }
            }

            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    } //!-- public class PsudoGigantic : IPart

    [PlayerMutator]
    public class GigantifyStartingLoadout : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            Debug.Entry(2, "##################################################################");
            Debug.Entry(2, "public class GigantifyStartingLoadout : IPlayerMutator");
            Debug.Entry(3, "[{}] public void mutate(GameObject player)");

            Debug.Entry(2, "Checking if Gigantification of starting gear should occur");
            Debug.Entry(4, "**if ((player.HasPart(\"GigantismPlus\") && Options.EnableGiganticStartingGear)");
            // Check for either mutation OR cybernetic as source of gigantism
            if (player.HasPart("GigantismPlus") && Options.EnableGiganticStartingGear)
            {
                Debug.Entry(3, "- Player is Gigantic && Option is [Enabled]");

                Debug.Entry(3, "- Checking if Grenades should be included");
                Debug.Entry(4, "**if (Options.EnableGiganticStartingGear_Grenades)");
                if (Options.EnableGiganticStartingGear_Grenades)
                {
                    Debug.Entry(3, "-- Option is [Enabled] - Grenades will be Gigantified");
                }
                else
                {
                    Debug.Entry(3, "-- Option is [Disabled] - Grenades won't be Gigantified");
                }

                Debug.Entry(3, "- Performing Gigantification");
                Debug.Entry(3, "**foreach (GameObject item in player.GetInventoryAndEquipment())");
                // Cycle the player's inventory and equipped items.
                foreach (GameObject item in player.GetInventoryAndEquipment())
                {
                    Debug.Entry(3, "-------------------------------------------");
                    string ItemName = item.DebugName;
                    Debug.Entry(3, $"--@ Item Entry: {ItemName}");
                    ItemName = "--- " + item.Blueprint;
                    // Can the item have the gigantic modifier applied?
                    if (ItemModding.ModificationApplicable("ModGigantic", item))
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        // Is the item already gigantic? Don't attempt to apply it again.
                        if (item.HasPart<ModGigantic>())
                        {
                            Debug.Entry(3, ItemName, "is already gigantic");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not gigantic");

                        // Is the item a grenade, and is the option not set to include them?
                        if (!Options.EnableGiganticStartingGear_Grenades && item.HasTag("Grenade"))
                        {
                            Debug.Entry(3, ItemName, "is a grenade (excluded)");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        if (!item.HasTag("Grenade")) Debug.Entry(3, ItemName, "not a grenade");
                        if (item.HasTag("Grenade")) Debug.Entry(3, ItemName, "is a grenade");

                        // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                        if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                        {
                            Debug.Entry(3, ItemName, "is TradeGoods");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not TradeGoods");

                        // Is the item a tonic? Double doses are basically useless in the early game
                        if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                        {
                            Debug.Entry(3, ItemName, "is Tonics_NonRare");
                            Debug.Entry(4, "--X Skipping");
                            Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            continue;
                        }
                        Debug.Entry(3, ItemName, "not Tonics_NonRare");

                        // apply the gigantic mod to the item and attempt to auto-equip it
                        ItemModding.ApplyModification(item, "ModGigantic");
                        Debug.Entry(2, ItemName, "has been Gigantified");
                        // player.AutoEquip(item); Debug.Entry(2, ItemName, "AutoEquip Attempted");
                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                    }
                    else
                    {
                        Debug.Entry(2, ItemName, "cannot be made gigantic");
                        Debug.Entry(4, "--X Skipping");
                        Debug.Entry(3, $"{ItemName} ]//");
                        continue;
                    }

                    Debug.Entry(3, $"{ItemName} ]//");
                }
                Debug.Entry(3, "-------------------------------------------");
                Debug.Entry(3, "- Gigantification of starting gear finished");
                player.WantToReequip();
                Debug.Entry(3, "- Attempting to reequip items");
                Debug.Entry(2, "##################################################################");
            }
            else
            {
                Debug.Entry(4, "- Player not Gigantic || Option is [Disabled]");
                Debug.Entry(3, "- Check Failed");
                Debug.Entry(2, "##################################################################");
            }
        }
    }

    public class GiganticModifierAdjustments
    {
        public static void AdjustGiganticModifier()
        {
            bool ShouldDerarify = Options.SelectGiganticDerarification;
            bool ShouldGiganticTinkerable = Options.SelectGiganticTinkering;
            Debug.Entry(3, "[{}] AdjustGiganticModifier()");

            Debug.Entry(3, "Attempting ModList adjustment process");
            Debug.Entry(4, "**foreach (ModEntry mod in ModificationFactory.ModList)");
            // find the gigantic modifier ModEntry in the ModList
            foreach (ModEntry mod in ModificationFactory.ModList)
            {
                Debug.Entry(3, "-------------------------------------------");
                string ModPart = mod.Part;
                Debug.Entry(3, $"--@ Mod Entry: {ModPart}");
                ModPart = "--- " + ModPart;
                if (mod.Part == "ModGigantic")
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, ModPart, "Found");
                    // should the rarity be adjusted? 
                    // - change the rarity from R2 (3) to R (2) 
                    Debug.Entry(4, "**if (ShouldDerarify)");
                    if (ShouldDerarify)
                    {
                        Debug.Entry(4, "---- Should");
                        mod.Rarity = 2;
                        Debug.Entry(2, ModPart, "Rarity is R (decreased)");
                    }
                    else
                    {
                        Debug.Entry(4, "---- Shouldn't");
                        mod.Rarity = 3;
                        Debug.Entry(2, ModPart, "Rarity is R2 (default)");
                    }

                    // should tinkering be allowed? 
                    // - change the tinkerability and add it to the list of recipes
                    Debug.Entry(4, "**if (ShouldGiganticTinkerable)");
                    if (ShouldGiganticTinkerable)
                    {
                        Debug.Entry(4, "---- Should");
                        mod.TinkerAllowed = true;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Enabled]");

                        // Modifiers can actually be set to require an additional ingredient.
                        // mod.TinkerIngredient = "Torch";
                    }
                    else
                    {
                        Debug.Entry(4, "---- Shouldn't");
                        mod.TinkerAllowed = false;
                        Debug.Entry(2, ModPart, "Gigantic tinkering [Disabled] (default)");
                    }

                    // this is a workaround for what I'm sure is a more straightforward and simple solution
                    // - after adjusting the ModEntry to be tinkerable, it needs to be added to the list of recipes
                    // - flushing the list of recipes and then requesting the list uses an internal "get" function that cycles all the TinkerData and ModEntries and adds them to the TinkerRecipes list
                    // - only works if you flush it first since the "get" function checks if the _list is empty first and if it isn't just returns it
                    // it's probably NOT good, and could pose compatability issues with other mods if they do things post Blueprint pre-load, but I'm not nearly experienced enough to know what issues exactly

                    TinkerData._TinkerRecipes.RemoveAll(r => r != null); Debug.Entry(2, "--- Purged TinkerRecipes");
                    List<TinkerData> reinitialise = TinkerData.TinkerRecipes; Debug.Entry(2, "--- Reinitialised TinkerRecipes");
                    reinitialise = null; Debug.Entry(4, "--- Reinitialisation nulled");

                    Debug.Entry(4, "--- No Further Actions Required", "Exiting ModList");
                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    Debug.Entry(3, $"{ModPart} ]//");

                    break;

                }
                else Debug.Entry(2, ModPart, "Not ModGigantic");

                Debug.Entry(3, $"{ModPart} ]//");
            }
            Debug.Entry(3, "-------------------------------------------");
            Debug.Entry(1, "ModList exited, adjustment process finished");
        }
    } //!--- public class GiganticModifierAdjustments

    [PlayerMutator]
    public class OnPlayerLoad : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Entry(2, "public class OnPlayerLoad : IPlayerMutator");
            Debug.Entry(2, "public void mutate(GameObject player)");
            GiganticModifierAdjustments.AdjustGiganticModifier();
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    } //!--- public class OnPlayerLoad : IPlayerMutator

    [HasCallAfterGameLoadedAttribute]
    public class OnLoadGameHandler
    {
        [CallAfterGameLoadedAttribute]
        public static void OnLoadGameCallback()
        {
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Entry(2, "public class OnLoadGameHandler");
            Debug.Entry(2, "public static void OnLoadGameCallback()");
            GiganticModifierAdjustments.AdjustGiganticModifier();
            Debug.Entry(2, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    } //!--- public class OnLoadGameHandler
}