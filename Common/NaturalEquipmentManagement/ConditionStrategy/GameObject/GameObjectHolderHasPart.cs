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
            GameObject holder = GameObject?.Holder;
            Debug.Entry(4, $"{nameof(holder)} is {holder?.DebugName ?? Const.NULL}", Indent: Debug.LastIndent + 1, Toggle: true);
            Debug.LastIndent--;
            return base.Check(holder);
        }

        public static implicit operator GameObjectHolderHasPart(GameObjectWielderHasPart Condition)
        {
            return new(Condition);
        }
    }
}
