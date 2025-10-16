using System;
using System.Collections.Generic;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IList<IConditional<T>>
    {
        IConditional<T> IList<IConditional<T>>.this[int Index]
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
                    Items[Index] = (IConditional<T>)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int IndexOf(IConditional<T> Condition)
        {
            return Array.IndexOf(Items, Condition, 0, Length);
        }

        public virtual void Insert(int Index, IConditional<T> Condition)
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

            Items[Index] = Condition;
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
            Items[Length] = (IConditional<T>)default;
            Variant++;
        }
    }
}
