using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasPart : ICondition<GameObject>
    {
        public string Part;

        public IPart IPart;

        public GameObjectHasPart()
            : base()
        {
            Part = null;
            IPart = null;
        }
        public GameObjectHasPart(string Part = null, IPart IPart = null)
            : base()
        {
            this.Part = Part;
            this.IPart = IPart;
        }
        public GameObjectHasPart(GameObjectHasPart Source)
            : this(Source?.Part, Source?.IPart)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            Part ??= IPart?.Name;

            if (Part.IsNullOrEmpty() && IPart == null)
            {
                return true;
            }
            return GameObject == null 
                || Part.IsNullOrEmpty() 
                || GameObject.HasPart(Part);
        }
    }
}
