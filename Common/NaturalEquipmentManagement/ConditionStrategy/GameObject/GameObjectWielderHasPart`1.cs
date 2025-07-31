using System;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectWielderHasPart<T> : GameObjectHasPart<T>
        where T : IPart, new()
    {
        public GameObjectWielderHasPart()
            : base()
        {
        }
        public GameObjectWielderHasPart(GameObjectHolderHasPart<T> Source)
            : base(Source)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject?.GetPart<NaturalEquipmentOperator>()?.Wielder);
        }

        public static implicit operator GameObjectWielderHasPart<T>(GameObjectHolderHasPart<T> Condition)
        {
            return new(Condition);
        }
    }
}
