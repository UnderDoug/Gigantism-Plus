using System;
using System.Collections;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasAllParts : AllConditions<GameObject>
    {
        public GameObjectHasAllParts(IEnumerable<GameObjectHasPart> Source)
            : base(Source as AllConditions<GameObject>)
        {
        }
    }
}
