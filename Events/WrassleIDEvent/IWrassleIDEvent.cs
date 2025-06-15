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
    [GameEvent(Base = true, Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public abstract class IWrassleIDEvent<T> : ModPooledEvent<T>
        where T : IWrassleIDEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);
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

        public new static readonly int CascadeLevel = CASCADE_NONE;

        public static string RegisteredEventID => typeof(T).Name;

        public WrassleID WrassleID;

        public GameObject WrassleObject;

        public Guid FromWrassleID;

        public string Context;

        public IWrassleIDEvent()
        {
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(Reset)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            base.Reset();
            WrassleID = null;
            WrassleObject = null;
            FromWrassleID = default;
            Context = null;

            Debug.LastIndent = indent;
        }

        public static T FromPool(WrassleID WrassleID, GameObject WrassleObject, string Context = null)
        {
            T E = FromPool();
            if (WrassleID != null && WrassleObject != null)
            {
                E.WrassleID = WrassleID;
                E.WrassleObject = WrassleObject;
                E.Context = Context;
                return E;
            }
            E.Reset();
            return null;
        }

        public static T FromPool(GameObject WrassleObject, Guid FromWrassleID = default, string Context = null)
        {
            T E = FromPool();
            if (WrassleObject != null)
            {
                E.WrassleObject = WrassleObject;
                E.WrassleID = null;
                E.Context = Context;
                E.FromWrassleID = FromWrassleID;
                return E;
            }
            E.Reset();
            return null;
        }
        public static WrassleID GetFor(GameObject WrassleObject, Guid FromWrassleID = default, string Context = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(GetFor)}("
                + $"{nameof(WrassleObject)}: {WrassleObject?.DebugName}, "
                + $"{nameof(Context)}: {Context?.Quote()})",
                Indent: 0, Toggle: getDoDebug('X'));

            T E = FromPool(WrassleObject, FromWrassleID, Context);

            WrassleID wrassleID = E.GetFor();

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public virtual WrassleID GetFor()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(GetFor)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            WantToProceed(WrassleObject,
                out bool WrassleObjectWantsMin,
                out bool WrassleObjectWantsStr);
            ProcessEvent(WrassleObject, false, WrassleObjectWantsMin, WrassleObjectWantsStr);

            Debug.Entry(4, $"E.{nameof(WrassleID)} is {(WrassleID != null ? "not null" : "null")}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            WrassleID wrassleID = WrassleID;

            Debug.Entry(4, $"{nameof(wrassleID)} is {(wrassleID != null ? "not null" : "null")}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Reset();

            Debug.Entry(4, $"{nameof(GetFor)}() returning {nameof(wrassleID)}; it's {(wrassleID != null ? "not null" : "null")}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static T Send(WrassleID WrassleID, GameObject WrassleObject, string Context = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(WrassleID)}, "
                + $"{nameof(WrassleObject)}: {WrassleObject?.DebugName}, "
                + $"{nameof(Context)}: {Context?.Quote()})",
                Indent: 0, Toggle: getDoDebug('X'));

            T E = FromPool(WrassleID, WrassleObject, Context);

            E.CheckFor();

            Debug.LastIndent = indent;
            return E;
        }
        public virtual bool CheckFor()
        {
            WantToProceed(WrassleID, WrassleObject,
                out bool WrassleIDWantsMin,
                out bool WrassleObjectWantsMin,
                out bool WrassleObjectWantsStr);
            return ProcessEvent(WrassleObject, WrassleIDWantsMin, WrassleObjectWantsMin, WrassleObjectWantsStr);
        }
        public static bool CheckFor(WrassleID WrassleID, GameObject WrassleObject, string Context = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(WrassleID)}, "
                + $"{nameof(WrassleObject)}: {WrassleObject?.DebugName}, "
                + $"{nameof(Context)}: {Context?.Quote()})",
                Indent: indent, Toggle: getDoDebug('X'));

            T E = FromPool(WrassleID, WrassleObject, Context);

            bool checkResult = E.CheckFor();
            E.Reset();

            Debug.LastIndent = indent;
            return checkResult;
        }
        public static bool WantToProceed(WrassleID WrassleID, GameObject WrassleObject, out bool WrassleIDWantsMin, out bool WrassleObjectWantsMin, out bool WrassleObjectWantsStr)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}("
                + $"{nameof(WrassleID)}, "
                + $"{nameof(WrassleObject)}, "
                + $"out {nameof(WrassleIDWantsMin)}, "
                + $"out {nameof(WrassleObjectWantsMin)}, "
                + $"out {nameof(WrassleObjectWantsStr)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool wantWrassleID = WantToProceed(WrassleID, out WrassleIDWantsMin);
            bool wantWrassleObject = WantToProceed(WrassleObject, out WrassleObjectWantsMin, out WrassleObjectWantsStr);

            Debug.LastIndent = indent;
            return wantWrassleID
                || wantWrassleObject;
        }
        public static bool WantToProceed(WrassleID WrassleID, out bool WrassleIDWantsMin)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}("
                + $"{nameof(WrassleID)}, "
                + $"out {nameof(WrassleIDWantsMin)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool haveWrassleID = WrassleID != null;

            WrassleIDWantsMin = false;

            if (haveWrassleID)
            {
                WrassleIDWantsMin = WrassleID.WantEvent(ID, CascadeLevel);
            }

            Debug.LastIndent = indent;
            return WrassleIDWantsMin;
        }
        public static bool WantToProceed(GameObject WrassleObject, out bool WrassleObjectWantsMin, out bool WrassleObjectWantsStr)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}("
                + $"{nameof(WrassleObject)}, "
                + $"out {nameof(WrassleObjectWantsMin)}, "
                + $"out {nameof(WrassleObjectWantsStr)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool haveWrassleObject = WrassleObject != null;

            WrassleObjectWantsMin = false;
            WrassleObjectWantsStr = false;

            if (haveWrassleObject)
            {
                WrassleObjectWantsMin = WrassleObject.WantEvent(ID, CascadeLevel);
                WrassleObjectWantsStr = WrassleObject.HasRegisteredEvent(RegisteredEventID);
            }

            Debug.LastIndent = indent;
            return WrassleObjectWantsMin || WrassleObjectWantsStr;
        }
        internal bool ProcessEvent(GameObject WrassleObject = null, bool WrassleIDWantsMin = false, bool WrassleObjectWantsMin = false, bool WrassleObjectWantsStr = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(ProcessEvent)}("
                + $"{nameof(WrassleObject)}, "
                + $"{nameof(WrassleIDWantsMin)}: {WrassleIDWantsMin}, "
                + $"{nameof(WrassleObjectWantsMin)}: {WrassleObjectWantsMin}, "
                + $"{nameof(WrassleObjectWantsStr)}: {WrassleObjectWantsStr})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            WrassleObject ??= this?.WrassleObject;

            bool anyWants = WrassleIDWantsMin || WrassleObjectWantsMin || WrassleObjectWantsStr;
            bool proceed = true;

            Debug.Entry(4, $"{nameof(anyWants)}", $"{anyWants}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{nameof(proceed)}", $"{proceed}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            if (proceed && anyWants)
            {
                if (proceed && WrassleIDWantsMin)
                {
                    Debug.Entry(4, $"{nameof(WrassleID)}.{nameof(IPart.HandleEvent)}(this)",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = WrassleID.HandleEvent(this);
                }
                if (proceed && WrassleObjectWantsMin)
                {
                    Debug.Entry(4, $"{nameof(WrassleObject)}.{nameof(GameObject.HandleEvent)}(this)",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = WrassleObject.HandleEvent(this);
                }
                if (proceed && WrassleObjectWantsStr)
                {
                    Event @event = Event.New(GetRegisteredEventID());
                    @event.SetParameter(nameof(WrassleID), WrassleID);
                    @event.SetParameter(nameof(WrassleObject), WrassleObject);
                    @event.SetParameter(nameof(FromWrassleID), FromWrassleID);
                    @event.SetParameter(nameof(Context), Context);

                    Debug.Entry(4, $"{nameof(WrassleObject)}.{nameof(GameObject.FireEvent)}({nameof(@event)})",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = WrassleObject.FireEvent(@event);
                }
            }

            Debug.Entry(4, $"{nameof(ProcessEvent)}() returning {proceed}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return proceed;
        }
    }
}
