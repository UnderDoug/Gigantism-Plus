using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

[GameEvent(Cascade = CASCADE_EQUIPMENT + CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
public class AfterManageDefaultEquipmentEvent : ModPooledEvent<AfterManageDefaultEquipmentEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT + CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Object;

    public NaturalEquipmentManager Manager;

    public BodyPart BodyPart;

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
        Manager = null;
        BodyPart = null;
    }

    public static void Send(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
    {
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            AfterManageDefaultEquipmentEvent E = FromPool();
            E.Object = Object;
            E.Manager = Manager;
            E.BodyPart = BodyPart;
            flag = BodyPart.HandleEvent(E) || Object.HandleEvent(E);
        }
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("AfterManageDefaultEquipmentEvent"))
        {
            Event @event = Event.New("AfterManageDefaultEquipmentEvent");
            @event.SetParameter("Object", Object);
            @event.SetParameter("Manager", Manager);
            @event.SetParameter("BodyPart", BodyPart);
            BodyPart.FireEvent(@event);
            Object.FireEvent(@event);
        }
    }
}