using System.Collections.Generic;
using System;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class ManageDefaultEquipmentEvent : ModPooledEvent<ManageDefaultEquipmentEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public GameObject Wielder;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public string target;

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
            : this(Source.Object, Source.Wielder, Source.Manager, Source.BodyPart)
        {
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
            target = null;
        }

        public static ManageDefaultEquipmentEvent Manage(BeforeManageDefaultEquipmentEvent BeforeManageDefaultEquipmentEvent, GameObject Wielder)
        {
            Debug.Entry(4,
                $"! {typeof(ManageDefaultEquipmentEvent).Name}." +
                $"{nameof(Manage)}({typeof(BeforeManageDefaultEquipmentEvent).Name}) " + 
                $"for {Wielder?.ID}:{Wielder?.ShortDisplayNameStripped}",
                Indent: 0);
            
            ManageDefaultEquipmentEvent E = new(BeforeManageDefaultEquipmentEvent, Wielder);
            BeforeManageDefaultEquipmentEvent.Reset();

            bool flag = true;
            if (GameObject.Validate(ref E.Wielder) && GameObject.Validate(ref E.Object) )
            {
                E.target = "wielder";
                bool wielder = E.Wielder.HandleEvent(E);
                E.target = "object";
                bool @object = E.Object.HandleEvent(E);
                flag = wielder && @object;
                if (flag && (E.Wielder.HasRegisteredEvent(E.GetRegisteredEventID()) || E.Object.HasRegisteredEvent(E.GetRegisteredEventID())))
                {
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
            manageDefaultEquipmentEvent.target = "";
            return manageDefaultEquipmentEvent;
        }
    }
}