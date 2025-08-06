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
            if (Length == Size)
            {
                Resize(Length * 2);
            }
            Items[Length++] = Condition;
            Variant++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Variant++;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<ICondition<T>>())
            {
                int size = Size;
                Size = 0;
                if (size > 0)
                {
                    Array.Clear(Items, 0, size);
                }
            }
            else
            {
                Size = 0;
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
