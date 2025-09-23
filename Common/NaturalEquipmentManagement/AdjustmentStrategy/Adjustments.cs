using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using XRL;
using XRL.Collections;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public partial class Adjustments : IAdjustment
    {
        private static bool doDebug => getClassDoDebug(nameof(IAdjustment));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                nameof(Apply),
                nameof(Check),
                nameof(Checks),
                nameof(GetApplied),
                nameof(GetUnapplied),
            };
            List<object> dontList = new()
            {
                nameof(Add),
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

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

        public virtual void Add(IAdjustment Adjustment, GameObject Subject)
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
                    if (Adjustment.TryGetHigherPriorityAdjustment(Subject, Items[i], out higherPriorityAdjustment))
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
            Array.Copy(Items, array, Length);
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
                if (Item != null)
                {
                    Add(Item);
                }
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

        public bool Contains(Type AdjustmentType)
        {
            foreach (IAdjustment adjustment in this)
            {
                if (adjustment.GetType() == AdjustmentType)
                {
                    return true;
                }
            }
            return false;
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
            if (!this.IsNullOrEmpty())
            {
                foreach (IAdjustment adjustment in this)
                {
                    if (adjustment == null)
                    {
                        continue;
                    }
                    yield return adjustment.Check(Subject);
                }
            }
            yield break;
        }

        /// <summary>Returns an <see cref="IEnumerable{IAdjustment}" /> that contains each of the stored <see cref="IAdjustment" /> that have their <see cref="IAdjustment.Applied" /> member set to  <see langword="true" />.</summary>
        /// <remarks> Inverse of <see cref="GetUnapplied" />.</remarks>
        /// <returns>An <see cref="IEnumerable{IAdjustment}" /> that contains each of the stored <see cref="IAdjustment" /> that have their <see cref="IAdjustment.Applied" /> member set to  <see langword="true" />.</returns>
        public IEnumerable<IAdjustment> GetApplied()
        {
            if (!this.IsNullOrEmpty())
            {
                foreach (IAdjustment adjustment in this)
                {
                    if (adjustment == null)
                    {
                        continue;
                    }
                    if (adjustment.IsApplied())
                    {
                        yield return adjustment;
                    }
                }
            }
            yield break;
        }

        /// <summary>Returns an <see cref="IEnumerable{IAdjustment}" /> that contains each of the stored <see cref="IAdjustment" /> that have their <see cref="IAdjustment.Applied" /> member set to  <see langword="false" />.</summary>
        /// <remarks> Inverse of <see cref="GetApplied" />.</remarks>
        /// <returns>An <see cref="IEnumerable{IAdjustment}" /> that contains each of the stored <see cref="IAdjustment" /> that have their <see cref="IAdjustment.Applied" /> member set to  <see langword="false" />.</returns>
        public IEnumerable<IAdjustment> GetUnapplied()
        {
            if (!this.IsNullOrEmpty())
            {
                foreach (IAdjustment adjustment in this)
                {
                    if (adjustment == null)
                    {
                        continue;
                    }
                    if (!adjustment.IsApplied())
                    {
                        yield return adjustment;
                    }
                }
            }
            yield break;
        }

        public override bool Check(GameObject Subject)
        {
            bool anyCheckPasses = false;
            if (!this.IsNullOrEmpty())
            {
                foreach (bool result in Checks(Subject))
                {
                    if (result)
                    {
                        anyCheckPasses = true;
                        break;
                    }
                }
            }
            return anyCheckPasses
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                foreach (IAdjustment adjustment in this)
                {
                    if (adjustment == null)
                    {
                        continue;
                    }
                    if (!adjustment.IsApplied())
                    {
                        adjustment.Apply(Subject);
                    }
                }
            }
            return IsApplied();
        }
    }
}
