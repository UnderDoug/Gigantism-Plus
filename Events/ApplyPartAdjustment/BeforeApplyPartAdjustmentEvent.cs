using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using System;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class BeforeApplyPartAdjustmentEvent : ModPooledEvent<BeforeApplyPartAdjustmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeApplyPartAdjustmentEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = nameof(BeforeApplyPartAdjustmentEvent);

        public GameObject Equipment;

        public string NaturalEquipmentMod;

        public Type Target;

        public string Field;

        public object Value;

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
            NaturalEquipmentMod = null;
            Target = null;
            Field = null;
        }

        public static BeforeApplyPartAdjustmentEvent FromPool(GameObject Equipment, string NaturalEquipmentMod, Type Target, string Field, ref object Value)
        {
            BeforeApplyPartAdjustmentEvent E = FromPool();
            E.Equipment = Equipment;
            E.NaturalEquipmentMod = NaturalEquipmentMod;
            E.Target = Target;
            E.Field = Field;
            E.Value = Value;
            return E;
        }
        public static bool Send(GameObject Equipment, string NaturalEquipmentMod, Type Target, string Field, ref object Value)
        {
            BeforeApplyPartAdjustmentEvent E = FromPool(Equipment, NaturalEquipmentMod, Target, Field, ref Value);

            bool haveObject = Equipment != null;

            bool objectWantsMin = haveObject && Equipment.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Equipment.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = objectWantsMin || objectWantsStr;

            bool proceed = anyWants;

            if (proceed)
            {
                if (proceed && objectWantsMin)
                {
                    proceed = Equipment.HandleEvent(E);
                    Value = E.Value;
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(nameof(BeforeApplyPartAdjustmentEvent));
                    @event.SetParameter(nameof(Equipment), Equipment);
                    @event.SetParameter(nameof(Target), Target);
                    @event.SetParameter(nameof(Field), Field);
                    @event.SetParameter(nameof(Value), E.Value);
                    @event.SetParameter(nameof(NaturalEquipmentMod), NaturalEquipmentMod);
                    proceed = Equipment.FireEvent(@event);
                    Value = @event.GetParameter(nameof(Value), E.Value);
                    @event.Clear();
                }
            }
            E.Reset();
            return proceed;
        }
    }
}