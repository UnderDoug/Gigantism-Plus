using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAnyConditions<T> : AnyConditions<T>
        where T : class, new()
    {
        public NotAnyConditions()
            : base()
        {
        }
        public NotAnyConditions(IConditions<T> Conditions)
            : base(Conditions)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(NotAnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            bool check = base.NotCheck(Subject);

            Debug.LoopItem(4, $"{nameof(NotAnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {check}",
                Good: check, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return check;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(NotAnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            bool notCheck = base.Check(Subject);

            Debug.LoopItem(4, $"{nameof(NotAnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {notCheck}",
                Good: notCheck, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return notCheck;
        }
    }
}
