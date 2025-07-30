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
            Debug.Entry(4, $"* {nameof(NotAnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                Debug.Entry(4, $"x {nameof(NotAnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return base.NotCheck(Subject);
            }
            Debug.Entry(4, $"x {nameof(NotAnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            List<bool> results = new(Results(Subject));
            if (results.IsNullOrEmpty())
            {
                return base.Check(Subject);
            }
            return !FalseIfSubjectNull;
        }
    }
}
