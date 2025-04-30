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
        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public GameObjectBlueprint DefaultFistBlueprint => GameObjectFactory.Factory.GetBlueprint("DefaultFist");

        [SerializeField]
        public GameObject OriginalNaturalEquipmentCopy;
        
        public DieRoll DamageDie;
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

        public SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions;

        [SerializeField]
        private string _shortDescriptionCache = null;

        public SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods;

        public Dictionary<string, SortedDictionary<int, ModNaturalEquipmentBase>> NaturalEquipmentModsByPart;

        // Dictionary key is the Target, the Value Dictionary key is the field
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
            NaturalEquipmentModsByPart = new();
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
                Indent: 1);

            StringBuilder StringBuilder = Event.NewStringBuilder();

            foreach ((int priority, ModNaturalEquipmentBase mod) in ShortDescriptions)
            {
                StringBuilder.AppendRules(mod.GetInstanceDescription());
                Debug.CheckYeh(4, $"{priority}::{mod.GetSource()}:Description Appended", Indent: 1);
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions) *//",
                Indent: 1);
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
            Debug.Entry(4, $"* {nameof(ClearNaturalWeaponMods)}()", Indent: 1);
            NaturalEquipmentMods = new();
        }
        public void ClearAdjustmentTargets()
        {
            Debug.Entry(4, $"* {nameof(ClearAdjustmentTargets)}()", Indent: 1);
            AdjustmentTargets = new();
        }
        public void ResetShortDescriptions()
        {
            Debug.Entry(4, $"* {nameof(ResetShortDescriptions)}()", Indent: 1);
            ClearShortDescriptionCache();
            ClearShortDescriptions();
        }

        public void AddNaturalEquipmentMod(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalEquipmentMod.Name})",
                Indent: 3);

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
            Debug.Entry(4, $"NaturalEquipmentMods:", Indent: 4);
            foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}:{naturalEquipmentMod.GetColoredAdjective()}", Indent: 4);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}) @//",
                Indent: 3);
        }

        public void AddShortDescriptionEntry(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddShortDescriptionEntry)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}[{NaturalEquipmentMod.GetAdjective()}])",
                Indent: 4);

            ShortDescriptions ??= new();
            ShortDescriptions[NaturalEquipmentMod.DescriptionPriority] = NaturalEquipmentMod;
            Debug.Entry(4, $"ShortDescriptions:", Indent: 5);
            foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}[{naturalEquipmentMod.GetAdjective()}]", Indent: 5);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddShortDescriptionEntry)}(NaturalWeaponMod: {NaturalEquipmentMod.Name}[{NaturalEquipmentMod.GetAdjective()}]) @//",
                Indent: 4);
        }

        public bool RaplacedBasedOnPriority(Dictionary<string, (int Priority, string Value)> Dictionary, Adjustment Adjustment)
        {
            Debug.Entry(4, 
                $"* {nameof(RaplacedBasedOnPriority)}" + 
                $"(Dictionary<string, (int Priority: {Adjustment.Priority}, string Value: {Adjustment.Value})> Dictionary, " + 
                $"Adjustment Adjustment)",
                Indent: 4);

            bool flag = false;
            string @field = Adjustment.Field;
            (int Priority, string Value) entry = (Adjustment.Priority, Adjustment.Value);

            // does an entry for this field exist or, if it does, is its priority beat?
            Debug.Entry(4, $"Existing: {(Dictionary.ContainsKey(@field) ? Dictionary[@field].Priority : "no entry")} | Proposed: {entry.Priority}", Indent: 5);
            if (!Dictionary.ContainsKey(@field) || Dictionary[@field].Priority >= entry.Priority)
            {
                Dictionary[@field] = entry;
                flag = true;
            }
            Debug.LoopItem(4, $"{(flag ? "" : "Not ")}Replaced", Indent: 5, Good: flag);
            return flag;
        }
        public void PrepareNaturalEquipmentModAdjustments(ModNaturalEquipmentBase NaturalWeaponMod)
        {
            Debug.Entry(4, $"* {nameof(PrepareNaturalEquipmentModAdjustments)}(ModNaturalEquipmentBase NaturalWeaponMod: {NaturalWeaponMod.Name})", Indent: 1);
            if (NaturalWeaponMod?.Adjustments != null)
            {
                Debug.Entry(4, $"NaturalWeaponMod?.Adjustments != null", Indent: 2);
                Debug.Entry(4, $"> foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments)", Indent: 2);
                foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments)
                {
                    string target = adjustment.Target;
                    Debug.LoopItem(4, $"target", $"{target}", Indent: 3);
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
                Debug.Entry(4, $"x foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments) >//", Indent: 2);
            }
            Debug.Entry(4, $"x {nameof(PrepareNaturalEquipmentModAdjustments)}(ModNaturalEquipmentBase NaturalWeaponMod: {NaturalWeaponMod.Name}) *//", Indent: 1);
        }

        public virtual void AccumulateMeleeWeaponBonuses()
        {
            Debug.Entry(4, $"* {nameof(AccumulateMeleeWeaponBonuses)}()", Indent: 2);

            Debug.Entry(4, $"> foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 3);
            Debug.Divider(4, "-", 25, Indent: 4);
            foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                AccumulatedDamageDie.Count += naturalEquipmentMod.GetDamageDieCount();
                AccumulatedDamageDie.Size += naturalEquipmentMod.GetDamageDieSize();
                AccumulatedDamageDie.Bonus += naturalEquipmentMod.GetDamageBonus();
                AccumulatedHitBonus += naturalEquipmentMod.GetHitBonus();
                AccumulatedPenBonus += naturalEquipmentMod.GetPenBonus();

                Debug.Entry(4, $"{naturalEquipmentMod.Name}[{naturalEquipmentMod.GetAdjective()}]", Indent: 4);
                Debug.CheckYeh(4, $"DamageDieCount", $"{naturalEquipmentMod.GetDamageDieCount().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"DamageDiesize", $" {naturalEquipmentMod.GetDamageDieSize().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"DamageBonus", $"   {naturalEquipmentMod.GetDamageBonus().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"HitBonus", $"      {naturalEquipmentMod.GetHitBonus().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"PenBonus", $"      {naturalEquipmentMod.GetPenBonus().Signed()}", Indent: 4);

                Debug.Divider(4, "-", 25, Indent: 4);
            }
            Debug.Entry(4, $"x foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 3);

            DamageDie = new(1, AccumulatedDamageDie.Count, AccumulatedDamageDie.Size);
            DamageDie.AdjustResult(AccumulatedDamageDie.Bonus);
            Debug.Entry(4, $"x {nameof(AccumulateMeleeWeaponBonuses)}() *//", Indent: 2);
        }

        public virtual void ManageNaturalEquipment()
        {
            Debug.Header(4, 
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");

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
                Indent: 0);
            Debug.LoopItem(4, 
                $" ParentLimb: {parentLimbString}", 
                Indent: 0);

            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                AdjustmentTargets = new()
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
                Debug.Entry(4, $"Cycling NaturalEquipmentMods for PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod)", Indent: 1);
                // Cycle the NaturalEquipmentMods to prepare the final set of adjustments to make
                Debug.Entry(4, $"> foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)", Indent: 1);
                foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                {
                    PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod);
                }
                Debug.Entry(4, $"x foreach ((_,ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods) >//", Indent: 1);

                // Collect the "starting" values for damage if the NaturalEquipment is a defaultFistWeapon
                // Accumulate bonuses from NaturalEquipmentMods
                // Apply the finalised values over the top

                Debug.Entry(4, $"? if (ParentMeleeWeapon != null)", Indent: 1);
                if (ParentMeleeWeapon != null)
                {
                    OriginalNaturalEquipmentCopy = ParentObject.DeepCopy();
                    MeleeWeapon originalWeapon = OriginalNaturalEquipmentCopy.GetPart<MeleeWeapon>();
                    DamageDie = new(originalWeapon.BaseDamage);
                    DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2);

                    if (!int.TryParse(DamageDie.ToString(), out int damageDieValue))
                    {
                        if (ParentLimb.Type == "Hand" && DamageDie.ToString() == "1d3")
                        {
                            Debug.Entry(4, 
                                $"Non-standard fist: {OriginalNaturalEquipmentCopy.Blueprint}, " + 
                                $"attempting to adjust DamageDie", 
                                Indent: 3);
                            GameObject defaultFist = GameObjectFactory.Factory.CreateSampleObject(DefaultFistBlueprint);
                            if (defaultFist.TryGetPart(out MeleeWeapon defaultFistWeapon))
                            {
                                DamageDie = new(defaultFistWeapon.BaseDamage);
                                DamageDie.ToString()
                                    .Vomit(4, "DamageDie", Indent: 2);
                            }
                        }
                        AccumulatedDamageDie.Count = DamageDie.GetDieCount()
                            .Vomit(4, "AccumulatedDamageDie.Count", Indent: 2);
                        AccumulatedDamageDie.Size = (DamageDie.LeftValue > 0 ? DamageDie.RightValue : DamageDie.Left.RightValue)
                            .Vomit(4, "AccumulatedDamageDie.Size", Indent: 2);

                        bool dieBonusIsPenalty = DamageDie.FindTypeWithConstantRight(5) != null;
                        int dieBonus = (DamageDie.LeftValue > 0 ? 0 : DamageDie.RightValue);
                        if (dieBonus != 0 && dieBonusIsPenalty) dieBonus = -dieBonus;
                        AccumulatedDamageDie.Bonus = dieBonus
                                    .Vomit(4, "AccumulatedDamageDie.Bonus", Indent: 2);
                    }
                    else
                    {
                        Debug.Entry(4,
                            $"Natural Equipment: {OriginalNaturalEquipmentCopy.Blueprint} " + 
                            $"has static DamageDie value ({damageDieValue})",
                            Indent: 3);
                        AccumulatedDamageDie.Count = 0.Vomit(4, "AccumulatedDamageDie.Count", Indent: 2);
                        AccumulatedDamageDie.Size = 0.Vomit(4, "AccumulatedDamageDie.Size", Indent: 2);
                        AccumulatedDamageDie.Bonus = damageDieValue.Vomit(4, "AccumulatedDamageDie.Bonus", Indent: 2);
                    }

                    AccumulatedHitBonus = originalWeapon.HitBonus
                            .Vomit(4, "AccumulatedHitBonus", Indent: 2);
                    AccumulatedPenBonus = originalWeapon.PenBonus
                        .Vomit(4, "AccumulatedPenBonus", Indent: 2);

                    if (OriginalNaturalEquipmentBlueprint == DefaultFistBlueprint)
                    {
                        AccumulatedDamageDie.Bonus = AccumulatedDamageDie.Bonus < 0 ? 0 : AccumulatedDamageDie.Bonus;
                    }

                    AccumulateMeleeWeaponBonuses();

                    ParentMeleeWeapon.BaseDamage = DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2);
                    ParentMeleeWeapon.HitBonus = AccumulatedHitBonus.Vomit(4, "AccumulatedHitBonus", Indent: 2);
                    ParentMeleeWeapon.PenBonus = AccumulatedPenBonus.Vomit(4, "AccumulatedPenBonus", Indent: 2);
                }
                else
                {
                    Debug.Entry(4, $"ParentMeleeWeapon is null", Indent: 2);
                }
                Debug.Entry(4, $"x if (ParentMeleeWeapon != null) ?//", Indent: 1);

                Debug.Entry(4, $"Cycling Adjustments, Applying where applicable", Indent: 1);
                // Cycle through the AdjustmentTargets (GameObject, Render, MeleeWeapon, Armor)
                // |__ Cycle through each Target's set of adjustments, applying them if possible 
                //     |__ Where not possible, output a warning.
                Debug.Entry(4, $"> foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)", Indent: 1);
                foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)
                {
                    Debug.Entry(4, $"Target: {Target}", Indent: 2);
                    foreach ((string Field, (int Priority, string Value)) in Entries)
                    {
                        Debug.Entry(4, $"{Target}.{Field} = {Value}", Indent: 3);
                        if (TargetObject.SetPropertyOrFieldValue(Field, Value))
                            continue;
                        Debug.Entry(2,
                            $"WARN: {typeof(NaturalEquipmentManager).Name}." +
                            $"{nameof(ManageNaturalEquipment)}()",
                            $"failed set Property or Field \"{Field}\" in {Target} to {Value}",
                            Indent: 4);
                    }
                }
                Debug.Entry(4, $"x foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets) >//", Indent: 1);
                
                // Cycle the NaturalEquipmentMods, applying each one to the NaturalEquipment
                ApplyNaturalEquipmentMods();

                if (ParentObject.TryGetPart(out MakersMark makersMark))
                {
                    ParentObject.RemovePart(makersMark);
                }

                if (DoDynamicTile && ParentObject.IsDefaultEquipmentOf(ParentLimb))
                {
                    Debug.Entry(4, $"Attempting Dynamic Tile update", Indent: 1);
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

                    Debug.Divider(4, "-", 25, Indent: 2);
                    if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath)) ParentRender.Tile = tilePath;
                    Debug.Divider(4, "-", 25, Indent: 2);
                    if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath)) ParentRender.Tile = tilePath;
                    Debug.Divider(4, "-", 25, Indent: 2);
                }
                else
                {
                    Debug.Entry(4, "DynamicTile search/application overriden", Indent: 2);
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
                Debug.Entry(4, $"Nothing to Manage", Indent: 1);
            }

            Debug.Footer(4,
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");
        }

        public virtual void ApplyNaturalEquipmentMods()
        {
            Debug.Entry(4, $"* {nameof(ApplyNaturalEquipmentMods)}()", Indent: 1);
            foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.Entry(4, $"Applying {naturalEquipmentMod.Name} to {ParentObject?.ShortDisplayNameStripped}", Indent: 2);
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
            }
            Debug.Entry(4, $"x {nameof(ApplyNaturalEquipmentMods)}() *//", Indent: 1);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == BeforeBodyPartsUpdatedEvent.ID
                || ID == AfterBodyPartsUpdatedEvent.ID
                || ID == GetShortDescriptionEvent.ID;
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E.Object.DebugName})",
                Indent: 0);

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
                + $"{nameof(HandleEvent)}({nameof(BeforeBodyPartsUpdatedEvent)} E (Actor:{E.Actor.DebugName}))",
                Indent: 0);
            Debug.Entry(4,
                $" Actor:{E.Actor.ShortDisplayName} | Limb: [{ParentLimb.ID}:{ParentLimb.Type}] {ParentLimb.Description}",
                Indent: 0);

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
                Indent: 0);

            if (E.Actor == Wielder)
            {
                BeforeManageDefaultEquipmentEvent beforeEvent = BeforeManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
                ManageDefaultEquipmentEvent manageEvent = ManageDefaultEquipmentEvent.Manage(beforeEvent, Wielder);
                AfterManageDefaultEquipmentEvent.Send(manageEvent);
            }

            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("AdjustWeaponScore");
            Registrar.Register("AdjustArmorScore");
            base.Register(Object, Registrar);
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
            Writer.WriteGameObject(OriginalNaturalEquipmentCopy);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            OriginalNaturalEquipmentCopy = Reader.ReadGameObject();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalEquipmentManager = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            naturalEquipmentManager.OriginalNaturalEquipmentCopy = OriginalNaturalEquipmentCopy?.DeepCopy();
            naturalEquipmentManager.ShortDescriptions = null;
            naturalEquipmentManager._shortDescriptionCache = null;
            naturalEquipmentManager.NaturalEquipmentMods = null;
            return naturalEquipmentManager;
        }

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
