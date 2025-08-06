using System;
using System.Collections.Generic;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAllConditions<T> : AllConditions<T>
        where T : class, new()
    {
        private static bool doDebug => getClassDoDebug("NotAllConditions");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                // nameof(Check),
                // nameof(NotCheck),
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

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
            bool doDebug = getDoDebug(nameof(Check));
            Debug.Entry(4, $"[?] {nameof(NotAllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool check = base.NotCheck(Subject);

            Debug.LoopItem(4, $"{nameof(NotAllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {check}",
                Good: check, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return check;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(NotCheck));
            Debug.Entry(4, $"[?] {nameof(NotAllConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool notCheck = base.Check(Subject);

            Debug.LoopItem(4, $"{nameof(NotAllConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {notCheck}",
                Good: notCheck, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return notCheck;
        }
    }
}
