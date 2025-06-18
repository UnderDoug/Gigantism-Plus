using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotAnyConditions<T> : AnyConditions<T>
        where T : class
    {
        public NotAnyConditions(IConditions<T> Source)
            : base(Source.Conditions as IConditions<T>)
        {
        }

        public override bool Check(T Parameter)
        {
            return !base.Check(Parameter);
        }
    }
}
