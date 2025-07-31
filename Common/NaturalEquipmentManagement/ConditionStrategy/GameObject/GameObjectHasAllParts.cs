using System;
using System.Collections;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasAllParts : AllConditions<GameObject>
    {
        public GameObjectHasAllParts()
            : base()
        {
        }
        public GameObjectHasAllParts(GameObjectHasAllParts Source)
            : base(Source)
        {
        }
        public GameObjectHasAllParts(IEnumerable<GameObjectHasPart> Conditions)
            : base(Conditions as AllConditions<GameObject>)
        {
        }
        public GameObjectHasAllParts(IEnumerable<string> Parts)
            : base()
        {
            foreach (string part in Parts)
            {
                Add(new GameObjectHasPart(part));
            }
        }
    }
}
