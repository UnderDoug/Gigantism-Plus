using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, IReadOnlyCollection<ICondition<T>>
        where T : class, new()
    {
        public int Count => Length;

        public IConditions(IReadOnlyCollection<ICondition<T>> Conditions)
            : this(Conditions as IReadOnlyList<ICondition<T>>)
        {
        }
    }
}
