using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AnyConditions<T> : IConditions<T>
        where T : class, new()
    {
        public AnyConditions()
            : base()
        {
        }
        public AnyConditions(IConditions<T> Conditions)
            : base(Conditions)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    Debug.LoopItem(4, $"{nameof(result)}: {result}", Indent: indent + 1, Toggle: true);
                    if (result)
                    {
                        Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                        Debug.LastIndent = indent;
                        return true;
                    }
                }
                Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return false;
            }
            Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    Debug.LoopItem(4, $"{nameof(result)}: {result}", Indent: indent + 1, Toggle: true);
                    if (result)
                    {
                        Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                        Debug.LastIndent = indent;
                        return false;
                    }
                }
                Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return true;
            }
            Debug.Entry(4, $"x {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }
    }
}
