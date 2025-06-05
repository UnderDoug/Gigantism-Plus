using HNPS_GigantismPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager 
        : IScribedPart
        , IModEventHandler<BeforeBodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(NaturalEquipmentManager));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "OC",   // ObjectCreation
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "S"     // Serialisation
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public bool WantsToManage => ParentObject != null && ParentObject.IsNaturalEquipment();

        public bool HasManaged = false;

        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public GameObjectBlueprint DefaultFistBlueprint => GameObjectFactory.Factory.GetBlueprint("DefaultFist");
        
        public DieRoll DamageDie;

        [NonSerialized]
        public (int Count, int Size, int Bonus) AccumulatedDamageDie = (0, 0, 0);
        public int AccumulatedHitBonus = 0;
        public int AccumulatedPenBonus = 0;

        public bool DoDynamicTile = true;

        private BodyPart _parentLimb = null;

        public BodyPart ParentLimb => _parentLimb ??= ParentObject?.EquippingPart();

        private GameObject _wielder = null;
        public GameObject Wielder => _wielder ??= ParentObject?.Equipped;

        private Render _parentRender = null;
        public Render ParentRender => _parentRender ??= ParentObject?.GetPart<Render>();

        private MeleeWeapon _parentMeleeWeapon = null;
        public MeleeWeapon ParentMeleeWeapon => _parentMeleeWeapon ??= ParentObject?.GetPart<MeleeWeapon>();

        private Armor _parentArmor = null;
        public Armor ParentArmor => _parentArmor ??= ParentObject?.GetPart<Armor>();

        [NonSerialized]
        public SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions;

        [SerializeField]
        private string _shortDescriptionCache = null;


        /// <summary>
        /// Key: string (name of Target object) <br></br>
        /// Value: Tuple( TargetObject, Entry ) <br></br>
        /// - TargetObject: per ModNaturalEquipmentBase.HNPS_Adjustment class, one of: [GameObject] (the equipment itself), [Render], [MeleeWeapon], [Armor] <br></br>
        /// - Entry: Dictionary, <br></br>
        /// - - Key: string (field/property being targeted) <br></br>
        /// - - Value: Tuple( Priority, Value ), <br></br>
        /// - - - Priority: self-explanitory <br></br>
        /// - - - Value: the value to which the field is to be set
        /// </summary>
        // [NonSerialized]
        // public Dictionary<string, (object TargetObject, Dictionary<string, (int Priority, object Value)> Entry)> AdjustmentTargets;

        public NaturalEquipmentManager()
        {
            AccumulatedDamageDie = (0, 0, 0);
            AccumulatedHitBonus = 0;
            AccumulatedPenBonus = 0;
            NaturalEquipmentMods = new();
            ShortDescriptions = new();
            // AdjustmentTargets = GetEmptyAdjustmentTargets();
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
        }
        public override void Remove()
        {
            ResetShortDescriptions();
            ClearAdjustmentTargets();
            ClearNaturalWeaponMods();
            base.Remove();
        }

        public SortedDictionary<int, ModNaturalEquipmentBase> GetShortDescriptionEntries()
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(GetShortDescriptionEntries)}()",
                Indent: 4, Toggle: doDebug);
            SortedDictionary<int, ModNaturalEquipmentBase> shortDescriptions = new();
            List<ModNaturalEquipmentBase> attachedNaturalEquipmentMods = ParentObject.GetNaturalEquipmentMods();
            if (attachedNaturalEquipmentMods != null)
            {
                foreach (ModNaturalEquipmentBase attachedNaturalEquipmentMod in attachedNaturalEquipmentMods)
                {
                    Debug.Entry(4,
                        $"NaturalWeaponMod: {attachedNaturalEquipmentMod.Name}[{attachedNaturalEquipmentMod.GetAdjective()}])",
                        Indent: 4, Toggle: doDebug);

                    ShortDescriptions[attachedNaturalEquipmentMod.DescriptionPriority] = attachedNaturalEquipmentMod;
                    Debug.Entry(4, $"ShortDescriptions:", Indent: 5, Toggle: doDebug);

                    foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                    {
                        Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}[{naturalEquipmentMod.GetAdjective()}]", Indent: 5, Toggle: doDebug);
                    }
                }
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(GetShortDescriptionEntries)}()"
                + $" @//",
                Indent: 4, Toggle: doDebug);

            return shortDescriptions;
        }
        public string ProcessShortDescription(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions)
        {
            Debug.Entry(4,
                $"* {nameof(NaturalEquipmentManager)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions)",
                Indent: 1, Toggle: doDebug);

            StringBuilder StringBuilder = Event.NewStringBuilder();

            if (!ShortDescriptions.IsNullOrEmpty())
            {
                foreach ((int priority, ModNaturalEquipmentBase mod) in ShortDescriptions)
                {
                    StringBuilder.AppendRules(mod.GetInstanceDescription(ParentObject));
                    Debug.CheckYeh(4, $"{priority}::{mod.GetSource()}:Description Appended", Indent: 1, Toggle: doDebug);
                }
            }
            
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions) *//",
                Indent: 1, Toggle: doDebug);
            return Event.FinalizeString(StringBuilder);
        }

        public void ClearShortDescriptionCache()
        {
            _shortDescriptionCache = null;
        }
        public void ClearShortDescriptions()
        {
            ShortDescriptions = new();
        }
        public void ClearNaturalWeaponMods()
        {
            // Debug.Entry(4, $"* {nameof(ClearNaturalWeaponMods)}()", Indent: Debug.LastIndent, Toggle: getDoDebug('R'));
            // NaturalEquipmentMods = new();
        }
        public void ClearAdjustmentTargets()
        {
            // Debug.Entry(4, $"* {nameof(ClearAdjustmentTargets)}()", Indent: Debug.LastIndent, Toggle: getDoDebug('R'));
            // AdjustmentTargets = GetEmptyAdjustmentTargets();
        }
        public void ResetShortDescriptions()
        {
            Debug.Entry(4, $"* {nameof(ResetShortDescriptions)}()", Indent: Debug.LastIndent, Toggle: getDoDebug('R'));
            ClearShortDescriptionCache();
            ClearShortDescriptions();
        }

        public SortedDictionary<int, ModNaturalEquipmentBase> GetNaturalEquipmentMods()
        {
            return GetPrioritisedNaturalEquipmentModsEvent.GetFor(Wielder, ParentObject, ParentLimb);
        }
        public List<HNPS_AdjustmentBase> GetPrioritisedNaturalEquipmentModAdjustments(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, 
                $"* {nameof(GetPrioritisedNaturalEquipmentModAdjustments)}"
                + $"(List<ModNaturalEquipmentBase> NaturalEquipmentMods)", 
                Indent: indent, Toggle: getDoDebug());

            List<HNPS_AdjustmentBase> Adjustments = new();

            if (HasManaged)
            {
                NaturalEquipmentMods ??= ParentObject.GetNaturalEquipmentMods();
            }
            if (NaturalEquipmentMods != null)
            {
                Debug.Entry(4, $"> foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)", Indent: indent + 1, Toggle: getDoDebug());
                foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
                {
                    Debug.Divider(4, HONLY, Count: 60, Indent: indent + 2, Toggle: getDoDebug());
                    Debug.Entry(4, $"naturalEquipmentMod", naturalEquipmentMod.GetAdjective(), Indent: indent + 2, Toggle: getDoDebug());
                    if (naturalEquipmentMod?.Adjustments != null)
                    {
                        Debug.CheckYeh(4, $"Have Adjustments", Indent: indent + 3, Toggle: getDoDebug());
                        Debug.Entry(4, $"> foreach (HNPS_AdjustmentBase adjustment in naturalEquipmentMod.Adjustments)", Indent: indent + 3, Toggle: getDoDebug());
                        foreach (HNPS_AdjustmentBase adjustment in naturalEquipmentMod.Adjustments)
                        {
                            Debug.Divider(4, HONLY, Count: 40, Indent: indent + 4, Toggle: getDoDebug());
                            Debug.LoopItem(4, $"Target: {adjustment.Target}, Field: {adjustment.Target}", Indent: indent + 4, Toggle: getDoDebug());
                            if (Adjustments.IsNullOrEmpty())
                            {
                                Debug.CheckYeh(4, $"Adjustments Empty, Adding {adjustment}", Indent: indent + 4, Toggle: getDoDebug());
                                Adjustments = new()
                                {
                                    adjustment
                                };
                                continue;
                            }
                            List<HNPS_AdjustmentBase> storedAdjustments = new(Adjustments);

                            bool didStore = false;
                            Debug.Entry(4, $"> foreach (HNPS_AdjustmentBase storedAdjustment in storedAdjustments)", Indent: indent + 4, Toggle: getDoDebug());
                            foreach (HNPS_AdjustmentBase storedAdjustment in storedAdjustments)
                            {
                                Debug.Divider(4, HONLY, Count: 25, Indent: indent + 5, Toggle: getDoDebug());
                                Debug.Entry(4, $"Proposed", $"{adjustment}", Indent: indent + 5, Toggle: doDebug);
                                Debug.Entry(4, $"Existing", $"{storedAdjustment}", Indent: indent + 5, Toggle: doDebug);
                                if (adjustment.TryGetHigherPriorityAdjustment(storedAdjustment, out HNPS_AdjustmentBase replacementAdjustment))
                                {
                                    string debugText = $"Existing Adjustment is Higher Priority";
                                    if (storedAdjustment != replacementAdjustment)
                                    {
                                        debugText = $"Proposed Adjustment is Higher Priority";
                                        Adjustments.Remove(storedAdjustment);
                                        Adjustments.TryAdd(replacementAdjustment);
                                    }
                                    Debug.LoopItem(4, debugText, $"{replacementAdjustment}",
                                        Good: storedAdjustment != replacementAdjustment, Indent: indent + 6, Toggle: getDoDebug());

                                    didStore = true;
                                    break;
                                }
                                Debug.Divider(4, HONLY, Count: 25, Indent: indent + 5, Toggle: getDoDebug());
                                Debug.Entry(4, $"x foreach (HNPS_AdjustmentBase storedAdjustment in storedAdjustments) >//", Indent: indent + 4, Toggle: getDoDebug());
                            }
                            if (!didStore)
                            {
                                Debug.CheckYeh(4, $"No Competing Adjustments, Adding {adjustment}", Indent: indent + 4, Toggle: getDoDebug());
                                Adjustments.TryAdd(adjustment);
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 40, Indent: indent + 4, Toggle: getDoDebug());
                        Debug.Entry(4, $"x foreach (HNPS_AdjustmentBase adjustment in naturalEquipmentMod.Adjustments) >//", Indent: 2, Toggle: getDoDebug());
                    }
                }
                Debug.Divider(4, HONLY, Count: 60, Indent: indent + 2, Toggle: getDoDebug());
            }

            Debug.Entry(4,
                $"x {nameof(GetPrioritisedNaturalEquipmentModAdjustments)}"
                + $"(List<ModNaturalEquipmentBase> NaturalEquipmentMods)"
                + $" *//",
                Indent: indent, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return Adjustments;
        }

        public virtual SortedDictionary<int, ModNaturalEquipmentBase> AccumulateMeleeWeaponBonuses(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods)
        {
            Debug.Entry(4, $"* {nameof(AccumulateMeleeWeaponBonuses)}()", Indent: 2, Toggle: doDebug);

            Debug.Entry(4, $"> foreach ((_,{nameof(ModNaturalEquipmentBase)} naturalEquipmentMod) in {nameof(NaturalEquipmentMods)})", Indent: 3, Toggle: doDebug);
            Debug.Divider(4, HONLY, 25, Indent: 4, Toggle: doDebug);
            foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                AccumulatedDamageDie.Count += naturalEquipmentMod.GetDamageDieCount();
                AccumulatedDamageDie.Size += naturalEquipmentMod.GetDamageDieSize();
                AccumulatedDamageDie.Bonus += naturalEquipmentMod.GetDamageBonus();
                AccumulatedHitBonus += naturalEquipmentMod.GetHitBonus();
                AccumulatedPenBonus += naturalEquipmentMod.GetPenBonus();

                Debug.Entry(4, $"{naturalEquipmentMod.Name}[{naturalEquipmentMod.GetAdjective()}]", Indent: 4, Toggle: doDebug);
                Debug.CheckYeh(4, $"DamageDieCount", $"{naturalEquipmentMod.GetDamageDieCount().Signed()}", Indent: 4, Toggle: doDebug);
                Debug.CheckYeh(4, $"DamageDiesize", $" {naturalEquipmentMod.GetDamageDieSize().Signed()}", Indent: 4, Toggle: doDebug);
                Debug.CheckYeh(4, $"DamageBonus", $"   {naturalEquipmentMod.GetDamageBonus().Signed()}", Indent: 4, Toggle: doDebug);
                Debug.CheckYeh(4, $"HitBonus", $"      {naturalEquipmentMod.GetHitBonus().Signed()}", Indent: 4, Toggle: doDebug);
                Debug.CheckYeh(4, $"PenBonus", $"      {naturalEquipmentMod.GetPenBonus().Signed()}", Indent: 4, Toggle: doDebug);

                Debug.Divider(4, HONLY, 25, Indent: 4, Toggle: doDebug);
            }
            Debug.Entry(4, $"x foreach ((_,{nameof(ModNaturalEquipmentBase)} naturalEquipmentMod) in {nameof(NaturalEquipmentMods)}) >//", Indent: 3, Toggle: doDebug);

            Debug.Entry(4, $"x {nameof(AccumulateMeleeWeaponBonuses)}() *//", Indent: 2, Toggle: doDebug);
            
            return NaturalEquipmentMods;
        }

        public Dictionary<string, (object TargetObject, Dictionary<string, (int Priority, object Value)> Entry)> GetEmptyAdjustmentTargets()
        {
            return new()
                {
                    { GAMEOBJECT,
                        ( ParentObject, new() )
                    },
                    { RENDER,
                        ( ParentRender, new() )
                    },
                    { MELEEWEAPON,
                        ( ParentMeleeWeapon, new() )
                    },
                    { ARMOR,
                        ( ParentArmor, new() )
                    },
                };
        }
        public virtual void ManageNaturalEquipment()
        {
            Debug.Header(4, 
                $"{nameof(NaturalEquipmentManager)}",
                $"{nameof(ManageNaturalEquipment)}()", Toggle: doDebug);

            string parentLimbString = 
                ParentLimb != null 
                ? $"[{ParentLimb?.ID}:{ParentLimb?.Type}] {ParentLimb?.Description}" 
                : $"[null]";

            Debug.LoopItem(4, 
                $" Wielder: {Wielder?.DebugName ?? NULL}", 
                Indent: 0, Toggle: doDebug);
            Debug.LoopItem(4, 
                $" ParentLimb: {parentLimbString}", 
                Indent: 0, Toggle: doDebug);

            SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = GetNaturalEquipmentMods();

            if (!naturalEquipmentMods.IsNullOrEmpty())
            {
                // Collect the "starting" values for damage if the NaturalEquipment is a defaultFistWeapon
                // Accumulate bonuses from NaturalEquipmentMods
                // Apply the finalised values over the top

                Debug.Entry(4, $"? if (ParentMeleeWeapon != null)", Indent: 1, Toggle: doDebug);
                if (ParentMeleeWeapon != null)
                {

                    DamageDie = new(ParentMeleeWeapon.BaseDamage);
                    DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2, Toggle: doDebug);
                    
                    GameObject sampleNaturalEquipment = GameObjectFactory.Factory.CreateSampleObject(OriginalNaturalEquipmentBlueprint);
                    MeleeWeapon originalWeapon = sampleNaturalEquipment.GetPart<MeleeWeapon>();
                    if (OriginalNaturalEquipmentBlueprint == DefaultFistBlueprint)
                    {
                        Debug.Entry(4, $"{nameof(sampleNaturalEquipment)}", $"{sampleNaturalEquipment.Blueprint}", Indent: 2, Toggle: doDebug);
                        AccumulatedDamageDie.Bonus += 1;
                    }
                    if (GameObject.Validate(ref sampleNaturalEquipment))
                    {
                        GameObject.Release(ref sampleNaturalEquipment);
                    }

                    AccumulateMeleeWeaponBonuses(naturalEquipmentMods);

                    DamageDie.AdjustDieCount(AccumulatedDamageDie.Count.Vomit(4, "AdjustDieCount", Indent: 2, Toggle: doDebug));
                    DamageDie.AdjustDieSize(AccumulatedDamageDie.Size.Vomit(4, "AdjustDieSize", Indent: 2, Toggle: doDebug));
                    DamageDie.AdjustResult(AccumulatedDamageDie.Bonus.Vomit(4, "AdjustResult", Indent: 2, Toggle: doDebug));

                    ParentMeleeWeapon.BaseDamage = DamageDie.Vomit(4, "Final DamageDie", Indent: 2, Toggle: doDebug).ToString();
                    ParentMeleeWeapon.HitBonus = AccumulatedHitBonus.Vomit(4, "AccumulatedHitBonus", Indent: 2, Toggle: doDebug);
                    ParentMeleeWeapon.PenBonus = AccumulatedPenBonus.Vomit(4, "AccumulatedPenBonus", Indent: 2, Toggle: doDebug);

                }
                else
                {
                    Debug.Entry(4, $"ParentMeleeWeapon is null", Indent: 2, Toggle: doDebug);
                }
                Debug.Entry(4, $"x if (ParentMeleeWeapon != null) ?//", Indent: 1, Toggle: doDebug);

                Debug.Entry(4, $"Cycling Adjustments, Applying where applicable", Indent: 1, Toggle: doDebug);
                // Cycle through the AdjustmentTargets (GameObject, Render, MeleeWeapon, Armor)
                // |__ Cycle through each Target's set of adjustments, applying them if possible 
                //     |__ Where not possible, output a warning.
                Debug.Entry(4, $"> foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)", Indent: 1, Toggle: doDebug);
                Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);
                foreach (HNPS_AdjustmentBase adjustment in GetPrioritisedNaturalEquipmentModAdjustments(naturalEquipmentMods))
                {
                    Debug.Entry(4, $"Target: {Target}", Indent: 2, Toggle: doDebug);
                    foreach ((string Field, (int Priority, string Value)) in Entries)
                    {
                        Debug.Entry(4, $"{Target}.{Field} = {Value}", Indent: 3, Toggle: doDebug);
                        if (TargetObject.SetPropertyOrFieldValue(Field, Value))
                        {
                            continue;
                        }
                        Debug.Warn(2,
                            $"{nameof(NaturalEquipmentManager)}",
                            $"{nameof(ManageNaturalEquipment)}()",
                            $"failed set Property or Field \"{Field}\" in {Target} to {Value}",
                            Indent: Debug.LastIndent + 1);
                    }
                    Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);
                }
                Debug.Entry(4, $"x foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets) >//", Indent: 1, Toggle: doDebug);
                
                // Cycle the NaturalEquipmentMods, applying each one to the NaturalEquipment
                ApplyNaturalEquipmentMods();

                if (ParentObject.TryGetPart(out MakersMark makersMark))
                {
                    ParentObject.RemovePart(makersMark);
                }

                if (DoDynamicTile && ParentObject.IsDefaultEquipmentOf(ParentLimb))
                {
                    Debug.Entry(4, $"Attempting Dynamic Tile update...", Indent: 1, Toggle: doDebug);
                    // This lets us check whether there's a Tile been provided anywhere in a fairly sizeable list of locations
                    // named "AdjectiveAdjectiveAdjectiveNoun", allowing for tiles to be added for an arbitrary set of combinations
                    // provided the order of the adjectives is consistent (which should definitely be the case with this mod.
                    //  - "icy" and "flaming" were breaking it when the player also has flaming or freezing ray, so this will
                    //    check without them first, applying that, then checking with them for the edge-case it's been included
                    string icyString = "{{icy|icy}}";
                    string flamingString = "{{fiery|flaming}}";
                    string displayNameOnlySansRays = ParentObject.DisplayNameOnly;
                    displayNameOnlySansRays.Replace(icyString, "");
                    displayNameOnlySansRays.Replace(flamingString, "");

                    Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: doDebug);
                    if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath))
                    {
                        ParentRender.Tile = tilePath;
                        Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: doDebug);
                    }
                    if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath))
                    {
                        ParentRender.Tile = tilePath;
                        Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: doDebug);
                    }

                    Debug.Entry(4, $"Dynamic Tile update attempted", Indent: 1, Toggle: doDebug);
                }
                else
                {
                    Debug.Entry(4, "DynamicTile search/application overriden", Indent: 2, Toggle: doDebug);
                }

                // We want these sick as, modified Natural Equipments to show up as a physical feature.
                // The check for a defaultFistWeapon being undesirable unfortunately targets tags, but we set the IntProp to 0 just in case it changes
                // These are always temporary DefaultBehaviors and should be completely refreshed any time something would normally
                ParentObject.SetIntProperty("ShowAsPhysicalFeature", 1);
                ParentObject.SetIntProperty("UndesirableWeapon", 0);
                ParentObject.SetStringProperty("TemporaryDefaultBehavior", "NaturalEquipmentManager", false);

                _shortDescriptionCache = ProcessShortDescription(ShortDescriptions);
            }
            else
            {
                Debug.Entry(4, 
                    $"{ParentObject?.DebugName ?? NULL} has no {nameof(naturalEquipmentMods)} to Manage", 
                    Indent: 1, Toggle: doDebug);
            }

            Debug.Footer(4,
                $"{nameof(NaturalEquipmentManager)}",
                $"{nameof(ManageNaturalEquipment)}()", Toggle: doDebug);
        }

        public virtual void ApplyNaturalEquipmentMods()
        {
            Debug.Entry(4, $"* {nameof(ApplyNaturalEquipmentMods)}()", Indent: 1, Toggle: doDebug);
            foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.Entry(4, $"Applying {naturalEquipmentMod.Name} to {ParentObject?.ShortDisplayNameStripped}", Indent: 2, Toggle: doDebug);
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
                naturalEquipmentMod.ParentObject = ParentObject;
            }
            Debug.Entry(4, $"x {nameof(ApplyNaturalEquipmentMods)}() *//", Indent: 1, Toggle: doDebug);
        }

        public static bool RemoveThisIfNotNatural(GameObject Equipment, NaturalEquipmentManager Manager)
        {
            if (Equipment == null) return false;

            if (!Equipment.IsNaturalEquipment())
            {
                Equipment.RemovePart(Manager);
                Debug.CheckNah(4,
                    $"Removed {nameof(NaturalEquipmentManager)} from {Equipment?.DebugName}",
                    Indent: 1, Toggle: getDoDebug("OC"));
                return true;
            }
            else
            {
                Debug.CheckYeh(4,
                    $"Kept {nameof(NaturalEquipmentManager)} on {Equipment?.DebugName}",
                    Indent: 1, Toggle: getDoDebug("OC"));
                
                return false;
            }
        }
        public bool RemoveThisIfNotNatural(GameObject Equipment)
        {
            return RemoveThisIfNotNatural(Equipment, this);
        }
        public bool RemoveThisIfNotNatural()
        {
            return RemoveThisIfNotNatural(ParentObject);
        }

        public static List<string> WantStringEvents = new()
        {
            "AdjustWeaponScore",
            "AdjustArmorScore",
            "CanBeDisassembled",
        };
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            if (WantsToManage)
            {
                foreach (string EventID in WantStringEvents)
                {
                    Registrar.Register(EventID);
                }
            }
            base.Register(Object, Registrar);
        }
        public static List<int> WantEvents = new()
        {
            GetShortDescriptionEvent.ID,
            BeforeBodyPartsUpdatedEvent.ID,
            AfterBodyPartsUpdatedEvent.ID,
        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (WantsToManage && WantEvents.Contains(ID));
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            Debug.Entry(4,
            $"@ {nameof(NaturalEquipmentManager)}."
            + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E.Object.DebugName})",
            Indent: 0, Toggle: doDebug);

            if (E.Object.HasPartDescendedFrom<ModNaturalEquipmentBase>())
            {
                ShortDescriptions ??= new();
                _shortDescriptionCache ??= ProcessShortDescription(ShortDescriptions);
                E.Postfix.AppendRules(_shortDescriptionCache);
            }

            return base.HandleEvent(E);
        }
        public bool HandleEvent(BeforeBodyPartsUpdatedEvent E)
        {
            Debug.Entry(4,
            $"@ {nameof(NaturalEquipmentManager)}."
            + $"{nameof(HandleEvent)}({nameof(BeforeBodyPartsUpdatedEvent)} E)",
            Indent: 0, Toggle: doDebug);

            Debug.Entry(4,
                $"Creature: {E?.Creature?.DebugName ?? NULL} | Limb: [{ParentLimb?.ID}:{ParentLimb?.Type}] {ParentLimb?.Description ?? NULL}",
                Indent: 1, Toggle: doDebug);

            if (E.Creature == Wielder)
            {
                ClearNaturalWeaponMods();
                ClearAdjustmentTargets();
                ResetShortDescriptions();
            }

            return base.HandleEvent(E);
        }
        public bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            Debug.Entry(4,
            $"@ {nameof(NaturalEquipmentManager)}."
            + $"{nameof(HandleEvent)}({nameof(AfterBodyPartsUpdatedEvent)} E.Creature: {E.Creature?.DebugName})",
            Indent: 0, Toggle: doDebug);

            if (E.Creature != null)
            {
                if (E.Creature == Wielder)
                {
                    if (!ParentObject.TryGetPart(out TinkerItem tinkerItem))
                    {
                        tinkerItem = ParentObject.RequirePart<TinkerItem>();
                    }
                    tinkerItem.Bits = "";
                    tinkerItem.CanDisassemble = false;
                    tinkerItem.CanBuild = false;

                    Debug.LoopItem(4,
                        $"{ParentObject?.DebugName} Can Be Disassembled", $"{TinkeringHelpers.CanBeDisassembled(ParentObject)}",
                        Good: !TinkeringHelpers.CanBeDisassembled(ParentObject), Indent: 1, Toggle: getDoDebug());

                    BeforeManageDefaultNaturalEquipmentEvent beforeEvent = BeforeManageDefaultNaturalEquipmentEvent.Send(ParentObject, this, ParentLimb);
                    ManageDefaultNaturalEquipmentEvent manageEvent = ManageDefaultNaturalEquipmentEvent.Manage(beforeEvent, Wielder);
                    AfterManageDefaultNaturalEquipmentEvent.Send(manageEvent);
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}({nameof(AfterBodyPartsUpdatedEvent)} E.Creature: {E.Creature?.DebugName}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (WantsToManage)
            {
                if (E.ID == "AdjustWeaponScore" || E.ID == "AdjustArmorScore")
                {
                    GameObject User = E.GetGameObjectParameter("User");
                    int Score = E.GetIntParameter("Score");
                    Score = Math.Max(100, Score);

                    E.SetParameter("Score", Score);
                }
                if (E.ID == "CanBeDisassembled")
                {
                    return false;
                }
            }
            return base.FireEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.Write(AccumulatedDamageDie.Count);
            Writer.Write(AccumulatedDamageDie.Size);
            Writer.Write(AccumulatedDamageDie.Bonus);

            Writer.Write(ShortDescriptions ??= new());
            Writer.Write(NaturalEquipmentMods ??= new());
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            AccumulatedDamageDie = new()
            {
                Count = Reader.ReadInt32(),
                Size = Reader.ReadInt32(),
                Bonus = Reader.ReadInt32()
            };

            ShortDescriptions = new SortedDictionary<int, ModNaturalEquipmentBase>(Reader.ReadDictionary<int, ModNaturalEquipmentBase>()) ?? new();
            NaturalEquipmentMods = new SortedDictionary<int, ModNaturalEquipmentBase>(Reader.ReadDictionary<int, ModNaturalEquipmentBase>()) ?? new();
            
            AdjustmentTargets = GetEmptyAdjustmentTargets();
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    GetNaturalEquipmentModAdjustments(naturalEquipmentMod, Serialization: true);
                }
            }
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalEquipmentManager = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            naturalEquipmentManager.ShortDescriptions = null;
            naturalEquipmentManager._shortDescriptionCache = null;
            naturalEquipmentManager.NaturalEquipmentMods = null;
            naturalEquipmentManager.AdjustmentTargets = null;
            return naturalEquipmentManager;
        }

    } //!-- public class NaturalEquipmentManager 
      //: IScribedPart
      //, IModEventHandler<BeforeBodyPartsUpdatedEvent>
      //, IModEventHandler<AfterBodyPartsUpdatedEvent>
}
