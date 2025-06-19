using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHolderHasPart : GameObjectHasPart
    {
        public GameObjectHolderHasPart()
            : base()
        {
        }
        public GameObjectHolderHasPart(string Part = null)
            : base(Part)
        {
        }
        public GameObjectHolderHasPart(IPart IPart = null)
            : this(IPart.Name)
        {
        }
        public GameObjectHolderHasPart(GameObjectHolderHasPart Source)
            : this(Source?.Part)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return GameObject?.Holder == null 
                || Part.IsNullOrEmpty() 
                || GameObject.Holder.HasPart(Part);
        }
    }
}
