using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AllConditions<T> : IConditions<T>
        where T : class
    {
        public AllConditions(IConditions<T> Source)
            : base(Source.Conditions)
        {
        }

        public override bool Check(T Parameter)
        {
            foreach (bool result in Results(Parameter))
            {
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
