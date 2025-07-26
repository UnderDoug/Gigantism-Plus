using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, IEnumerable<ICondition<T>>
        where T : class, new()
    {
        [Serializable]
        public struct Enumerator : IEnumerator<ICondition<T>>, IEnumerator, IDisposable
        {
            private IConditions<T> List;

            private int Index;

            private int Variant;

            private ICondition<T> Item;

            public ICondition<T> Current => Item;

            object IEnumerator.Current => Item;

            public Enumerator(IConditions<T> List)
            {
                this.List = List;
                Index = 0;
                Variant = List.Variant;
                Item = default(ICondition<T>);
            }

            public bool MoveNext()
            {
                if (List.Variant != Variant)
                {
                    throw new InvalidOperationException();
                }
                if (Index >= List.Length)
                {
                    Item = default(ICondition<T>);
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
                Item = default(ICondition<T>);
            }

            public void Dispose()
            {
            }
        }

        public IConditions(IEnumerable<ICondition<T>> Conditions)
            : this(Conditions as IReadOnlyList<ICondition<T>>)
        {
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<ICondition<T>> IEnumerable<ICondition<T>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public static implicit operator ReadOnlySpan<ICondition<T>>(IConditions<T> Conditions)
        {
            return Conditions.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ICondition<T>> AsSpan()
        {
            return new ReadOnlySpan<ICondition<T>>(Items, 0, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ICondition<T>> AsSpan(int Start)
        {
            if ((uint)Start > (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Start));
            }
            return new ReadOnlySpan<ICondition<T>>(Items, Start, Length - Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ICondition<T>> AsSpan(int Start, int Length)
        {
            if ((uint)(Start + Length) > (uint)this.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Length));
            }
            return new ReadOnlySpan<ICondition<T>>(Items, Start, Length);
        }
        public Span<ICondition<T>> FillSpan(int Length)
        {
            EnsureCapacity(this.Length + Length);
            Span<ICondition<T>> result = new(Items, this.Length, Length);
            this.Length += Length;
            Variant++;
            return result;
        }
    }
}
