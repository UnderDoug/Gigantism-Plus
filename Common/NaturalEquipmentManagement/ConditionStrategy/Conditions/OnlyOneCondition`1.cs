using System;
using System.Collections.Generic;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class OnlyOneCondition<T> : IConditions<T>
        where T : class, new()
    {
        private static bool doDebug => getClassDoDebug("OnlyOneCondition");
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
            bool doDebug = getDoDebug(nameof(Check));
            Debug.Entry(4, $"[?] {nameof(OnlyOneCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

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
                    Good: oneCondition, Indent: indent + 1, Toggle: doDebug);
                Debug.LastIndent = indent;
                return oneCondition;
            }
            Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(NotCheck));
            Debug.Entry(4, $"[?] {nameof(OnlyOneCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

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
                    Good: !oneCondition, Indent: indent + 1, Toggle: doDebug);
                Debug.LastIndent = indent;
                return !oneCondition;
            }
            Debug.LoopItem(4, $"{nameof(OnlyOneCondition<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }
    }
}
