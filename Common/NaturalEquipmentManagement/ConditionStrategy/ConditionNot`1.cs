using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    /// <summary>
    /// Represents the inversion of an <see cref="Condition{T}"/>, where <see cref="Check(T)"/> will return the result of the supplied <see cref="Condition{T}.NotCheck(T)"/> and <see cref="NotCheck(T)"/> will return the result of the supplied <see cref="Condition{T}.Check(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of object on which to to perfom <see cref="Check(T)"/> and <see cref="NotCheck(T)"/></typeparam>
    [Serializable]
    public class ConditionNot<T> : Condition<T>
    {
        private static bool doDebug => getClassDoDebug("ConditionNot");
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

        public IConditional<T> Condition;

        public ConditionNot()
            : base()
        {
            Condition = null;
        }
        public ConditionNot(IConditional<T> Condition = null)
            : this()
        {
            this.Condition = Condition;
        }
        public ConditionNot(ConditionNot<T> Source)
            : this(Source?.Condition)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(Check));
            Debug.Entry(4, $"[?] {nameof(ConditionNot<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool check = base.Check(Subject)
                && (Condition == null || IfSubjectNull)
                && Condition.NotCheck(Subject);

            Debug.LoopItem(4, $"{nameof(ConditionNot<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {check}",
                Good: check, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return check;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(NotCheck));
            Debug.Entry(4, $"[?] {nameof(ConditionNot<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            bool notCheck = base.NotCheck(Subject)
                && (Condition == null || !IfSubjectNull)
                && Condition.Check(Subject);

            Debug.LoopItem(4, $"{nameof(ConditionNot<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {notCheck}",
                Good: notCheck, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return notCheck;
        }
    }
}
