using System;
using System.Collections;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IList
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
                    Items[Index] = (IAdjustment)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        int IList.Add(object Value)
        {
            Add((IAdjustment)Value);
            return Length - 1;
        }
        bool IList.Contains(object Value)
        {
            if (Value is IAdjustment item)
            {
                return Contains(item);
            }
            return false;
        }
        int IList.IndexOf(object Value)
        {
            if (Value is IAdjustment condition)
            {
                return IndexOf(condition);
            }
            return -1;
        }
        void IList.Insert(int Index, object Value)
        {
            Insert(Index, (IAdjustment)Value);
        }
        void IList.Remove(object Value)
        {
            if (Value is IAdjustment condition)
            {
                Remove(condition);
            }
        }
    }
}
