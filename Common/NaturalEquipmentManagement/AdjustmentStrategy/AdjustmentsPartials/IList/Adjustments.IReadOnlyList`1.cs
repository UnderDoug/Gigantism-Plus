using System;
using System.Collections.Generic;
using System.Linq;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IReadOnlyList<IAdjustment>
    {
        public Adjustments(int Capacity)
            : base()
        {
            EnsureCapacity(Capacity);
        }

        public Adjustments(IReadOnlyList<IAdjustment> Adjustments)
            : this()
        {
            if (Adjustments != null)
            {
                int count = Adjustments.Count;
                EnsureCapacity(count);
                for (int i = 0; i < count; i++)
                {
                    Add((IAdjustment)Adjustments.ElementAt(i)); // cast is here so it throws errors if I change something in the inheritance and need to update it
                }
            }
        }

        IAdjustment IReadOnlyList<IAdjustment>.this[int Index]
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
