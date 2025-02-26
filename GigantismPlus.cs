using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.Tinkering;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;
using static Mods.GigantismPlus.HelperMethods;
using static Mods.GigantismPlus.Secrets;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GigantismPlus : BaseDefaultEquipmentMutation
    {

        public int FistDamageDieCount;
        public int FistDamageDieSize;
        private string FistBaseDamage;
        public int FistHitBonus;
        public int FistMaxStrengthBonus = 999;

        public int appliedJumpBonus = 0;
        public double StunningForceLevelFactor = 0.5;
        public int StunningForceDistance = 3;

        public GameObject GiganticFistObject;
        public GameObject GiganticElongatedPawObject;
        public GameObject GiganticBurrowingClawObject;
        public GameObject GiganticElongatedBurrowingClawObject;

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
                if (this.IsCyberGiant)
                    return "Compact";
                return "Hunched";
            } 
        }
        private string HunchedOverAbilityUpright
        {
            get
            {
                if (this.IsCyberGiant)
                    return "Regular"; // was "Standard" but it's one too many characters
                return "Upright";
            }
        }

        public static int GetFistDamageDieCount(int Level)
        {
            return 1 + (int)Math.Floor((double)Level / 5.0);
        }
        public static int GetFistDamageDieSize(int Level)
        {
            return 3 + (int)Math.Floor((double)Level / 3.0);
        }
        public static string GetFistBaseDamage(int Level)
        {
            return $"{GetFistDamageDieCount(Level)}d{GetFistDamageDieSize(Level)}+3";
        }
        public static int GetFistHitBonus(int Level)
        {
            return -3 + (int)Math.Floor((double)Level / 2.0);
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
            return Math.Min(-70 + (int)Math.Floor((double)Level * 10.0),-10);
        }
        public static int GetHunchedOverMSModifier(int Level)
        {
            return Math.Min(-70 + (int)Math.Floor((double)Level * 10.0),-10);
        }

        public int GetJumpDistanceBonus(int Level)
        {
            int baseBonus = 1;
            var cybernetics = ParentObject.Body.GetBody().Cybernetics;
            if (cybernetics != null && cybernetics.TryGetPart<CyberneticsGiganticExoframe>(out CyberneticsGiganticExoframe exoframe))
            {
                return baseBonus + exoframe.JumpDistanceBonus;
            }
            return baseBonus;
        }
        public int GetStunningForceLevelFactor(int Level)
        {
            double factor = StunningForceLevelFactor;
            var cybernetics = ParentObject.Body.GetBody().Cybernetics;
            if (cybernetics != null && cybernetics.TryGetPart<CyberneticsGiganticExoframe>(out CyberneticsGiganticExoframe exoframe))
            {
                factor = exoframe.StunningForceLevelFactor;
            }
            return (int)Math.Floor((double)Level * factor);
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
                if (this.IsHunchFree)
                {
                    Debug.Entry(3, "Hunch Is Free");
                    this.IsHunchFree = false;
                    return 0;
                }
                Debug.Entry(4, "Hunch Cost given", this._hunchOverEnergyCost.ToString());
                return this._hunchOverEnergyCost;
            }
            private set
            {
                Debug.Entry(3, "attempt to set HunchEnergyCost");
                this._hunchOverEnergyCost = value;
                Debug.Entry(4, "new HunchEnergyCost", this._hunchOverEnergyCost.ToString());
            }
        }

        private string _NaturalWeaponBlueprintName = "GiganticFist";
        public string NaturalWeaponBlueprintName 
        {
            get 
            {
                if (this.IsCyberGiant)
                {
                    return ParentObject.Body.GetPartByName("body").Cybernetics.GetPart<CyberneticsGiganticExoframe>().ManipulatorBlueprintName;
                }
                return _NaturalWeaponBlueprintName; 
            }
            private set
            {
                this._NaturalWeaponBlueprintName = value;
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
                Debug.Entry(4, $"Superseding Count is {count}");
                return count > 0;
            }
        }

        public GigantismPlus()
        {
            DisplayName = "{{gigantism|Gigantism}} ({{r|D}})";
            base.Type = "Physical";
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            GigantismPlus gigantism = base.DeepCopy(Parent, MapInv) as GigantismPlus;
            gigantism.GiganticFistObject = null;
            gigantism.GiganticElongatedPawObject = null;
            gigantism.GiganticBurrowingClawObject = null;
            gigantism.GiganticElongatedBurrowingClawObject = null;
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
            if (ParentObject.TryGetPart<StunningForceOnJump>(out StunningForceOnJump stunning))
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
            Debug.Footer(3, "GigantismPlus", "SwapMutationCategory(bool Before = true)");
        } //!--- private void SwapMutationCategory(bool Before = true)

        private bool ShouldRapidAdvance(int Level, GameObject Actor)
        {
            bool IsMutant = Actor.IsMutant();
            bool RapidAdvancement = IsMutant
                                 && (Level + 5) % 10 == 0
                                 && !Actor.IsEsper()
                                 && Mods.GigantismPlus.Options.EnableGigantismRapidAdvance;

            return RapidAdvancement;
        } //!--- private bool ShouldRapidAdvance(int Level, GameObject Actor)

        public override bool WantEvent(int ID, int cascade)
        {
            // InterfaceWithBecomingNook
            // Add once Hunch Over Stat-Shift is implemented: SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == BeforeLevelGainedEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == GetMaxCarriedWeightEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == InventoryActionEvent.ID
                || ID == EarlyBeforeBeginTakeActionEvent.ID;
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
                if (body != null)
                {
                    body.UpdateBodyParts();
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
        {
            E.Features.Add("{{gianter|gigantic}} stature");
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
            Debug.Entry(2, "__________________________________________________________________");
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2,  "@START GigantismPlus.OnRegenerateDefaultEquipment(Body body)");
            Debug.Entry(2, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            FistDamageDieCount = GetFistDamageDieCount(Level);
            FistDamageDieSize = GetFistDamageDieSize(Level);
            FistBaseDamage = GetFistBaseDamage(Level);
            FistHitBonus = GetFistHitBonus(Level);

            if (!this.IsNaturalWeaponSuperseded && body != null)
            {
                Debug.Entry(3, "- NaturalEquipment not Superseded");

                string GiganticBlueprintName = "Gigantic";
                string ElongatedBlueprintName = "Elongated";
                string BaseBlueprintName = "Fist";
                string blueprintName = GiganticBlueprintName;

                Debug.Entry(3, "Generating Stats");

                int dieCount = FistDamageDieCount;
                int dieSize = FistDamageDieSize;
                int damageBonus = 3;
                int maxStrBonus = FistMaxStrengthBonus;
                int hitBonus = FistHitBonus;

                Debug.Entry(3, $"|^ Starting Stats");
                Debug.Entry(3, $"|> dieCount: {dieCount} \n"
                             + $"|> dieSize: {dieSize} \n"
                             + $"|> damageBonus: {damageBonus} \n"
                             + $"|> maxStrBonus: {maxStrBonus} \n"
                             + $"|> hitBonus: {hitBonus}\n"
                             + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");

                bool HasElongated = ParentObject.HasPart<ElongatedPaws>();

                Debug.Entry(3, "Accumulating stats");

                Debug.Entry(4, "* if (HasElongated)");
                if (HasElongated)
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, "+ ElongatedPaws Mutation is present");
                    var elongated = ParentObject.GetPart<ElongatedPaws>();
                    if (elongated != null)
                    {
                        // Add "Elongated" Adjective
                        blueprintName += ElongatedBlueprintName;
                        BaseBlueprintName = "Paw";
                        // add damage
                        damageBonus += elongated.ElongatedBonusDamage;

                        Debug.Entry(4, $"|? blueprintName: {blueprintName}");
                        Debug.Entry(4, $"|> dieCount: {dieCount}\n"
                                     + $"|> dieSize: {dieSize}\n"
                                     + $"|> damageBonus: {damageBonus}\n"
                                     + $"|> maxStrBonus: {maxStrBonus}\n"
                                     + $"|> hitBonus: {hitBonus}\n"
                                     + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                    }
                    else
                    {
                        Debug.Entry(3, "! Failed to instantiate elongated part");
                    }
                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<");
                }
                else
                {
                    Debug.Entry(3, "- ElongatedPaws Mutation not present");
                }

                Debug.Entry(3, "[] Finished accumulating stats");

                blueprintName += BaseBlueprintName;

                Debug.Entry(4, $"|: blueprintName: {blueprintName}");
                Debug.Entry(4, $"|> dieCount: {dieCount} \n"
                             + $"|> dieSize: {dieSize} \n"
                             + $"|> damageBonus: {damageBonus} \n"
                             + $"|> maxStrBonus: {maxStrBonus} \n"
                             + $"|> hitBonus: {hitBonus}\n"
                             + $"|L {WeaponDamageString(dieCount, dieSize, damageBonus)}");
                Debug.Entry(3, "vvvvvvvvvvvvvvvvvvvvvvv");

                Debug.Entry(3, "Performing application of behavior to parts");

                string targetPartType = "Hand";
                Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
                Debug.Entry(4, "Generating List<BodyPart> list");
                // Just change the body part search logic
                List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                       where p.Type == targetPartType  // Changed from VariantType to Type
                                       select p).ToList<BodyPart>();

                Debug.Entry(4, "Checking list of parts for expected entries");
                Debug.Entry(4, "* foreach (BodyPart part in list)");
                foreach (BodyPart part in list)
                {
                    Debug.Entry(4, $"-- {part.Type}");
                    if (part.Type == "Hand")
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Debug.Entry(3, $"--- {part.Type} Found");

                        Debug.Entry(4, "-- Saving copy of current DefaultBehavior in case creation fails");

                        AddAccumulatedNaturalEquipmentTo(
                            Creature: ParentObject,
                            Part: part,
                            BlueprintName: blueprintName,
                            OldDefaultBehavior: part.DefaultBehavior,
                            BaseDamage: WeaponDamageString(dieCount, dieSize, damageBonus),
                            MaxStrBonus: maxStrBonus,
                            HitBonus: hitBonus,
                            AssigningMutation: "GigantismPlus"
                            );

                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        continue;
                    }
                }
                Debug.Entry(3, "x foreach (BodyPart part in list) ]//");
            }
            else
            {
                Debug.Entry(3, "NaturalEquipment is Superseded");
                Debug.Entry(4, "x Aborting GigantismPlus.OnRegenerateDefaultEquipment() Generation of Equipment ]//");
            }

            Debug.Entry(3, "* base.OnRegenerateDefaultEquipment(body)");
            base.OnRegenerateDefaultEquipment(body);

            Debug.Entry(2, "==================================================================");
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

                Debug.Entry(2, "IsPseudoGiganticCreature", (IsPseudoGiganticCreature ? "true" : "false"));
                Debug.Entry(2, "IsGiganticCreature", (IsGiganticCreature ? "true" : "false"));

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
                                this.HunchedOverAbilityUpright + "\n" +
                    "{{W|[}}" + this.HunchedOverAbilityHunched + "{{W|]}}\n" +
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
                    "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}\n" +
                                this.HunchedOverAbilityHunched + "\n" +
                       "}}";
            }

            Debug.Entry(1, "Should be Standing Tall");
        } //!--- public void StraightenUp(bool Message = false)

    } //!--- public class GigantismPlus : BaseDefaultEquipmentMutation

}
