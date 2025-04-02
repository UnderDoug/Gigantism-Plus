using XRL;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

[GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
public class AfterBodyPartsUpdatedEvent : ModPooledEvent<AfterBodyPartsUpdatedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_ALL; // CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON;

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
        Debug.Entry(4, $"{typeof(AfterBodyPartsUpdatedEvent).Name}.{nameof(Send)}(GameObject Object: {Actor?.DebugName})", Indent: 0);
        bool flag = true;
        AfterBodyPartsUpdatedEvent E = FromPool();
        if (GameObject.Validate(ref Actor))
        {
            if (Actor.WantEvent(ID, CascadeLevel))
            {
                E.Actor = Actor;
                flag = Actor.HandleEvent(E);
            }
            if (flag && Actor.HasRegisteredEvent(E.GetRegisteredEventID()))
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter("Object", Actor);
                Actor.FireEvent(@event);
            }
        }
    }
}