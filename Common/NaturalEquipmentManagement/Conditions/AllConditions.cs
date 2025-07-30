using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AllConditions<T> : IConditions<T>
        where T : class, new()
    {
        public AllConditions()
            : base()
        {
        }
        public AllConditions(IConditions<T> Source)
            : base(Source)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {nameof(AllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    Debug.LoopItem(4, $"{nameof(result)}: {result}", Indent: indent + 1, Toggle: true);
                    if (!result)
                    {
                        Debug.Entry(4, $"x {nameof(AllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
                        Debug.LastIndent = indent;
                        return false;
                    }
                }
            }
            Debug.Entry(4, $"x {nameof(AllConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject) *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                return !Check(Subject);
            }
            return !FalseIfSubjectNull;
        }
    }
}
