using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AnyConditions<T> : IConditions<T>
        where T : class
    {
        public AnyConditions(IConditions<T> Source)
            : base(Source.Conditions)
        {
        }

        public override bool Check(T Parameter)
        {
            foreach (bool result in Results(Parameter))
            {
                if (result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
