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
    public class GetNaturalEquipmentOperatorsEvent : IManageDefaultNaturalEquipmentEvent<GetNaturalEquipmentOperatorsEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(GetNaturalEquipmentOperatorsEvent));

        public List<NaturalEquipmentOperator> Operators;

        public GetNaturalEquipmentOperatorsEvent()
            : base()
        {
            Operators = null;
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public override void Reset()
        {
            base.Reset(); 
            Operators = null;
        }

        public virtual List<NaturalEquipmentOperator> GetForCreature()
        {
            return ProcessEvent() ? Operators : null;
        }
        public virtual NaturalEquipmentOperator GetForEquipment()
        {
            return ProcessEvent() ? Operator : null;
        }
        public static List<NaturalEquipmentOperator> GetForCreature(GameObject Creature, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {nameof(GetNaturalEquipmentOperatorsEvent)}."
                + $"{nameof(GetForCreature)}("
                + $"{nameof(Creature)}: {Creature?.DebugName}, "
                + $"{nameof(Manager)})",
                Indent: indent, Toggle: doDebug);

            GetNaturalEquipmentOperatorsEvent E = FromPoolCreature(Creature, Manager);
            E.Operators = new();

            List<NaturalEquipmentOperator> getResult = E.GetForCreature();
            E.Reset();

            Debug.LastIndent = indent;
            return getResult;
        }
        public static NaturalEquipmentOperator GetForEquipment(GameObject Equipment, NaturalEquipmentManager Manager)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"! {nameof(GetNaturalEquipmentOperatorsEvent)}."
                + $"{nameof(GetForEquipment)}("
                + $"{nameof(Equipment)}: {Equipment?.DebugName}, "
                + $"{nameof(Manager)})",
                Indent: indent, Toggle: doDebug);

            GetNaturalEquipmentOperatorsEvent E = FromPoolEquipment(Equipment, Manager);

            NaturalEquipmentOperator getResult = E.GetForEquipment();
            E.Reset();

            Debug.LastIndent = indent;
            return getResult;
        }

        public virtual List<NaturalEquipmentOperator> AddOperator(NaturalEquipmentOperator Operator)
        {
            Operators ??= new();
            this.Operator = Operator;
            if (Operators.TryAdd(Operator))
            {
                Operator.Manager = Manager;
            }
            return Operators;
        }
    }
}