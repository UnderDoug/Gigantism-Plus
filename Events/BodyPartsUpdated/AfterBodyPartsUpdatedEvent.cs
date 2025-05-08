using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class AfterBodyPartsUpdatedEvent : ModPooledEvent<AfterBodyPartsUpdatedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Actor;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(AfterBodyPartsUpdatedEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Actor = null;
    }

    public static void Send(GameObject Actor)
    {
        Debug.Entry(4, 
            $"{typeof(AfterBodyPartsUpdatedEvent).Name}." + 
            $"{nameof(Send)}(GameObject Actor: {Actor?.DebugName})", 
            Indent: 0);
        
        AfterBodyPartsUpdatedEvent E = FromPool();

        bool flag = true;
        if (Actor.WantEvent(ID, CascadeLevel))
        {
            E.Actor = Actor;
            flag = Actor.HandleEvent(E);
        }
        if (flag && Actor.HasRegisteredEvent(E.GetRegisteredEventID()))
        {
            Event @event = Event.New(E.GetRegisteredEventID());
            @event.SetParameter(nameof(Actor), Actor);
            Actor.FireEvent(@event);
        }
    }
}