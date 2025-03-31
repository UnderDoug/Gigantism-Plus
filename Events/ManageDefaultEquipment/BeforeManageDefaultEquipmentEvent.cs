using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using XRL.World.Effects;

public class BeforeManageDefaultEquipmentEvent : ModPooledEvent<BeforeManageDefaultEquipmentEvent>
{
    public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Object;

    public NaturalEquipmentManager Manager;

    public BodyPart BodyPart;

    public BeforeManageDefaultEquipmentEvent()
    {
    }

    public BeforeManageDefaultEquipmentEvent(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        : this()
    {
        BeforeManageDefaultEquipmentEvent @new = FromPool(Object, Manager, BodyPart);
        this.Object = @new.Object;
        this.Manager = @new.Manager;
        this.BodyPart = @new.BodyPart;
    }
    public BeforeManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent Source)
        : this()
    {
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
        return $"{typeof(BeforeManageDefaultEquipmentEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Object = null;
        Manager = null;
        BodyPart = null;
    }

    public static BeforeManageDefaultEquipmentEvent Send(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
    {
        Debug.Entry(4, 
            $"{typeof(BeforeManageDefaultEquipmentEvent).Name}." + 
            $"{nameof(Send)}(GameObject Object: {Object?.ShortDisplayNameStripped}, NaturalEquipmentManager Manager, BodyPart BodyPart: [{BodyPart?.ID}:{BodyPart?.Type}])",
            Indent: 0);
        Debug.Entry(4, $"Wielder", Object?.Equipped != null ? Object.Equipped.ShortDisplayNameStripped : "[null]", Indent: 1);

        bool flag = true;
        BeforeManageDefaultEquipmentEvent E = new(Object, Manager, BodyPart);
        if (GameObject.Validate(ref Object) && BodyPart != null)
        {
            Debug.Entry(4, $"flag = Object.HandleEvent(E)", Indent: 2);
            flag = Object.HandleEvent(E);
            Debug.LoopItem(4, $"flag = {flag}", Indent: 2, Good: flag);
            if (flag && Object.HasRegisteredEvent(E.GetRegisteredEventID()))
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter("Object", Object);
                @event.SetParameter("Manager", Manager);
                @event.SetParameter("BodyPart", BodyPart);
                Object.FireEvent(@event);
            }
        }
        return E;
    }

    public static BeforeManageDefaultEquipmentEvent FromPool(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
    {
        BeforeManageDefaultEquipmentEvent beforeManageDefaultEquipmentEvent = FromPool();
        beforeManageDefaultEquipmentEvent.Object = Object;
        beforeManageDefaultEquipmentEvent.Manager = Manager;
        beforeManageDefaultEquipmentEvent.BodyPart = BodyPart;
        return beforeManageDefaultEquipmentEvent;
    }
}