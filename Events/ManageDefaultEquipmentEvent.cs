using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public class ManageDefaultEquipmentEvent : ModPooledEvent<ManageDefaultEquipmentEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_EQUIPMENT + CASCADE_EXCEPT_THROWN_WEAPON;

        public GameObject Object;

        public GameObject Wielder;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public ManageDefaultEquipmentEvent()
        {
        }

        public ManageDefaultEquipmentEvent(GameObject Object, GameObject Wielder, NaturalEquipmentManager Manager, BodyPart BodyPart)
            : this()
        {
            ManageDefaultEquipmentEvent @new = FromPool(Object, Wielder, Manager, BodyPart);
            this.Object = @new.Object;
            this.Wielder = @new.Wielder;
            this.Manager = @new.Manager;
            this.BodyPart = @new.BodyPart;
        }
        public ManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent Source)
            : this()
        {
            Object = Source.Object;
            Wielder = Source.Wielder;
            Manager = Source.Manager;
            BodyPart = Source.BodyPart;
        }
        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{typeof(ManageDefaultEquipmentEvent).Name}";
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            Wielder = null;
            Manager = null;
            BodyPart = null;
        }

        public ManageDefaultEquipmentEvent Send()
        {
            Debug.Entry(4, 
                $"{typeof(ManageDefaultEquipmentEvent).Name}." + 
                $"{nameof(Send)}(GameObject Object: {Object.ShortDisplayNameStripped}, NaturalEquipmentManager Manager, BodyPart BodyPart: [{BodyPart.ID}:{BodyPart.Type}])",
                Indent: 0);

            bool flag = true;
            if (GameObject.Validate(ref Object) && Wielder.WantEvent(ID, CascadeLevel))
            {
                ManageDefaultEquipmentEvent manageDefaultEquipmentEvent = FromPool();
                manageDefaultEquipmentEvent.Object = Object;
                manageDefaultEquipmentEvent.Wielder = Wielder;
                manageDefaultEquipmentEvent.Manager = Manager;
                manageDefaultEquipmentEvent.BodyPart = BodyPart;
                flag = Wielder.HandleEvent(manageDefaultEquipmentEvent);
            }
            if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent(GetRegisteredEventID()))
            {
                Event @event = Event.New(GetRegisteredEventID());
                @event.SetParameter("Object", Object);
                @event.SetParameter("Wielder", Wielder);
                @event.SetParameter("Manager", Manager);
                @event.SetParameter("BodyPart", BodyPart);
                Wielder.FireEvent(@event);
            }
            return this;
        }

        public static ManageDefaultEquipmentEvent FromPool(GameObject Object, GameObject Wielder, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            ManageDefaultEquipmentEvent manageDefaultEquipmentEvent = FromPool();
            manageDefaultEquipmentEvent.Object = Object;
            manageDefaultEquipmentEvent.Wielder = Wielder;
            manageDefaultEquipmentEvent.Manager = Manager;
            manageDefaultEquipmentEvent.BodyPart = BodyPart;
            return manageDefaultEquipmentEvent;
        }
    }
}