using System;
using XRL.World;

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

        public override bool Check(GameObject GameObject)
        {
            return GameObject == null 
                || PropertyOrTag.IsNullOrEmpty() 
                || GameObject.HasPropertyOrTag(PropertyOrTag);
        }
    }
}
