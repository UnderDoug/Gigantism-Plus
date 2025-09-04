using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IEnumerable<IAdjustment>
    {
        [Serializable]
        public struct Enumerator : IEnumerator<IAdjustment>, IEnumerator, IDisposable
        {
            private Adjustments List;

            private int Index;

            private int Variant;

            private IAdjustment Item;

            public IAdjustment Current => Item;

            object IEnumerator.Current => Item;

            public Enumerator(Adjustments List)
            {
                this.List = List;
                Index = 0;
                Variant = List.Variant;
                Item = default(IAdjustment);
            }

            public bool MoveNext()
            {
                if (List.Variant != Variant)
                {
                    throw new InvalidOperationException();
                }
                if (Index >= List.Length)
                {
                    Item = default(IAdjustment);
                    return false;
                }
                Item = List.Items[Index++];
                return true;
            }

            public void Reset()
            {
                if (List.Variant != Variant)
                {
                    throw new InvalidOperationException();
                }
                Index = 0;
                Item = default(IAdjustment);
            }

            public void Dispose()
            {
            }
        }

        public Adjustments(IEnumerable<IAdjustment> Adjustments)
            : this(Adjustments as IReadOnlyList<IAdjustment>)
        {
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<IAdjustment> IEnumerable<IAdjustment>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public static implicit operator ReadOnlySpan<IAdjustment>(Adjustments Adjustments)
        {
            return Adjustments.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IAdjustment> AsSpan()
        {
            return new ReadOnlySpan<IAdjustment>(Items, 0, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IAdjustment> AsSpan(int Start)
        {
            if ((uint)Start > (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Start));
            }
            return new ReadOnlySpan<IAdjustment>(Items, Start, Length - Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IAdjustment> AsSpan(int Start, int Length)
        {
            if ((uint)(Start + Length) > (uint)this.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Length));
            }
            return new ReadOnlySpan<IAdjustment>(Items, Start, Length);
        }
        public Span<IAdjustment> FillSpan(int Length)
        {
            EnsureCapacity(this.Length + Length);
            Span<IAdjustment> result = new(Items, this.Length, Length);
            this.Length += Length;
            Variant++;
            return result;
        }
    }
}
