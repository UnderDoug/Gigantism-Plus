using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class MatchBlueprint : ICondition<GameObject>
    {
        public string Blueprint;

        public MatchBlueprint(string Blueprint)
            : base()
        {
            this.Blueprint = Blueprint;
        }

        public override bool Check(GameObject GameObject)
        {
            return !Blueprint.IsNullOrEmpty() && GameObject.Blueprint == Blueprint;
        }
    }
}
