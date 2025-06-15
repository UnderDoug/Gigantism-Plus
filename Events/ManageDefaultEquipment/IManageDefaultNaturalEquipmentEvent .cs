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
    [GameEvent(Base = true, Cascade = CASCADE_EQUIPMENT, Cache = Cache.Pool)]
    public abstract class IManageDefaultNaturalEquipmentEvent<T> : ModPooledEvent<T>
        where T : IManageDefaultNaturalEquipmentEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);

        public new static readonly int CascadeLevel = CASCADE_EQUIPMENT;

        public static string RegisteredEventID => typeof(T).Name;

        public GameObject Equipment;

        public GameObject Creature;

        public BodyPart BodyPart;

        public NaturalEquipmentManager Manager;

        public IManageDefaultNaturalEquipmentEvent()
        {
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
            BodyPart = null;
            Manager = null;
        }

        public static T FromPool(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentManager Manager)
        {
            T E = FromPool();
            if (Equipment != null && Creature != null && Manager != null && BodyPart != null)
            {
                E.Equipment = Equipment;
                E.Creature = Creature;
                E.BodyPart = BodyPart;
                E.Manager = Manager;
                return E;
            }
            E.Reset();
            return null;
        }
        public static T Send(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName()}, "
                + $"{nameof(Manager)})",
                Indent: 0, Toggle: doDebug);

            T E = FromPool(Equipment, Creature, BodyPart, Manager);

            E.CheckFor();

            Debug.LastIndent = indent;
            return E;
        }
        public virtual bool CheckFor()
        {
            return ProcessEvent();
        }
        public static bool CheckFor(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName()}, "
                + $"{nameof(Manager)})",
                Indent: indent, Toggle: doDebug);

            T E = FromPool(Equipment, Creature, BodyPart, Manager);

            bool checkResult = E.CheckFor();
            E.Reset();

            Debug.LastIndent = indent;
            return checkResult;
        }
        public static bool WantToProceed(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentManager Manager, out bool CreatureWantsMin, out bool EquipmentWantsMin, out bool CreatureWantsStr, out bool EquipmentWantsStr)
        {
            bool haveCreature = Creature != null;
            bool haveEquipment = Equipment != null;
            bool haveBodyPart = BodyPart != null;
            bool haveManager = Manager != null;

            CreatureWantsMin = haveCreature && Creature.WantEvent(ID, CascadeLevel);
            EquipmentWantsMin = haveEquipment && Equipment.WantEvent(ID, CascadeLevel);

            CreatureWantsStr = haveCreature && Creature.HasRegisteredEvent(RegisteredEventID);
            EquipmentWantsStr = haveEquipment && Equipment.HasRegisteredEvent(RegisteredEventID);

            if (!haveBodyPart || !haveManager)
            {
                CreatureWantsMin = false;
                EquipmentWantsMin = false;
                CreatureWantsStr = false;
                EquipmentWantsStr= false;
            }
            return  CreatureWantsMin || EquipmentWantsMin || CreatureWantsStr || EquipmentWantsStr;
        }
        private bool ProcessEvent()
        {
            bool proceed = WantToProceed(Equipment, Creature, BodyPart, Manager,
                out bool CreatureWantsMin,
                out bool EquipmentWantsMin,
                out bool CreatureWantsStr,
                out bool EquipmentWantsStr);
            if (proceed)
            {
                if (proceed && CreatureWantsMin)
                {
                    proceed = Creature.HandleEvent(this);
                }
                if (proceed && EquipmentWantsMin)
                {
                    proceed = Equipment.HandleEvent(this);
                }
                if (proceed && (CreatureWantsStr || EquipmentWantsStr))
                {
                    Event @event = Event.New(GetRegisteredEventID());
                    @event.SetParameter(nameof(Equipment), Equipment);
                    @event.SetParameter(nameof(Creature), Creature);
                    @event.SetParameter(nameof(BodyPart), BodyPart);
                    @event.SetParameter(nameof(Manager), Manager);

                    if (proceed && CreatureWantsStr)
                    {
                        proceed = Creature.FireEvent(@event);
                    }
                    if (proceed && EquipmentWantsStr)
                    {
                        proceed = Equipment.FireEvent(@event);
                    }
                }
            }
            return proceed;
        }
    }
}