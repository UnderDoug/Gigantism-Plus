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
            : base(Source.Conditions)
        {
        }

        public override bool Check(T Subject)
        {
            if (!Results(Subject).IsNullOrEmpty())
            {
                foreach (bool result in Results(Subject))
                {
                    if (!result)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool NotCheck(T Subject)
        {
            if (!Results(Subject).IsNullOrEmpty())
            {
                return !Check(Subject);
            }
            return true;
        }
    }
}
