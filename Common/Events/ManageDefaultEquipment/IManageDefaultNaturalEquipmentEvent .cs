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

        public NaturalEquipmentOperator Operator;

        public NaturalEquipmentManager Manager;

        public IManageDefaultNaturalEquipmentEvent()
        {
            Equipment = null;
            Creature = null;
            BodyPart = null;
            Operator = null;
            Manager = null;
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
            Operator = null;
            Manager = null;
        }

        public static T FromPool(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentOperator Operator)
        {
            T E = FromPool();
            E.Reset();
            if (Equipment != null && Creature != null && Operator != null && BodyPart != null)
            {
                E.Equipment = Equipment;
                E.Creature = Creature;
                E.BodyPart = BodyPart;
                E.Operator = Operator;
                return E;
            }
            return null;
        }
        public static T FromPoolCreature(GameObject Creature, NaturalEquipmentManager Manager)
        {
            T E = FromPool();
            E.Reset();
            if (Creature != null && Manager != null)
            {
                E.Creature = Creature;
                E.Manager = Manager;
                return E;
            }
            return null;
        }
        public static T FromPoolEquipment(GameObject Equipment, NaturalEquipmentManager Manager)
        {
            T E = FromPool();
            E.Reset();
            if (Equipment != null && Manager != null)
            {
                E.Equipment = Equipment;
                E.Manager = Manager;
                return E;
            }
            return null;
        }

        public static T Send(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentOperator Operator)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(Send)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName()}, "
                + $"{nameof(Operator)})",
                Indent: 0, Toggle: doDebug);

            T E = FromPool(Equipment, Creature, BodyPart, Operator);

            E.CheckFor();

            Debug.LastIndent = indent;
            return E;
        }
        public virtual bool CheckFor()
        {
            return ProcessEvent();
        }
        public static bool CheckFor(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentOperator Operator)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(CheckFor)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName()}, "
                + $"{nameof(Operator)})",
                Indent: indent, Toggle: doDebug);

            T E = FromPool(Equipment, Creature, BodyPart, Operator);

            bool checkResult = E.CheckFor();
            E.Reset();

            Debug.LastIndent = indent;
            return checkResult;
        }
        public virtual bool GetFor()
        {
            return ProcessEvent();
        }
        public static bool GetForCreature(GameObject Creature, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(GetForCreature)}("
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(Manager)})",
                Indent: indent, Toggle: doDebug);

            T E = FromPoolCreature(Creature, Manager);

            bool checkResult = E.GetFor();
            E.Reset();

            Debug.LastIndent = indent;
            return checkResult;
        }
        public static bool GetForEquipment(GameObject Equipment, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {typeof(T).Name}."
                + $"{nameof(GetForEquipment)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Manager)})",
                Indent: indent, Toggle: doDebug);

            T E = FromPoolEquipment(Equipment, Manager);

            bool checkResult = E.GetFor();
            E.Reset();

            Debug.LastIndent = indent;
            return checkResult;
        }
        public static bool WantToProceed(GameObject Equipment, GameObject Creature, BodyPart BodyPart, NaturalEquipmentOperator Operator, NaturalEquipmentManager Manager, out bool CreatureWantsMin, out bool EquipmentWantsMin, out bool CreatureWantsStr, out bool EquipmentWantsStr)
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"* {typeof(T).Name}."
                + $"{nameof(WantToProceed)}() for"
                + $"{nameof(Equipment)}: {Equipment?.DebugName ?? NULL}, "
                + $"{nameof(Creature)}: {Creature?.DebugName ?? NULL}, "
                + $"{nameof(BodyPart)}: {BodyPart?.DebugName() ?? NULL}",
                Indent: indent + 1, Toggle: doDebug);

            bool haveCreature = Creature != null;
            bool haveEquipment = Equipment != null;
            bool haveBodyPart = BodyPart != null;
            bool haveOperator = Operator != null;
            bool haveManager = Manager != null;

            Debug.LoopItem(4, $"{nameof(haveCreature)}", $"{haveCreature}",
                Good: haveCreature, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(haveEquipment)}", $"{haveEquipment}",
                Good: haveEquipment, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(haveBodyPart)}", $"{haveBodyPart}",
                Good: haveBodyPart, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(haveOperator)}", $"{haveOperator}",
                Good: haveOperator, Indent: indent + 2, Toggle: doDebug);

            Debug.LoopItem(4, $"{nameof(haveManager)}", $"{haveManager}",
                Good: haveManager, Indent: indent + 2, Toggle: doDebug);

            CreatureWantsMin = false;
            EquipmentWantsMin = false;
            CreatureWantsStr = false;
            EquipmentWantsStr = false;

            bool haveNecessaryParts = (haveBodyPart && haveOperator) || haveManager;

            Debug.LoopItem(4, $"{nameof(haveNecessaryParts)}", $"{haveNecessaryParts}",
                Good: haveNecessaryParts, Indent: indent + 2, Toggle: doDebug);

            if ((haveBodyPart && haveOperator) || haveManager)
            {
                CreatureWantsMin = haveCreature && Creature.WantEvent(ID, CascadeLevel);
                EquipmentWantsMin = haveEquipment && Equipment.WantEvent(ID, CascadeLevel);

                CreatureWantsStr = haveCreature && Creature.HasRegisteredEvent(RegisteredEventID);
                EquipmentWantsStr = haveEquipment && Equipment.HasRegisteredEvent(RegisteredEventID);

                Debug.LoopItem(4, $"{nameof(CreatureWantsMin)}", $"{CreatureWantsMin}",
                    Good: CreatureWantsMin, Indent: indent + 3, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(EquipmentWantsMin)}", $"{EquipmentWantsMin}",
                    Good: EquipmentWantsMin, Indent: indent + 3, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(CreatureWantsStr)}", $"{CreatureWantsStr}",
                    Good: CreatureWantsStr, Indent: indent + 3, Toggle: doDebug);

                Debug.LoopItem(4, $"{nameof(EquipmentWantsStr)}", $"{EquipmentWantsStr}",
                    Good: EquipmentWantsStr, Indent: indent + 3, Toggle: doDebug);
            }
            Debug.LastIndent = indent;
            return  CreatureWantsMin || EquipmentWantsMin || CreatureWantsStr || EquipmentWantsStr;
        }
        private bool ProcessEvent()
        {
            bool anyWants = WantToProceed(Equipment, Creature, BodyPart, Operator, Manager,
                out bool CreatureWantsMin,
                out bool EquipmentWantsMin,
                out bool CreatureWantsStr,
                out bool EquipmentWantsStr);

            bool proceed = true;

            if (anyWants)
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
                    @event.SetParameter(nameof(Operator), Operator);

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