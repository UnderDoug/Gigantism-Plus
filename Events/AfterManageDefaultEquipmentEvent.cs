using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;

public class AfterManageDefaultEquipmentEvent : ModPooledEvent<AfterManageDefaultEquipmentEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT + CASCADE_EXCEPT_THROWN_WEAPON;

    public ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent;

    public GameObject Object;

    public NaturalEquipmentManager Manager;

    public BodyPart BodyPart;

    public AfterManageDefaultEquipmentEvent()
    {
    }

    public AfterManageDefaultEquipmentEvent(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        : this()
    {
        AfterManageDefaultEquipmentEvent @new = FromPool(new(), Object, Manager, BodyPart);
        this.ManageDefaultEquipmentEvent = @new.ManageDefaultEquipmentEvent;
        this.Object = @new.Object;
        this.Manager = @new.Manager;
        this.BodyPart = @new.BodyPart;
    }
    public AfterManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent, GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        : this(Object, Manager, BodyPart)
    {
        this.ManageDefaultEquipmentEvent = ManageDefaultEquipmentEvent;
    }
    public AfterManageDefaultEquipmentEvent(AfterManageDefaultEquipmentEvent Source)
        : this()
    {
        ManageDefaultEquipmentEvent = Source.ManageDefaultEquipmentEvent;
        Object = Source.Object;
        Manager = Source.Manager;
        BodyPart = Source.BodyPart;
    }
    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(AfterManageDefaultEquipmentEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        ManageDefaultEquipmentEvent = null;
        Object = null;
        Manager = null;
        BodyPart = null;
    }

    public static AfterManageDefaultEquipmentEvent Send(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent, GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
    {
        Debug.Entry(4,
            $"{typeof(AfterManageDefaultEquipmentEvent).Name}." +
            $"{nameof(Send)}(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent, GameObject Object: {Object.ShortDisplayNameStripped}, NaturalEquipmentManager Manager, BodyPart BodyPart: [{BodyPart.ID}:{BodyPart.Type}])",
            Indent: 0);
        
        bool flag = true;
        if (GameObject.Validate(ref Object))
        {
            AfterManageDefaultEquipmentEvent E = new(ManageDefaultEquipmentEvent, Object, Manager, BodyPart);

            Manager = Manager.ManageNaturalEquipment();

            if (Object.WantEvent(ID, CascadeLevel))
            {
                flag = Object.HandleEvent(E);
            }
            if (flag && Object.HasRegisteredEvent(E.GetRegisteredEventID()))
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter("ManageDefaultEquipmentEvent", E.ManageDefaultEquipmentEvent);
                @event.SetParameter("Object", E.Object);
                @event.SetParameter("Manager", E.Manager);
                @event.SetParameter("BodyPart", E.BodyPart);
                Object.FireEvent(@event);

                E.ManageDefaultEquipmentEvent = @event.GetParameter("ManageDefaultEquipmentEvent") as ManageDefaultEquipmentEvent;
                E.Object = @event.GetParameter("Object") as GameObject;
                E.Manager = @event.GetParameter("NaturalEquipmentManager") as NaturalEquipmentManager;
                E.BodyPart = @event.GetParameter("BodyPart") as BodyPart;
            }
            return E;
        }
        return null;
    }

    public static AfterManageDefaultEquipmentEvent FromPool(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent, GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
    {
        AfterManageDefaultEquipmentEvent afterManageDefaultEquipmentEvent = FromPool();
        afterManageDefaultEquipmentEvent.ManageDefaultEquipmentEvent = ManageDefaultEquipmentEvent;
        afterManageDefaultEquipmentEvent.Object = Object;
        afterManageDefaultEquipmentEvent.Manager = Manager;
        afterManageDefaultEquipmentEvent.BodyPart = BodyPart;
        return afterManageDefaultEquipmentEvent;
    }

    public bool ManageDefaultEquipment()
    {

        return true;
    }
}