using XRL.World;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class SetGiganticCreatureEvent : IGiganticEvent<SetGiganticCreatureEvent>
    {
        public SetGiganticCreatureEvent()
            : base()
        {
        }
    }
}