using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class AfterModGiganticAppliedEvent : ModPooledEvent<AfterModGiganticAppliedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(AfterModGiganticAppliedEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = nameof(AfterModGiganticAppliedEvent);

        public GameObject Object;

        public ModGigantic Modification;

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
            Object = null;
            Modification = null;
        }

        public static AfterModGiganticAppliedEvent FromPool(GameObject Object, ModGigantic Modification)
        {
            AfterModGiganticAppliedEvent E = FromPool();
            E.Object = Object;
            E.Modification = Modification;
            return E;
        }
        public static void Send(GameObject Object, ModGigantic Modification)
        {
            AfterModGiganticAppliedEvent E = FromPool(Object, Modification);

            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && haveObject && The.Game.WantEvent(ID, CascadeLevel);

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
                    Event @event = Event.New(nameof(AfterModGiganticAppliedEvent));
                    @event.SetParameter("Object", Object);
                    @event.SetParameter("ModPart", Modification);
                    proceed = Object.FireEvent(@event);
                    @event.Clear();
                }
            }
        }
    }
}