using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAllConditions<T> : AllConditions<T>
        where T : class
    {
        public NotAllConditions(IConditions<T> Source)
            : base(Source.Conditions as IConditions<T>)
        {
        }

        public override bool Check(T Parameter)
        {
            return !base.Check(Parameter);
        }
    }
}
