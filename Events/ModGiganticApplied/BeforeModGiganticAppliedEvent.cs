using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public class BeforeModGiganticAppliedEvent : ModPooledEvent<BeforeModGiganticAppliedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeModGiganticAppliedEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = GetRegisteredEventID();

        public GameObject Object;

        public ModGigantic Modification;

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
        public static string GetRegisteredEventID()
        {
            return $"{nameof(BeforeModGiganticAppliedEvent)}";
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
            BeforeModGiganticAppliedEvent E = FromPool(Object, Modification);

            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Object.HasRegisteredEvent(RegisteredEventID);

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
                    Event @event = Event.New(nameof(BeforeModGiganticAppliedEvent));
                    @event.SetParameter("Object", Object);
                    @event.SetParameter("ModPart", Modification);
                    proceed = Object.FireEvent(@event);
                    @event.Clear();
                }
            }
        }
    }
}