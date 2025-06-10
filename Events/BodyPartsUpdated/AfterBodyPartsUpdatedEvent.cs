using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class AfterBodyPartsUpdatedEvent : ModPooledEvent<AfterBodyPartsUpdatedEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(AfterBodyPartsUpdatedEvent));

    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Creature;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{nameof(AfterBodyPartsUpdatedEvent)}";
    }

    public override void Reset()
    {
        base.Reset();
        Creature = null;
    }

    public static void Send(GameObject Creature)
    {
        Debug.Entry(4, 
            $"! {nameof(AfterBodyPartsUpdatedEvent)}."
            + $"{nameof(Send)}(GameObject Creature: {Creature?.DebugName ?? NULL})", 
            Indent: 0, Toggle: doDebug);

        AfterBodyPartsUpdatedEvent E = FromPool();

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