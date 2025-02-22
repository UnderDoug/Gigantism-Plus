using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;

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
                        return cybernetics.GetBlueprint().Inherits == "BaseMassiveExoframe";
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
        
        public bool IsHunchFree = false;

        public bool UnHunchNextTurn = false;

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
                    return ParentObject.Body.GetPartByName("body").Cybernetics.GetPart<CyberneticsMassiveExoframe>().ManipulatorBlueprintName;
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
          //"MassiveExoframe",
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

            // Straighten up if hunching.
            // update HunchOver ability stats.
            // Hunch over if hunched before level up.
            bool WasHunched = false;
            if (IsPseudoGiganticCreature && !IsVehicleCreature)
            {
                WasHunched = true;
                IsHunchFree = true;
                StraightenUp(Message: false);
            }
            
            HunchedOverAVModifier = GetHunchedOverAVModifier(NewLevel);
            HunchedOverDVModifier = GetHunchedOverDVModifier(NewLevel);
            HunchedOverMSModifier = GetHunchedOverMSModifier(NewLevel);
            
            if (WasHunched && !IsVehicleCreature)
            {
                IsHunchFree = true;
                HunchOver(Message: false);
            }

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
            // prefer this for repeated uses of strings.
            string Physical = "Physical";
            string PhysicalDefects = "PhysicalDefects";

            // direction of swap depends on whether before or after LevelGain
            string IntoCategory = Before ? Physical : PhysicalDefects;
            string OutOfCategory = Before ? PhysicalDefects : Physical;
            MutationEntry GigantismEntry = MutationFactory.GetMutationEntryByName(this.Name);
            foreach (MutationCategory category in MutationFactory.GetCategories())
            {
                if (category.Name == IntoCategory)
                {
                    // UnityEngine.Debug.LogError("Adding " + GigantismEntry.DisplayName + " to " + IntoCategory + "Category");
                    category.Add(GigantismEntry);
                    category.Entries.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
                    /* Debug Logging. May turn this into an option.
                    foreach (MutationEntry entry in category.Entries)
                    {
                        UnityEngine.Debug.LogError(entry.DisplayName);
                    }*/
                }
                if (category.Name == OutOfCategory)
                {
                    // UnityEngine.Debug.LogError("Removing " + GigantismEntry.DisplayName + " from " + OutOfCategory + "Category");
                    category.Entries.RemoveAll(r => r == GigantismEntry);
                }
            }
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
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            return base.HandleEvent(E);
            /* Currently being handled elsewhere.
             * 
            // Should let you install cybernetics that are a disparate size to you.
            if (E.Object.HasPart<CyberneticsBaseItem>())
            {
                if (!E.Actor.IsGiganticCreature && E.Object.IsGiganticEquipment)
                    E.Decreases++;
                else if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment)
                    E.Increases++;

                E.CanBeTooSmall = false;
            }
            return base.HandleEvent(E);
            */
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

        public override bool HandleEvent(EarlyBeforeBeginTakeActionEvent E)
        {
            if(this.UnHunchNextTurn == true && ParentObject != null)
            {
                Debug.Entry(3, "...........................................");
                Debug.Entry(4, "**public override bool HandleEvent(EarlyBeforeBeginTakeActionEvent E)");
                Debug.Entry(3, "Automatically Unhunching");
                UnHunchNextTurn = false;
                IsHunchFree = true;
                CommandEvent.Send(ParentObject, HUNCH_OVER_COMMAND_NAME);
                Debug.Entry(4, "x- Deferring to base.HandleEvent(E)");
                Debug.Entry(3, "...........................................");
            }
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
            string WeaponName = this.NaturalWeaponBlueprint.DisplayName();
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

            string WeaponName = this.NaturalWeaponBlueprint.DisplayName();
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
            }

            if (!GO.HasPart<Vehicle>())
            {
                Debug.Entry(2, "- Mutating: Not Vehicle");
                /* AddActivatedAbility() - Full Method Arguments.
                 * AddActivatedAbility(Name, Command, Class, Description, Icon, DisabledMessage, Toggleable, DefaultToggleState, ActiveToggle, IsAttack, IsRealityDistortionBased, IsWorldMapUsable, Silent, AIDisable, AlwaysAllowToggleOff, AffectedByWillpower, TickPerTurn, Distinct: false, Cooldown, CommandForDescription, UITileDefault, UITileToggleOn, UITileDisabled, UITileCoolingDown); */
                EnableActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: "{{C|" + "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}/" + this.HunchedOverAbilityUpright + "}}",
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
                StraightenUp();
                GO.RemovePart<PseudoGigantism>();
                GO.IsGiganticCreature = false; // Revert the Gigantic flag

                if (EnableActivatedAbilityID != Guid.Empty)
                {
                    RemoveMyActivatedAbility(ref EnableActivatedAbilityID);
                }

                CheckAffected(GO, GO.Body);
            }

            Debug.Entry(2, $"GigantismPlus -> base.Unmutate {GO.DebugName}");
            return base.Unmutate(GO);
        }

        public void AddGiganticNaturalEquipmentTo(BodyPart Part, string BlueprintName, GameObject OldDefaultBehaviour, int DieCount, int DieSize, int DamageBonus, int MaxStrBonus, int HitBonus)
        {
            Debug.Entry(2, "* AddGiganticNaturalEquipmentTo(BodyPart part)");
            if (Part != null && Part.Type == "Hand")
            {
                Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);

                string baseDamage = $"{DieCount}d{DieSize}";
                if (DamageBonus > 0)
                {
                    baseDamage += $"+{DamageBonus}";
                }

                if (Part.DefaultBehavior != null)
                {
                    Debug.Entry(3, "---- Part.DefaultBehaviour not null, assigning stats");
                    Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);

                    MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = baseDamage;
                    if (HitBonus != 0) weapon.HitBonus = HitBonus;
                    weapon.MaxStrengthBonus = MaxStrBonus;

                    Debug.Entry(4, $"---- hand.DefaultBehavior = {BlueprintName}");
                    Debug.Entry(4, $"---- MaxStrBonus: {weapon.MaxStrengthBonus} | Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus}");
                }
                else
                {
                    Debug.Entry(3, $"---- part.DefaultBehaviour was null, invalid blueprint name \"{BlueprintName}\"");
                    Part.DefaultBehavior = OldDefaultBehaviour;
                    Debug.Entry(3, $"---- OldDefaultBehaviour reassigned");
                }
                /* Old Code. Bypassing for now.
                 * 
                Debug.Entry(3, "* if (ParentObject.HasPart<ElongatedPaws>())");
                Debug.Entry(3, "* else if (ParentObject.HasPart<BurrowingClaws>())");
                if (ParentObject.HasPart<ElongatedPaws>())
                {
                    Debug.Entry(3, "-- ElongatedPaws is Present");
                    Debug.Entry(4, "**if (ParentObject.HasPart<BurrowingClaws>())");
                    if (ParentObject.HasPart<BurrowingClaws>())
                    {
                        Debug.Entry(3, "--- BurrowingClaws is Present");
                        var burrowingClaws = ParentObject.GetPart<BurrowingClaws>();
                        int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                        int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);

                        Debug.Entry(4, "**if (GiganticElongatedBurrowingClawObject == null)");
                        if (GiganticElongatedBurrowingClawObject == null)
                        {
                            Debug.Entry(3, "---- GiganticElongatedBurrowingClawObject was null, init");
                            GiganticElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedBurrowingClaw");
                        }
                        part.DefaultBehavior = GiganticElongatedBurrowingClawObject;
                        part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "GigantismPlus", false);
                        var elongatedPaws = ParentObject.GetPart<ElongatedPaws>();
                        var weapon = GiganticElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{FistDamageDieCount}d{FistDamageDieSize}+{(elongatedPaws.StrengthModifier / 2) + 3 + burrowingBonus}";
                        weapon.HitBonus = FistHitBonus;
                        weapon.MaxStrengthBonus = FistMaxStrengthBonus;

                        Debug.Entry(4, "**part.DefaultBehavior = GiganticElongatedBurrowingClawObject");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                    }//GiganticElongatedBurrowingClawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                    else
                    {
                        Debug.Entry(3, "--- BurrowingClaws not Present");
                        Debug.Entry(4, "**if (GiganticElongatedPawObject == null)");
                        if (GiganticElongatedPawObject == null)
                        {
                            Debug.Entry(3, "---- GiganticElongatedPawObject was null, init");
                            GiganticElongatedPawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedPaw");
                        }
                        part.DefaultBehavior = GiganticElongatedPawObject;
                        part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "GigantismPlus", false);
                        var elongatedPaws = ParentObject.GetPart<ElongatedPaws>();
                        var weapon = GiganticElongatedPawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{FistDamageDieCount}d{FistDamageDieSize}+{(elongatedPaws.StrengthModifier / 2) + 3}";
                        weapon.HitBonus = FistHitBonus;
                        weapon.MaxStrengthBonus = FistMaxStrengthBonus;

                        Debug.Entry(4, "**part.DefaultBehavior = GiganticElongatedPawObject");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                    }//GiganticElongatedPawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                }
                else if (ParentObject.HasPart<BurrowingClaws>())
                {
                    Debug.Entry(3, "-- ElongatedPaws not Present");
                    Debug.Entry(3, "-- BurrowingClaws is Present");
                    var burrowingClaws = ParentObject.GetPart<BurrowingClaws>();
                    int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                    int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);

                    Debug.Entry(4, "**if (GiganticBurrowingClawObject == null)");
                    if (GiganticBurrowingClawObject == null)
                    {
                        Debug.Entry(3, "--- GiganticBurrowingClawObject was null, init");
                        GiganticBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticBurrowingClaw");
                    }
                    part.DefaultBehavior = GiganticBurrowingClawObject;
                    part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "GigantismPlus", false);
                    var weapon = GiganticBurrowingClawObject.GetPart<MeleeWeapon>();
                    string baseDamage = GetFistBaseDamage(burrowingClaws.Level);
                    int plusIndex = baseDamage.LastIndexOf('+');
                    if (plusIndex != -1)
                    {
                        int baseBonus = int.Parse(baseDamage.Substring(plusIndex + 1));
                        weapon.BaseDamage = $"{baseDamage.Substring(0, plusIndex)}+{baseBonus + burrowingBonus}";
                    }
                    else
                    {
                        weapon.BaseDamage = $"{baseDamage}+{burrowingBonus}";
                    }
                    weapon.HitBonus = FistHitBonus;
                    weapon.MaxStrengthBonus = FistMaxStrengthBonus;

                    Debug.Entry(4, "**part.DefaultBehavior = GiganticBurrowingClawObject");
                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                }//GiganticBurrowingClawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                else
                {
                    Debug.Entry(3, "-- ElongatedPaws not Present");
                    Debug.Entry(3, "-- BurrowingClaws not Present");

                    Debug.Entry(4, "**if (GiganticFistObject == null)");
                    if (GiganticFistObject == null)
                    {
                        Debug.Entry(3, "--- GiganticFistObject was null, init");
                        GiganticFistObject = GameObjectFactory.Factory.CreateObject(NaturalWeaponBlueprint);
                    }
                    part.DefaultBehavior = GiganticFistObject;
                    part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "GigantismPlus", false);
                    var weapon = GiganticFistObject.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = FistBaseDamage;
                    weapon.HitBonus = FistHitBonus;
                    weapon.MaxStrengthBonus = FistMaxStrengthBonus;

                    Debug.Entry(4, "**part.DefaultBehavior = GiganticFistObject");
                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                }//GiganticFistObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                */
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"");
            }

            Debug.Entry(2, "x AddGiganticNaturalEquipmentTo(BodyPart part) ]//");
        } //!--- public void AddGiganticFistTo(BodyPart part)

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
                GameObject OldDefaultBehaviour = null;

                Debug.Entry(3, "Generating Stats");

                int dieCount = FistDamageDieCount;
                int dieSize = FistDamageDieSize;
                int damageBonus = 3;
                int maxStrBonus = FistMaxStrengthBonus;
                int hitBonus = FistHitBonus;

                Debug.Entry(3, $"dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                             + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

                bool HasElongated = ParentObject.HasPart<ElongatedPaws>();

                Debug.Entry(3, "Accumulating stats");

                Debug.Entry(4, "* if (HasElongated)");
                if (HasElongated)
                {
                    Debug.Entry(3, "- ElongatedPaws Mutation is present");
                    var elongated = ParentObject.GetPart<ElongatedPaws>();
                    if (elongated != null)
                    {
                        blueprintName += ElongatedBlueprintName;
                        Debug.Entry(4, $"> blueprintName: {blueprintName}");
                        BaseBlueprintName = "Paw";
                        Debug.Entry(4, $"> BaseBlueprintName: {BaseBlueprintName}");

                        damageBonus += elongated.ElongatedBonusDamage;
                        Debug.Entry(4, $"- damageBonus: {damageBonus}");

                        Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                                     + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");
                    }
                }
                Debug.Entry(3, "Finished accumulating stats");

                Debug.Entry(4, $"- dieCount: {dieCount} | dieSize: {dieSize} | damageBonus: {damageBonus}\n"
                             + $"maxStrBonus: {maxStrBonus} | hitBonus: {hitBonus}");

                blueprintName += BaseBlueprintName;

                Debug.Entry(3, $"> blueprintName: {blueprintName}");

                Debug.Entry(3, "* foreach (BodyPart hand in body.GetParts(EvenIfDismembered: true))\n* if (hand.Type == \"Hand\")");
                foreach (BodyPart part in body.GetParts(EvenIfDismembered: true))
                {
                    Debug.Entry(4, $"-- {part.Type}");
                    if (part.Type == "Hand")
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Debug.Entry(3, $"--- {part.Type} Found");

                        Debug.Entry(4, "-- Saving copy of current DefaultBehaviour in case creation fails");
                        OldDefaultBehaviour = part.DefaultBehavior;

                        AddGiganticNaturalEquipmentTo(
                        Part: part,
                        BlueprintName: blueprintName,
                        OldDefaultBehaviour: OldDefaultBehaviour,
                        DieCount: dieCount,
                        DieSize: dieSize,
                        DamageBonus: damageBonus,
                        MaxStrBonus: maxStrBonus,
                        HitBonus: hitBonus
                        );

                        // Add the EF-enforced tag to the beginning of the name of the current natural weapon
                        if (part.DefaultBehavior != null && ParentObject.HasPart<CyberneticsMassiveExoframe>())
                        {
                            string color = "Y"; // Default color
                            var exoframe = ParentObject.GetPart<CyberneticsMassiveExoframe>();
                            color = exoframe.Model switch
                            {
                                "Alpha" => "b",
                                "Delta" => "K",
                                "Sigma" => "G",
                                "Omega" => "zetachrome",
                                _ => "Y"
                            };
                            part.DefaultBehavior.DisplayName = $"{{Y|E{{c|F}}-{{{color}|enforced}} {part.DefaultBehavior.DisplayName}";
                            Debug.Entry(3, $"Updated DisplayName to: {part.DefaultBehavior.DisplayName}");
                        }

                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        continue;
                    }
                }
                Debug.Entry(3, "x foreach (BodyPart hand in body.GetParts(EvenIfDismembered: true)) ]//");
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
                UnHunchNextTurn = false;
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

                // Old weight change code. Keeping just in case.
                /*
                int baseWeight = actor.GetBodyWeight();
                int WeightAdjustment = baseWeight - (int)Math.Floor((double)baseWeight / 5);
                int _Weight = actor.Physics._Weight;
                actor.Physics._Weight = _Weight - WeightAdjustment;
                Debug.Entry(3,"baseWeight", baseWeight.ToString());
                Debug.Entry(3,"_Weight", _Weight.ToString());
                Debug.Entry(3,"Adjustment", WeightAdjustment.ToString());
                Debug.Entry(3,"New Weight", actor.Physics._Weight.ToString());
                */
            }

            Debug.Entry(1, "Should be Standing Tall");
        } //!--- public void StraightenUp(bool Message = false)

    } //!--- public class GigantismPlus : BaseDefaultEquipmentMutation

}