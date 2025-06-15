using XRL.World;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
    public abstract class IBodyPartsUpdatedEvent<T> : ModPooledEvent<T>
            where T : IBodyPartsUpdatedEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);

        public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

        public static readonly string RegisteredEventID = typeof(T).Name;

        public GameObject Creature;

        public IBodyPartsUpdatedEvent()
        {
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            base.Reset();
            Creature = null;
        }

        public static T FromPool(GameObject Creature)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(FromPool)}("
                + $"{nameof(Creature)}: {Creature?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            T E = FromPool();
            Debug.LoopItem(4, $"{nameof(Creature)} not {NULL}", $"{Creature != null}",
                Good: Creature != null, Indent: indent + 2, Toggle: doDebug);
            if (Creature != null)
            {
                E.Creature = Creature;
                Debug.LastIndent = indent;
                return E;
            }
            E.Reset();
            Debug.LastIndent = indent;
            return null;
        }
        public void Send()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(Send)}()"
                + $" for {nameof(Creature)}: {Creature?.DebugName ?? NULL},"
                + $" {nameof(CascadeLevel)}: {CascadeLevel}",
                Indent: indent + 1, Toggle: doDebug);

            bool wantsMin = Creature.WantEvent(ID, CascadeLevel);
            bool wantsStr = Creature.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = wantsMin || wantsStr;

            Debug.LoopItem(4, $"{nameof(wantsMin)}", $"{wantsMin}",
                Good: wantsMin, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(wantsStr)}", $"{wantsStr}",
                Good: wantsStr, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(anyWants)}", $"{anyWants}",
                Good: anyWants, Indent: indent + 2, Toggle: doDebug);

            bool proceed = anyWants;
            if (proceed)
            {
                if (proceed && wantsMin)
                {
                    proceed = Creature.HandleEvent(this);
                }
                if (proceed && wantsStr)
                {
                    Event @event = Event.New(RegisteredEventID);
                    @event.SetParameter(nameof(Creature), Creature);
                    proceed = Creature.FireEvent(@event);
                }
            }

            Debug.LastIndent = indent;
            Reset();
        }
        public static void Send(GameObject Creature)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Creature)}: {Creature?.DebugName ?? NULL})",
                Indent: 0, Toggle: doDebug);

            FromPool(Creature).Send();
            Debug.LastIndent = indent;
        }
    }
}