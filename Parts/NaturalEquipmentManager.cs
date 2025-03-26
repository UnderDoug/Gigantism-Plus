using System;
using System.Collections.Generic;
using System.Text;
using XRL.Rules;
using XRL.World.Parts.Mutation;
using static XRL.World.Parts.ModNaturalEquipmentBase;
using SerializeField = UnityEngine.SerializeField;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Extensions;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager : IScribedPart
    {
        public const string GAMEOBJECT = "GameObject";
        public const string RENDER = "Render";
        public const string MELEEWEAPON = "MeleeWeapon";
        public const string ARMOR = "Armor";

        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);

        public GameObject OriginalNaturalEquipmentCopy;
        
        public DieRoll DamageDie;
        public (int Count, int Size, int Bonus) AccumulatedDamageDie;
        public int HitBonus;
        public int PenBonus;

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
        }

        public override void Initialize()
        {

            base.Initialize();
        }
        public override void Attach()
        {
            OriginalNaturalEquipmentCopy = ParentObject?.DeepCopy();

            base.Attach();
        }

        public string ProcessDescription(SortedDictionary<int, string> Descriptions, bool IsShort = true)
        {
            StringBuilder StringBuilder = Event.NewStringBuilder();

            CollectNaturalWeaponMods();

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
                + $"{nameof(AddNaturalEquipmentMod)}(Priority: {Priority}, NaturalWeaponMod: {NaturalWeaponMod.Name})",
                Indent: 7);

            NaturalEquipmentMods ??= new();
            NaturalEquipmentMods.Add(NaturalWeaponMod);
        }

        public bool RaplacedBasedOnPriority(Dictionary<string, (int Priority, string Value)> Dictionary, Adjustment Adjustment)
        {
            string @field = Adjustment.Field;
            (int Priority, string Value) entry = (Adjustment.Priority, Adjustment.Value);
            // does an entry for this field exist or, if it does, is its priority beat?
            if (!Dictionary.ContainsKey(@field) || Dictionary[@field].Priority > entry.Priority)
            {
                Dictionary[@field] = entry;
                return true;
            }
            return false;
        }
        public void PrepareNaturalEquipmentModAdjustments(ModNaturalEquipmentBase NaturalWeaponMod)
        {
            if (NaturalWeaponMod?.Adjustments != null && !NaturalWeaponMod.Adjustments.IsNullOrEmpty())
            {
                foreach (Adjustment adjustment in NaturalWeaponMod.Adjustments)
                {
                    string target = adjustment.Target;
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
            }
        }

        public void ProcessNaturalWeaponModsShortDescriptions()
        {
            if (NaturalEquipmentMods.IsNullOrEmpty()) return;

            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                AddShortDescriptionEntry(naturalEquipmentMod.DescriptionPriority, naturalEquipmentMod.GetInstanceDescription());
            }
        }

        public virtual void CollectNaturalWeaponMods()
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

        public virtual void ManageNaturalEquipment()
        {
            Debug.Header(4, 
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");

            CollectNaturalWeaponMods();

            if (ParentMeleeWeapon != null)
            {
                DamageDie = new(ParentMeleeWeapon.BaseDamage);
                AccumulatedDamageDie.Count = DamageDie.GetDieCount();
                AccumulatedDamageDie.Size = DamageDie.LeftValue > 0 ? DamageDie.RightValue : DamageDie.Left.RightValue;
                AccumulatedDamageDie.Bonus = DamageDie.LeftValue > 0 ? 0 : DamageDie.RightValue;
                HitBonus = ParentMeleeWeapon.HitBonus;
                PenBonus = ParentMeleeWeapon.PenBonus;
            }

            // Put all the collected mods into the 
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                PrepareNaturalEquipmentModAdjustments(naturalEquipmentMod);
            }

            ApplyNaturalEquipmentMods();

            foreach ((string Target, (object TargetObject, Dictionary<string, (int Priority, string Value)> Entries)) in AdjustmentTargets)
            {
                foreach ((string Field, (int Priority, string Value)) in Entries)
                {
                    if (TargetObject.SetPropertyValue(Field, Value))
                        continue;
                    if (!TargetObject.SetFieldValue(Field, Value))
                        Debug.Entry(4,
                            $"WARN: {typeof(NaturalEquipmentManager).Name}." +
                            $"{nameof(ManageNaturalEquipment)}()",
                            $"failed find Property or Field \"{Field}\" in {Target}",
                            Indent: 2);
                }
            }

            string icyString = "{{icy|icy}}";
            string flamingString = "{{fiery|flaming}}";
            string displayNameOnlySansRays = ParentObject.DisplayNameOnly;
            displayNameOnlySansRays.Replace(icyString, "");
            displayNameOnlySansRays.Replace(flamingString, "");

            if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath)) ParentRender.Tile = tilePath;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath)) ParentRender.Tile = tilePath;
            ParentObject.SetIntProperty("ShowAsPhysicalFeature", 1);
            ParentObject.SetIntProperty("UndesirableWeapon", 0);
            ParentObject.SetStringProperty("TemporaryDefaultBehavior", "NaturalEquipmentManager", false);

            Debug.Footer(4,
                $"{typeof(NaturalEquipmentManager).Name}",
                $"{nameof(ManageNaturalEquipment)}()");
        }

        public virtual void ApplyNaturalEquipmentMods()
        {
            foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
            {
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID;
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
                BeforeManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
                ManageNaturalEquipment();
                AfterManageDefaultEquipmentEvent.Send(ParentObject, this, ParentLimb);
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
            naturalEquipmentManager.OriginalNaturalEquipmentCopy = OriginalNaturalEquipmentCopy.DeepCopy();
            naturalEquipmentManager.ShortDescriptions = null;
            naturalEquipmentManager._shortDescriptionCache = null;
            naturalEquipmentManager.NaturalEquipmentMods = null;
            return naturalEquipmentManager;
        }

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
