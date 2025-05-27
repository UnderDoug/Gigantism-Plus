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
            AfterModGiganticAppliedEvent E = FromPool(Object, Modification);
            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Object.HasRegisteredEvent(nameof(AfterModGiganticAppliedEvent));

            bool anyWants = gameWants || objectWantsMin || objectWantsStr;

            bool proceed = anyWants;

            if (proceed)
            {
                if (proceed && gameWants)
                {
                    proceed = The.Game.HandleEvent(E);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = Object.HandleEvent(E);
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(nameof(AfterModGiganticAppliedEvent));
                    @event.SetParameter("Object", Object);
                    @event.SetParameter("Modification", Modification);
                    proceed = Object.FireEvent(@event);
                    @event.Clear();
                }
            }
        }
    }
}