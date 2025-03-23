using System;
using System.Collections.Generic;
using System.Text;
using XRL.World.Parts.Mutation;
using SerializeField = UnityEngine.SerializeField;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalWeaponDescriber : IScribedPart
    {
        [NonSerialized]
        public SortedDictionary<int, string> ShortDescriptions = new();

        [SerializeField]
        private string _shortDescriptionCache = null;

        [NonSerialized]
        public SortedDictionary<int, ModNaturalWeaponBase> NaturalWeaponMods = new();

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
                $"@ {nameof(NaturalWeaponDescriber)}."
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

        public void AddNaturalWeaponMod(int Priority, ModNaturalWeaponBase NaturalWeaponMod)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalWeaponDescriber)}."
                + $"{nameof(AddNaturalWeaponMod)}(Priority: {Priority}, NaturalWeaponMod: {NaturalWeaponMod.Name})",
                Indent: 7);

            NaturalWeaponMods[Priority] = NaturalWeaponMod;
        }

        public void ProcessNaturalWeaponModsShortDescriptions()
        {
            if (NaturalWeaponMods.IsNullOrEmpty()) return;

            foreach ((int priority, ModNaturalWeaponBase weaponMod) in NaturalWeaponMods)
            {
                AddShortDescriptionEntry(priority, weaponMod.GetInstanceDescription());
            }
        }

        public void CollectNaturalWeaponMods()
        {
            ResetShortDescription();

            foreach (ModNaturalWeaponBase naturalWeaponMod in ParentObject.GetPartsDescendedFrom<ModNaturalWeaponBase>())
            {
                AddNaturalWeaponMod(naturalWeaponMod.GetDescriptionPriority(), naturalWeaponMod);
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
                $"@ {nameof(NaturalWeaponDescriber)}."
                + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E.Object.ShortDisplayName})",
                Indent: 0);

            if(E.Object.HasPartDescendedFrom<ModNaturalWeaponBase>())
            {
                _shortDescriptionCache ??= ProcessDescription(ShortDescriptions);
                E.Postfix.AppendRules(_shortDescriptionCache);
            }

            return base.HandleEvent(E);
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
            NaturalWeaponMods = new SortedDictionary<int, ModNaturalWeaponBase>(Reader.ReadDictionary<int, ModNaturalWeaponBase>());
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalWeaponDescriber naturalWeaponDescriber = base.DeepCopy(Parent, MapInv) as NaturalWeaponDescriber;
            naturalWeaponDescriber.ShortDescriptions = null;
            naturalWeaponDescriber._shortDescriptionCache = null;
            naturalWeaponDescriber.NaturalWeaponMods = null;
            return naturalWeaponDescriber;
        }

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
