using System;
using System.Collections.Generic;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IList<IAdjustment>
    {
        IAdjustment IList<IAdjustment>.this[int Index]
        {
            get
            {
                if ((uint)Index < (uint)Length)
                {
                    return Items[Index];
                }
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                if ((uint)Index < (uint)Length)
                {
                    Variant++;
                    Items[Index] = (IAdjustment)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int IndexOf(IAdjustment Adjustment)
        {
            return Array.IndexOf(Items, Adjustment, 0, Length);
        }

        public virtual void Insert(int Index, IAdjustment Adjustment)
        {
            if ((uint)Index > (uint)Size)
            {
                throw new ArgumentOutOfRangeException(nameof(Index));
            }

            if (Size == Items.Length)
            {
                EnsureCapacity(Size + 1);
            }

            if (Index < Size)
            {
                Array.Copy(Items, Index, Items, Index + 1, Size - Index);
            }

            Items[Index] = Adjustment;
            Size++;
            Variant++;
        }

        public virtual void RemoveAt(int Index)
        {
            if (Index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Index));
            }
            Length--;
            if (Index < Length)
            {
                Array.Copy(Items, Index + 1, Items, Index, Length - Index);
            }
            Items[Length] = default(IAdjustment);
            Variant++;
        }
    }
}
