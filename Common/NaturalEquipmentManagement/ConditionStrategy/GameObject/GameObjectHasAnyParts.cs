using System;
using System.Collections;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasAnyParts : AnyConditions<GameObject>
    {
        public GameObjectHasAnyParts()
            : base()
        {
        }
        public GameObjectHasAnyParts(GameObjectHasAnyParts Source)
            : base(Source)
        {
        }
        public GameObjectHasAnyParts(IEnumerable<GameObjectHasPart> Conditions)
            : base(Conditions as AllConditions<GameObject>)
        {
        }
        public GameObjectHasAnyParts(IEnumerable<string> Parts)
            : base()
        {
            foreach (string part in Parts)
            {
                Add(new GameObjectHasPart(part));
            }
        }
    }
}
