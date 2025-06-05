using System.Collections.Generic;
using System;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class ManageDefaultNaturalEquipmentEvent : ModPooledEvent<ManageDefaultNaturalEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(ManageDefaultNaturalEquipmentEvent));

        public new static readonly int CascadeLevel = CASCADE_NONE; // CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

        public GameObject Equipment;

        public GameObject Creature;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public ManageDefaultNaturalEquipmentEvent()
        {
        }

        public ManageDefaultNaturalEquipmentEvent(GameObject Equipment, GameObject Creature, NaturalEquipmentManager Manager, BodyPart BodyPart)
            : this()
        {
            ManageDefaultNaturalEquipmentEvent @new = FromPool(Equipment, Creature, Manager, BodyPart);
            this.Equipment = @new.Equipment;
            this.Creature = @new.Creature;
            this.Manager = @new.Manager;
            this.BodyPart = @new.BodyPart;
        }
        public ManageDefaultNaturalEquipmentEvent(BeforeManageDefaultNaturalEquipmentEvent beforeManageDefaultEquipmentEvent, GameObject Wielder)
            : this(beforeManageDefaultEquipmentEvent.Equipment, 
                  Wielder, 
                  beforeManageDefaultEquipmentEvent.Manager, 
                  beforeManageDefaultEquipmentEvent.BodyPart)
        {
        }
        public ManageDefaultNaturalEquipmentEvent(ManageDefaultNaturalEquipmentEvent Source)
            : this(Source.Equipment, Source.Creature, Source.Manager, Source.BodyPart)
        {
        }
        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{nameof(ManageDefaultNaturalEquipmentEvent)}";
        }

        public override void Reset()
        {
            base.Reset();
            Equipment = null;
            Creature = null;
            Manager = null;
            BodyPart = null;
        }

        public static ManageDefaultNaturalEquipmentEvent Manage(BeforeManageDefaultNaturalEquipmentEvent BeforeManageDefaultEquipmentEvent, GameObject Wielder)
        {
            Debug.Entry(4,
                $"! {nameof(ManageDefaultNaturalEquipmentEvent)}."
                + $"{nameof(Manage)}({typeof(BeforeManageDefaultNaturalEquipmentEvent).Name}) "
                + $"for {Wielder?.DebugName ?? NULL}",
                Indent: 0, Toggle: doDebug);
            
            ManageDefaultNaturalEquipmentEvent E = new(BeforeManageDefaultEquipmentEvent, Wielder);

            bool validWielder = E.Creature != null;
            bool validObject = E.Equipment != null;

            bool wielderWantsMin = validWielder && E.Creature.WantEvent(ID, CascadeLevel);
            bool objectWantsMin = validObject && E.Equipment.WantEvent(ID, CascadeLevel);

            bool wielderWantsStr = validWielder && E.Creature.HasRegisteredEvent(E.GetRegisteredEventID());
            bool objectWantsStr = validObject && E.Equipment.HasRegisteredEvent(E.GetRegisteredEventID());

            bool anyWantsMin = wielderWantsMin || objectWantsMin;
            bool anyWantsStr = wielderWantsStr || objectWantsStr;

            bool proceed = true;
            if (proceed && anyWantsMin)
            {
                if (proceed && wielderWantsMin)
                {
                    proceed = E.Creature.HandleEvent(E);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = E.Equipment.HandleEvent(E);
                }

                if (proceed && anyWantsStr)
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter("Object", E.Equipment);
                    @event.SetParameter("Wielder", E.Creature);
                    @event.SetParameter("Manager", E.Manager);
                    @event.SetParameter("BodyPart", E.BodyPart);

                    if (proceed && wielderWantsStr)
                    {
                        proceed = E.Creature.FireEvent(@event);
                    }
                    if (proceed && objectWantsStr)
                    {
                        proceed = E.Equipment.FireEvent(@event);
                    }
                }
            }
            if (proceed)
            {
                E.Manager.ManageNaturalEquipment();
            }
            BeforeManageDefaultEquipmentEvent.Reset();
            return E;
        }
        public static ManageDefaultNaturalEquipmentEvent Send(NaturalEquipmentManager Manager, GameObject Wielder)
        {
            Debug.Entry(4,
                $"! {nameof(ManageDefaultNaturalEquipmentEvent)}."
                + $"{nameof(Manage)}({typeof(BeforeManageDefaultNaturalEquipmentEvent).Name}) "
                + $"for {Wielder?.DebugName ?? NULL}",
                Indent: 0, Toggle: doDebug);
            
            ManageDefaultNaturalEquipmentEvent E = new(Manager, Wielder);

            bool validWielder = E.Creature != null;
            bool validObject = E.Equipment != null;

            bool wielderWantsMin = validWielder && E.Creature.WantEvent(ID, CascadeLevel);
            bool objectWantsMin = validObject && E.Equipment.WantEvent(ID, CascadeLevel);

            bool wielderWantsStr = validWielder && E.Creature.HasRegisteredEvent(E.GetRegisteredEventID());
            bool objectWantsStr = validObject && E.Equipment.HasRegisteredEvent(E.GetRegisteredEventID());

            bool anyWantsMin = wielderWantsMin || objectWantsMin;
            bool anyWantsStr = wielderWantsStr || objectWantsStr;

            bool proceed = true;
            if (proceed && anyWantsMin)
            {
                if (proceed && wielderWantsMin)
                {
                    proceed = E.Creature.HandleEvent(E);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = E.Equipment.HandleEvent(E);
                }

                if (proceed && anyWantsStr)
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter("Object", E.Equipment);
                    @event.SetParameter("Wielder", E.Creature);
                    @event.SetParameter("Manager", E.Manager);
                    @event.SetParameter("BodyPart", E.BodyPart);

                    if (proceed && wielderWantsStr)
                    {
                        proceed = E.Creature.FireEvent(@event);
                    }
                    if (proceed && objectWantsStr)
                    {
                        proceed = E.Equipment.FireEvent(@event);
                    }
                }
            }
            if (proceed)
            {
                E.Manager.ManageNaturalEquipment();
            }
            BeforeManageDefaultEquipmentEvent.Reset();
            return E;
        }

        public static ManageDefaultNaturalEquipmentEvent FromPool(GameObject Object, GameObject Creature, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            ManageDefaultNaturalEquipmentEvent manageDefaultEquipmentEvent = FromPool();
            manageDefaultEquipmentEvent.Equipment = Object;
            manageDefaultEquipmentEvent.Creature = Creature;
            manageDefaultEquipmentEvent.Manager = Manager;
            manageDefaultEquipmentEvent.BodyPart = BodyPart;
            return manageDefaultEquipmentEvent;
        }
    }
}