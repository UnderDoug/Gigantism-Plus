using System;
using System.Collections;
using System.Collections.Generic;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, ICollection
    {
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public Adjustments(ICollection<IAdjustment> Conditions)
            : this(Conditions as IReadOnlyList<IAdjustment>)
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
