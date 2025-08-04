using System;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectWielderHasPart<T> : GameObjectHasPart<T>
        where T : IPart
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
            int indent = Debug.LastIndent;

            GameObject wielder = GameObject?.Wielder();
            Debug.Entry(4, $"{nameof(wielder)} is {wielder?.DebugName ?? Const.NULL}", Indent: indent + 1, Toggle: false);

            bool check = base.Check(wielder);

            Debug.LastIndent = indent;
            return check;
        }

        public static implicit operator GameObjectWielderHasPart<T>(GameObjectHolderHasPart<T> Condition)
        {
            return new(Condition);
        }
    }
}
