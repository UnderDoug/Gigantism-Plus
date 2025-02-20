using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
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
                if (ParentObject != null)
                    return ParentObject.HasPart<CyberneticsMassiveExoframe>();
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
        
        private bool IsHunchFree = false;

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
                    return ParentObject.GetPart<CyberneticsMassiveExoframe>().ManipulatorBlueprintName;
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
                return count > 0;
            }
        }

        public GigantismPlus()
        {
            DisplayName = "{{gigantism|Gigantism}} ({{r|D}})";
            base.Type = "Physical";
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

            // Add once Hunch Over Stat-Shift is implemented: SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == BeforeLevelGainedEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == GetMaxCarriedWeightEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID;
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

        public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
        {
            E.Features.Add("{{gianter|gigantic stature}}");
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

        /* This was part of the code we were using, I thought, to enable entering interiors while gigantic.
         * Debug-logging revealed that it wasn't firing at all. Leaving it here for the time being.
         * 
        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "EnterInterior")
            {
                Debug.Entry("A) Attempting InteriorEntry");
                GameObject actor = E.Actor;
                if (actor.IsGiganticCreature && !actor.HasPart<Vehicle>())
                {
                    Debug.Entry("A)A) We are big, so we'll HunchOver");
                    IsHunchFree = true;
                    CommandEvent.Send(actor, HUNCH_OVER_COMMAND_NAME);
                    Debug.Entry("A)A)A) HunchOver Sent for Enter InventoryActionEvent");
                }
                else
                {
                    Debug.Entry("A)A) InventoryActionEvent - We aren't big");
                }
                Debug.Entry("A)A)A)A) Sending to base InventoryActionEvent");
            }
            return base.HandleEvent(E);
        }
        */

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
            string WeaponName = this.NaturalWeaponBlueprint.DisplayName();
            return WeaponName + " {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{W|" + GetFistDamageDieCount(Level) + "}}{{rules|d}}{{B|" + GetFistDamageDieSize(Level) + "}}{{rules|+3}}\n"
                 + "and {{rules|" + GetFistHitBonus(Level) + "}} To-Hit\n"; /*+ "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}";
                 + "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}"; */
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Body body = GO.Body;
            if (body != null)
            {
                GO.RemovePart<Gigantism>();
                IsGiganticCreature = true; // Enable the Gigantic flag
            }

            if (!GO.HasPart<Vehicle>())
            {
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

                ActivatedAbilityEntry abilityEntry = GO.GetActivatedAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = 
                    "{{C|" + 
                    "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}\n" +
                                this.HunchedOverAbilityHunched + "\n" +
                       "}}";

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

            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
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

            return base.Unmutate(GO);
        }

        public void AddGiganticNaturalEquipmentTo(BodyPart part)
        {
            Debug.Entry(2, "**AddGiganticNaturalEquipmentTo(BodyPart part)");
            if (part != null && part.Type == "Hand")
            {
                Debug.Entry(3, "**if (ParentObject.HasPart<ElongatedPaws>())");
                Debug.Entry(3, "**else if (ParentObject.HasPart<BurrowingClaws>())");
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

                    /*Debug.Entry(4, "**if (GiganticFistObject == null)");
                    if (GiganticFistObject == null)
                    {
                        Debug.Entry(3, "--- GiganticFistObject was null, init");*/
                        GiganticFistObject = GameObjectFactory.Factory.CreateObject(NaturalWeaponBlueprint);/*
                    }*/
                    part.DefaultBehavior = GiganticFistObject;
                    var weapon = GiganticFistObject.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = FistBaseDamage;
                    weapon.HitBonus = FistHitBonus;
                    weapon.MaxStrengthBonus = FistMaxStrengthBonus;

                    Debug.Entry(4, "**part.DefaultBehavior = GiganticFistObject");
                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                }//GiganticFistObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
            }
            else
            {
                Debug.Entry(2, "part null or not hand");
            }
            Debug.Entry(2, "xxAddGiganticNaturalEquipmentTo(BodyPart part)");
        } //!--- public void AddGiganticFistTo(BodyPart part)

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Debug.Entry(2, "__________________________________________________________________");
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[No Zone?]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2, "**GigantismPlus.OnRegenerateDefaultEquipment(Body body)");
            Debug.Entry(2, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            FistDamageDieCount = GetFistDamageDieCount(Level);
            FistDamageDieSize = GetFistDamageDieSize(Level);
            FistBaseDamage = GetFistBaseDamage(Level);
            FistHitBonus = GetFistHitBonus(Level);

            if (!this.IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, "- NaturalEquipment not Superseded");

                Debug.Entry(3, "**foreach (BodyPart hand in body.GetParts())\n**if (hand.Type == \"Hand\")");
                foreach (BodyPart hand in body.GetParts(EvenIfDismembered: true))
                {
                    Debug.Entry(4, $"-- {hand.Type}");
                    if (hand.Type == "Hand")
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Debug.Entry(3, $"--- {hand.Type} Found");

                        AddGiganticNaturalEquipmentTo(hand);

                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    }
                }
                Debug.Entry(3, "xxforeach (BodyPart hand in body.GetParts())");
            }
            else
            {
                Debug.Entry(3, "NaturalEquipment is Superseded");
                Debug.Entry(4, "xxAborting GigantismPlus.OnRegenerateDefaultEquipment() Generation of Equipment");
            }

            Debug.Entry(3, "**base.OnRegenerateDefaultEquipment(body)");
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

                actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_negativeVitality");
                Popup.Show("You stand tall, relaxing into your immense stature.");

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