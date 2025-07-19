using System;
using XRL.World;

namespace XRL
{
    [Serializable]
    [HasGameBasedStaticCache]
    public class TrueWolves_ExperienceGameSystem : IGameSystem
    {
        public static TrueWolves_ExperienceGameSystem Instance = new();

        public override void Register(XRLGame Game, IEventRegistrar Registrar)
        {
            Registrar.Register(AwardingXPEvent.ID, EventOrder.EXTREMELY_EARLY);
        }

        public override bool HandleEvent(AwardingXPEvent E)
        {
            // Do your ENTIRE XP thing here, including cascading XP to followers
            // return false; // This is important, it stops Experience.cs from continuing to handle AwardXPEvent
            /*
            UnityEngine.Debug.LogError(
                $"{nameof(TrueWolves_ExperienceGameSystem)}." +
                $"{nameof(HandleEvent)}({nameof(AwardingXPEvent)} E)" +
                $" Proof of Concept");
            */
            return base.HandleEvent(E);
        }

        [GameBasedCacheInit]
        public static void AdditionalSetup()
        {
            The.Game?.AddSystem(Instance);
        }
    }
}
