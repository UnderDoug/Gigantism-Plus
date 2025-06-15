using System.Collections.Generic;
using System;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class GetWrassleIDEvent : IWrassleIDEvent<GetWrassleIDEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(GetWrassleIDEvent));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                'X',    // Trace
            };
            List<object> dontList = new()
            {
                "WID"   // WrassleID
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public GetWrassleIDEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public void SetWrassleIDTo(WrassleID WrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(GetWrassleIDEvent)}."
                + $"{nameof(SetWrassleIDTo)}("
                + $"{nameof(WrassleID)}: {WrassleID?.ID})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            this.WrassleID = WrassleID;

            Debug.Entry(4, $"E.{nameof(WrassleID)}: {WrassleID}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
        }
    }
}