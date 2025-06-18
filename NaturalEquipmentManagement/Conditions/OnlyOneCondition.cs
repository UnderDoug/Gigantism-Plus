using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class OnlyOneCondition<T> : IConditions<T>
        where T : class
    {
        public OnlyOneCondition(IConditions<T> Source)
            : base(Source.Conditions)
        {
        }

        public override bool Check(T Parameter)
        {
            bool OneCondition = false;
            foreach (bool result in Results(Parameter))
            {
                if (result)
                {
                    if (!OneCondition)
                    {
                        OneCondition = true;
                    }
                    else
                    {
                        OneCondition = false;
                        break;
                    }
                }
            }
            return OneCondition;
        }
    }
}
