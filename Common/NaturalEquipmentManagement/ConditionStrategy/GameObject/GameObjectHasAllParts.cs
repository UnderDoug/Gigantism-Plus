using System;
using System.Collections;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasAllParts : ConditionsAll<GameObject>
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
            : base(Conditions as ConditionsAll<GameObject>)
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
        public GameObjectHasAllParts(IEnumerable<Type> Parts)
            : base()
        {
            foreach (Type part in Parts)
            {
                Add(new GameObjectHasPart(part.Name));
            }
        }
    }
}
