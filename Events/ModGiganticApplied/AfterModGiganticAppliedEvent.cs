using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public class AfterModGiganticAppliedEvent : ModPooledEvent<AfterModGiganticAppliedEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public ModGigantic Modification;

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            Modification = null;
        }

        public static AfterModGiganticAppliedEvent FromPool(GameObject Object, ModGigantic Modification)
        {
            AfterModGiganticAppliedEvent afterModGiganticAppliedEvent = FromPool();
            afterModGiganticAppliedEvent.Object = Object;
            afterModGiganticAppliedEvent.Modification = Modification;
            return afterModGiganticAppliedEvent;
        }
        public static void Send(GameObject Object, ModGigantic Modification)
        {
            AfterModGiganticAppliedEvent afterModGiganticAppliedEvent = FromPool(Object, Modification);

            bool flag = The.Game.HandleEvent(afterModGiganticAppliedEvent) && Object.HandleEvent(afterModGiganticAppliedEvent);

            if (flag && Object.HasRegisteredEvent(typeof(AfterModGiganticAppliedEvent).Name))
            {
                Event @event = Event.New(typeof(AfterModGiganticAppliedEvent).Name);
                @event.SetParameter("Object", Object);
                @event.SetParameter("Modification", Modification);
                Object.FireEvent(@event);
                @event.Clear();
            }
        }
    }
}