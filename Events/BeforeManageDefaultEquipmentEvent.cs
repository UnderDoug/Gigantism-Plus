using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

[GameEvent(Cascade = 17, Cache = Cache.Pool)]
public class BeforeManageDefaultEquipmentEvent : ModPooledEvent<BeforeManageDefaultEquipmentEvent>
{
    public new static readonly int CascadeLevel = 17;

    public GameObject Object;

    public NaturalEquipmentManager Manager;

    public BodyPart Part;

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
        Part = null;
    }

    public static void Send(GameObject Object, NaturalEquipmentManager Manager,  BodyPart Part)
    {
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("BeforeManageDefaultEquipmentEvent"))
        {
            Event @event = Event.New("BeforeManageDefaultEquipmentEvent");
            @event.SetParameter("Object", Object);
            @event.SetParameter("Manager", Manager);
            @event.SetParameter("Body", Part);
            flag = Object.FireEvent(@event);
        }
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            BeforeManageDefaultEquipmentEvent beforeManageDefaultEquipmentEvent = FromPool();
            beforeManageDefaultEquipmentEvent.Object = Object;
            beforeManageDefaultEquipmentEvent.Manager = Manager;
            beforeManageDefaultEquipmentEvent.Part = Part;
            flag = Object.HandleEvent(beforeManageDefaultEquipmentEvent);
        }
    }
}