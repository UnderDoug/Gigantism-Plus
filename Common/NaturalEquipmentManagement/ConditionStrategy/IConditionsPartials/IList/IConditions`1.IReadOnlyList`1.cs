using System;
using System.Collections.Generic;
using System.Linq;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IReadOnlyList<IConditional<T>>
    {
        public IConditions(int Capacity)
            : base()
        {
            EnsureCapacity(Capacity);
        }

        public IConditions(IReadOnlyList<IConditional<T>> Conditions)
            : this()
        {
            if (Conditions != null)
            {
                int count = Conditions.Count;
                EnsureCapacity(count);
                for (int i = 0; i < count; i++)
                {
                    Add((IConditional<T>)Conditions.ElementAt(i)); // cast is here so it throws errors if I change something in the inheritance and need to update it
                }
            }
        }

        IConditional<T> IReadOnlyList<IConditional<T>>.this[int Index]
        {
            get
            {
                if ((uint)Index >= (uint)Size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return Items[Index];
            }
        }
    }
}
