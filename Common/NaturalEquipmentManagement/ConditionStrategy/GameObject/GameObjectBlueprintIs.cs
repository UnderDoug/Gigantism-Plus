using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectBlueprintIs : ICondition<GameObject>
    {
        public string Blueprint;

        public GameObjectBlueprintIs()
            : base()
        {
            Blueprint = null;
        }
        public GameObjectBlueprintIs(string Blueprint = null)
            : this()
        {
            this.Blueprint = Blueprint;
        }
        public GameObjectBlueprintIs(GameObjectBlueprintIs Source)
            : this(Source?.Blueprint)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                Blueprint.Quote(),
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                && !Blueprint.IsNullOrEmpty()
                && GameObject?.Blueprint == Blueprint;
        }
    }
}
