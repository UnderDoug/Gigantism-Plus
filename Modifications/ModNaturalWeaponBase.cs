using System;
using SerializeField = UnityEngine.SerializeField;
using System.Collections.Generic;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : ModNaturalWeaponBase
        where T : IPart, IManagedDefaultNaturalWeapon<T>, new()
    {
        public const string CURRENT_PRIORITY = "CurrentPriority";

        private GameObject _wielder = null;
        public GameObject Wielder
        {
            get
            {
                return _wielder ??= ParentObject?.Equipped;
            }
            set
            {
                Type valueType = value?.GetType();
                if (value != null && valueType.IsEquivalentTo(typeof(GameObject)))
                {
                    _wielder = value;
                    return;
                }
                _wielder = null;
            }
        }

        private T _assigningPart = null;
        public T AssigningPart
        {
            get
            {
                return _assigningPart ??= Wielder?.GetNaturalWeaponCompatiblePart<T>();
            }
            set
            {
                Type valueType = value?.GetType();
                if (value != null && valueType.IsEquivalentTo(typeof(T)))
                {
                    _assigningPart = value;
                    return;
                }
                _assigningPart = null;
            }
        }

        private NaturalWeaponSubpart<T> _naturalWeaponSubpart = null;
        public NaturalWeaponSubpart<T> NaturalWeaponSubpart
        {
            get
            {
                return _naturalWeaponSubpart ??= AssigningPart.GetNaturalWeaponSubpart(Object: ParentObject);
            }
            set
            {
                Type valueType = value?.GetType();
                if (value != null && valueType.IsEquivalentTo(typeof(NaturalWeaponSubpart<T>)))
                {
                    _naturalWeaponSubpart = value;
                    return;
                }
                _naturalWeaponSubpart = null;
            }
        }

        private int? _level = 1;
        public int Level => _level ??= (int)NaturalWeaponSubpart?.Level;

        public ModNaturalWeaponBase()
        {
        }
        public ModNaturalWeaponBase(int Tier)
            : base(Tier)
        {
        }
        public ModNaturalWeaponBase(NaturalWeaponSubpart<T> Subpart)
            : this()
        {
            NaturalWeaponSubpart = new(Subpart);
        }
        public ModNaturalWeaponBase(NaturalWeaponSubpart<T> Subpart, int Tier)
            : this(Tier)
        {
            NaturalWeaponSubpart = Subpart;
        }

        public override int GetDamageDieCount()
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }
        public override int GetDamageDieSize()
        {
            return NaturalWeaponSubpart.DamageDieSize;
        }

        public override int GetDamageBonus()
        {
            return NaturalWeaponSubpart.DamageBonus;
        }

        public override int GetHitBonus()
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public bool ShouldSetProperty(GameObject Object, string Property, string CategoryPriorityProp)
        {
            Debug.Entry(4, $"* {nameof(ApplyDamageChanges)}(Object: {Object.ShortDisplayNameStripped})", Indent: 5);
            // Setup
            string PropertyPriorityProp = $"{CategoryPriorityProp}::{Property}";
            bool CurrentPropertyPriorityExists = Object.HasIntProperty(PropertyPriorityProp);
            int CurrentPropertyPriority = Object.GetIntProperty(PropertyPriorityProp);
            int? ThisPropertyPriority = NaturalWeaponSubpart.GetPropertyPriority(Property);
            int TargetPriority = Math.Max(CurrentPropertyPriority, Object.GetIntProperty(CategoryPriorityProp));

            // tracing
            Debug.LoopItem(4, $" {nameof(CurrentPropertyPriorityExists)}", $"{CurrentPropertyPriorityExists}", Indent: 6);
            Debug.LoopItem(4, $" {nameof(CurrentPropertyPriority)}", $"{CurrentPropertyPriority}", Indent: 6);
            Debug.LoopItem(4, $" {nameof(ThisPropertyPriority)}", $"{ThisPropertyPriority}", Indent: 6);
            Debug.LoopItem(4, $" {nameof(TargetPriority)}", $"{TargetPriority}", Indent: 6);

            if (!CurrentPropertyPriorityExists || ThisPropertyPriority == null || ThisPropertyPriority > TargetPriority)
            {
                if (ThisPropertyPriority != null)
                {
                    Debug.LoopItem(4, $" Before: {nameof(CurrentPropertyPriority)}:", $"{CurrentPropertyPriority}", Indent: 7);
                    Object.SetIntProperty(PropertyPriorityProp, (int)ThisPropertyPriority);
                    Debug.LoopItem(4, $"  After: {nameof(CurrentPropertyPriority)}:", $"{CurrentPropertyPriority}", Indent: 7);
                }
                return true;
            }
            return false;
        }

        public virtual void ApplyDamageChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyDamageChanges)}(Object: {Object.ShortDisplayNameStripped})", Indent: 4);
            if (!NaturalWeaponSubpart.IsCosmeticOnly())
            {
                // Setup
                string Category = NaturalWeaponSubpart<T>.DAMAGE_CATEGORY_NAME;
                string ID = $"{NaturalWeaponSubpart.GetNaturalWeaponModName()}::{Category}";

                string DamageDieCountID = GetModPropertyID(nameof(NaturalWeaponSubpart.DamageDieCount));
                string DamageDieSizeID = GetModPropertyID(nameof(NaturalWeaponSubpart.DamageDieSize));
                string DamageBonusID = GetModPropertyID(nameof(NaturalWeaponSubpart.DamageBonus));
                string HitBonusID = GetModPropertyID(nameof(NaturalWeaponSubpart.HitBonus));

                int CurrentDamageDieCountApplied = Object.GetIntProperty(DamageDieCountID);
                int CurrentDamageDieSizeApplied = Object.GetIntProperty(DamageDieSizeID);
                int CurrentDamageBonusApplied = Object.GetIntProperty(DamageBonusID);
                int CurrentHitBonusApplied = Object.GetIntProperty(HitBonusID);

                Debug.LoopItem(4, $"{DamageDieCountID}", $"{CurrentDamageDieCountApplied}", Indent: 5);
                Debug.LoopItem(4, $"{DamageDieSizeID}", $"{CurrentDamageDieSizeApplied}", Indent: 5);
                Debug.LoopItem(4, $"{DamageBonusID}", $"{CurrentDamageBonusApplied}", Indent: 5);
                Debug.LoopItem(4, $"{HitBonusID}", $"{CurrentHitBonusApplied}", Indent: 5);

                MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

                weapon.AdjustDamageDieCount(GetDamageDieCount() - CurrentDamageDieCountApplied);
                weapon.AdjustDamageDieSize(GetDamageDieSize() - CurrentDamageDieSizeApplied);
                weapon.AdjustDamage(GetDamageBonus() - CurrentDamageBonusApplied);
                weapon.AdjustDamage(GetHitBonus() - CurrentHitBonusApplied);

                Object.SetIntProperty(DamageDieCountID, GetDamageDieCount());
                Object.SetIntProperty(DamageDieSizeID, GetDamageDieSize());
                Object.SetIntProperty(DamageBonusID, GetDamageBonus());
                Object.SetIntProperty(HitBonusID, GetHitBonus());

                Debug.LoopItem(4, $"{DamageDieCountID}", $"{Object.GetIntProperty(DamageDieCountID)}", Indent: 5);
                Debug.LoopItem(4, $"{DamageDieSizeID}", $"{Object.GetIntProperty(DamageDieSizeID)}", Indent: 5);
                Debug.LoopItem(4, $"{DamageBonusID}", $"{Object.GetIntProperty(DamageBonusID)}", Indent: 5);
                Debug.LoopItem(4, $"{HitBonusID}", $"{Object.GetIntProperty(HitBonusID)}", Indent: 5);


                /*
                string CategoryPriorityProp = $"{CURRENT_PRIORITY}::{Category}";
                bool CurrentDamagePriorityExists = Object.HasIntProperty(CategoryPriorityProp);
                int CurrentDamagePriority = Object.GetIntProperty(CategoryPriorityProp);
                int? ThisDamagePriority = NaturalWeaponSubpart.GetCategoryPriority(Category);


                // tracing
                Debug.LoopItem(4, $" {nameof(CurrentDamagePriorityExists)}:", $"{CurrentDamagePriorityExists}", Indent: 5);
                Debug.LoopItem(4, $" {nameof(CurrentDamagePriority)}:", $"{CurrentDamagePriority}", Indent: 5);
                Debug.LoopItem(4, $" {nameof(ThisDamagePriority)}:", $"{ThisDamagePriority}", Indent: 5);


                Debug.Entry(4, @"? if (!CurrentDamagePriorityExists || ThisDamagePriority == null || ThisDamagePriority > CurrentDamagePriority)", Indent: 5);
                if (!CurrentDamagePriorityExists || ThisDamagePriority == null || ThisDamagePriority > CurrentDamagePriority)
                {
                    Debug.LoopItem(4, $" Before: {nameof(CurrentDamagePriority)}:", $"{CurrentDamagePriority}", Indent: 6);
                    if (ThisDamagePriority != null) Object.SetIntProperty(CategoryPriorityProp, (int)ThisDamagePriority);
                    Debug.LoopItem(4, $"  After: {nameof(CurrentDamagePriority)}:", $"{CurrentDamagePriority}", Indent: 6);

                    int DamageDieCount = GetDamageDieCount();
                    if (ShouldSetProperty(Object, nameof(DamageDieCount), CategoryPriorityProp)) return;
                }
                else
                {
                    Debug.Entry(4, @"x if (!CurrentDamagePriorityExists || ThisDamagePriority == null || ThisDamagePriority > CurrentDamagePriority) ?//", Indent: 5);
                }
                */
            }
            else
            {
                Debug.Entry(4, $"NaturalWeaponSubpart.IsCosmeticOnly", "Aborting", Indent: 5);
            }
            Debug.Entry(4, $"x {nameof(ApplyDamageChanges)}(Object: {Object.ShortDisplayNameStripped}) *//", Indent: 4);
        }


        public virtual void ApplyGenericChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyGenericChanges)}(GameObject Object)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {Level}", Indent: 5);

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-1 into 1d2+0
            if (Object.GetNaturalWeaponModsCount() < 1) weapon.AdjustDamage(1);

            List<string> vomitCats = new() { "Damage" };
            weapon.Vomit(4, "Generic, Before", vomitCats, Indent: 4);
            Debug.Divider(4, "\u2500", 40, Indent: 4);

            Debug.Entry(4, $"NaturalWeaponSubpart Adjustments", Indent: 4);
            Debug.LoopItem(4, $"DamageDieCount", $"{GetDamageDieCount().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"DamageDieSize", $"{GetDamageDieSize().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"DamageBonus", $"{GetDamageBonus().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"HitBonus", $"{GetHitBonus().Signed()}", Indent: 5);

            weapon.AdjustDamageDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();
            weapon.MaxStrengthBonus = 999;

            Debug.Divider(4, "\u2500", 40, Indent: 4);
            weapon.Vomit(4, "Generic, After", vomitCats, Indent: 4);

            Debug.Entry(4, $"x {nameof(ApplyGenericChanges)}(GameObject Object) *//", Indent: 4);
        }

        public virtual int ApplyPriorityChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyPriorityChanges)}(GameObject Object)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}", Indent: 5);

            Render render = Object.Render;
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            List<string> vomitCats = new() { "Combat", "Render" };

            int NounPriority = NaturalWeaponSubpart.NounPriority;
            int AdjectivePriority = NaturalWeaponSubpart.AdjectivePriority;
            int CurrentNounPriority = Object.GetIntProperty(CURRENT_NOUN_PRIORITY);
            int CurrentAdjectivePriority = Object.GetIntProperty(CURRENT_ADJECTIVE_PRIORITY);

            Debug.Entry(4, $"? if (NounPriority != 0 and NounPriority < CurrentPriority)", Indent: 5);
            if (NounPriority != 0 && NounPriority < CurrentNounPriority )
            {
                Debug.Entry(4, 
                    $"+ NounPriority != 0 and NounPriority ({NounPriority}) < CurrentNounPriority ({CurrentNounPriority})", 
                    Indent: 6);

                Object.SetIntProperty(CURRENT_NOUN_PRIORITY, NounPriority);
                
                weapon.Vomit(4, "NounPriority, Before", vomitCats, Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);

                Debug.Entry(4, $"NaturalWeaponSubpart Attribute", Indent: 6);
                Debug.LoopItem(4, $"Skill", $"{NaturalWeaponSubpart.Skill} ", Indent: 7);
                Debug.LoopItem(4, $"Noun", $"{NaturalWeaponSubpart.Noun} ", Indent: 7);
                Debug.LoopItem(4, $"Tile", $"{NaturalWeaponSubpart.Tile} ", Indent: 7);
                Debug.LoopItem(4, $"ColorString", $"{NaturalWeaponSubpart.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"SecondColorString", $"{NaturalWeaponSubpart.SecondColorString} ", Indent: 7);
                Debug.LoopItem(4, $"DetailColor", $"{NaturalWeaponSubpart.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"SecondDetailColor", $"{NaturalWeaponSubpart.SecondDetailColor} ", Indent: 7);
                Debug.LoopItem(4, $"SwingSound", $"{NaturalWeaponSubpart.SwingSound} ", Indent: 7);
                Debug.LoopItem(4, $"BlockedSound", $"{NaturalWeaponSubpart.BlockedSound} ", Indent: 7);

                weapon.Skill = NaturalWeaponSubpart.Skill ?? weapon.Skill;
                render.DisplayName = NaturalWeaponSubpart.Noun ?? render.DisplayName;
                render.Tile = NaturalWeaponSubpart.Tile ?? render.Tile;

                render.ColorString = 
                    (render.ColorString == NaturalWeaponSubpart.ColorString) 
                    ? NaturalWeaponSubpart.SecondColorString 
                    : NaturalWeaponSubpart.ColorString;

                render.DetailColor = 
                    (render.DetailColor == NaturalWeaponSubpart.DetailColor) 
                    ? NaturalWeaponSubpart.SecondDetailColor 
                    : NaturalWeaponSubpart.DetailColor;

                Object.SetSwingSound(NaturalWeaponSubpart.SwingSound);
                Object.SetBlockedSound(NaturalWeaponSubpart.BlockedSound);

                Debug.Divider(4, "\u2500", 40, Indent: 6);
                weapon.Vomit(4, "NounPriority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4, 
                    $"- NounPriority == 0 || NounPriority ({NounPriority}) >= CurrentNounPriority ({CurrentNounPriority})", 
                    Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);
                weapon.Vomit(4, "Priority, Unchanged", vomitCats, Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);
            }
            Debug.Entry(4, $"x if (NounPriority != 0 and NounPriority < CurrentNounPriority) ?//", Indent: 5);

            Debug.Entry(4, $"? if (AdjectivePriority != 0 and AdjectivePriority < CurrentAdjectivePriority)", Indent: 5);
            if (AdjectivePriority != 0 && AdjectivePriority < CurrentAdjectivePriority)
            {
                Debug.Entry(4,
                    $"+ AdjectivePriority != 0 and " 
                    + $"CurrentAdjectivePriority AdjectivePriority ({AdjectivePriority}) < ({CurrentAdjectivePriority})",
                    Indent: 6);

                Object.SetIntProperty(CURRENT_ADJECTIVE_PRIORITY, AdjectivePriority);

                weapon.Vomit(4, "AdjectivePriority, Before", vomitCats, Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);

                Debug.Entry(4, $"NaturalWeaponSubpart Attribute", Indent: 6);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.EquipmentFrameColors", $"{NaturalWeaponSubpart.EquipmentFrameColors} ", Indent: 7);

                Object.SetEquipmentFrameColors(NaturalWeaponSubpart.EquipmentFrameColors);

                Debug.Divider(4, "\u2500", 40, Indent: 6);
                weapon.Vomit(4, "AdjectivePriority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4,
                    $"- AdjectivePriority == 0 || " 
                    + $"AdjectivePriority ({AdjectivePriority}) >= CurrentAdjectivePriority ({CurrentAdjectivePriority})",
                    Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);
                weapon.Vomit(4, "AdjectivePriority, Unchanged", vomitCats, Indent: 6);
                Debug.Divider(4, "\u2500", 40, Indent: 6);
            }
            Debug.Entry(4, $"x if (AdjectivePriority != 0 and AdjectivePriority < CurrentAdjectivePriority) ?//", Indent: 5);

            Debug.Entry(4, $"x {nameof(ApplyPriorityChanges)}(GameObject Object) *//", Indent: 4);
            return NaturalWeaponSubpart.ModPriority;
        }

        public virtual void ApplyPartAndPropChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyPartAndPropChanges)}(GameObject Object)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {Level}", Indent: 5);

            if (NaturalWeaponSubpart.AddedParts != null)
            {
                Debug.Entry(4, "> foreach (string part in NaturalWeaponSubpart.GetAddedParts())", Indent: 5);
                foreach (string part in NaturalWeaponSubpart.AddedParts)
                {
                    Debug.LoopItem(4, "part", part, Indent: 6);
                    Object.RequirePart(part);
                }
                Debug.Entry(4, $"x foreach (string part in NaturalWeaponSubpart.GetAddedParts()) >//", Indent: 5);
            }

            if (NaturalWeaponSubpart.AddedStringProps != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, string> entry in NaturalWeaponSubpart.GetAddedStringProps())", Indent: 5);
                foreach ((string Name, string Value) in NaturalWeaponSubpart.AddedStringProps)
                {
                    Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6);
                    Object.SetStringProperty(Name: Name, Value: Value, RemoveIfNull: true);
                }
                Debug.Entry(4, $"x foreach (KeyValuePair<string, string> entry in NaturalWeaponSubpart.GetAddedStringProps()) >//", Indent: 5);
            }

            if (NaturalWeaponSubpart.AddedIntProps != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, int> entry in NaturalWeaponSubpart.GetAddedIntProps())", Indent: 5);
                foreach ((string Name, int Value) in NaturalWeaponSubpart.AddedIntProps)
                {
                    Debug.LoopItem(4, $"{Name}", $"{Value}", Indent: 6);
                    Object.SetIntProperty(Name: Name, Value: Value, RemoveIfZero: true);
                }
                Debug.Entry(4, $"x foreach ((string Name, int Value) in NaturalWeaponSubpart.GetAddedIntProps()) >//", Indent: 5);
            }

            Debug.Entry(4, $"x {nameof(ApplyPartAndPropChanges)}(GameObject Object) *//", Indent: 4);
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ {Name}.{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\")", Indent: 3);
            Object.RequirePart<NaturalWeaponDescriber>();
            Debug.Entry(4, "? if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))", Indent: 4);
            if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))
            {
                Debug.Entry(4, "+ NaturalWeaponDescriber is Present", Indent: 5);
                NaturalWeaponDescriber.AddNaturalWeaponMod(NaturalWeaponSubpart.ModPriority, this);
                NaturalWeaponDescriber.ProcessNaturalWeaponModsShortDescriptions();
            }
            else
            {
                Debug.Entry(4, "- NaturalWeaponDescriber not Present, this is an abnormal state", Indent: 5);
            }
            Debug.Entry(4, $"x if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber)) ?//", Indent: 4);

            ApplyGenericChanges(Object);
            ApplyPriorityChanges(Object);
            ApplyPartAndPropChanges(Object);

            Render render = Object.Render;

            string icyString = "{{icy|icy}}";
            string flamingString = "{{fiery|flaming}}";
            string displayNameOnlySansRays = ParentObject.DisplayNameOnly;
            displayNameOnlySansRays.Replace(icyString, "");
            displayNameOnlySansRays.Replace(flamingString, "");

            if (TryGetTilePath(BuildCustomTilePath(displayNameOnlySansRays), out string tilePath)) render.Tile = tilePath;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out tilePath)) render.Tile = tilePath;
            Object.SetIntProperty("ShowAsPhysicalFeature", 1);
            Object.SetIntProperty("UndesirableWeapon", 0);
            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningPart.Name, false);
            Object.ModIntProperty("ModNaturalWeaponCount", 1);

            Debug.LoopItem(4, $"ShowAsPhysicalFeature", $"{Object.GetIntProperty("ShowAsPhysicalFeature")}", Indent: 4);
            Debug.LoopItem(4, $"UndesirableWeapon", $"{Object.GetIntProperty("UndesirableWeapon")}", Indent: 4);
            Debug.LoopItem(4, $"TemporaryDefaultBehavior", $"{Object.GetStringProperty("TemporaryDefaultBehavior")}", Indent: 4);
            Debug.LoopItem(4, $"ModNaturalWeaponCount", $"{Object.GetIntProperty("ModNaturalWeaponCount")}", Indent: 4);

            Debug.Entry(4, $"x {Name}.{nameof(ApplyModification)}(Object: \"{Object.ShortDisplayNameStripped}\") @//", Indent: 3);
            base.ApplyModification(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            return null;
        }

        public override int GetDescriptionPriority()
        {
            return NaturalWeaponSubpart.ModPriority;
        }

        public override string GetModPropertyID(string Property)
        {
            string output = $"{NaturalWeaponSubpart.GetNaturalWeaponModName()}::";
            string category = NaturalWeaponSubpart<T>.PropertyCategories[Property];
            if (category == null) return null;
            output += category + "::";
            return output + "::" + Property;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalWeaponBase<T> modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalWeaponBase<T>;
            return ClearForCopy(modNaturalWeaponBase);
        }
        public static ModNaturalWeaponBase<T> ClearForCopy(ModNaturalWeaponBase<T> ModNaturalWeaponBase)
        {
            ModNaturalWeaponBase.AssigningPart = null;
            ModNaturalWeaponBase.Wielder = null;
            ModNaturalWeaponBase.NaturalWeaponSubpart = null;
            return ModNaturalWeaponBase;
        }

    } //!-- public class ModNaturalWeaponBase<T> : IMeleeModification where T : IPart, IManagedDefaultNaturalWeapon<T>, new()

    public abstract class ModNaturalWeaponBase : IMeleeModification
    {
        public ModNaturalWeaponBase()
        {
        }
        public ModNaturalWeaponBase(int Tier)
            : base(Tier)
        {
        }

        public override void Configure()
        {
            base.Configure();
            WorksOnSelf = true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() &&
                !Object.HasPart<Physics>() &&
                !Object.HasPart<NaturalEquipment>())
            {
                return false;
            }
            return true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            if (obj.Physics.Equipped != who) obj.Physics.Equipped = who;
            return base.BeingAppliedBy(obj, who);
        }

        public abstract int GetDamageDieCount();
        public abstract int GetDamageDieSize();
        public abstract int GetDamageBonus();
        public abstract int GetHitBonus();

        public abstract string Skill();
        public abstract string Stat();

        public abstract string GetTile();
        public abstract string ColorString();
        public abstract string DetailColor();
        public abstract string SecondColorString();
        public abstract string SecondDetailColor();
        public abstract string SwingSound();
        public abstract string BlockedSound();
        public abstract string EquipmentFrameColors();

        public abstract string GetModPropertyID(string Property);

        public override void ApplyModification(GameObject Object)
        {
            base.ApplyModification(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }

        public abstract string GetInstanceDescription();
        public abstract int GetDescriptionPriority();

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalWeaponBase modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalWeaponBase;
            return ClearForCopy(modNaturalWeaponBase);
        }
        public static ModNaturalWeaponBase ClearForCopy(ModNaturalWeaponBase ModNaturalWeaponBase)
        {
            return ModNaturalWeaponBase;
        }

    } //!-- public class ModNaturalWeaponBase<T> : IMeleeModification where T : IPart, IManagedDefaultNaturalWeapon<T>, new()
}