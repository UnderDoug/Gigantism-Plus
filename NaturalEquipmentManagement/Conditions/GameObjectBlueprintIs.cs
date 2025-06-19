using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectBlueprintIs : ICondition<GameObject>
    {
        public string Variant;

        public GameObjectBlueprintIs()
            : base()
        {
            Variant = null;
        }
        public GameObjectBlueprintIs(string Blueprint = null)
            : this()
        {
            this.Variant = Blueprint;
        }
        public GameObjectBlueprintIs(GameObjectBlueprintIs Source)
            : this(Source?.Variant)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return Variant.IsNullOrEmpty()
                || GameObject == null
                || GameObject?.Blueprint == Variant;
        }
    }
}
