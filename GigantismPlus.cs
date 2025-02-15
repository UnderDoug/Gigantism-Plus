using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using Mods.GigantismPlus;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GigantismPlus : BaseDefaultEquipmentMutation
    {

        public int NaturalWeaponDamageDieCount;

        public int NaturalWeaponDamageDieSize;

        private string NaturalWeaponBaseDamage;

        public int NaturalWeaponHitBonus;

        public int NaturalWeaponDamageBonus;

        public int StrMod => ParentObject.StatMod("Strength");

        public static readonly string[] NaturalWeapons = new string[4] 
        {
            "GiganticFist",                                  // 0: Just Gigantism(0)
            "GiganticElongatedPaw",                          // 1: ElongatedPaws(1)
            "GiganticBurrowingClaw",                         // 2: BurrowingClaws(2)
            "GiganticElongatedBurrowingClaw"                 // 3: ElongatedPaws(1) + BurrowingClaws(2)
            /* These are yet to be added but MUST be added 
             * in the order they appear below. Any other
             * entry in the list can serve as a placeholder
             * just make sure to note what the original was
            "GiganticCrystallinePoint",                      // 4: Crystallinity(4)
            "GiganticElongatedCrystallinePoint",             // 5: ElongatedPaws(1) + Crystallinity(4)
            "GiganticBurrowingCrystallinePoint",             // 6: BurrowingClaws(2) + Crystallinity(4)
            "GiganticElongatedBurrowingCrystallinePoint"     // 7: ElongatedPaws(1) + BurrowingClaws(2) + Crystallinity(4)
            */
        };

        public static readonly string[] CompatibleMutations = new string[3]
        {
            // These are converted to the int value of their "bit position"
            // GigantismPlus is always present, and forms the 0 value.
            "ElongatedPaws",    // 0 -> 1
            "BurrowingClaws",   // 1 -> 2
            "Crystallinity"     // 2 -> 4
        };

        private int _NaturalWeaponIndex;

        private int NaturalWeaponIndex
        {
            get
            {
                _NaturalWeaponIndex = 0;
                string i = "" + _NaturalWeaponIndex;
                Debug.Entry(2, this.GetType().Name + "| NaturalWeaponIndex", i);
                foreach (var entry in CompatibleMutations.Select((Value, Index) => (Value, Index)) )
                {
                    if (ParentObject.HasPart(entry.Value))
                    {
                        // 2^Index converts the index to the int value of its "bit position".
                        // adding these together according to which mutations are present
                        // gives a "unique" ID to each combination.
                        _NaturalWeaponIndex += (int)Math.Pow(2.0, (double)entry.Index);
                    }
                    i = "" + _NaturalWeaponIndex;
                    string j = "" + (int)Math.Pow(2.0, (double)entry.Index);
                    Debug.Entry(2, $"{this.GetType().Name}| NaturalWeaponIndex: {i} | [{j}] {entry.Value}");
                }
                return _NaturalWeaponIndex;
            }
        }

        public GameObject NaturalWeaponObject;

        private string _NaturalWeaponBlueprintName;

        public string NaturalWeaponBlueprintName
        {
            get
            {
                _NaturalWeaponBlueprintName = NaturalWeapons[NaturalWeaponIndex];
                return _NaturalWeaponBlueprintName;
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
        }

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

        public static int GetNaturalWeaponDamageDieCount(int Level)
        {
            return 1 + (int)Math.Floor((double)Level / 5.0);
        }

        public static int GetNaturalWeaponDamageDieSize(int Level)
        {
            return 3 + (int)Math.Floor((double)Level / 3.0);
        }

        public int GetNaturalWeaponDamageBonus()
        {
            int Bonus = StrMod;
            return (int)Math.Floor((double)Bonus / 2.0);
        }

        public string GetNaturalWeaponBaseDamage(int Level)
        {
            return $"{GetNaturalWeaponDamageDieCount(Level)}d{GetNaturalWeaponDamageDieSize(Level)}+{GetNaturalWeaponDamageBonus()}";
        }

        public static int GetNaturalWeaponHitBonus(int Level)
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

        public override bool CanLevel() { return true; } // Enable leveling

        public override bool GeneratesEquipment() { return true; } // Flag Equipment Generation

        public GigantismPlus()
        {
            DisplayName = "{{gigantism|Gigantism}} ({{r|D}})";
            base.Type = "Physical";
        }

        public override string GetDescription()
        {
            return "You are unusually large, will {{rules|struggle to enter small spaces}} without {{g|hunching over}}, and can typically {{rules|only}} use {{gigantic|gigantic}} equipment.\n"
                 + "You are {{rules|heavy}}, can carry {{rules|twice}} as much weight, and all your natural weapons are {{gigantic|gigantic}}.\n\n"
                 + "Your gigantic fists gain:\n"
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
            return "{{gigantic|Gigantic}} Fists {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{W|" + GetNaturalWeaponDamageDieCount(Level) + "}}{{rules|d}}{{B|" + GetNaturalWeaponDamageDieSize(Level) + "}}{{rules|+3}}\n"
                 + "and {{rules|" + GetNaturalWeaponHitBonus(Level) + "}} To-Hit\n"; /*+ "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}";
                 + "{{rules|" + GetHunchedOverQNModifier(Level) + " QN}} and {{rules|" + GetHunchedOverMSModifier(Level) + " MS}} when {{g|Hunched Over}}"; */
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            // TODO: This doesn't plug into anything active yet.
            int HunchedOverAV = GetHunchedOverAVModifier(Level);
            int HunchedOverDV = GetHunchedOverDVModifier(Level);
            int HunchedOverMS = GetHunchedOverMSModifier(Level);
            stats.Set("HunchedOverAV", "+" + HunchedOverAV);
            stats.Set("HunchedOverDV", HunchedOverDV);
            stats.Set("HunchedOverMS", HunchedOverMS);
        }

        public GameObject GenerateNaturalWeapon()
        {
            NaturalWeaponObject = GameObjectFactory.Factory.CreateObject(NaturalWeaponBlueprintName);
            if (NaturalWeaponObject != null)
            {
                MeleeWeapon NaturalWeapon = NaturalWeaponObject.GetPart<MeleeWeapon>();
                NaturalWeapon.BaseDamage = NaturalWeaponBaseDamage;
                NaturalWeapon.HitBonus = NaturalWeaponHitBonus;
            }
            return NaturalWeaponObject;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            // update the Fist properties.
            // update the GiganticFist MeleeWeapon with new properties
            NaturalWeaponDamageDieCount = GetNaturalWeaponDamageDieCount(NewLevel);
            NaturalWeaponDamageDieSize = GetNaturalWeaponDamageDieSize(NewLevel);
            NaturalWeaponBaseDamage = GetNaturalWeaponBaseDamage(NewLevel);
            NaturalWeaponHitBonus = GetNaturalWeaponHitBonus(NewLevel);
            NaturalWeaponDamageBonus = GetNaturalWeaponDamageBonus();

            GenerateNaturalWeapon();

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

        public void AddNaturalWeaponTo(BodyPart part)
        {
            if (part != null)
            {
                // code to skip existing default behaviours if they're blacklisted in the Items.xml file could go here.
                part.DefaultBehavior = NaturalWeaponObject;
            }
        } //!--- public void AddNaturalWeaponTo(BodyPart part)

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            foreach (BodyPart part in body.GetParts())
            {
                if (part.Type == "Hand")
                {
                    AddNaturalWeaponTo(part);
                }
            }

            base.OnRegenerateDefaultEquipment(body);
        } //!--- public override void OnRegenerateDefaultEquipment(Body body)

        private void SwapMutationCategory(bool Before = true)
        {
            // method to swap Gigantism mutation category between Physical and PhysicalDefects
            // - Rapid advancement checks the Physical MutationCategory Entries.

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

        public override bool Mutate(GameObject GO, int Level)
        {
            Body body = GO.Body;
            if (body != null)
            {
                GO.RemovePart<Gigantism>();
                IsGiganticCreature = true; // Enable the Gigantic flag

                ChangeLevel(this.Level);
            }

            if (!GO.HasPart<Vehicle>())
            {
                //  AddActivatedAbility(Name, Command, Class, Description, Icon, DisabledMessage, Toggleable, DefaultToggleState, ActiveToggle, IsAttack, IsRealityDistortionBased, IsWorldMapUsable, Silent, AIDisable, AlwaysAllowToggleOff, AffectedByWillpower, TickPerTurn, Distinct: false, Cooldown, CommandForDescription, UITileDefault, UITileToggleOn, UITileDisabled, UITileCoolingDown);
                EnableActivatedAbilityID =
                    AddMyActivatedAbility(Name: "{{C|" + "{{W|[}}Upright{{W|]}}/Hunched" + "}}",
                                          Command: HUNCH_OVER_COMMAND_NAME,
                                          Class: "Physical Mutations",
                                          Description: null,
                                          Icon: "&#214",
                                          DisabledMessage: null,
                                          Toggleable: true,
                                          DefaultToggleState: false,
                                          ActiveToggle: true, IsAttack: false,
                                          IsRealityDistortionBased: false,
                                          IsWorldMapUsable: false);

                ActivatedAbilityEntry abilityEntry = GO.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = "{{C|" + "{{W|[}}Upright{{W|]}}\nHunched\n" + "}}";
            }
            GO.SyncMutationLevelAndGlimmer();
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (GO != null)
            {
                string ClassAndMethod = this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                Debug.Entry(3, ClassAndMethod, "Game Object not null");
                GO.RemovePart<PseudoGigantism>();
                Debug.Entry(3, ClassAndMethod, "PseudoGigantism Part removed if possible");
                GO.IsGiganticCreature = false; // Revert the Gigantic flag
                Debug.Entry(3, ClassAndMethod, "IsGiganticCreature set to false");
                Body body = GO.Body;
                
                /*
                 * Seeing if commenting this out lets the game do its own garbage collection on the natural weapons.
                 * 
                if (body != null)
                {
                    Debug.Entry(3, ClassAndMethod, "Body not null, about to loop -");
                    foreach (BodyPart part in body.GetParts())
                    {
                        Debug.Entry(4, "- L|" + ClassAndMethod, "PartType is " + part.Type);
                        if (part.Type == "Hand")
                        {
                            part.DefaultBehavior = null;
                            Debug.Entry(3, "- - L|"+ ClassAndMethod, part.Type + " DefaultBehaviour nulled");
                        }
                    }
                }
                */

                if (EnableActivatedAbilityID != Guid.Empty)
                {
                    RemoveMyActivatedAbility(ref EnableActivatedAbilityID);
                }
            }
            return base.Unmutate(GO);
        }

        public override void AfterUnmutate(GameObject GO)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (GO != null)
            {
                Body body = GO.Body;
                
                CheckAffected(GO, body);
            }
            ParentObject.SyncMutationLevelAndGlimmer();
        }

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
        } //!--- public void CheckAffected(GameObject Actor, Body Body)

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

                ToggleMyActivatedAbility(EnableActivatedAbilityID, null, Silent: true, null);
                Debug.Entry(3, "Hunch Ability Toggled");

                Debug.Entry(3, "Proceeding to Hunch Ability Effects");
                if (IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID))
                {
                    // Hunch
                    HunchOver(true);
                }
                else
                {
                    // Stand upright
                    StraightenUp(true);
                }
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
                abilityEntry.DisplayName = "{{C|" + "Upright\n{{W|[}}Hunched{{W|]}}\n" + "}}";

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
                abilityEntry.DisplayName = "{{C|" + "{{W|[}}Upright{{W|]}}\nHunched\n" + "}}";

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
            /*
            if (!base.WantEvent(ID, cascade) && ID != SingletonEvent<AfterGameLoadedEvent>.ID && ID != PooledEvent<PartSupportEvent>.ID && ID != PooledEvent<PreferDefaultBehaviorEvent>.ID)
            {
                return ID == SingletonEvent<BeforeAbilityManagerOpenEvent>.ID;
            }
            */
            // Check if the ID parameter matches
            // or if a Wanted Event.ID comes through
            // SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == BeforeLevelGainedEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == GetMaxCarriedWeightEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == InventoryActionEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == StatChangeEvent.ID
                || ID == SyncMutationLevelsEvent.ID;
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
            E.Features.Add("{{gianter|gigantic}} stature");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(CanEnterInteriorEvent E)
        {
            Debug.Entry(1, "Checking CanEnterInteriorEvent");
            if (ParentObject == E.Object)
            {
                Debug.Entry(1, "Parent Object is the Target of Entry, Skip to base CanEnterInteriorEvent");
                return base.HandleEvent(E);
            }
            GameObject actor = E.Actor;
            if (actor != null && actor.IsGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(2, "We are big, gonna HunchOver");
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

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Name == "Strength")
            {
                Debug.Entry(2, this.GetType().Name, "StatChangeEvent, \"Strength\"");
                ChangeLevel(this.Level);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(SyncMutationLevelsEvent E)
        {
            Debug.Entry(2, this.GetType().Name, "SyncMutationLevelsEvent");

            ChangeLevel(this.Level);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            // DescribeMyActivatedAbility(EnableActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }

    } //!--- public class GigantismPlus : BaseDefaultEquipmentMutation

}