using XRL;
using XRL.World;

[GameEvent(Cascade = CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class BodyPartsUpdatedEvent : ModPooledEvent<BodyPartsUpdatedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON;

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
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            BodyPartsUpdatedEvent E = FromPool();
            E.Object = Object;
            flag = Object.HandleEvent(E);
        }
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("BeforeFireEventOnBodypartsEvent"))
        {
            Event @event = Event.New("BeforeFireEventOnBodypartsEvent");
            @event.SetParameter("Object", Object);
            Object.FireEvent(@event);
        }
    }
}