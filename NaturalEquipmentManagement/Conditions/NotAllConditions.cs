using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAllConditions<T> : AllConditions<T>
        where T : class, new()
    {
        public NotAllConditions()
            : base()
        {
        }
        public NotAllConditions(IConditions<T> Source)
            : base(Source.Conditions as IConditions<T>)
        {
        }

        public override bool Check(T Subject)
        {
            if (!Results(Subject).IsNullOrEmpty())
            {
                return base.NotCheck(Subject);
            }
            return true;
        }

        public override bool NotCheck(T Subject)
        {
            if (!Results(Subject).IsNullOrEmpty())
            {
                return base.Check(Subject);
            }
            return true;
        }
    }
}
