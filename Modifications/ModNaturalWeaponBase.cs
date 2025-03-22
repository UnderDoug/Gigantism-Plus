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

        public override void Configure()
        {
            base.Configure();
        }
        public override int GetModificationSlotUsage()
        {
            return base.GetModificationSlotUsage();
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            return base.ModificationApplicable(Object);
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            return base.BeingAppliedBy(obj, who);
        }

        public override int GetDamageDieCount()
        {
            return Math.Max(0, NaturalWeaponSubpart.GetDamageDieCount());
        }
        public override int GetDamageDieSize()
        {
            return Math.Max(0, NaturalWeaponSubpart.GetDamageDieSize());
        }

        public override int GetDamageBonus()
        {
            // base damage bonus is 0
            return NaturalWeaponSubpart.GetDamageBonus();
        }

        public override int GetHitBonus()
        {
            // base hit bonus is 0
            return NaturalWeaponSubpart.GetHitBonus();
        }

        public virtual void ApplyGenericChanges(GameObject Object)
        {
            Debug.Entry(4, $"* {nameof(ApplyGenericChanges)}(GameObject Object)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {Level}", Indent: 5);

            Object.RequirePart<NaturalWeaponDescriber>();
            Debug.Entry(4, "? if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))", Indent: 5);
            if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))
            {
                Debug.Entry(4, "+ NaturalWeaponDescriber is Present", Indent: 6);
                if (!Object.HasNaturalWeaponMods())
                {
                    Debug.Entry(4, "No NaturalWeaponMods", "Resetting Short Description", Indent: 6);
                    NaturalWeaponDescriber.ResetShortDescription();
                }
                else
                {
                    Debug.Entry(4, "Have NaturalWeaponMods", "Continuting to accumulate descriptions", Indent: 6);
                }

                if (GetInstanceDescription() != null && GetInstanceDescription() != string.Empty)
                {
                    NaturalWeaponDescriber.AddNaturalWeaponMod(NaturalWeaponSubpart.ModPriority, this);
                }
            }
            else
            {
                Debug.Entry(4, "- NaturalWeaponDescriber not Present", Indent: 6);
            }
            Debug.Entry(4, $"x if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber)) ?//", Indent: 5);

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-1 into 1d2+0
            if (!Object.HasNaturalWeaponMods()) weapon.AdjustDamage(1);

            List<string> vomitCats = new() { "Damage" };
            weapon.Vomit(4, "Generic, Before", vomitCats, Indent: 4);
            Debug.Divider(4, "\u2500", 40, Indent: 4);

            Debug.Entry(4, $"NaturalWeaponSubpart Adjustments", Indent: 4);
            Debug.LoopItem(4, $"DamageDieCount", $"{GetDamageDieCount().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageDieSize", $"{GetDamageDieSize().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageBonus", $"{GetDamageBonus().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetHitBonus", $"{GetHitBonus().Signed()}", Indent: 5);

            weapon.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();

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
                Debug.LoopItem(4, $"NaturalWeaponSubpart.Skill", $"{NaturalWeaponSubpart.Skill}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.Noun", $"{NaturalWeaponSubpart.Noun}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.Tile", $"{NaturalWeaponSubpart.Tile}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.ColorString", $"{NaturalWeaponSubpart.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.SecondColorString", $"{NaturalWeaponSubpart.SecondColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.DetailColor", $"{NaturalWeaponSubpart.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.SecondDetailColor", $"{NaturalWeaponSubpart.SecondDetailColor} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.SwingSound", $"{NaturalWeaponSubpart.SwingSound} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeaponSubpart.BlockedSound", $"{NaturalWeaponSubpart.BlockedSound} ", Indent: 7);

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
        public const string CURRENT_ADJECTIVE_PRIORITY = "CurrentNaturalWeaponAdjectivePriority";
        public const string CURRENT_NOUN_PRIORITY = "CurrentNaturalWeaponNounPriority";

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