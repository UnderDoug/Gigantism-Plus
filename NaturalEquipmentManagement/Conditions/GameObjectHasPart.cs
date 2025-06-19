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
            : this(Source?.Part)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return GameObject == null 
                || Part.IsNullOrEmpty() 
                || GameObject.HasPart(Part);
        }
    }
}
