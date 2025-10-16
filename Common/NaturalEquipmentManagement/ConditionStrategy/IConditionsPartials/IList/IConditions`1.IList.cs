using System;
using System.Collections;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IList
    {
        bool IList.IsFixedSize => false;

        object IList.this[int Index]
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
                    Items[Index] = (Condition<T>)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        int IList.Add(object Value)
        {
            Add((IConditional<T>)Value);
            return Length - 1;
        }
        bool IList.Contains(object Value)
        {
            if (Value is IConditional<T> item)
            {
                return Contains(item);
            }
            return false;
        }
        int IList.IndexOf(object Value)
        {
            if (Value is IConditional<T> condition)
            {
                return IndexOf(condition);
            }
            return -1;
        }
        void IList.Insert(int Index, object Value)
        {
            Insert(Index, (IConditional<T>)Value);
        }
        void IList.Remove(object Value)
        {
            if (Value is IConditional<T> condition)
            {
                Remove(condition);
            }
        }
    }
}
