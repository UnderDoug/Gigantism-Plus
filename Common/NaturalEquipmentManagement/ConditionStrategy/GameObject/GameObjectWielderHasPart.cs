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
            : base(Source)
        {
            Part = Source.Part;
        }
        public GameObjectWielderHasPart(GameObjectHolderHasPart Source)
            : base(Source)
        {
            Part = Source.Part;
        }

        public override bool Check(GameObject GameObject)
        {
            GameObject wielder = GameObject?.Wielder();
            Debug.Entry(4, $"{nameof(wielder)} is {wielder?.DebugName ?? Const.NULL}", Indent: Debug.LastIndent + 1, Toggle: true);
            Debug.LastIndent--;
            return base.Check(wielder);
        }

        public static implicit operator GameObjectWielderHasPart(GameObjectHolderHasPart Condition)
        {
            return new(Condition);
        }
    }
}
