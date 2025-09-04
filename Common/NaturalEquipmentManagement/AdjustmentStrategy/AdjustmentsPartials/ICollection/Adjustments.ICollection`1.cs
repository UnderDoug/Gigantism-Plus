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
            bool doConditionsDebug = Options.doConditionsDebug;
            Options.doConditionsDebug = getDoDebug(nameof(Add));

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
                        Variant++;
                        break;
                    }
                }
            }
            if (higherPriorityAdjustment == null)
            {
                EnsureCapacity(Length + 1);
                Items[Length++] = Adjustment;
                Variant++;
            }
            Options.doConditionsDebug = doConditionsDebug;
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
