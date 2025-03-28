using System;
using System.Collections.Generic;
using System.Text;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static XRL.World.Parts.ModNaturalEquipmentBase;
using SerializeField = UnityEngine.SerializeField;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Extensions;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager : IScribedPart
    {
        public bool WantToManage = false;
        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public GameObjectBlueprint DefaultFistBlueprint => GameObjectFactory.Factory.GetBlueprint("DefaultFist");

        public GameObject OriginalNaturalEquipmentCopy;
        
        public DieRoll DamageDie;
        public (int Count, int Size, int Bonus) AccumulatedDamageDie;
        public int AccumulatedHitBonus;
        public int AccumulatedPenBonus;

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

        public SortedDictionary<int, string> ShortDescriptions;

        [SerializeField]
        private string _shortDescriptionCache = null;

        public List<ModNaturalEquipmentBase> NaturalEquipmentMods;

        // Disctionary key is the Target, the Value Dictionary key is the field
        public Dictionary<string, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entry)> AdjustmentTargets;
        // AdjustmentTargets: Dictionary,
        //      Key: string (name of target object)
        //      Value: Tuple( TargetObject, Entry ),
        //          TargetObject: per Adjustment struct: GameObject (the equipment itself), Render, MeleeWeapon, MissileWeapon, Armor
        //          Entry: Dictionary,
        //              Key: string (field/property being targeted)
        //              Value: Tuple( Priority, Value ),
        //                  Priority: self-explanitory
        //                  Value: the value to set the field as

        public NaturalEquipmentManager()
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
            NaturalEquipmentMods = new();
        }

        public override void Initialize()
        {

            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
        }

        public string ProcessDescription(SortedDictionary<int, string> Descriptions, bool IsShort = true)
        {
            StringBuilder StringBuilder = Event.NewStringBuilder();

            CollectAppliedNaturalWeaponMods();

            ProcessNaturalWeaponModsShortDescriptions();

            foreach ((_, string description) in Descriptions)
            {
                if (IsShort)
                {
                    StringBuilder.AppendRules(description);
                    continue;
                }
                StringBuilder.AppendLine(description);
            }

            return Event.FinalizeString(StringBuilder);
        }

        public void AddShortDescriptionEntry(int Priority, string Description)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddShortDescriptionEntry)}(int Priority: {Priority}, string Description)",
                Indent: 7);

            ShortDescriptions[Priority] = Description;
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
            NaturalEquipmentMods = new();
        }
        public void ResetShortDescriptions()
        {
            ClearShortDescriptionCache();
            ClearShortDescriptions();
        }

        public void AddNaturalEquipmentMod(ModNaturalEquipmentBase NaturalWeaponMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalWeaponMod.Name})",
                Indent: 4);

            NaturalEquipmentMods ??= new();
            NaturalEquipmentMods.Add(NaturalWeaponMod);
            Debug.Entry(4, $"NaturalEquipmentMods:", Indent: 5);
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                Debug.CheckYeh(4, $"{naturalEquipmentMod.Name}", Indent: 6);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalEquipmentMod)}(NaturalWeaponMod: {NaturalWeaponMod.Name}) @//",
                Indent: 4);
        }

        public bool RaplacedBasedOnPriority(Dictionary<string, (int Priority, string Value)> Dictionary, Adjustment Adjustment)
        {
            Debug.Entry(4, 
                $"* {nameof(RaplacedBasedOnPriority)}" + 
                $"(Dictionary<string, (int Priority: {Adjustment.Priority}, string Value: {Adjustment.Value})> Dictionary, " + 
                $"Adjustment Adjustment: {Adjustment.ID})",
                Indent: 4);

            bool flag = false;
            string @field = Adjustment.Field;
            (int Priority, string Value) entry = (Adjustment.Priority, Adjustment.Value);

            // does an entry for this field exist or, if it does, is its priority beat?
            Debug.Entry(4, $"Dictionary[@field].Priority {(Dictionary.ContainsKey(@field) ? Dictionary[@field].Priority : "no entry")} | entry.Priority: {entry.Priority}", Indent: 5);
            if (!Dictionary.ContainsKey(@field) || Dictionary[@field].Priority > entry.Priority)
            {
                Dictionary[@field] = entry;
                flag = true;
            }
            Debug.Entry(4, $"Replaced = {flag}", Indent: 5);

            Debug.Entry(4,
                $"x {nameof(RaplacedBasedOnPriority)}" +
                $"(Dictionary<string, (int Priority: {Adjustment.Priority}, string Value: {Adjustment.Value})> Dictionary, " +
                $"Adjustment Adjustment: {Adjustment.ID}) *//",
                Indent: 4);
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
                        Debug.Entry(4,
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

        public void ProcessNaturalWeaponModsShortDescriptions()
        {
            if (NaturalEquipmentMods.IsNullOrEmpty()) return;

            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                AddShortDescriptionEntry(naturalEquipmentMod.DescriptionPriority, naturalEquipmentMod.GetInstanceDescription());
            }
        }

        public virtual void CollectAppliedNaturalWeaponMods()
        {
            List<ModNaturalEquipmentBase> appliedNaturalEquipmentMods = ParentObject.GetPartsDescendedFrom<ModNaturalEquipmentBase>();
            if (!appliedNaturalEquipmentMods.IsNullOrEmpty())
            {
                ClearNaturalWeaponMods();
                foreach (ModNaturalEquipmentBase naturalEquipmentMod in appliedNaturalEquipmentMods)
                {
                    AddNaturalEquipmentMod(naturalEquipmentMod);
                }
            }            
        }

        public virtual void AccumulateMeleeWeaponBonuses()
        {
            Debug.Entry(4, $"* {nameof(AccumulateMeleeWeaponBonuses)}()", Indent: 1);

            Debug.Entry(4, $"> foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)", Indent: 2);
            Debug.Divider(4, "-", 25, Indent: 3);
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                AccumulatedDamageDie.Count += naturalEquipmentMod.GetDamageDieCount();
                AccumulatedDamageDie.Size += naturalEquipmentMod.GetDamageDieSize();
                AccumulatedDamageDie.Bonus += naturalEquipmentMod.GetDamageBonus();
                AccumulatedHitBonus += naturalEquipmentMod.GetHitBonus();
                AccumulatedPenBonus += naturalEquipmentMod.GetPenBonus();

                Debug.Entry(4, $"{naturalEquipmentMod.Name}", Indent: 3);
                Debug.CheckYeh(4, $"DamageDieCount", $"{naturalEquipmentMod.GetDamageDieCount().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"DamageDiesize", $" {naturalEquipmentMod.GetDamageDieSize().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"DamageBonus", $"   {naturalEquipmentMod.GetDamageBonus().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"HitBonus", $"      {naturalEquipmentMod.GetHitBonus().Signed()}", Indent: 4);
                Debug.CheckYeh(4, $"PenBonus", $"      {naturalEquipmentMod.GetPenBonus().Signed()}", Indent: 4);

                Debug.Divider(4, "-", 25, Indent: 3);
            }
            Debug.Entry(4, $"x foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods) >//", Indent: 2);

            DamageDie = new(1, AccumulatedDamageDie.Count, AccumulatedDamageDie.Size);
            DamageDie.AdjustResult(AccumulatedDamageDie.Bonus);
            Debug.Entry(4, $"x {nameof(AccumulateMeleeWeaponBonuses)}() *//", Indent: 1);
        }

        public virtual void ManageNaturalEquipment()
        {
            Debug.Header(4, 
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");

            // 
            // CollectAppliedNaturalWeaponMods();

            // Cycle the NaturalEquipmentMods to prepare the final set of adjustments to make
            Debug.Entry(4, $"> foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)", Indent: 1);
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod);
            }
            Debug.Entry(4, $"x foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods) >//", Indent: 1);

            // Collect the "starting" values for damage if the NaturalEquipment is a weapon
            // Accumulate bonuses from NaturalEquipmentMods
            // Apply the finalised values over the top

            Debug.Entry(4, $"? if (ParentMeleeWeapon != null)", Indent: 1);
            if (ParentMeleeWeapon != null)
            {

                OriginalNaturalEquipmentCopy = ParentObject.DeepCopy();
                MeleeWeapon originalWeapon = OriginalNaturalEquipmentCopy.GetPart<MeleeWeapon>();
                DamageDie = new(originalWeapon.BaseDamage);
                DamageDie.ToString().Vomit(4, "DamageDie", Indent: 2);
                AccumulatedDamageDie.Count = DamageDie.GetDieCount().Vomit(4, "AccumulatedDamageDie.Count", Indent: 2);
                AccumulatedDamageDie.Size = (DamageDie.LeftValue > 0 ? DamageDie.RightValue : DamageDie.Left.RightValue).Vomit(4, "AccumulatedDamageDie.Size", Indent: 2);
                AccumulatedDamageDie.Bonus = (DamageDie.LeftValue > 0 ? 0 : DamageDie.RightValue).Vomit(4, "AccumulatedDamageDie.Bonus", Indent: 2);
                AccumulatedHitBonus = originalWeapon.HitBonus.Vomit(4, "AccumulatedHitBonus", Indent: 2);
                AccumulatedPenBonus = originalWeapon.PenBonus.Vomit(4, "AccumulatedPenBonus", Indent: 2);

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

            // Cycle the NaturalEquipmentMods, applying each one to the NaturalEquipment
            ApplyNaturalEquipmentMods();

            // Cycle through the AdjustmentTargets (GameObject, Render, MeleeWeapon, Armor)
            // |__ Cycle through each Target's set of adjustments, applying them if possible 
            //     |__ Where not possible, output a warning.
            Debug.Entry(4, $"> foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)", Indent: 1);
            foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)
            {
                Debug.Entry(4, $"Target: {Target}", Indent: 2);
                Debug.Entry(4, $"> foreach ((string Field, (int Priority, string Value)) in Entries)", Indent: 2);
                foreach ((string Field, (int Priority, string Value)) in Entries)
                {
                    Debug.Entry(4, $"Field: {Field}, Value: {Value}", Indent: 3);
                    if (TargetObject.SetPropertyValue(Field, Value) || TargetObject.SetFieldValue(Field, Value))
                        continue;
                    Debug.Entry(4,
                        $"WARN: {typeof(NaturalEquipmentManager).Name}." +
                        $"{nameof(ManageNaturalEquipment)}()",
                        $"failed find Property or Field \"{Field}\" in {Target}",
                        Indent: 2);
                }
                Debug.Entry(4, $"x foreach ((string Field, (int Priority, string Value)) in Entries) >//", Indent: 2);
            }
            Debug.Entry(4, $"x foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets) >//", Indent: 1);

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

            if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath)) ParentRender.Tile = tilePath;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath)) ParentRender.Tile = tilePath;

            // We want these sick as, modified Natural Equipments to show up as a physical feature.
            // The check for a weapon being undesirable unfortunately targets tags, but we set the IntProp to 0 just in case it changes
            // These are always temporary DefaultBehaviors and should be completely refreshed any time something would normally
            ParentObject.SetIntProperty("ShowAsPhysicalFeature", 1);
            ParentObject.SetIntProperty("UndesirableWeapon", 0);
            ParentObject.SetStringProperty("TemporaryDefaultBehavior", "NaturalEquipmentManager", false);

            Debug.Footer(4,
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");
        }

        public virtual void ApplyNaturalEquipmentMods()
        {
            Debug.Entry(4, $"* {nameof(ApplyNaturalEquipmentMods)}()", Indent: 1);
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                Debug.Entry(4, $"Applying {naturalEquipmentMod.Name} to {ParentObject?.ShortDisplayNameStripped}", Indent: 2);
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
            }
            Debug.Entry(4, $"x {nameof(ApplyNaturalEquipmentMods)}() *//", Indent: 1);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID;
             // || ID == BodyPartsUpdatedEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E.Object.ShortDisplayName})",
                Indent: 0);

            if(E.Object.HasPartDescendedFrom<ModNaturalEquipmentBase>())
            {
                _shortDescriptionCache ??= ProcessDescription(ShortDescriptions);
                E.Postfix.AppendRules(_shortDescriptionCache);
            }

            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BodypartsUpdated");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BodypartsUpdated")
            {
                Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(FireEvent)}(Event E.ID: {E.ID})",
                Indent: 0);

                if (WantToManage && NaturalEquipmentMods != null)
                {
                    BeforeManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
                    ManageNaturalEquipment();
                    AfterManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
                }
                else if (WantToManage)
                {
                    Debug.Entry(4, $"No NaturalEquipmentMods, nothing to manage", Indent: 1);
                }
                else
                {
                    Debug.Entry(4, $"Don't want to manage", Indent: 1);
                }
                Debug.Entry(4,
                    $"x {nameof(NaturalEquipmentManager)}."
                    + $"{nameof(FireEvent)}(Event E.ID: {E.ID}) @//",
                    Indent: 0);
            }
            return base.FireEvent(E);
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
