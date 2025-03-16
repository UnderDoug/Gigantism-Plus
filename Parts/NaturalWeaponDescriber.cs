using System;
using System.Collections.Generic;
using System.Text;
using SerializeField = UnityEngine.SerializeField;
using HNPS_GigantismPlus;
using System.IO;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalWeaponDescriber : IScribedPart
    {
        [NonSerialized]
        public SortedDictionary<int, string> ShortDescriptions = new();

        [SerializeField]
        private string _shortDescriptionCache = string.Empty;

        public string ProcessDescription(SortedDictionary<int, string> Descriptions, bool IsShort = true)
        {
            StringBuilder StringBuilder = Event.NewStringBuilder();

            foreach (KeyValuePair<int, string> description in Descriptions)
            {
                if (IsShort)
                {
                    StringBuilder.AppendRules(description.Value);
                    continue;
                }
                StringBuilder.AppendLine(description.Value);
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
            _shortDescriptionCache = string.Empty;
        }
        public void ClearShortDescriptions()
        {
            ShortDescriptions.Clear();
        }
        public void ResetShortDescription()
        {
            ClearShortDescriptionCache();
            ClearShortDescriptions();
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
            _shortDescriptionCache = _shortDescriptionCache == "" ? ProcessDescription(ShortDescriptions) : _shortDescriptionCache;
            E.Postfix.AppendRules(_shortDescriptionCache);

            return base.HandleEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalWeaponDescriber naturalWeaponDescriber = base.DeepCopy(Parent, MapInv) as NaturalWeaponDescriber;
            naturalWeaponDescriber.ShortDescriptions = null;
            naturalWeaponDescriber._shortDescriptionCache = null;
            return naturalWeaponDescriber;
        }
        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(ShortDescriptions);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            ShortDescriptions = new SortedDictionary<int, string>(Reader.ReadDictionary<int, string>());
        }

    } //!-- public class NaturalWeaponDescriber : IScribedPart
}
