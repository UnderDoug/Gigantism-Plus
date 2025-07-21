using System;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectWielderHasPart : GameObjectHasPart
    {
        public GameObjectWielderHasPart()
            : base()
        {
        }
        public GameObjectWielderHasPart(string Part = null)
            : base(Part)
        {
        }
        public GameObjectWielderHasPart(IPart IPart = null)
            : this(IPart.Name)
        {
        }
        public GameObjectWielderHasPart(GameObjectWielderHasPart Source)
            : this(Source?.Part)
        {
        }
        public GameObjectWielderHasPart(GameObjectHolderHasPart Source)
            : this(Source?.Part)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            GameObject wielder = GameObject?.GetPart<NaturalEquipmentOperator>()?.Wielder;
            return (wielder == null && !FalseIfSubjectNull)
                || Part.IsNullOrEmpty() 
                || wielder.HasPart(Part);
        }

        public static implicit operator GameObjectWielderHasPart(GameObjectHolderHasPart Condition)
        {
            return new(Condition);
        }
    }
}
