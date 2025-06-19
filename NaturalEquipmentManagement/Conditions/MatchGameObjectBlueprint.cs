using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class MatchGameObjectBlueprint : ICondition<GameObject>
    {
        public string Blueprint;

        public MatchGameObjectBlueprint()
            : base()
        {
            Blueprint = null;
        }
        public MatchGameObjectBlueprint(string Blueprint = null)
            : this()
        {
            this.Blueprint = Blueprint;
        }
        public MatchGameObjectBlueprint(MatchGameObjectBlueprint Source)
            : this(Source?.Blueprint)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return Blueprint.IsNullOrEmpty()
                || GameObject == null
                || GameObject?.Blueprint == Blueprint;
        }
    }
}
