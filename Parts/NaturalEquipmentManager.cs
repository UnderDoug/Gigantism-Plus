using System;
using System.Collections.Generic;
using System.Text;

using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Extensions;

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
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "OC",   // ObjectCreation
                "S"     // Serialisation
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public GameObjectBlueprint DefaultFistBlueprint => GameObjectFactory.Factory.GetBlueprint("DefaultFist");
        
        public DieRoll DamageDie;

        [NonSerialized]
        public (int Count, int Size, int Bonus) AccumulatedDamageDie;
        public int AccumulatedHitBonus;
        public int AccumulatedPenBonus;

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

        [NonSerialized]
        public SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods;

        // Dictionary key is the Target, the Value Dictionary key is the field
        [NonSerialized]
        public Dictionary<string, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entry)> AdjustmentTargets;
        // AdjustmentTargets: Dictionary,
        //      Key: string (name of target object)
        //      Value: Tuple( TargetObject, Entry ),
        //          TargetObject: per Adjustment struct:
        //              GameObject (the equipment itself),
        //              Render,
        //              MeleeWeapon,
        //              Armor
        //          Entry: Dictionary,
        //              Key: string (field/property being targeted)
        //              Value: Tuple( Priority, Value ),
        //                  Priority: self-explanitory
        //                  Value: the value to set the field as

        public NaturalEquipmentManager()
        {
            NaturalEquipmentMods = new();
            ShortDescriptions = new();
        }

        public override void Initialize()
        {

            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
        }

        public string ProcessShortDescription(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions)
        {
            Debug.Entry(4,
                $"* {nameof(NaturalEquipmentManager)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions)",
                Indent: 1, Toggle: doDebug);

            StringBuilder StringBuilder = Event.NewStringBuilder();

            foreach ((int priority, ModNaturalEquipmentBase mod) in ShortDescriptions)
            {
                StringBuilder.AppendRules(mod.GetInstanceDescription());
                Debug.CheckYeh(4, $"{priority}::{mod.GetSource()}:Description Appended", Indent: 1, Toggle: doDebug);
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
            Debug.Entry(4, $"* {nameof(ClearNaturalWeaponMods)}()", Indent: 1, Toggle: getDoDebug('R'));
            NaturalEquipmentMods = new();
        }
        public void ClearAdjustmentTargets()
        {
            Debug.Entry(4, $"* {nameof(ClearAdjustmentTargets)}()", Indent: 1, Toggle: getDoDebug('R'));
            AdjustmentTargets = new();
        }
        public void ResetShortDescriptions()
        {
            Debug.Entry(4, $"* {nameof(ResetShortDescriptions)}()", Indent: 1, Toggle: getDoDebug('R'));
            ClearShortDescriptionCache();
            ClearShortDescriptions();
        }

        public void AddNaturalEquipmentMod(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalEquipmentMod.Name})",
                Indent: 3, Toggle: doDebug);

            NaturalEquipmentMods ??= new();
            if (NaturalEquipmentMods.ContainsKey(NaturalEquipmentMod.ModPriority))
            {
                Debug.Entry(2,
                    $"WARN: {typeof(NaturalEquipmentManager).Name}." +
                    $"{nameof(AddNaturalEquipmentMod)}()",
                    $"[{NaturalEquipmentMod.ModPriority}]" + 
                    $"{NaturalEquipmentMods[NaturalEquipmentMod.ModPriority]} " + 
                    $"in {nameof(NaturalEquipmentMods)} overwritten: Same ModPriority",
                    Indent: 4);
            }
            NaturalEquipmentMods[NaturalEquipmentMod.ModPriority] = NaturalEquipmentMod;
            Debug.Entry(4, $"NaturalEquipmentMods:", Indent: 4, Toggle: doDebug);
            foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}:{naturalEquipmentMod.GetColoredAdjective()}", Indent: 4, Toggle: doDebug);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}) @//",
                Indent: 3, Toggle: doDebug);
        }

        public void AddShortDescriptionEntry(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddShortDescriptionEntry)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}[{NaturalEquipmentMod.GetAdjective()}])",
                Indent: 4, Toggle: doDebug);

            ShortDescriptions ??= new();
            ShortDescriptions[NaturalEquipmentMod.DescriptionPriority] = NaturalEquipmentMod;
            Debug.Entry(4, $"ShortDescriptions:", Indent: 5, Toggle: doDebug);
            foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}[{naturalEquipmentMod.GetAdjective()}]", Indent: 5, Toggle: doDebug);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddShortDescriptionEntry)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}[{NaturalEquipmentMod.GetAdjective()}]) @//",
                Indent: 4, Toggle: doDebug);
        }

        public bool RaplacedBasedOnPriority(Dictionary<string, (int Priority, string Value)> Dictionary, Adjustment Adjustment)
        {
            Debug.Entry(4, 
                $"* {nameof(RaplacedBasedOnPriority)}" + 
                $"(Dictionary<string, (int Priority: {Adjustment.Priority}, string Value: {Adjustment.Value})> Dictionary, " + 
                $"Adjustment Adjustment)",
                Indent: 4, Toggle: doDebug);

            bool flag = false;
            string @field = Adjustment.Field;
            (int Priority, string Value) entry = (Adjustment.Priority, Adjustment.Value);

            // does an entry for this field exist or, if it does, is its priority beat?
            Debug.Entry(4, $"Existing: {(Dictionary.ContainsKey(@field) ? Dictionary[@field].Priority : "no entry")} | Proposed: {entry.Priority}", Indent: 5, Toggle: doDebug);
            if (!Dictionary.ContainsKey(@field) || Dictionary[@field].Priority >= entry.Priority)
            {
                Dictionary[@field] = entry;
                flag = true;
            }
            Debug.LoopItem(4, $"{(flag ? "" : "Not ")}Replaced", Indent: 5, Good: flag, Toggle: doDebug);
            return flag;
        }
        public void PrepareNaturalEquipmentModAdjustments(ModNaturalEquipmentBase NaturalWeaponMod, bool Serialization = false)
        {
            bool doDebug = !Serialization && getDoDebug();
            Debug.Entry(4, $"* {nameof(PrepareNaturalEquipmentModAdjustments)}(ModNaturalEquipmentBase NaturalWeaponMod: {NaturalWeaponMod.Name})", Indent: 1, Toggle: doDebug);
            if (NaturalWeaponMod?.Adjustments != null)
            {
                Debug.Entry(4, $"NaturalWeaponMod?.Adjustments != null", Indent: 2, Toggle: doDebug);
                Debug.Entry(4, $"> foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments)", Indent: 2, Toggle: doDebug);
                foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments)
                {
                    string target = adjustment.Target;
                    Debug.LoopItem(4, $"target", $"{target}", Indent: 3, Toggle: doDebug);
                    if (AdjustmentTargets != null && AdjustmentTargets.ContainsKey(target))
                    {
                        RaplacedBasedOnPriority(AdjustmentTargets[target].Entry, adjustment);
                    }
                    else
                    {
                        Debug.Entry(2,
                            $"WARN: {typeof(NaturalEquipmentManager).Name}."+
                            $"{nameof(PrepareNaturalEquipmentModAdjustments)}()",
                            $"failed to find Target \"{target}\" in {nameof(AdjustmentTargets)}",
                            Indent: 2);
                    }
                }
                Debug.Entry(4, $"x foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments) >//", Indent: 2, Toggle: doDebug);
            }
            Debug.Entry(4, $"x {nameof(PrepareNaturalEquipmentModAdjustments)}(ModNaturalEquipmentBase NaturalWeaponMod: {NaturalWeaponMod.Name}) *//", Indent: 1, Toggle: doDebug);
        }

        public virtual void AccumulateMeleeWeaponBonuses()
        {
            Debug.Entry(4, $"* {nameof(AccumulateMeleeWeaponBonuses)}()", Indent: 2, Toggle: doDebug);

            Debug.Entry(4, $"> foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 3, Toggle: doDebug);
            Debug.Divider(4, "-", 25, Indent: 4, Toggle: doDebug);
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

                Debug.Divider(4, "-", 25, Indent: 4, Toggle: doDebug);
            }
            Debug.Entry(4, $"x foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 3, Toggle: doDebug);

            DamageDie = new(1, AccumulatedDamageDie.Count, AccumulatedDamageDie.Size);
            DamageDie.AdjustResult(AccumulatedDamageDie.Bonus);
            Debug.Entry(4, $"x {nameof(AccumulateMeleeWeaponBonuses)}() *//", Indent: 2, Toggle: doDebug);
        }

        public Dictionary<string, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entry)> GetEmptyAdjustmentTargets()
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
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()", Toggle: doDebug);

            string wielderString = 
                Wielder != null 
                ? Wielder.DebugName 
                : $"[null]";
            string parentLimbString = 
                ParentLimb != null 
                ? $"[{ParentLimb?.ID}:{ParentLimb?.Type}] {ParentLimb?.Description}" 
                : $"[null]";

            Debug.LoopItem(4, 
                $" Wielder: {wielderString}", 
                Indent: 0, Toggle: doDebug);
            Debug.LoopItem(4, 
                $" ParentLimb: {parentLimbString}", 
                Indent: 0, Toggle: doDebug);

            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                AdjustmentTargets = GetEmptyAdjustmentTargets();

                Debug.Entry(4, $"Cycling NaturalEquipmentMods for PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod)", Indent: 1, Toggle: doDebug);
                // Cycle the NaturalEquipmentMods to prepare the final set of adjustments to make
                Debug.Entry(4, $"> foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1, Toggle: doDebug);
                foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod);
                }
                Debug.Entry(4, $"x foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1, Toggle: doDebug);

                // Collect the "starting" values for damage if the NaturalEquipment is a defaultFistWeapon
                // Accumulate bonuses from NaturalEquipmentMods
                // Apply the finalised values over the top

                Debug.Entry(4, $"? if (ParentMeleeWeapon != null)", Indent: 1, Toggle: doDebug);
                if (ParentMeleeWeapon != null)
                {
                    GameObject originalNaturalEquipment = GameObjectFactory.Factory.CreateSampleObject(OriginalNaturalEquipmentBlueprint);
                    MeleeWeapon originalWeapon = originalNaturalEquipment.GetPart<MeleeWeapon>();
                    DamageDie = new(originalWeapon.BaseDamage);
                    DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2, Toggle: doDebug);
                    int damageDieValue = 0;
                    if (DamageDie.FindType(TargetType: 1) != null || !int.TryParse(DamageDie.ToString(), out damageDieValue))
                    {
                        if (ParentLimb.Type == "Hand" && DamageDie.ToString() == "1d3")
                        {
                            Debug.Entry(4, 
                                $"Non-standard fist: {OriginalNaturalEquipmentBlueprint}, " + 
                                $"attempting to adjust DamageDie", 
                                Indent: 3, Toggle: doDebug);
                            GameObject defaultFist = GameObjectFactory.Factory.CreateSampleObject(DefaultFistBlueprint);
                            if (defaultFist.TryGetPart(out MeleeWeapon defaultFistWeapon))
                            {
                                DamageDie = new(defaultFistWeapon.BaseDamage);
                                DamageDie.ToString()
                                    .Vomit(4, "DamageDie", Indent: 2, Toggle: doDebug);
                            }
                        }

                        DieRoll DamageDieTypeDie = DamageDie.FindType(TargetType: 1);

                        if (DamageDieTypeDie != null)
                        {
                            AccumulatedDamageDie.Count = DamageDieTypeDie.GetDieCount()
                                .Vomit(4, "AccumulatedDamageDie.Count", Indent: 2, Toggle: doDebug);
                            if (AccumulatedDamageDie.Count < 1)
                            {
                                AccumulatedDamageDie.Count = 1
                                    .Vomit(4, "AccumulatedDamageDie.Count (default)", Indent: 3, Toggle: doDebug);
                            }

                            AccumulatedDamageDie.Size = DamageDieTypeDie.RightValue
                                .Vomit(4, "AccumulatedDamageDie.Size", Indent: 2, Toggle: doDebug);
                            if (AccumulatedDamageDie.Size < 2)
                            {
                                AccumulatedDamageDie.Size = 2
                                    .Vomit(4, "AccumulatedDamageDie.Size (default)", Indent: 3, Toggle: doDebug);
                            }
                        }
                        else
                        {
                            AccumulatedDamageDie.Count.Vomit(4, "AccumulatedDamageDie.Count (base)", Indent: 2, Toggle: doDebug);
                            AccumulatedDamageDie.Size.Vomit(4, "AccumulatedDamageDie.Size (base)", Indent: 2, Toggle: doDebug);
                        }

                        bool dieBonusIsPenalty = DamageDie.FindTypeWithConstantRight(5) != null;
                        int dieBonus = (DamageDie.LeftValue > 0 ? 0 : DamageDie.RightValue);
                        if (dieBonus != 0 && dieBonusIsPenalty) dieBonus = -dieBonus;
                        AccumulatedDamageDie.Bonus = dieBonus
                                    .Vomit(4, "AccumulatedDamageDie.Bonus", Indent: 2, Toggle: doDebug);
                    }
                    else
                    {
                        Debug.Entry(4,
                            $"Natural Equipment: {OriginalNaturalEquipmentBlueprint} " + 
                            $"has static DamageDie value ({damageDieValue})",
                            Indent: 3, Toggle: doDebug);
                        AccumulatedDamageDie.Count = 0.Vomit(4, "AccumulatedDamageDie.Count", Indent: 2, Toggle: doDebug);
                        AccumulatedDamageDie.Size = 0.Vomit(4, "AccumulatedDamageDie.Size", Indent: 2, Toggle: doDebug);
                        AccumulatedDamageDie.Bonus = damageDieValue.Vomit(4, "AccumulatedDamageDie.Bonus", Indent: 2, Toggle: doDebug);
                    }

                    AccumulatedHitBonus = originalWeapon.HitBonus
                            .Vomit(4, "AccumulatedHitBonus", Indent: 2, Toggle: doDebug);
                    AccumulatedPenBonus = originalWeapon.PenBonus
                        .Vomit(4, "AccumulatedPenBonus", Indent: 2, Toggle: doDebug);

                    if (OriginalNaturalEquipmentBlueprint == DefaultFistBlueprint)
                    {
                        AccumulatedDamageDie.Bonus = AccumulatedDamageDie.Bonus < 0 ? 0 : AccumulatedDamageDie.Bonus;
                    }

                    AccumulateMeleeWeaponBonuses();

                    ParentMeleeWeapon.BaseDamage = DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2, Toggle: doDebug);
                    ParentMeleeWeapon.HitBonus = AccumulatedHitBonus.Vomit(4, "AccumulatedHitBonus", Indent: 2, Toggle: doDebug);
                    ParentMeleeWeapon.PenBonus = AccumulatedPenBonus.Vomit(4, "AccumulatedPenBonus", Indent: 2, Toggle: doDebug);

                    originalNaturalEquipment.Obliterate();
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
                foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)
                {
                    Debug.Entry(4, $"Target: {Target}", Indent: 2, Toggle: doDebug);
                    foreach ((string Field, (int Priority, string Value)) in Entries)
                    {
                        Debug.Entry(4, $"{Target}.{Field} = {Value}", Indent: 3, Toggle: doDebug);
                        if (TargetObject.SetPropertyOrFieldValue(Field, Value))
                            continue;
                        Debug.Entry(2,
                            $"WARN: {typeof(NaturalEquipmentManager).Name}." +
                            $"{nameof(ManageNaturalEquipment)}()",
                            $"failed set Property or Field \"{Field}\" in {Target} to {Value}",
                            Indent: 4);
                    }
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
                    Debug.Entry(4, $"Attempting Dynamic Tile update", Indent: 1, Toggle: doDebug);
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

                    Debug.Divider(4, "-", 25, Indent: 2, Toggle: doDebug);
                    if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath)) ParentRender.Tile = tilePath;
                    Debug.Divider(4, "-", 25, Indent: 2, Toggle: doDebug);
                    if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath)) ParentRender.Tile = tilePath;
                    Debug.Divider(4, "-", 25, Indent: 2, Toggle: doDebug);
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
                Debug.Entry(4, $"Nothing to Manage", Indent: 1, Toggle: doDebug);
            }

            Debug.Footer(4,
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()", Toggle: doDebug);
        }

        public virtual void ApplyNaturalEquipmentMods()
        {
            Debug.Entry(4, $"* {nameof(ApplyNaturalEquipmentMods)}()", Indent: 1, Toggle: doDebug);
            foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.Entry(4, $"Applying {naturalEquipmentMod.Name} to {ParentObject?.ShortDisplayNameStripped}", Indent: 2, Toggle: doDebug);
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
            }
            Debug.Entry(4, $"x {nameof(ApplyNaturalEquipmentMods)}() *//", Indent: 1, Toggle: doDebug);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("AdjustWeaponScore");
            Registrar.Register("AdjustArmorScore");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || ID == BeforeBodyPartsUpdatedEvent.ID
                || ID == AfterBodyPartsUpdatedEvent.ID;
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (GameObject.Validate(ParentObject) && E.Object == ParentObject)
            {
                if (!ParentObject.IsNaturalEquipment())
                {
                    ParentObject.RemovePart(this);
                }
                else
                {
                    Debug.Entry(4,
                        $"{nameof(NaturalEquipmentManager)}." + 
                        $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E.Object: {E?.Object?.DebugName ?? NULL})" + 
                        $" Kept {nameof(NaturalEquipmentManager)}",
                        Indent: 0, Toggle: getDoDebug("OC"));
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E.Object.DebugName})",
                Indent: 0, Toggle: doDebug);

            if(E.Object.HasPartDescendedFrom<ModNaturalEquipmentBase>())
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
                + $"{nameof(HandleEvent)}({nameof(BeforeBodyPartsUpdatedEvent)} E (Actor:{E?.Actor?.DebugName}))",
                Indent: 0, Toggle: doDebug);
            Debug.Entry(4,
                $" Actor:{E?.Actor?.ShortDisplayName} | Limb: [{ParentLimb?.ID}:{ParentLimb?.Type}] {ParentLimb?.Description}",
                Indent: 0, Toggle: doDebug);

            if (E.Actor == Wielder)
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
                + $"{nameof(HandleEvent)}({nameof(AfterBodyPartsUpdatedEvent)} E (Actor:{E.Actor.DebugName}))",
                Indent: 0, Toggle: doDebug);

            if (E.Actor == Wielder)
            {
                BeforeManageDefaultEquipmentEvent beforeEvent = BeforeManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
                ManageDefaultEquipmentEvent manageEvent = ManageDefaultEquipmentEvent.Manage(beforeEvent, Wielder);
                AfterManageDefaultEquipmentEvent.Send(manageEvent);
            }

            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "AdjustWeaponScore" || E.ID == "AdjustArmorScore")
            {
                GameObject User = E.GetGameObjectParameter("User");
                int Score = E.GetIntParameter("Score");
                Score = Math.Max(100, Score);

                E.SetParameter("Score", Score);
            }
            return base.FireEvent(E);
        }

        public override void Remove()
        {
            ResetShortDescriptions();
            ClearAdjustmentTargets();
            ClearNaturalWeaponMods();
            base.Remove();
        }
        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.Write(AccumulatedDamageDie.Count);
            Writer.Write(AccumulatedDamageDie.Size);
            Writer.Write(AccumulatedDamageDie.Bonus);

            ShortDescriptions ??= new();
            Writer.Write(ShortDescriptions.Count);
            if (!ShortDescriptions.IsNullOrEmpty())
            {
                foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in ShortDescriptions)
                {
                    Writer.Write(priority);
                    naturalEquipmentMod.Write(Basis, Writer);
                }
            }

            NaturalEquipmentMods ??= new();
            Writer.Write(NaturalEquipmentMods.Count);
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    Writer.Write(priority);
                    naturalEquipmentMod.Write(Basis, Writer);
                }
            }
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            AccumulatedDamageDie.Count = Reader.ReadInt32();
            AccumulatedDamageDie.Size = Reader.ReadInt32();
            AccumulatedDamageDie.Bonus = Reader.ReadInt32();

            ShortDescriptions = new();
            int shortDescriptionsCount = Reader.ReadInt32();
            for (int i = 0; i < shortDescriptionsCount; i++)
            {
                ShortDescriptions.Add(Reader.ReadInt32(), (ModNaturalEquipmentBase)Reader.ReadObject());
            }

            NaturalEquipmentMods = new();
            int naturalEquipmentModsCount = Reader.ReadInt32();
            for (int i = 0; i < naturalEquipmentModsCount; i++)
            {
                NaturalEquipmentMods.Add(Reader.ReadInt32(), (ModNaturalEquipmentBase)Reader.ReadObject());
            }

            AdjustmentTargets = GetEmptyAdjustmentTargets();
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod, true);
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

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
