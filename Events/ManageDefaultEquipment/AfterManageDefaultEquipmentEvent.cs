using System.Collections.Generic;
using System;using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using static HNPS_GigantismPlus.Utils;


namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class AfterManageDefaultEquipmentEvent : ModPooledEvent<AfterManageDefaultEquipmentEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public AfterManageDefaultEquipmentEvent()
        {
        }

        public AfterManageDefaultEquipmentEvent(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
            : this()
        {
            AfterManageDefaultEquipmentEvent @new = FromPool(Object, Manager, BodyPart);
            this.Object = @new.Object;
            this.Manager = @new.Manager;
            this.BodyPart = @new.BodyPart;
        }
        public AfterManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
            : this(ManageDefaultEquipmentEvent.Object, ManageDefaultEquipmentEvent.Manager, ManageDefaultEquipmentEvent.BodyPart)
        {
        }
        public AfterManageDefaultEquipmentEvent(AfterManageDefaultEquipmentEvent Source)
            : this(Source.Object, Source.Manager, Source.BodyPart)
        {
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
            Object = null;
            Manager = null;
            BodyPart = null;
        }

        public static void Send(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
        {
            GameObject Wielder = ManageDefaultEquipmentEvent.Wielder;
            GameObject Object = ManageDefaultEquipmentEvent.Object;
            Debug.Entry(4,
                $"! {typeof(AfterManageDefaultEquipmentEvent).Name}." +
                $"{nameof(Send)}({typeof(ManageDefaultEquipmentEvent).Name}) for {Wielder.ID}:{Wielder.ShortDisplayNameStripped}'s {Object.ShortDisplayNameStripped}",
                Indent: 0);

            AfterManageDefaultEquipmentEvent E = new(ManageDefaultEquipmentEvent);
            ManageDefaultEquipmentEvent.Reset();

            bool flag = true;
            if (E.BodyPart != null && GameObject.Validate(ref E.Object))
            {
                flag = E.Object.HandleEvent(E);
                if (flag && E.Object.HasRegisteredEvent(E.GetRegisteredEventID()))
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter("Object", E.Object);
                    @event.SetParameter("Manager", E.Manager);
                    @event.SetParameter("BodyPart", E.BodyPart);
                    E.Object.FireEvent(@event);
                }
            }
            E.Reset();
        }

        public static AfterManageDefaultEquipmentEvent FromPool(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            AfterManageDefaultEquipmentEvent afterManageDefaultEquipmentEvent = FromPool();
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
}
