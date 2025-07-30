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
            return base.Check(GameObject)
                && Part.IsNullOrEmpty() 
                || (GameObject != null && GameObject.HasPart(Part));
        }
    }
}
