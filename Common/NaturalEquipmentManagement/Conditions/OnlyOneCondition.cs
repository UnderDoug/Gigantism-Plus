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
            if (Subject == null)
            {
                bool OneCondition = false;
                foreach (bool result in Results(Subject))
                {
                    if (result)
                    {
                        if (OneCondition)
                        {
                            OneCondition = false;
                            break;
                        }
                        else
                        {
                            OneCondition = true;
                        }
                    }
                }
                return OneCondition;
            }
            return !FalseIfSubjectNull;
        }
    }
}
