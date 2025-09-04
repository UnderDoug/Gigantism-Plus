using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, ICollection<ICondition<T>>
        where T : class, new()
    {
        public bool IsReadOnly => false;

        public virtual void Add(ICondition<T> Condition)
        {
            EnsureCapacity(Length + 1);
            Items[Length++] = Condition;
            Variant++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Variant++;
            if (Length > 0)
            {
                Array.Clear(Items, 0, Length);
                Size = 0;
                Length = 0;
                Variant = 0;
            }
        }

        public bool Contains(ICondition<T> Condition)
        {
            if (Size != 0)
            {
                return IndexOf(Condition) != -1;
            }
            return false;
        }

        public void CopyTo(ICondition<T>[] Array, int ArrayIndex)
        {
            CopyTo(Array, ArrayIndex);
        }

        public virtual bool Remove(ICondition<T> Condition)
        {
            int index = IndexOf(Condition);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }
    }
}
