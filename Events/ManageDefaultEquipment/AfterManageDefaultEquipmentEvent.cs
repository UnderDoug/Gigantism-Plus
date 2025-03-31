using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;

public class AfterManageDefaultEquipmentEvent : ModPooledEvent<AfterManageDefaultEquipmentEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

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
    public AfterManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
        : this(ManageDefaultEquipmentEvent.Object, ManageDefaultEquipmentEvent.Manager, ManageDefaultEquipmentEvent.BodyPart)
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

    public static void Send(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
    {
        Debug.Entry(4,
            $"{typeof(AfterManageDefaultEquipmentEvent).Name}." +
            $"{nameof(Send)}(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)",
            Indent: 0);

        AfterManageDefaultEquipmentEvent E = new(ManageDefaultEquipmentEvent);

        Debug.Entry(4, $"Wielder", E.Object?.Equipped != null ? E.Object.Equipped.ShortDisplayNameStripped : "[null]", Indent: 1);

        bool flag = true;
        if (E.BodyPart != null && GameObject.Validate(ref E.Object))
        {

            Debug.Entry(4, $"flag = Object.HandleEvent(E)", Indent: 2);
            flag = E.Object.HandleEvent(E);
            Debug.LoopItem(4, $"flag = {flag}", Indent: 2, Good: flag);
            if (flag && E.Object.HasRegisteredEvent(E.GetRegisteredEventID()))
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter("ManageDefaultEquipmentEvent", E.ManageDefaultEquipmentEvent);
                @event.SetParameter("Object", E.Object);
                @event.SetParameter("Manager", E.Manager);
                @event.SetParameter("BodyPart", E.BodyPart);
                E.Object.FireEvent(@event);
            }
        }
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