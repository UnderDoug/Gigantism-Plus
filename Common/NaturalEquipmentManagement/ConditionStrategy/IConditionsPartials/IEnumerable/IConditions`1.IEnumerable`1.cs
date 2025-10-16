using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IEnumerable<IConditional<T>>
    {
        [Serializable]
        public struct Enumerator : IEnumerator<IConditional<T>>, IEnumerator, IDisposable
        {
            private IConditions<T> List;

            private int Index;

            private int Variant;

            private IConditional<T> Item;

            public IConditional<T> Current => Item;

            object IEnumerator.Current => Item;

            public Enumerator(IConditions<T> List)
            {
                this.List = List;
                Index = 0;
                Variant = List.Variant;
                Item = (IConditional<T>)default;
            }

            public bool MoveNext()
            {
                if (List.Variant != Variant)
                {
                    throw new InvalidOperationException();
                }
                if (Index >= List.Length)
                {
                    Item = (IConditional<T>)default;
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
                Item = (IConditional<T>)default;
            }

            public void Dispose()
            {
            }
        }

        public IConditions(IEnumerable<IConditional<T>> Conditions)
            : this(Conditions as IReadOnlyList<IConditional<T>>)
        {
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<IConditional<T>> IEnumerable<IConditional<T>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public static implicit operator ReadOnlySpan<IConditional<T>>(IConditions<T> Conditions)
        {
            return Conditions.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IConditional<T>> AsSpan()
        {
            return new ReadOnlySpan<IConditional<T>>(Items, 0, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IConditional<T>> AsSpan(int Start)
        {
            if ((uint)Start > (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Start));
            }
            return new ReadOnlySpan<IConditional<T>>(Items, Start, Length - Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<IConditional<T>> AsSpan(int Start, int Length)
        {
            if ((uint)(Start + Length) > (uint)this.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Length));
            }
            return new ReadOnlySpan<IConditional<T>>(Items, Start, Length);
        }
        public Span<IConditional<T>> FillSpan(int Length)
        {
            EnsureCapacity(this.Length + Length);
            Span<IConditional<T>> result = new(Items, this.Length, Length);
            this.Length += Length;
            Variant++;
            return result;
        }
    }
}
