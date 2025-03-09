using System;
using System.Collections.Generic;
using System.Text;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalWeaponDescriber : IPart
    {
        public SortedDictionary<int, string> ShortDescriptions = new();

        private string _ShortDescriptionCache = null;

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
            Debug.Entry(4, $"NaturalWeaponDescriber.AddShortDescriptionEntry(int Priority: {Priority}, string Description)", Indent: 6);
            ShortDescriptions[Priority] = Description;
        }

        public void ClearShortDescriptionCache()
        {
            _ShortDescriptionCache = null;
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
            Debug.Entry(4, "@ NaturalWeaponDescriber.HandleEvent(GetShortDescriptionEvent E)", Indent: 6);
            _ShortDescriptionCache ??= ProcessDescription(ShortDescriptions);
            Debug.Entry(4, "_ShortDescriptionCache", _ShortDescriptionCache, Indent: 6);
            E.Postfix.AppendRules(_ShortDescriptionCache);
            return base.HandleEvent(E);
        }

    }

}
