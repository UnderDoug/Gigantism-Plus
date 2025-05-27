using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class BeforeBodyPartsUpdatedEvent : ModPooledEvent<BeforeBodyPartsUpdatedEvent>
{
    private static bool doDebug => true;

    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Creature;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{nameof(BeforeBodyPartsUpdatedEvent)}";
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

        bool wantsMin = validCreature && Creature.WantEvent(ID, E.GetCascadeLevel());
        bool wantsStr = validCreature && Creature.HasRegisteredEvent(E.GetRegisteredEventID());

        bool anyWants = wantsMin || wantsStr;

        bool proceed = validCreature;
        if (anyWants)
        {
            if (proceed && wantsMin)
            {
                E.Creature = Creature;
                proceed = Creature.HandleEvent(E);
            }
            if (proceed && wantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Creature), Creature);
                proceed = Creature.FireEvent(@event);
            }
        }
    }
}