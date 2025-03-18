using System;
using SerializeField = UnityEngine.SerializeField;
using System.Collections.Generic;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.IManagedDefaultNaturalWeapon;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : IMeleeModification
        where T : IPart, IManagedDefaultNaturalWeapon, new()
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
        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            if (obj.Physics.Equipped != who) obj.Physics.Equipped = who;
            return base.BeingAppliedBy(obj, who);
        }

        private GameObject _wielder = null;
        public GameObject Wielder => _wielder ??= ParentObject?.Equipped;

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

        [SerializeField]
        private int _level = 1;
        public int Level => _level = NaturalWeapon?.GetLevel() != null ? NaturalWeapon.GetLevel() : _level;

        private NaturalWeaponSubpart _naturalWeapon = null;
        public NaturalWeaponSubpart NaturalWeapon => _naturalWeapon ??= AssigningPart.GetNaturalWeapon();

        public const string CURRENT_ADJECTIVE_PRIORITY = "CurrentNaturalWeaponAdjectivePriority";
        public const string CURRENT_NOUN_PRIORITY = "CurrentNaturalWeaponNounPriority";

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalWeaponBase<T> modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalWeaponBase<T>;
            return ClearForCopy(modNaturalWeaponBase);
        }
        public static ModNaturalWeaponBase<T> ClearForCopy(ModNaturalWeaponBase<T> ModNaturalWeaponBase)
        {
            ModNaturalWeaponBase._assigningPart = null;
            ModNaturalWeaponBase._wielder = null;
            ModNaturalWeaponBase._naturalWeapon = null;
            return ModNaturalWeaponBase;
        }

        public virtual int GetDamageDieCount()
        {
            return Math.Max(0, NaturalWeapon.GetDamageDieCount());
        }
        public virtual int GetDamageDieSize()
        {
            return Math.Max(0, NaturalWeapon.GetDamageDieSize());
        }

        public virtual int GetDamageBonus()
        {
            // base damage bonus is 0
            return NaturalWeapon.GetDamageBonus();
        }

        public virtual int GetHitBonus()
        {
            // base hit bonus is 0
            return NaturalWeapon.GetHitBonus();
        }

        public virtual void ApplyGenericChanges(GameObject Object, NaturalWeaponSubpart NaturalWeapon, string InstanceDescription)
        {
            Debug.Entry(4, $"* {nameof(ApplyGenericChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon, string InstanceDescription)", Indent: 4);
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
                    Debug.Entry(4, "Have NaturalWeaponMods", "Continuting to accumulate descripiptions", Indent: 6);
                }
                if (InstanceDescription != null && InstanceDescription != string.Empty)
                NaturalWeaponDescriber.AddShortDescriptionEntry(NaturalWeapon.GetPriority(), InstanceDescription);
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

            string[] vomitCats = new string[] { "Damage" };
            weapon.Vomit(4, "Generic, Before", vomitCats, Indent: 4);

            Debug.Entry(4, $"NaturalWeapon Adjustments", Indent: 4);
            Debug.LoopItem(4, $"DamageDieCount", $"{GetDamageDieCount().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageDieSize", $"{GetDamageDieSize().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageBonus", $"{GetDamageBonus().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetHitBonus", $"{GetHitBonus().Signed()}", Indent: 5);

            weapon.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();
            weapon.Vomit(4, "Generic, After", vomitCats, Indent: 4);
            Debug.Entry(4, $"x {nameof(ApplyGenericChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon, string InstanceDescription) *//", Indent: 4);
        }

        public virtual int ApplyPriorityChanges(GameObject Object, NaturalWeaponSubpart NaturalWeapon)
        {
            Debug.Entry(4, $"* {nameof(ApplyPriorityChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}", Indent: 5);

            Render render = Object.Render;
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            string[] vomitCats = new string[] { "Combat", "Render" };

            int NounPriority = NaturalWeapon.GetNounPriority();
            int AdjectivePriority = NaturalWeapon.GetAdjectivePriority();
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

                Debug.Entry(4, $"NaturalWeapon Attribute", Indent: 6);
                Debug.LoopItem(4, $"NaturalWeapon.Skill", $"{NaturalWeapon.Skill}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.Noun", $"{NaturalWeapon.Noun}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.Tile", $"{NaturalWeapon.Tile}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.ColorString", $"{NaturalWeapon.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SecondColorString", $"{NaturalWeapon.SecondColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.DetailColor", $"{NaturalWeapon.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SecondDetailColor", $"{NaturalWeapon.SecondDetailColor} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SwingSound", $"{NaturalWeapon.GetSwingSound()} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.BlockedSound", $"{NaturalWeapon.GetBlockedSound()} ", Indent: 7);

                weapon.Skill = NaturalWeapon.Skill ?? weapon.Skill;
                render.DisplayName = NaturalWeapon.Noun ?? render.DisplayName;
                render.Tile = NaturalWeapon.Tile ?? render.Tile;

                render.ColorString = 
                    (render.ColorString == NaturalWeapon.ColorString) 
                    ? NaturalWeapon.SecondColorString 
                    : NaturalWeapon.ColorString;

                render.DetailColor = 
                    (render.DetailColor == NaturalWeapon.DetailColor) 
                    ? NaturalWeapon.SecondDetailColor 
                    : NaturalWeapon.DetailColor;

                Object.SetSwingSound(NaturalWeapon.GetSwingSound());
                Object.SetBlockedSound(NaturalWeapon.GetBlockedSound());

                weapon.Vomit(4, "NounPriority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4, 
                    $"- NounPriority == 0 || NounPriority ({NounPriority}) >= CurrentNounPriority ({CurrentNounPriority})", 
                    Indent: 6);
                weapon.Vomit(4, "Priority, Unchanged", vomitCats, Indent: 6);
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

                Debug.Entry(4, $"NaturalWeapon Attribute", Indent: 6);
                Debug.LoopItem(4, $"NaturalWeapon.EquipmentFrameColors", $"{NaturalWeapon.GetEquipmentFrameColors()} ", Indent: 7);

                Object.SetEquipmentFrameColors(NaturalWeapon.GetEquipmentFrameColors());

                weapon.Vomit(4, "AdjectivePriority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4,
                    $"- AdjectivePriority == 0 || " 
                    + $"AdjectivePriority ({AdjectivePriority}) >= CurrentAdjectivePriority ({CurrentAdjectivePriority})",
                    Indent: 6);
                weapon.Vomit(4, "AdjectivePriority, Unchanged", vomitCats, Indent: 6);
            }
            Debug.Entry(4, $"x if (AdjectivePriority != 0 and AdjectivePriority < CurrentAdjectivePriority) ?//", Indent: 5);

            Debug.Entry(4, $"x {nameof(ApplyPriorityChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon) *//", Indent: 4);
            return NaturalWeapon.Priority;
        }

        public virtual void ApplyPartAndPropChanges(GameObject Object, NaturalWeaponSubpart NaturalWeapon)
        {
            Debug.Entry(4, $"* {nameof(ApplyPartAndPropChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {Level}", Indent: 5);

            if (NaturalWeapon.GetAddedParts() != null)
            {
                Debug.Entry(4, "> foreach (string part in NaturalWeapon.GetAddedParts())", Indent: 5);
                foreach (string part in NaturalWeapon.GetAddedParts())
                {
                    Debug.LoopItem(4, "part", part, Indent: 6);
                    Object.RequirePart(part);
                }
                Debug.Entry(4, $"x foreach (string part in NaturalWeapon.GetAddedParts()) >//", Indent: 5);
            }

            if (NaturalWeapon.GetAddedStringProps() != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, string> entry in NaturalWeapon.GetAddedStringProps())", Indent: 5);
                foreach (KeyValuePair<string, string> entry in NaturalWeapon.GetAddedStringProps())
                {
                    Debug.LoopItem(4, "entry", $"{entry.Key}, {entry.Value}", Indent: 6);
                    Object.SetStringProperty(Name: entry.Key, Value: entry.Value, RemoveIfNull: true);
                }
                Debug.Entry(4, $"x foreach (KeyValuePair<string, string> entry in NaturalWeapon.GetAddedStringProps()) >//", Indent: 5);
            }

            if (NaturalWeapon.GetAddedIntProps() != null)
            {
                Debug.Entry(4, "> foreach (KeyValuePair<string, int> entry in NaturalWeapon.GetAddedIntProps())", Indent: 5);
                foreach (KeyValuePair<string, int> entry in NaturalWeapon.GetAddedIntProps())
                {
                    Debug.LoopItem(4, "entry", $"{entry.Key}, {entry.Value}", Indent: 6);
                    Object.SetIntProperty(Name: entry.Key, Value: entry.Value, RemoveIfZero: true);
                }
                Debug.Entry(4, $"x foreach (KeyValuePair<string, int> entry in NaturalWeapon.GetAddedIntProps()) >//", Indent: 5);
            }

            Debug.Entry(4, $"x {nameof(ApplyPartAndPropChanges)}(GameObject Object, NaturalWeaponSubpart NaturalWeapon) *//", Indent: 4);
        }

        public override void ApplyModification(GameObject Object)
        {
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

        public virtual string GetInstanceDescription()
        {
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            return $"{descriptionName}: This weapon has been constructed by modifications.";
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

    } //!-- public class ModNaturalWeaponBase : IMeleeModification where T : IPart, IManagedDefaultNaturalWeapon, new()
}