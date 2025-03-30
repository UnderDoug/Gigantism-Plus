using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;

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
            Debug.Entry(4, $"{typeof(AfterModGiganticAppliedEvent).Name}.{nameof(FromPool)}(Object: {Object?.DebugName})", Indent: 0);
            AfterModGiganticAppliedEvent afterModGiganticAppliedEvent = FromPool();
            afterModGiganticAppliedEvent.Object = Object;
            afterModGiganticAppliedEvent.Modification = Modification;
            return afterModGiganticAppliedEvent;
        }
        public static void Send(GameObject Object, ModGigantic Modification)
        {
            Debug.Entry(4,
                $"{typeof(AfterModGiganticAppliedEvent).Name}." +
                $"{nameof(Send)}(Object: {Object?.ShortDisplayNameStripped}, ModGigantic Modification)",
                Indent: 0);

            AfterModGiganticAppliedEvent afterModGiganticAppliedEvent = FromPool(Object, Modification);

            bool flag = The.Game.HandleEvent(afterModGiganticAppliedEvent) || Object.HandleEvent(afterModGiganticAppliedEvent);

            ResetTo(ref afterModGiganticAppliedEvent);

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