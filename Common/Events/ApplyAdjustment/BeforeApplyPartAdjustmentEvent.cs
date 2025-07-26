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

        public GameObject Subject;

        public PartAdjustment Adjustment;

        public string NaturalEquipmentMod;

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
            Subject = null;
            Adjustment = null;
            NaturalEquipmentMod = null;
        }

        public static bool CheckFor(GameObject Equipment, string NaturalEquipmentMod, PartAdjustment Adjustment)
        {
            BeforeApplyPartAdjustmentEvent E = FromPool();
            E.Subject = Equipment;
            E.NaturalEquipmentMod = NaturalEquipmentMod;
            E.Adjustment = Adjustment;

            bool haveObject = Equipment != null;

            bool objectWantsMin = haveObject && Equipment.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Equipment.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = objectWantsMin || objectWantsStr;

            bool proceed = haveObject;

            if (proceed && anyWants)
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
                    @event.SetParameter(nameof(NaturalEquipmentMod), NaturalEquipmentMod.ToString());
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