using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using XRL;

using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, ICollection<IAdjustment>
    {
        public bool IsReadOnly => false;

        public virtual void Add(IAdjustment Adjustment)
        {
            IAdjustment higherPriorityAdjustment = null;
            if (!Items.IsNullOrEmpty())
            {
                for (int i = 0; i < Size; i++)
                {
                    if (Items[i] == null)
                    {
                        continue;
                    }
                    if (Adjustment.TryGetHigherPriorityAdjustment(Items[i], out higherPriorityAdjustment))
                    {
                        Items[i] = higherPriorityAdjustment;
                        break;
                    }
                }
            }
            if (higherPriorityAdjustment == null)
            {
                if (Length == Size)
                {
                    Resize(Length * 2);
                }
                Items[Length++] = Adjustment;
                Variant++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Variant++;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<IAdjustment>())
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

        public bool Contains(IAdjustment Adjustment)
        {
            if (Size != 0)
            {
                return IndexOf(Adjustment) != -1;
            }
            return false;
        }

        public void CopyTo(IAdjustment[] Array, int ArrayIndex)
        {
            CopyTo(Array, ArrayIndex);
        }

        public virtual bool Remove(IAdjustment Adjustment)
        {
            int index = IndexOf(Adjustment);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }
    }
}
