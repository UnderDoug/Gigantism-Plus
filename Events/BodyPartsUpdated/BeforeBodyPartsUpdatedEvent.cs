using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

[GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class BeforeBodyPartsUpdatedEvent : ModPooledEvent<BeforeBodyPartsUpdatedEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(BeforeBodyPartsUpdatedEvent));

    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

    public static readonly string RegisteredEventID = nameof(BeforeBodyPartsUpdatedEvent);

    public GameObject Creature;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
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

    public static void Send(GameObject Creature)
    {
        Debug.Entry(4,
            $"! {nameof(BeforeBodyPartsUpdatedEvent)}."
            + $"{nameof(Send)}(GameObject Creature: {Creature?.DebugName ?? NULL})",
            Indent: 0, Toggle: doDebug);

        BeforeBodyPartsUpdatedEvent E = FromPool();

        bool validCreature = Creature != null;

        E.Creature = Creature;

        bool wantsMin = validCreature && E.Creature.WantEvent(ID, E.GetCascadeLevel());
        bool wantsStr = validCreature && E.Creature.HasRegisteredEvent(E.GetRegisteredEventID());

        bool anyWants = wantsMin || wantsStr;

        bool proceed = validCreature;
        if (anyWants)
        {
            if (proceed && wantsMin)
            {
                proceed = E.Creature.HandleEvent(E);
            }
            if (proceed && wantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(E.Creature), E.Creature);
                proceed = Creature.FireEvent(@event);
            }
        }
    }
}