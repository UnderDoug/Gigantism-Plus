using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IReadOnlyCollection<IConditional<T>>
    {
        public int Count => Length;

        public IConditions(IReadOnlyCollection<IConditional<T>> Conditions)
            : this(Conditions as IReadOnlyList<IConditional<T>>)
        {
        }
    }
}
