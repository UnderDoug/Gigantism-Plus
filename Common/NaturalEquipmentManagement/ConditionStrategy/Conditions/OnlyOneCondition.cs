using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class OnlyOneCondition<T> : IConditions<T>
        where T : class, new()
    {
        public OnlyOneCondition()
            : base()
        {
        }
        public OnlyOneCondition(IConditions<T> Source)
            : base(Source)
        {
        }

        public override bool Check(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(OnlyOneCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                bool oneCondition = false;
                foreach (bool result in results)
                {
                    if (result)
                    {
                        if (oneCondition)
                        {
                            oneCondition = false;
                            break;
                        }
                        else
                        {
                            oneCondition = true;
                        }
                    }
                }
                Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {oneCondition}",
                    Good: oneCondition, Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return oneCondition;
            }
            Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"[?] {nameof(OnlyOneCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                bool oneCondition = false;
                foreach (bool result in results)
                {
                    if (result)
                    {
                        if (oneCondition)
                        {
                            oneCondition = false;
                            break;
                        }
                        else
                        {
                            oneCondition = true;
                        }
                    }
                }
                Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {!oneCondition}",
                    Good: !oneCondition, Indent: indent + 1, Toggle: true);
                Debug.LastIndent = indent;
                return !oneCondition;
            }
            Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }
    }
}
