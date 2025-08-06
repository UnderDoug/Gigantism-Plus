using System;
using System.Collections;
using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, ICollection
        where T : class, new()
    {
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public IConditions(ICollection<ICondition<T>> Conditions)
            : this(Conditions as IReadOnlyList<ICondition<T>>)
        {
        }

        public void CopyTo(Array Array, int Index)
        {
            if (Array != null && Array.Rank != 1)
            {
                throw new ArgumentException("Multidimensional arrays are not supported");
            }

            try
            {
                Array.Copy(Items, 0, Array, Index, Size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArrayTypeMismatchException("Invalid Array type");
            }
        }
    }
}
