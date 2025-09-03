using System;
using System.Collections.Generic;
using XRL.World;

using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPropertyOrTag : ICondition<GameObject>
    {
        public string PropertyOrTag;

        public GameObjectHasPropertyOrTag()
            : base()
        {
            PropertyOrTag = null;
        }
        public GameObjectHasPropertyOrTag(string PropertyOrTag)
            : base()
        {
            this.PropertyOrTag = PropertyOrTag;
        }
        public GameObjectHasPropertyOrTag(GameObjectHasPropertyOrTag Source)
            : this(Source.PropertyOrTag)
        {
        }
        public GameObjectHasPropertyOrTag(GameObjectHasPropertyOrTagEqualTo Source)
            : this(Source.PropertyOrTag)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                PropertyOrTag ?? NULL
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                && !PropertyOrTag.IsNullOrEmpty() 
                && GameObject.HasPropertyOrTag(PropertyOrTag);
        }
    }
}
