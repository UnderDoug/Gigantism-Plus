using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class IConditions<T> : ICondition<T> 
        where T : class
    {
        public List<ICondition<T>> Conditions = new();

        public IConditions(List<ICondition<T>> Conditions)
            : base()
        {
            this.Conditions = Conditions;
        }

        public virtual IEnumerable<bool> Results(T Parameter)
        {
            if (!Conditions.IsNullOrEmpty())
            {
                foreach (ICondition<T> condition in Conditions)
                {
                    yield return condition.Check(Parameter);
                }
            }
            yield break;
        }
    }
}
