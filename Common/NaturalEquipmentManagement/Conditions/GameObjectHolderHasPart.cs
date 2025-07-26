using System;
using XRL.World;
using XRL.World.Parts;

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
            : base(Source)
        {
            Part = Source.Part;
        }
        public GameObjectHolderHasPart(GameObjectWielderHasPart Source)
            : base(Source)
        {
            Part = Source.Part;
        }

        public override bool Check(GameObject GameObject)
        {
            return (GameObject?.Holder == null && !FalseIfSubjectNull)
                || Part.IsNullOrEmpty() 
                || GameObject.Holder.HasPart(Part);
        }

        public static implicit operator GameObjectHolderHasPart(GameObjectWielderHasPart Condition)
        {
            return new(Condition);
        }
    }
}
