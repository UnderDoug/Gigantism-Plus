using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotCondition<T> : ICondition<T>
        where T : class, new()
    {
        private static bool doDebug => getClassDoDebug("NotCondition");
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

            return Options.getDoDebug(what, doList, dontList, doDebug);
        }

        public ICondition<T> Condition;

        public NotCondition()
            : base()
        {
            Condition = null;
        }
        public NotCondition(ICondition<T> Condition = null)
            : this()
        {
            this.Condition = Condition;
        }
        public NotCondition(NotCondition<T> Source)
            : this(Source?.Condition)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(Check));
            Debug.Entry(4, $"[?] {nameof(NotCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool check = base.Check(Subject)
                && (Condition == null || !FalseIfSubjectNull)
                && Condition.NotCheck(Subject);

            Debug.LoopItem(4, $"{nameof(NotCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {check}",
                Good: check, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return check;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(NotCheck));
            Debug.Entry(4, $"[?] {nameof(NotCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool notCheck = base.NotCheck(Subject)
                && (Condition == null || !FalseIfSubjectNull)
                && Condition.Check(Subject);

            Debug.LoopItem(4, $"{nameof(NotCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {notCheck}",
                Good: notCheck, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return notCheck;
        }
    }
}
