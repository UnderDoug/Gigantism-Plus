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
            Debug.Entry(4, $"[?] {nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    if (result)
                    {
                        Debug.CheckYeh(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {true}", Indent: indent + 1, Toggle: true);
                        Debug.LastIndent = indent;
                        return true;
                    }
                }
                Debug.CheckNah(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {false}", Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return false;
            }
            Debug.LoopItem(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    if (result)
                    {
                        Debug.CheckNah(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {false}", Indent: indent + 1, Toggle: true);
                        Debug.LastIndent = indent;
                        return false;
                    }
                }
                Debug.CheckYeh(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {true}", Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return true;
            }
            Debug.LoopItem(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }
    }
}
