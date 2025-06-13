using XRL.World;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
    public class IBodyPartsUpdatedEvent<T> : ModPooledEvent<T>
            where T : IBodyPartsUpdatedEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug("IBodyPartsUpdatedEvent");

        public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

        public static readonly string RegisteredEventID = typeof(T).Name;

        public GameObject Creature;

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
            T E = FromPool();
            if (Creature != null)
            {
                E.Creature = Creature;
                return E;
            }
            E.Reset();
            return null;
        }
        public void Send()
        {
            bool wantsMin = Creature.WantEvent(ID, CascadeLevel);
            bool wantsStr = Creature.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = wantsMin || wantsStr;

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
            Reset();
        }
        public static void Send(GameObject Creature)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Creature)}: {Creature?.DebugName})",
                Indent: 0, Toggle: doDebug);

            FromPool(Creature).Send();
            Debug.LastIndent = indent;
        }
    }
}