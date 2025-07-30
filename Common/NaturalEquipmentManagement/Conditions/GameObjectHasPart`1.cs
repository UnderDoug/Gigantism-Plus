using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPart<T> : ICondition<GameObject>
        where T : IPart, new()
    {
        public GameObjectHasPart()
            : base()
        {
        }
        public GameObjectHasPart(GameObjectHasPart<T> Source)
            : base(Source)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                || (GameObject != null && GameObject.HasPart<T>());
        }
    }
}
