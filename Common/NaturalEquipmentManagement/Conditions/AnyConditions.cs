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
            if (!Results(Subject).IsNullOrEmpty())
            {
                foreach (bool result in Results(Subject))
                {
                    if (result)
                    {
                        return true;
                    }
                }
                return false;
            }
            return !FalseIfSubjectNull;
        }

        public override bool NotCheck(T Subject)
        {
            if (!Results(Subject).IsNullOrEmpty())
            {
                return !Check(Subject);
            }
            return !FalseIfSubjectNull;
        }
    }
}
