using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPropertyOrTagEqualTo : GameObjectHasPropertyOrTag
    {
        public string Value;

        public GameObjectHasPropertyOrTagEqualTo()
            : base()
        {
            Value = null;
        }
        public GameObjectHasPropertyOrTagEqualTo(string PropertyOrTag = null, string Value = null)
            : base(PropertyOrTag)
        {
            this.Value = Value;
        }
        public GameObjectHasPropertyOrTagEqualTo(GameObjectHasPropertyOrTagEqualTo Source)
            : base(Source.PropertyOrTag)
        {
            Value = Source.Value;
        }
        public GameObjectHasPropertyOrTagEqualTo(GameObjectHasPropertyOrTag Source, string Value)
            : base(Source)
        {
            this.Value = Value;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                $"{nameof(Value)}, {Value}"
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return Value.IsNullOrEmpty() ? base.Check(GameObject) : GameObject?.GetPropertyOrTag(PropertyOrTag) == Value;
        }
    }
}
