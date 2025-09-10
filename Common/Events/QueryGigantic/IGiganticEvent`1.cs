using System.Collections.Generic;
using XRL;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public abstract class IGiganticEvent<T> : ModPooledEvent<T>
        where T : IGiganticEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);
        private static readonly List<object> doList = new()
        {
            'V',    // Vomit
        };
        private static readonly List<object> dontList = new()
        {
            'R',    // Reset
            'X',    // Trace
        };
        private static bool getDoDebug(object what = null) => Options.getDoDebug(what, doList, dontList, doDebug);

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = typeof(T).Name;

        public GameObject Object;

        public bool IsGigantic;

        public bool Override;

        public string Context;

        public IGiganticEvent()
        {
            Reset();
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            IsGigantic = false;
            Override = false;
            Context = null;
        }

        public static T FromPool(GameObject Object, bool IsGigantic = false, string Context = null)
        {
            T E = FromPool();
            if (GameObject.Validate(Object))
            {
                E.Object = Object;
                E.IsGigantic = IsGigantic;
                E.Context = Context;
                return E;
            }
            E.Reset();
            return null;
        }

        public virtual bool WantToProceed(out bool ObjectWantsMin, out bool ObjectWantsStr, out bool GameWantsMin)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}("
                + $"{nameof(GameObject)}, "
                + $"out {nameof(ObjectWantsMin)}, "
                + $"out {nameof(ObjectWantsStr)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool haveObject = Object != null;
            bool haveGame = The.Game != null;

            ObjectWantsMin = false;
            ObjectWantsStr = false;

            GameWantsMin = false;

            if (haveObject)
            {
                ObjectWantsMin = Object.WantEvent(ID, CascadeLevel);
                ObjectWantsStr = Object.HasRegisteredEvent(RegisteredEventID);
            }
            if (haveGame)
            {
                GameWantsMin = The.Game.WantEvent(ID, CascadeLevel);
            }

            Debug.LastIndent = indent;
            return ObjectWantsMin || ObjectWantsStr || GameWantsMin;
        }
        internal bool ProcessEvent()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(ProcessEvent)}("
                + $"{nameof(Object)}: {Object?.GetType()?.Name ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool anyWants = WantToProceed(out bool ObjectWantsMin, out bool SubjectWantsStr, out bool GameWantsMin);

            bool haveObject = Object != null;

            bool proceed = haveObject;

            Debug.Entry(4, $"{nameof(anyWants)}", $"{anyWants}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{nameof(proceed)}", $"{proceed}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            if (proceed && anyWants)
            {
                if (proceed && GameWantsMin)
                {
                    Debug.Entry(4, $"{nameof(The)}.{nameof(The.Game)}.{nameof(The.Game.HandleEvent)}(this)",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = The.Game.HandleEvent(this);
                }
                if (proceed && ObjectWantsMin)
                {
                    Debug.Entry(4, $"{nameof(Object)}.{nameof(Object.HandleEvent)}(this)",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = Object.HandleEvent(this);
                }
                if (proceed && SubjectWantsStr)
                {
                    Event @event = Event.New(GetRegisteredEventID());
                    @event.SetParameter(nameof(Object), Object);
                    @event.SetParameter(nameof(Context), Context);

                    Debug.Entry(4, $"{nameof(Object)}.{nameof(GameObject.FireEvent)}({nameof(@event)})",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = Object.FireEvent(@event);
                }
            }

            Debug.Entry(4, $"{nameof(ProcessEvent)}() returning {IsGigantic}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return IsGigantic;
        }

        public virtual bool CheckFor(out bool Override)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(CheckFor)}(out {nameof(Override)}) for "
                + $"{nameof(Object)}: {Object?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool result = ProcessEvent(); // returns IsGigantic
            Override = this.Override;
            Debug.LastIndent = indent;
            Reset();
            return result;
        }
        public static bool CheckFor(GameObject Object, out bool Override, bool IsGigantic = false, string Context = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(Object)},"
                + $" out {nameof(Override)}, "
                + $"{nameof(Context)})",
                Indent: 0, Toggle: getDoDebug('X'));

            return FromPool(Object, IsGigantic, Context).CheckFor(out Override);
        }
        public static bool CheckFor(GameObject Object, bool IsGigantic = false, string Context = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(Object)}, "
                + $"{nameof(Context)})",
                Indent: 0, Toggle: getDoDebug('X'));

            return FromPool(Object, IsGigantic, Context).CheckFor(out _);
        }
    }
}