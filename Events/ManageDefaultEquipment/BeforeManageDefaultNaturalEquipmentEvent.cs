using System;
using System.Collections.Generic;

using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class BeforeManageDefaultNaturalEquipmentEvent : ModPooledEvent<BeforeManageDefaultNaturalEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeManageDefaultNaturalEquipmentEvent));

        public new static readonly int CascadeLevel = CASCADE_NONE; //CASCADE_EQUIPMENT | CASCADE_SLOTS | CASCADE_EXCEPT_THROWN_WEAPON;

        public GameObject Equipment;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public BeforeManageDefaultNaturalEquipmentEvent()
        {
        }

        public BeforeManageDefaultNaturalEquipmentEvent(GameObject Equipment, NaturalEquipmentManager Manager, BodyPart BodyPart)
            : this()
        {
            BeforeManageDefaultNaturalEquipmentEvent @new = FromPool(Equipment, Manager, BodyPart);
            this.Equipment = @new.Equipment;
            this.Manager = @new.Manager;
            this.BodyPart = @new.BodyPart;
        }
        public BeforeManageDefaultNaturalEquipmentEvent(ManageDefaultNaturalEquipmentEvent Source)
            : this(Source.Equipment, Source.Manager, Source.BodyPart)
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{nameof(BeforeManageDefaultNaturalEquipmentEvent)}";
        }

        public override void Reset()
        {
            base.Reset();
            Equipment = null;
            Manager = null;
            BodyPart = null;
        }

        public static BeforeManageDefaultNaturalEquipmentEvent Send(GameObject Equipment, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            Debug.Entry(4,
                $"! {nameof(BeforeManageDefaultNaturalEquipmentEvent)}."
                + $"{nameof(Send)}(Equipment: {Manager?.Wielder?.DebugName ?? NULL}'s "
                + $"{Equipment?.DebugName ?? NULL}, Manager,"
                + $" BodyPart: [{BodyPart?.ID}:{BodyPart?.Type}])",
                Indent: 0, Toggle: doDebug);

            BeforeManageDefaultNaturalEquipmentEvent E = new(Equipment, Manager, BodyPart);

            bool progress = E.BodyPart != null && E.Equipment != null;
            if (progress && E.Equipment.WantEvent(ID, CascadeLevel))
            {
                progress = E.Equipment.HandleEvent(E);
            }
            if (progress && Equipment.HasRegisteredEvent(E.GetRegisteredEventID()))
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter("Object", E.Equipment);
                @event.SetParameter("Manager", E.Manager);
                @event.SetParameter("BodyPart", E.BodyPart);
                progress = Equipment.FireEvent(@event);
            }
            return E;
        }

        public static BeforeManageDefaultNaturalEquipmentEvent FromPool(GameObject Object, NaturalEquipmentManager Manager, BodyPart BodyPart)
        {
            BeforeManageDefaultNaturalEquipmentEvent beforeManageDefaultEquipmentEvent = FromPool();
            beforeManageDefaultEquipmentEvent.Equipment = Object;
            beforeManageDefaultEquipmentEvent.Manager = Manager;
            beforeManageDefaultEquipmentEvent.BodyPart = BodyPart;
            return beforeManageDefaultEquipmentEvent;
        }
    }
}