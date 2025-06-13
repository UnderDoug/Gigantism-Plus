using XRL.World;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_EXCEPT_THROWN_WEAPON, Cache = Cache.Pool)]
    public class BeforeBodyPartsUpdatedEvent : IBodyPartsUpdatedEvent<BeforeBodyPartsUpdatedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeBodyPartsUpdatedEvent));

        public BeforeBodyPartsUpdatedEvent()
        {
        }
    }
}