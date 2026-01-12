using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Labs.Controls.Utils;

internal static class CollectionExtensions
{
    internal static void InsertMany<T>(this List<T> list, int index, T item, int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count == 0)
            return;

        list.InsertRange(index, new RepeatCollection<T>(item, count));
    }

    private sealed class RepeatCollection<T> : ICollection<T>
    {
        private readonly T _item;

        public RepeatCollection(T item, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            _item = item;
            Count = count;
        }

        public int Count { get; }
        public bool IsReadOnly => true;

        public void Add(T item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(T item) => throw new NotSupportedException();
        public bool Remove(T item) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return _item;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if ((uint)arrayIndex > (uint)array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array is not long enough.", nameof(array));

            var end = arrayIndex + Count;
            for (var i = arrayIndex; i < end; i++)
                array[i] = _item;
        }
    }
}
