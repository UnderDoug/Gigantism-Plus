using XRL;
using XRL.World;
using HNPS_GigantismPlus;

[GameEvent(Cascade = CASCADE_DESIRED_OBJECT, Cache = Cache.Pool)]
public class BodyPartsUpdatedEvent : ModPooledEvent<BodyPartsUpdatedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_DESIRED_OBJECT; // CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Object;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public override bool Dispatch(IEventHandler Handler)
    {
        return Handler.HandleEvent(this);
    }

    public override void Reset()
    {
        base.Reset();
        Object = null;
    }

    public static void Send(GameObject Object)
    {
        Debug.Entry(4, $"{typeof(BodyPartsUpdatedEvent).Name}.{nameof(Send)}(GameObject Object: {Object?.DebugName})", Indent: 0);
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            BodyPartsUpdatedEvent E = FromPool();
            E.Object = Object;
            flag = Object.HandleEvent(E);
        }
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("BodyPartsUpdatedEvent"))
        {
            Event @event = Event.New("BodyPartsUpdatedEvent");
            @event.SetParameter("Object", Object);
            Object.FireEvent(@event);
        }
    }
}