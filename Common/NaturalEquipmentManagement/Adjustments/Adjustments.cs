using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using XRL;
using XRL.Collections;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public partial class Adjustments : IAdjustment
    {
        protected IAdjustment[] Items = Array.Empty<IAdjustment>();

        protected int Size;

        protected int Length;

        protected int Variant;

        public int Capacity => Size;

        public int Version => Variant;

        protected virtual int DefaultCapacity => 4;

        public bool WantFieldReflection => false;

        public Adjustments()
            : base()
        {
        }

        public Adjustments(List<IAdjustment> Adjustments)
            : this()
        {
            Items = Adjustments.ToArray();
        }
        public Adjustments(Adjustments Adjustments)
            : this(Adjustments as IReadOnlyCollection<IAdjustment>)
        {
        }
        public virtual IAdjustment this[int Index]
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
            IAdjustment[] array = new IAdjustment[Capacity];
            Array.Copy(Items, 0, array, 0, Length);
            Items = array;
            Size = Capacity;
        }

        public void AddRange(ReadOnlySpan<IAdjustment> Adjustments)
        {
            EnsureCapacity(Length + Adjustments.Length);
            Adjustments.CopyTo(Items.AsSpan(Length, Adjustments.Length));
            Length += Adjustments.Length;
            Variant++;
        }
        public void AddRange(IReadOnlyList<IAdjustment> Adjustments)
        {
            int count = Adjustments.Count;
            EnsureCapacity(Length + count);
            for (int i = 0; i < count; i++)
            {
                Add(Adjustments[i]);
            }
        }
        public void AddRange(IReadOnlyCollection<IAdjustment> Adjustments)
        {
            if (Adjustments is IReadOnlyList<IAdjustment> adjustments)
            {
                AddRange(adjustments);
                return;
            }
            EnsureCapacity(Length + Adjustments.Count);
            foreach (IAdjustment Item in Adjustments)
            {
                Add(Item);
            }
        }
        public void AddRange(IEnumerable<IAdjustment> Adjustments)
        {
            if (Adjustments is IReadOnlyList<IAdjustment> adjustments)
            {
                AddRange(adjustments);
                return;
            }
            EnsureCapacity(Length + Adjustments.Count());
            foreach (IAdjustment Item in Adjustments)
            {
                Add(Item);
            }
        }

        public virtual int RemoveAll(Predicate<IAdjustment> Match)
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

            if (RuntimeHelpers.IsReferenceOrContainsReferences<Adjustments>())
            {
                Array.Clear(Items, i, Size - i);
            }

            int result = Size - i;
            Size = i;
            Variant++;
            return result;
        }

        public int FindIndex(int StartIndex, int Count, Predicate<IAdjustment> Match)
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
        public int FindIndex(int StartIndex, Predicate<IAdjustment> Match)
        {
            return FindIndex(StartIndex, Size - StartIndex, Match);
        }
        public int FindIndex(Predicate<IAdjustment> Match)
        {
            return FindIndex(0, Size, Match);
        }
        public bool Exists(Predicate<IAdjustment> Match)
        {
            return FindIndex(Match) != -1;
        }

        /// <summary>Retrieve a value by reference.</summary>
        public ref IAdjustment GetReference(int Index)
        {
            if ((uint)Index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return ref Items[Index];
        }

        public IAdjustment[] ToArray()
        {
            IAdjustment[] array = new IAdjustment[Length];
            Array.Copy(Items, 0, array, 0, Length);
            return array;
        }

        public void CopyTo(int Index, IAdjustment[] Array, int ArrayIndex, int Count)
        {
            if (Size - Index < Count)
            {
                throw new ArgumentException(
                    $"The number of elements from {nameof(Index)} to the end of the source {nameof(IAdjustment)}" +
                    $" is greater than the available space from {nameof(ArrayIndex)} to the end of the destination {nameof(Array)}");
            }
            System.Array.Copy(Items, Index, Array, ArrayIndex, Count);
        }
        public void CopyTo(IAdjustment[] Array)
        {
            CopyTo(Array, 0);
        }

        /// <summary>Performs the specified action on each element of the Adjustments <see cref="List{IAdjustment}" />.</summary>
        /// <param name="Action">The <see cref="T:System.Action`1" /> <see langword="delegate" /> to perform on each element of the Adjustments <see cref="List{IAdjustment}" />.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="Action" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An element in the collection has been modified.</exception>
        public void ForEach(Action<IAdjustment> Action)
        {
            if (Action == null)
            {
                throw new ArgumentNullException($"{nameof(Action)} is null");
            }
            int version = Version;
            for (int i = 0; i < Size; i++)
            {
                if (version != Version)
                {
                    break;
                }
                Action(Items[i]);
            }
            if (version != Version)
            {
                throw new InvalidOperationException($"An element in the collection has been modified.");
            }
        }

        /// <summary>Returns an <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="IAdjustment.Check(GameObject)" /> on each of the elements contained in the Adjustments <see cref="List{IAdjustment}" />.</summary>
        /// <param name="Subject">An instance of the <see cref="IEventHandler" /> on which <see cref="IAdjustment.Check(GameObject)" /> is performed.</param>
        /// <returns>An <see cref="IEnumerable{bool}" /> that contains each of the <see cref="bool" /> results of calling <see cref="IAdjustment.Check(GameObject)" /> on each of the elements contained in the Adjustments <see cref="List{IAdjustment}" />.</returns>
        public IEnumerable<bool> Checks(GameObject Subject)
        {
            if (!Items.IsNullOrEmpty())
            {
                foreach (IAdjustment adjustment in Items)
                {
                    yield return adjustment.Check(Subject);
                }
            }
            yield break;
        }

        public override bool Apply(GameObject Subject)
        {
            if (!Items.IsNullOrEmpty() && base.Apply(Subject))
            {
                foreach (IAdjustment adjustment in Items)
                {
                    adjustment.Apply(Subject);
                }
                return true;
            }
            return false;
        }
    }
}
