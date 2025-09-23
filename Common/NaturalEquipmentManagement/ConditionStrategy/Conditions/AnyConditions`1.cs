using System;
using System.Collections.Generic;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AnyConditions<T> : IConditions<T>
        where T : class, new()
    {
        private static bool doDebug => getClassDoDebug("AnyConditions");
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
            bool doDebug = getDoDebug(nameof(Check));
            Debug.Entry(4, $"[?] {nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    if (result)
                    {
                        Debug.CheckYeh(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {true}",
                            Indent: indent + 1, Toggle: doDebug);
                        Debug.LastIndent = indent;
                        return true;
                    }
                }
                Debug.CheckNah(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {false}",
                    Indent: indent + 1, Toggle: doDebug);
                Debug.LastIndent = indent;
                return false;
            }
            Debug.LoopItem(4, $"{nameof(AnyConditions<T>)}.{nameof(Check)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(NotCheck));
            Debug.Entry(4, $"[?] {nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: doDebug);

            List<bool> results = new(Results(Subject));
            if (!results.IsNullOrEmpty())
            {
                foreach (bool result in results)
                {
                    if (result)
                    {
                        Debug.CheckNah(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {false}",
                            Indent: indent + 1, Toggle: doDebug);
                        Debug.LastIndent = indent;
                        return false;
                    }
                }
                Debug.CheckYeh(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {true}",
                    Indent: indent + 1, Toggle: doDebug);
                Debug.LastIndent = indent;
                return true;
            }
            Debug.LoopItem(4, $"{nameof(AnyConditions<T>)}.{nameof(NotCheck)}({typeof(T).Name} Subject): {!FalseIfSubjectNull}",
                Good: !FalseIfSubjectNull, Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return !FalseIfSubjectNull;
        }
    }
}
