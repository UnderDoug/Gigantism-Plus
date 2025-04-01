using XRL;
using XRL.World;

namespace HNPS_GigantismPlus
{
    public class ExampleEvent : ModPooledEvent<ExampleEvent>
    {

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public string Value;

        // A static method that fires your event,
        // this isn't strictly necessary but is how the game prefers to organize it
        public static string GetFor(GameObject Object)
        {
            ExampleEvent E = FromPool();
            // Object.HandleEvent(E);
            // The.Game.HandleEvent(E);
            return E.Value;
        }

        // Resets the event before it's returned to the pool
        public override void Reset()
        {
            base.Reset();
            Value = null;
        }

        // How far our event will cascade,
        // this example will cascade to equipped items
        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

    }
    public class ExampleHandler : IEventHandler, IModEventHandler<ExampleEvent>
    {

        private static readonly ExampleHandler Instance = new();

        public static void Register()
        {
            // The.Game.RegisterEvent(Instance, ExampleEvent.ID);
        }

        public bool HandleEvent(ExampleEvent E)
        {
            E.Value = "Handled!";
            Debug.Entry(4, E.Value);
            return true;
        }
    }
}