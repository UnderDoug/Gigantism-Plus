using System.Collections;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, IEnumerable
        where T : class, new()
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
