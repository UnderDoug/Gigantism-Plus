using System;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHolderHasPart<T> : GameObjectHasPart<T>
        where T : IPart
    {
        public GameObjectHolderHasPart()
            : base()
        {
        }
        public GameObjectHolderHasPart(GameObjectHolderHasPart<T> Source)
            : base(Source)
        {
        }
        public GameObjectHolderHasPart(GameObjectWielderHasPart<T> Source)
            : base(Source)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject?.Holder);
        }

        public static implicit operator GameObjectHolderHasPart<T>(GameObjectWielderHasPart<T> Condition)
        {
            return new(Condition);
        }
    }
}
