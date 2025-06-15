using System.Collections.Generic;
using System;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using XRL.World.Capabilities;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class AddWrassleIDEvent : IWrassleIDEvent<AddWrassleIDEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(AddWrassleIDEvent));
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

        public AddWrassleIDEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public override WrassleID GetFor()
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4, $"override {nameof(GetFor)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            WrassleID = new();
            Debug.Entry(4, 
                $"{nameof(WrassleID)} assigned new() preemptively;" +
                $" it's {(WrassleID != null ? "not null" : "null")}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            if (WrassleObject?.Holder != null && UD_QWE.IsWrassler(WrassleObject.Holder))
            {
                Debug.Entry(4, $"{nameof(WrassleObject.Holder)}: {WrassleObject.Holder?.DebugName}",
                    Indent: indent + 1, Toggle: getDoDebug('X'));

                if (WantToProceed(WrassleObject.Holder, 
                    out bool WrassleObjectWantsMin, 
                    out bool WrassleObjectWantsStr)
                 && ProcessEvent(WrassleObject.Holder, 
                    WrassleObjectWantsMin, 
                    WrassleObjectWantsStr)
                 && FromWrassleID != Guid.Empty
                 && FromWrassleID != default)
                {
                    Debug.Entry(4,
                        $"{nameof(FromWrassleID)} Intercepted by {nameof(WrassleObject.Holder)}: {FromWrassleID}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));

                    WrassleID = new WrassleID(FromWrassleID);
                    Debug.LastIndent = indent;
                    return WrassleID;
                }

                Debug.LastIndent = indent;
            }
            Debug.Entry(4, $"{nameof(FromWrassleID)} not intercepted",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            return base.GetFor();
        }
    }
}