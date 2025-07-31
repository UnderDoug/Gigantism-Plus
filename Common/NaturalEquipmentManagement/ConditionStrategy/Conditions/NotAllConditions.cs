using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAllConditions<T> : AllConditions<T>
        where T : class, new()
    {
        public NotAllConditions()
            : base()
        {
        }
        public NotAllConditions(IConditions<T> Source)
            : base(Source)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(NotAllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            bool check = base.NotCheck(Subject);

            Debug.LoopItem(4, $"{nameof(NotAllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {check}",
                Good: check, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return check;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(NotAllConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            bool notCheck = base.Check(Subject);

            Debug.LoopItem(4, $"{nameof(NotAllConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {notCheck}",
                Good: notCheck, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return notCheck;
        }
    }
}
