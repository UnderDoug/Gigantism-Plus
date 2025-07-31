using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using XRL.Collections;
using XRL.World;

using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract partial class IConditions<T> : ICondition<T>
        where T : class, new()
    {
        protected ICondition<T>[] Items = Array.Empty<ICondition<T>>();

        protected int Size;

        protected int Length;

        protected int Variant;

        public int Capacity => Size;

        public int Version => Variant;

        protected virtual int DefaultCapacity => 4;

        public bool WantFieldReflection => false;

        public IConditions()
            : base()
        {
        }

        public IConditions(List<ICondition<T>> Conditions)
            : this()
        {
            Items = Conditions.ToArray();
        }
        public IConditions(IConditions<T> Conditions)
            : this(Conditions as IReadOnlyCollection<ICondition<T>>)
        {
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

        public void AddRange(ReadOnlySpan<ICondition<T>> Conditions)
        {
            EnsureCapacity(Length + Conditions.Length);
            Conditions.CopyTo(Items.AsSpan(Length, Conditions.Length));
            Length += Conditions.Length;
            Variant++;
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
        public void AddRange(IEnumerable<ICondition<T>> Conditions)
        {
            if (Conditions is IReadOnlyList<ICondition<T>> conditions)
            {
                AddRange(conditions);
                return;
            }
            EnsureCapacity(Length + Conditions.Count());
            foreach (ICondition<T> Item in Conditions)
            {
                Add(Item);
            }
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

        /// <summary>Retrieve a value by reference.</summary>
        public ref ICondition<T> GetReference(int Index)
        {
            if ((uint)Index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return ref Items[Index];
        }

        public ICondition<T>[] ToArray()
        {
            ICondition<T>[] array = new ICondition<T>[Length];
            Array.Copy(Items, 0, array, 0, Length);
            return array;
        }

        public void CopyTo(int Index, ICondition<T>[] Array, int ArrayIndex, int Count)
        {
            if (Size - Index < Count)
            {
                throw new ArgumentException(
                    $"The number of elements from {nameof(Index)} to the end of the source {nameof(ICondition<T>)}" +
                    $" is greater than the available space from {nameof(ArrayIndex)} to the end of the destination {nameof(Array)}");
            }
            System.Array.Copy(Items, Index, Array, ArrayIndex, Count);
        }
        public void CopyTo(ICondition<T>[] Array)
        {
            CopyTo(Array, 0);
        }

        /// <summary>Performs the specified action on each element of the collection allowing for the inclusion of a <paramref name="Subject"/> for each <see cref="ICondition{T}" /> to interact with.</summary>
        /// <param name="Action">The <see cref="Action{ICondition{T},T}" /> <see langword="delegate" /> to perform on each element of the Conditions <see cref="List{ICondition{T}}" />.</param>
        /// <param name="Subject">A <see cref="T" /> on which each <see cref="ICondition{T}" /> can interact with while performing the <paramref name="Action"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">An element in the collection has been modified.</exception>
        public void ForEach(Action<ICondition<T>, T> Action, T Subject = null)
        {
            if (Action == null)
            {
                throw new ArgumentNullException($"{nameof(Action)} is null");
            }
            int version = Version;
            foreach (ICondition<T> condition in this)
            {
                if (version != Version)
                {
                    break;
                }
                if (condition != null)
                {
                    Action(condition, Subject);
                }
            }
            if (version != Version)
            {
                throw new InvalidOperationException($"An element in the collection has been modified.");
            }
        }

        /// <summary>Performs the specified action on each element of the collection.</summary>
        /// <param name="Action">The <see cref="Action{ICondition{T}}" /> <see langword="delegate" /> to perform on each element of the collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">An element in the collection has been modified.</exception>
        public void ForEach(Action<ICondition<T>> Action)
        {
            if (Action == null)
            {
                throw new ArgumentNullException($"{nameof(Action)} is null");
            }
            int version = Version;
            foreach (ICondition<T> condition in this)
            {
                if (version != Version)
                {
                    break;
                }
                if (condition != null)
                {
                    Action(condition);
                }
            }
            if (version != Version)
            {
                throw new InvalidOperationException($"An element in the collection has been modified.");
            }
        }

        /// <summary>Returns an <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the collection.</summary>
        /// <param name="Subject">An instance of the <see langword="class" /> on which <see cref="ICondition{T}.Check(T)" /> is performed.</param>
        /// <returns>An <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.Check(T)" /> on each of the elements contained in the collection.</returns>
        public IEnumerable<bool> Results(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"> {nameof(IConditions<T>)}.{nameof(Results)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);
            if (!this.IsNullOrEmpty())
            {
                foreach (ICondition<T> condition in this)
                {
                    Debug.Entry(4, $"{condition.ToString(ShowResult: true, Subject)}", Indent: indent + 2, Toggle: true);
                    if (condition != null)
                    {
                        Debug.LastIndent = indent;
                        yield return condition.Check(Subject);
                    }
                }
            }
            Debug.LastIndent = indent;
            yield break;
        }

        /// <summary>Returns an <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.NotCheck(T)" /> on each of the elements contained in the collection.</summary>
        /// <param name="Subject">An instance of the <see langword="class" /> on which <see cref="ICondition{T}.NotCheck(T)" /> is performed.</param>
        /// <returns>An <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="ICondition{T}.NotCheck(T)" /> on each of the elements contained in the collection.</returns>
        public IEnumerable<bool> NotResults(T Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"> {nameof(IConditions<T>)}.{nameof(NotResults)}({typeof(T).Name} Subject)", Indent: indent + 1, Toggle: true);
            if (!this.IsNullOrEmpty())
            {
                foreach (ICondition<T> condition in this)
                {
                    Debug.Entry(4, $"{condition.ToString(ShowResult: true, Subject)}", Indent: indent + 2, Toggle: true);
                    if (condition != null)
                    {
                        Debug.LastIndent = indent;
                        yield return condition.NotCheck(Subject);
                    }
                }
            }
            Debug.LastIndent = indent;
            yield break;
        }
    }
}
