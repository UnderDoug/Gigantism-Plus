using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPart : ICondition<GameObject>
    {
        public string Part;

        public GameObjectHasPart()
            : base()
        {
            Part = null;
        }
        public GameObjectHasPart(string Part = null)
            : this()
        {
            this.Part = Part;
        }
        public GameObjectHasPart(IPart IPart = null)
            : this(IPart.Name)
        {
        }
        public GameObjectHasPart(GameObjectHasPart Source)
            : base(Source)
        {
            Part = Source.Part;
        }

        public override bool Check(GameObject GameObject)
        {
            return (GameObject == null && !FalseIfSubjectNull)
                || Part.IsNullOrEmpty() 
                || GameObject.HasPart(Part);
        }
    }
}
