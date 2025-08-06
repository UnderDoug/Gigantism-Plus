using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPart<T> : ICondition<GameObject>
        where T : IPart
    {
        public GameObjectHasPart()
            : base()
        {
        }
        public GameObjectHasPart(GameObjectHasPart<T> Source)
            : base(Source)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(T).ToStringWithGenerics()
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                && GameObject.HasPart<T>();
        }
    }
}
