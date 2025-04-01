using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using System.Collections.Generic;
using System;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class BeforeManageDefaultEquipmentEvent : ModPooledEvent<BeforeManageDefaultEquipmentEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

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
            : this(Source.Object, Source.Manager, Source.BodyPart)
        {
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
                $"! {typeof(BeforeManageDefaultEquipmentEvent).Name}." +
                $"{nameof(Send)}(Object: {Manager.Wielder.ID}:{Manager.Wielder.ShortDisplayNameStripped}'s " + 
                $"{Object?.ShortDisplayNameStripped}, Manager, BodyPart: [{BodyPart?.ID}:{BodyPart?.Type}])",
                Indent: 0);

            bool flag = true;
            BeforeManageDefaultEquipmentEvent E = new(Object, Manager, BodyPart);
            if (GameObject.Validate(ref Object) && BodyPart != null)
            {
                flag = Object.HandleEvent(E);

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
}