using System;
using System.Collections;
using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, ICollection
    {
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public IConditions(ICollection<IConditional<T>> Conditions)
            : this(Conditions as IReadOnlyList<IConditional<T>>)
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
