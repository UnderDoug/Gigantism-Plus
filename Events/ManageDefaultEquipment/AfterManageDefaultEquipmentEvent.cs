using System;
using System.Collections.Generic;
using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;


namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class AfterManageDefaultEquipmentEvent : ModPooledEvent<AfterManageDefaultEquipmentEvent>
    {
        private static bool doDebug => true;

        public new static readonly int CascadeLevel = CASCADE_NONE; // CASCADE_ALL;

        public GameObject Equipment;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public AfterManageDefaultEquipmentEvent()
        {
        }

        public AfterManageDefaultEquipmentEvent(GameObject Equipment, NaturalEquipmentManager Manager, BodyPart BodyPart)
            : this()
        {
            AfterManageDefaultEquipmentEvent @new = FromPool(Equipment, Manager, BodyPart);
            this.Equipment = @new.Equipment;
            this.Manager = @new.Manager;
            this.BodyPart = @new.BodyPart;
        }
        public AfterManageDefaultEquipmentEvent(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
            : this(
                  Equipment: ManageDefaultEquipmentEvent.Equipment, 
                  Manager: ManageDefaultEquipmentEvent.Manager, 
                  BodyPart: ManageDefaultEquipmentEvent.BodyPart)
        {
        }
        public AfterManageDefaultEquipmentEvent(AfterManageDefaultEquipmentEvent Source)
            : this(Source.Equipment, Source.Manager, Source.BodyPart)
        {
        }
        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{nameof(AfterManageDefaultEquipmentEvent)}";
        }

        public override void Reset()
        {
            base.Reset();
            Equipment = null;
            Manager = null;
            BodyPart = null;
        }

        public static void Send(ManageDefaultEquipmentEvent ManageDefaultEquipmentEvent)
        {
            GameObject Creature = ManageDefaultEquipmentEvent?.Creature;
            GameObject Equipment = ManageDefaultEquipmentEvent?.Equipment;
            Debug.Entry(4,
                $"! {nameof(AfterManageDefaultEquipmentEvent)}."
                + $"{nameof(Send)}({typeof(ManageDefaultEquipmentEvent).Name}) for "
                + $"{Creature?.DebugName ?? NULL}'s {Equipment?.DebugName ?? NULL}",
                Indent: 0, Toggle: doDebug);

            AfterManageDefaultEquipmentEvent E = new(ManageDefaultEquipmentEvent);

            bool validEquipment = E.Equipment != null;
            bool limbExists = E.BodyPart != null;

            bool proceed = validEquipment && limbExists;

            bool wantsMin = proceed && E.Equipment.WantEvent(ID, E.GetCascadeLevel());
            bool wantsStr = proceed && E.Equipment.HasRegisteredEvent(E.GetRegisteredEventID());

            bool anyWants = wantsMin || wantsStr;
            
            if (anyWants)
            {
                if (proceed && wantsMin)
                {
                    proceed = E.Equipment.HandleEvent(E);
                }
                if (proceed && wantsStr)
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter("Object", E.Equipment);
                    @event.SetParameter("Manager", E.Manager);
                    @event.SetParameter("BodyPart", E.BodyPart);
                    proceed = E.Equipment.FireEvent(@event);
                }
                
            }

            ManageDefaultEquipmentEvent.Reset();
            E.Reset();
        }

        public static AfterManageDefaultEquipmentEvent FromPool(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            AfterManageDefaultEquipmentEvent afterManageDefaultEquipmentEvent = FromPool();
            afterManageDefaultEquipmentEvent.Equipment = Object;
            afterManageDefaultEquipmentEvent.Manager = Manager;
            afterManageDefaultEquipmentEvent.BodyPart = BodyPart;
            return afterManageDefaultEquipmentEvent;
        }
    }
}
