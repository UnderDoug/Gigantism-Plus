using System;
using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public abstract class IAdjustmentEvent<T> : ModPooledEvent<T>
        where T : IAdjustmentEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
                'R',    // Reset
                'X',    // Trace
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = typeof(T).Name;

        public GameObject Subject;

        public Type Source;

        public IAdjustment Adjustment;

        public IAdjustmentEvent()
        {
            Subject = null;
            Source = null;
            Adjustment = null;
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            base.Reset();
            Subject = null;
            Source = null;
            Adjustment = null;
        }

        public static T FromPool(GameObject Subject, Type Source, IAdjustment Adjustment)
        {
            T E = FromPool();
            if (Subject != null && Source != null && Adjustment != null)
            {
                E.Subject = Subject;
                E.Source = Source;
                E.Adjustment = Adjustment;
                return E;
            }
            E.Reset();
            return null;
        }

        public virtual bool WantToProceed(out bool SubjectWantsMin, out bool SubjectWantsStr)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}("
                + $"{nameof(GameObject)}, "
                + $"out {nameof(SubjectWantsMin)}, "
                + $"out {nameof(SubjectWantsStr)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool haveSubject = Subject != null;
            bool haveSource = Source != null;
            bool haveAdjustment = Adjustment != null;

            SubjectWantsMin = false;
            SubjectWantsStr = false;

            if (haveSubject && haveSource && haveAdjustment)
            {
                SubjectWantsMin = Subject.WantEvent(ID, CascadeLevel);
                SubjectWantsStr = Subject.HasRegisteredEvent(RegisteredEventID);
            }

            Debug.LastIndent = indent;
            return SubjectWantsMin || SubjectWantsStr;
        }
        internal bool ProcessEvent()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(ProcessEvent)}("
                + $"{nameof(Subject)}: {Subject?.GetType()?.Name ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool anyWants = WantToProceed(out bool SubjectWantsMin, out bool SubjectWantsStr);

            bool haveSubject = Subject != null;
            bool haveSource = Source != null;
            bool haveAdjustment = Adjustment != null;

            bool proceed = haveSubject && haveSource && haveAdjustment;

            Debug.Entry(4, $"{nameof(anyWants)}", $"{anyWants}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{nameof(proceed)}", $"{proceed}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            if (proceed && anyWants)
            {
                if (proceed && SubjectWantsMin)
                {
                    Debug.Entry(4, $"{nameof(Subject)}.{nameof(Subject.HandleEvent)}(this)",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = Subject.HandleEvent(this);
                }
                if (proceed && SubjectWantsStr)
                {
                    Event @event = Event.New(GetRegisteredEventID());
                    @event.SetParameter(nameof(Subject), Subject);
                    @event.SetParameter(nameof(Source), Source);
                    @event.SetParameter(nameof(Adjustment), Adjustment);

                    Debug.Entry(4, $"{nameof(Subject)}.{nameof(GameObject.FireEvent)}({nameof(@event)})",
                        Indent: indent + 3, Toggle: getDoDebug('X'));

                    proceed = Subject.FireEvent(@event);
                }
            }

            Debug.Entry(4, $"{nameof(ProcessEvent)}() returning {proceed}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return proceed;
        }

        public virtual void Send()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(Send)}()"
                + $" for {nameof(Subject)}: {Subject?.GetType()?.Name ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            ProcessEvent();
            Debug.LastIndent = indent;
            Reset();
        }
        public static void Send(GameObject Subject, Type Source, IAdjustment Adjustment)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Subject)}, "
                + $"{nameof(Source)}, "
                + $"{nameof(Adjustment)})",
                Indent: 0, Toggle: getDoDebug('X'));

            FromPool(Subject, Source, Adjustment).Send();
        }

        public virtual bool CheckFor()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(CheckFor)}()"
                + $" for {nameof(Subject)}: {Subject?.GetType()?.Name ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool result = ProcessEvent();
            Debug.LastIndent = indent;
            Reset();
            return result;
        }
        public static bool CheckFor(GameObject Subject, Type Source, IAdjustment Adjustment)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Subject)}, "
                + $"{nameof(Source)}, "
                + $"{nameof(Adjustment)})",
                Indent: 0, Toggle: getDoDebug('X'));

            return FromPool(Subject, Source, Adjustment).CheckFor();
        }
    }
}