using System;
using System.Collections;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, IList
        where T : class, new()
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
                    Items[Index] = (ICondition<T>)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        int IList.Add(object Value)
        {
            Add((ICondition<T>)Value);
            return Length - 1;
        }
        bool IList.Contains(object Value)
        {
            if (Value is ICondition<T> item)
            {
                return Contains(item);
            }
            return false;
        }
        int IList.IndexOf(object Value)
        {
            if (Value is ICondition<T> condition)
            {
                return IndexOf(condition);
            }
            return -1;
        }
        void IList.Insert(int Index, object Value)
        {
            Insert(Index, (ICondition<T>)Value);
        }
        void IList.Remove(object Value)
        {
            if (Value is ICondition<T> condition)
            {
                Remove(condition);
            }
        }
    }
}
