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

        public static readonly string RegisteredEventID = nameof(ManageDefaultNaturalEquipmentEvent);

        public GameObject Equipment;

        public GameObject Creature;

        public NaturalEquipmentManager Manager;

        public BodyPart BodyPart;

        public ManageDefaultNaturalEquipmentEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            base.Reset();
            Equipment = null;
            Creature = null;
            Manager = null;
            BodyPart = null;
        }

        public static bool CheckFor(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentManager Manager)
        {
            Debug.Entry(4,
                $"! {nameof(ManageDefaultNaturalEquipmentEvent)}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName()}, "
                + $"{nameof(Manager)})",
                Indent: 0, Toggle: doDebug);
            
            ManageDefaultNaturalEquipmentEvent E = FromPool();

            E.Equipment = Equipment;
            E.Creature = Creature;
            E.BodyPart = BodyPart;
            E.Manager = Manager;

            bool haveCreature = E.Creature != null;
            bool haveEquipment = E.Equipment != null;

            bool creatureWantsMin = haveCreature && E.Creature.WantEvent(ID, CascadeLevel);
            bool equipmentWantsMin = haveEquipment && E.Equipment.WantEvent(ID, CascadeLevel);

            bool creatureWantsStr = haveCreature && E.Creature.HasRegisteredEvent(RegisteredEventID);
            bool equipmentWantsStr = haveEquipment && E.Equipment.HasRegisteredEvent(RegisteredEventID);

            bool anyWantsMin = creatureWantsMin || equipmentWantsMin;
            bool anyWantsStr = creatureWantsStr || equipmentWantsStr;

            bool anyWant = anyWantsMin || anyWantsStr;

            bool proceed = anyWant;
            if (proceed)
            {
                if (proceed && anyWantsMin)
                {
                    if (proceed && creatureWantsMin)
                    {
                        proceed = E.Creature.HandleEvent(E);
                    }
                    if (proceed && equipmentWantsMin)
                    {
                        proceed = E.Equipment.HandleEvent(E);
                    }
                }
                if (proceed && anyWantsStr)
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter(nameof(Equipment), E.Equipment);
                    @event.SetParameter(nameof(Creature), E.Creature);
                    @event.SetParameter(nameof(BodyPart), E.BodyPart);
                    @event.SetParameter(nameof(Manager), E.Manager);

                    if (proceed && creatureWantsStr)
                    {
                        proceed = E.Creature.FireEvent(@event);
                    }
                    if (proceed && equipmentWantsStr)
                    {
                        proceed = E.Equipment.FireEvent(@event);
                    }
                }
            }
            return proceed;
        }
    }
}