using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public class ManageDefaultEquipmentEvent : ModPooledEvent<ManageDefaultEquipmentEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON;

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
        public ManageDefaultEquipmentEvent(BeforeManageDefaultEquipmentEvent beforeManageDefaultEquipmentEvent, GameObject Wielder)
            : this(beforeManageDefaultEquipmentEvent.Object, 
                  Wielder, 
                  beforeManageDefaultEquipmentEvent.Manager, 
                  beforeManageDefaultEquipmentEvent.BodyPart)
        {
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

        public static ManageDefaultEquipmentEvent Manage(BeforeManageDefaultEquipmentEvent BeforeManageDefaultEquipmentEvent, GameObject Wielder)
        {
            Debug.Entry(4,
                $"{typeof(ManageDefaultEquipmentEvent).Name}." +
                $"{nameof(Manage)}()",
                Indent: 0);
            Debug.Entry(4, $"Wielder", Wielder != null ? Wielder.ShortDisplayNameStripped : "[null]", Indent: 1);
            
            ManageDefaultEquipmentEvent E = new(BeforeManageDefaultEquipmentEvent, Wielder);

            bool flag = true;
            if (GameObject.Validate(ref E.Wielder) && GameObject.Validate(ref E.Object) )
            {
                Debug.Entry(4, $"flag = E.Wielder.HandleEvent(E) && E.Object.HandleEvent(E)", Indent: 2);
                flag = E.Wielder.HandleEvent(E) && E.Object.HandleEvent(E);
                Debug.LoopItem(4, $"flag = {flag}", Indent: 2, Good: flag);
                if (flag && (E.Wielder.HasRegisteredEvent(E.GetRegisteredEventID()) || E.Object.HasRegisteredEvent(E.GetRegisteredEventID())))
                {
                    Debug.Entry(4, $"(flag and (Wielder.HasRegisteredEvent(E.GetRegisteredEventID()) || Object.HasRegisteredEvent(E.GetRegisteredEventID())))", Indent: 2);
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter("Object", E.Object);
                    @event.SetParameter("Wielder", E.Wielder);
                    @event.SetParameter("Manager", E.Manager);
                    @event.SetParameter("BodyPart", E.BodyPart);
                    E.Wielder.FireEvent(@event);
                    E.Object.FireEvent(@event);
                }
            }
            E.Manager.ManageNaturalEquipment();
            return E;
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