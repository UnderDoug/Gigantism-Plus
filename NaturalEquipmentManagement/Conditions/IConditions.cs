using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using XRL.Collections;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class IConditions<T> : ICondition<T>
        , IList<ICondition<T>>
        , IList
        , IReadOnlyList<ICondition<T>>
        , IEnumerable<ICondition<T>>
        , IEnumerable
        , IReadOnlyCollection<ICondition<T>>
        , ICollection<ICondition<T>>
        , ICollection
        , IComposite
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

        protected ICondition<T>[] Items = Array.Empty<ICondition<T>>();

        protected int Size;

        protected int Length;

        protected int Variant;

        public bool IsReadOnly => false;

        public int Capacity => Size;

        public int Count => Length;

        public int Version => Variant;

        protected virtual int DefaultCapacity => 4;

        public bool WantFieldReflection => false;

        bool IList.IsFixedSize => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public List<ICondition<T>> Conditions => Items as List<ICondition<T>>;

        public IConditions()
            : base()
        {
        }

        public IConditions(int Capacity)
            : base()
        {
            EnsureCapacity(Capacity);
        }

        public IConditions(List<ICondition<T>> Conditions)
            : this()
        {
            Items = Conditions.ToArray();
        }
        public IConditions(IReadOnlyList<ICondition<T>> List)
            : this()
        {
            if (List != null)
            {
                int count = List.Count;
                EnsureCapacity(count);
                for (int i = 0; i < count; i++)
                {
                    Add((ICondition<T>)List[i]);
                }
            }
        }
        public virtual ICondition<T> this[int Index]
        {
            get
            {
                if ((uint)Index >= (uint)Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return Items[Index];
            }
            set
            {
                if ((uint)Index >= (uint)Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                Items[Index] = value;
                Variant++;
            }
        }
        ICondition<T> IReadOnlyList<ICondition<T>>.this[int Index]
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
        ICondition<T> IList<ICondition<T>>.this[int Index]
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
                    Items[Index] = (ICondition<T>)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
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
                    Items[Index] = (ICondition<T>)value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        public void EnsureCapacity(int Capacity)
        {
            if (Size < Capacity)
            {
                Resize(Capacity);
            }
        }
        protected void Resize(int Capacity)
        {
            if (Capacity == 0)
            {
                Capacity = DefaultCapacity;
            }
            ICondition<T>[] array = new ICondition<T>[Capacity];
            Array.Copy(Items, 0, array, 0, Length);
            Items = array;
            Size = Capacity;
        }

        /// <summary>Retrieve a value by reference.</summary>
        public ref ICondition<T> GetReference(int Index)
        {
            if ((uint)Index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return ref Items[Index];
        }

        /// <remarks>
        ///     See See <seealso cref="List{T}.Add(T)" /> for implementation.
        /// </remarks>
        public virtual void Add(ICondition<T> Condition)
        {
            Variant++;
            ICondition<T>[] conditions = Items;
            int size = Size;
            if ((uint)size < (uint)conditions.Length)
            {
                Size = size + 1;
                conditions[size] = Condition;
            }
            else
            {
                AddWithResize(Condition);
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(ICondition<T> Condition)
        {
            int size = Size;
            EnsureCapacity(size + 1);
            Size = size + 1;
            Items[size] = Condition;
        }
        /// <remarks>
        ///     See <seealso cref="List{T}.AddRange(IEnumerable{T})" /> for implementation.
        /// </remarks>
        public void AddRange(IEnumerable<ICondition<T>> Conditions)
        {
            this.Conditions.AddRange(Conditions);
        }
        public void AddRange(IReadOnlyList<ICondition<T>> Conditions)
        {
            int count = Conditions.Count;
            EnsureCapacity(Length + count);
            for (int i = 0; i < count; i++)
            {
                Add(Conditions[i]);
            }
        }
        public void AddRange(IReadOnlyCollection<ICondition<T>> Conditions)
        {
            if (Conditions is IReadOnlyList<ICondition<T>> conditions)
            {
                AddRange(conditions);
                return;
            }
            EnsureCapacity(Length + Conditions.Count);
            foreach (ICondition<T> Item in Conditions)
            {
                Add(Item);
            }
        }
        public void AddRange(ReadOnlySpan<ICondition<T>> Conditions)
        {
            EnsureCapacity(Length + Conditions.Length);
            Conditions.CopyTo(Items.AsSpan(Length, Conditions.Length));
            Length += Conditions.Length;
            Variant++;
        }
        public Span<ICondition<T>> FillSpan(int Length)
        {
            EnsureCapacity(this.Length + Length);
            Span<ICondition<T>> result = new (Items, this.Length, Length);
            this.Length += Length;
            Variant++;
            return result;
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
        public virtual int RemoveAll(Predicate<ICondition<T>> Match)
        {
            if (Match == null)
            {
                throw new ArgumentNullException(nameof(Match));
            }

            int i;
            for (i = 0; i < Size && !Match(Items[i]); i++)
            {
            }

            if (i >= Size)
            {
                return 0;
            }

            int j = i + 1;
            while (j < Size)
            {
                for (; j < Size && Match(Items[j]); j++)
                {
                }

                if (j < Size)
                {
                    Items[i++] = Items[j++];
                }
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(Items, i, Size - i);
            }

            int result = Size - i;
            Size = i;
            Variant++;
            return result;
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
        public virtual void Insert(int Index, ICondition<T> Condition)
        {
            if ((uint)Index > (uint)Size)
            {
                throw new ArgumentOutOfRangeException(nameof(Index));
            }

            if (Size == Items.Length)
            {
                EnsureCapacity(Size + 1);
            }

            if (Index < Size)
            {
                Array.Copy(Items, Index, Items, Index + 1, Size - Index);
            }

            Items[Index] = Condition;
            Size++;
            Variant++;
        }
        public virtual void RemoveAt(int Index)
        {
            if (Index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(Index));
            }
            Length--;
            if (Index < Length)
            {
                Array.Copy(Items, Index + 1, Items, Index, Length - Index);
            }
            Items[Length] = default(ICondition<T>);
            Variant++;
        }
        public bool Contains(ICondition<T> Condition)
        {
            if (Size != 0)
            {
                return IndexOf(Condition) != -1;
            }
            return false;
        }
        public int FindIndex(int StartIndex, int Count, Predicate<ICondition<T>> Match)
        {
            if ((uint)StartIndex > (uint)Size)
            {
                throw new ArgumentOutOfRangeException(nameof(StartIndex));
            }

            if (Count < 0 || StartIndex > Size - Count)
            {
                throw new ArgumentOutOfRangeException(nameof(Count));
            }

            if (Match == null)
            {
                throw new ArgumentNullException(nameof(Match));
            }

            int num = StartIndex + Count;
            for (int i = StartIndex; i < num; i++)
            {
                if (Match(Items[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        public int FindIndex(int StartIndex, Predicate<ICondition<T>> Match)
        {
            return FindIndex(StartIndex, Size - StartIndex, Match);
        }
        public int FindIndex(Predicate<ICondition<T>> Match)
        {
            return FindIndex(0, Size, Match);
        }
        public bool Exists(Predicate<ICondition<T>> Match)
        {
            return FindIndex(Match) != -1;
        }
        public void CopyTo(int Index, ICondition<T>[] Array, int ArrayIndex, int Count)
        {
            if (Size - Index < Count)
            {
                throw new ArgumentException(
                    "The number of elements from Index to the end of the source List`1" +
                    " is greater than the available space from ArrayIndex to the end of the destination Array");
            }
            System.Array.Copy(Items, Index, Array, ArrayIndex, Count);
        }
        public void CopyTo(ICondition<T>[] Array, int ArrayIndex)
        {
            CopyTo(Array, ArrayIndex);
        }
        public void CopyTo(ICondition<T>[] Array)
        {
            CopyTo(Array);
        }
        public void CopyTo(Array Array, int Index)
        {
            if (Array != null && Array.Rank != 1)
            {
                throw new ArgumentException("Multidimensional arrays are not supported");
            }

            try
            {
                Array.Copy(Items, 0, Array, Index, Size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArrayTypeMismatchException("Invalid Array type");
            }
        }
        public int IndexOf(ICondition<T> Condition)
        {
            return Array.IndexOf(Items, Condition, 0, Length);
        }

        public virtual void Write(SerializationWriter Writer)
        {
            Writer.WriteOptimized(Length);
            for (int i = 0; i < Length; i++)
            {
                Writer.WriteObject(Items[i]);
            }
        }
        public virtual void Read(SerializationReader Reader)
        {
            Size = (Length = Reader.ReadOptimizedInt32());
            Items = new ICondition<T>[Size];
            for (int i = 0; i < Length; i++)
            {
                Items[i] = (ICondition<T>)Reader.ReadObject();
            }
        }

        public ICondition<T>[] ToArray()
        {
            ICondition<T>[] array = new ICondition<T>[Length];
            Array.Copy(Items, 0, array, 0, Length);
            return array;
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
                throw new ArgumentOutOfRangeException("Start");
            }
            return new ReadOnlySpan<ICondition<T>>(Items, Start, Length - Start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<ICondition<T>> AsSpan(int Start, int Length)
        {
            if ((uint)(Start + Length) > (uint)this.Length)
            {
                throw new ArgumentOutOfRangeException("Length");
            }
            return new ReadOnlySpan<ICondition<T>>(Items, Start, Length);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator<ICondition<T>> IEnumerable<ICondition<T>>.GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        ///     Returns an <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the Conditions <see cref="List{ICondition{T}}" />.
        /// </summary>
        /// 
        /// <param name="Subject">An instance of the class on which <see cref="ICondition{T}.Check(T)" /> is performed.</param>
        /// 
        /// <returns>
        ///     An <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the Conditions <see cref="List{ICondition{T}}" />.
        /// </returns>
        public IEnumerable<bool> Results(T Subject)
        {
            if (!Conditions.IsNullOrEmpty())
            {
                foreach (ICondition<T> condition in Conditions)
                {
                    yield return condition.Check(Subject);
                }
            }
            yield break;
        }

        /// <summary>
        ///     Returns an <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the Conditions <see cref="List{ICondition{T}}" />.
        /// </summary>
        /// 
        /// <param name="Subject">An instance of the class on which <see cref="ICondition{T}.Check(T)" /> is performed.</param>
        /// 
        /// <returns>
        ///     An <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the Conditions <see cref="List{ICondition{T}}" />.
        /// </returns>
        public IEnumerable<bool> NotResults(T Subject)
        {
            if (!Conditions.IsNullOrEmpty())
            {
                foreach (ICondition<T> condition in Conditions)
                {
                    yield return condition.NotCheck(Subject);
                }
            }
            yield break;
        }

        int IList.Add(object Value)
        {
            Add((ICondition<T>)Value);
            return Length - 1;
        }

        bool IList.Contains(object Value)
        {
            if (Value is ICondition<T> item)
            {
                return Contains(item);
            }
            return false;
        }
        int IList.IndexOf(object Value)
        {
            if (Value is ICondition<T> condition)
            {
                return IndexOf(condition);
            }
            return -1;
        }
        void IList.Insert(int Index, object Value)
        {
            Insert(Index, (ICondition<T>)Value);
        }
        void IList.Remove(object Value)
        {
            if (Value is ICondition<T> condition)
            {
                Remove(condition);
            }
        }
    }
}
