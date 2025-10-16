using System.Collections;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
