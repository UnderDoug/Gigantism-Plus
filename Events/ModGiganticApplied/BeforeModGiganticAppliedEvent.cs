using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public class BeforeModGiganticAppliedEvent : ModPooledEvent<BeforeModGiganticAppliedEvent>
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

        public static BeforeModGiganticAppliedEvent FromPool(GameObject Object, ModGigantic Modification)
        {
            BeforeModGiganticAppliedEvent beforeModGiganticAppliedEvent = FromPool();
            beforeModGiganticAppliedEvent.Object = Object;
            beforeModGiganticAppliedEvent.Modification = Modification;
            return beforeModGiganticAppliedEvent;
        }
        public static void Send(GameObject Object, ModGigantic Modification)
        {
            BeforeModGiganticAppliedEvent beforeModGiganticAppliedEvent = FromPool(Object, Modification);

            bool flag = The.Game.HandleEvent(beforeModGiganticAppliedEvent) && Object.HandleEvent(beforeModGiganticAppliedEvent);

            if (flag && Object.HasRegisteredEvent(typeof(BeforeModGiganticAppliedEvent).Name))
            {
                Event @event = Event.New(typeof(BeforeModGiganticAppliedEvent).Name);
                @event.SetParameter("Object", Object);
                @event.SetParameter("Modification", Modification);
                Object.FireEvent(@event);
                @event.Clear();
            }
        }
    }
}