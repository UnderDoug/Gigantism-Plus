using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static XRL.World.Parts.ModNaturalEquipmentBase;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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

        public PartAdjustment Adjustment;

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
            Adjustment = null;
        }

        public static BeforeApplyPartAdjustmentEvent FromPool(GameObject Equipment, string NaturalEquipmentMod, PartAdjustment Adjustment)
        {
            BeforeApplyPartAdjustmentEvent E = FromPool();
            E.Equipment = Equipment;
            E.NaturalEquipmentMod = NaturalEquipmentMod;
            E.Adjustment = Adjustment;
            return E;
        }
        public static bool Send(GameObject Equipment, string NaturalEquipmentMod, PartAdjustment Adjustment)
        {
            BeforeApplyPartAdjustmentEvent E = FromPool(Equipment, NaturalEquipmentMod, Adjustment);

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
                    Adjustment.SetValue(E.Adjustment.Value);
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(nameof(BeforeApplyPartAdjustmentEvent));
                    @event.SetParameter(nameof(Equipment), Equipment);
                    @event.SetParameter(nameof(NaturalEquipmentMod), NaturalEquipmentMod);
                    @event.SetParameter(nameof(Adjustment), Adjustment);
                    proceed = Equipment.FireEvent(@event);
                    Adjustment.SetValue(@event.GetParameter(nameof(Adjustment), E.Adjustment).Value);
                    @event.Clear();
                }
            }
            E.Reset();
            return proceed;
        }
    }
}