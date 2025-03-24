using System;
using System.Collections.Generic;
using System.Text;
using XRL.Rules;
using XRL.World.Parts.Mutation;
using SerializeField = UnityEngine.SerializeField;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager : IScribedPart
    {
        private GameObject _wielder = null;
        public GameObject Wielder => _wielder ??= ParentObject?.Equipped;

        public GameObjectBlueprint OriginalNaturalEquipment => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public int DamageDieSize;
        public int DamageDieCount;
        public int DamageBonus;
        public int HitBonus;

        private MeleeWeapon _parentWeapon = null;
        public MeleeWeapon ParentWeapon => _parentWeapon ??= ParentObject?.GetPart<MeleeWeapon>();

        private Armor _parentArmor = null;
        public Armor ParentArmor => _parentArmor ??= ParentObject?.GetPart<Armor>();

        private Render _parentRender = null;
        public Render ParentRender => _parentRender ??= ParentObject?.GetPart<Render>();

        [NonSerialized]
        public SortedDictionary<int, string> ShortDescriptions = new();

        [SerializeField]
        private string _shortDescriptionCache = null;

        [NonSerialized]
        public SortedDictionary<int, ModNaturalEquipmentBase> NaturalWeaponMods = new();

        public override void Attach()
        {

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
            NaturalWeaponMods = new();
        }
        public void ResetShortDescription()
        {
            ClearShortDescriptionCache();
            ClearShortDescriptions();
            ClearNaturalWeaponMods();
        }

        public void AddNaturalWeaponMod(int Priority, ModNaturalEquipmentBase NaturalWeaponMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(AddNaturalWeaponMod)}(Priority: {Priority}, NaturalWeaponMod: {NaturalWeaponMod.Name})",
                Indent: 7);

            NaturalWeaponMods[Priority] = NaturalWeaponMod;
        }

        public void ProcessNaturalWeaponModsShortDescriptions()
        {
            if (NaturalWeaponMods.IsNullOrEmpty()) return;

            foreach ((int priority, ModNaturalEquipmentBase weaponMod) in NaturalWeaponMods)
            {
                AddShortDescriptionEntry(priority, weaponMod.GetInstanceDescription());
            }
        }

        public void CollectNaturalWeaponMods()
        {
            ResetShortDescription();

            foreach (ModNaturalEquipmentBase naturalWeaponMod in ParentObject.GetPartsDescendedFrom<ModNaturalEquipmentBase>())
            {
                AddNaturalWeaponMod(naturalWeaponMod.GetDescriptionPriority(), naturalWeaponMod);
            }
        }

        public void ManageNaturalEquipment()
        {
            Debug.Entry(4, $"* {typeof(NaturalEquipmentManager).Name}.{nameof(ManageNaturalEquipment)}()", Indent: 0);
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
                ManageNaturalEquipment();
            }
            return base.FireEvent(E);
        }


        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(ShortDescriptions);
            Writer.Write(NaturalWeaponMods);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            ShortDescriptions = new SortedDictionary<int, string>(Reader.ReadDictionary<int, string>());
            NaturalWeaponMods = new SortedDictionary<int, ModNaturalEquipmentBase>(Reader.ReadDictionary<int, ModNaturalEquipmentBase>());
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalWeaponDescriber = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            naturalWeaponDescriber.ShortDescriptions = null;
            naturalWeaponDescriber._shortDescriptionCache = null;
            naturalWeaponDescriber.NaturalWeaponMods = null;
            return naturalWeaponDescriber;
        }

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
