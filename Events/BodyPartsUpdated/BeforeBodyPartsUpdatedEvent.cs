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
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Actor;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(BeforeBodyPartsUpdatedEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Actor = null;
    }

    public static void Send(GameObject Actor)
    {
        Debug.Entry(4, 
            $"{typeof(BeforeBodyPartsUpdatedEvent).Name}." + 
            $"{nameof(Send)}(GameObject Object: {Actor?.DebugName})", 
            Indent: 0);
        
        BeforeBodyPartsUpdatedEvent E = FromPool();

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