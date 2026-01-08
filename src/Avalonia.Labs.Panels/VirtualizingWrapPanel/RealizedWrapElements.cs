using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Utils;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// Stores the realized element state for a virtualizing panel that arranges its children
    /// in a wrap layout, such as <see cref="VirtualizingWrapPanel"/>.
    /// </summary>
    internal class RealizedWrapElements
    {
        private int _firstIndex;
        private readonly List<Control?> _elements;
        private readonly List<Size> _sizes;
        private readonly Dictionary<Control, int> _elementToIndex = new();

        public RealizedWrapElements()
        {
            // Pre-allocate with reasonable capacity to reduce reallocations
            _elements = new List<Control?>(32);
            _sizes = new List<Size>(32);
        }

        /// <summary>
        /// Gets the number of realized elements.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Gets the index of the first realized element, or -1 if no elements are realized.
        /// </summary>
        public int FirstIndex => _elements.Count > 0 ? _firstIndex : -1;

        /// <summary>
        /// Gets the index of the last realized element, or -1 if no elements are realized.
        /// </summary>
        public int LastIndex => _elements.Count > 0 ? _firstIndex + _elements.Count - 1 : -1;

        /// <summary>
        /// Gets the elements.
        /// </summary>
        public IReadOnlyList<Control?> Elements => _elements;

        /// <summary>
        /// Gets the sizes of the elements on the primary axis.
        /// </summary>
        public IReadOnlyList<Size> Sizes => _sizes;
        
        /// <summary>
        /// Adds a newly realized element to the collection.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <param name="element">The element.</param>
        /// <param name="size">The size of the element on the primary axis.</param>
        public void Add(int index, Control element, Size size)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var count = _elements.Count;

            if (count == 0)
            {
                _elements.Add(element);
                _sizes.Add(size);
                _elementToIndex[element] = index;
                _firstIndex = index;
            }
            else if (index == _firstIndex + count)
            {
                _elements.Add(element);
                _sizes.Add(size);
                _elementToIndex[element] = index;
            }
            else if (index == _firstIndex - 1)
            {
                --_firstIndex;
                _elements.Insert(0, element);
                _sizes.Insert(0, size);
                _elementToIndex[element] = index;
            }
            else
            {
                throw new NotSupportedException("Can only add items to the beginning or end of realized elements.");
            }
        }

        /// <summary>
        /// Gets the element at the specified index, if realized.
        /// </summary>
        /// <param name="index">The index in the source collection of the element to get.</param>
        /// <returns>The element if realized; otherwise null.</returns>
        public Control? GetElement(int index)
        {
            var i = index - _firstIndex;
            var count = _elements.Count;
            if (i >= 0 && i < count)
                return _elements[i];
            return null;
        }

        /// <summary>
        /// Gets the Size of the element, if realized.
        /// </summary>
        /// <returns>
        /// The size of the element or Infinite if not found
        /// </returns>
        public Size? GetElementSize(Control? child)
        {
            if (child == null) return null;
            
            var index = GetIndex(child);
            
            if (index < 0)
                return null;

            var localIndex = index - _firstIndex;
            if (localIndex < 0 || localIndex >= _sizes.Count)
                return null;

            return _sizes[localIndex];
        }

        /// <summary>
        /// Gets the Size of the element, if realized.
        /// </summary>
        /// <param name="index">The index to lookup</param>
        /// <returns>The size of the element or null if not found</returns>
        public Size? GetElementSize(int index)
        {
            if (index < FirstIndex)
                return null;

            var localIndex = index - _firstIndex;
            if (localIndex >= _sizes.Count)
                return null;

            return _sizes[localIndex];
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The index or -1 if the element is not present in the collection.</returns>
        public int GetIndex(Control element)
        {
            return _elementToIndex.TryGetValue(element, out var index) ? index : -1;
        }

        /// <summary>
        /// Updates the elements in response to items being inserted into the source collection.
        /// </summary>
        /// <param name="index">The index in the source collection of the insert.</param>
        /// <param name="count">The number of items inserted.</param>
        /// <param name="updateElementIndex">A method used to update the element indexes.</param>
        public void ItemsInserted(int index, int count, Action<Control, int, int> updateElementIndex)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var elementCount = _elements.Count;
            if (elementCount == 0)
                return;

            // Get the index within the realized _elements collection.
            var first = _firstIndex;
            var realizedIndex = index - first;

            if (realizedIndex < elementCount)
            {
                // The insertion point affects the realized elements. Update the index of the
                // elements after the insertion point.
                var start = Math.Max(realizedIndex, 0);

                for (var i = start; i < elementCount; ++i)
                {
                    if (_elements[i] is not { } element)
                        continue;
                    var oldIndex = i + first;
                    var newIndex = oldIndex + count;
                    updateElementIndex(element, oldIndex, newIndex);
                    _elementToIndex[element] = newIndex;
                }

                if (realizedIndex < 0)
                {
                    // The insertion point was before the first element, update the first index.
                    _firstIndex += count;
                }
                else
                {
                    // The insertion point was within the realized elements, insert an empty space
                    // in _elements and _sizes.
                    _elements.InsertMany(realizedIndex, null, count);
                    _sizes.InsertMany(realizedIndex, Size.Infinity, count);
                }
            }
        }

        /// <summary>
        /// Updates the elements in response to items being removed from the source collection.
        /// </summary>
        /// <param name="index">The index in the source collection of the remove.</param>
        /// <param name="count">The number of items removed.</param>
        /// <param name="updateElementIndex">A method used to update the element indexes.</param>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void ItemsRemoved(
            int index,
            int count,
            Action<Control, int, int> updateElementIndex,
            Action<Control> recycleElement)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var elementCount = _elements.Count;
            if (elementCount == 0)
                return;

            // Get the removal start and end index within the realized _elements collection.
            var first = _firstIndex;
            var last = first + elementCount - 1;
            var startIndex = index - first;
            var endIndex = (index + count) - first;

            if (endIndex < 0)
            {
                // The removed range was before the realized elements. Update the first index and
                // the indexes of the realized elements.
                _firstIndex -= count;

                var newIndex = _firstIndex;
                for (var i = 0; i < elementCount; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        updateElementIndex(element, newIndex + count, newIndex);
                        _elementToIndex[element] = newIndex;
                    }

                    ++newIndex;
                }
            }
            else if (startIndex < elementCount)
            {
                // Recycle and remove the affected elements.
                var start = Math.Max(startIndex, 0);
                var end = Math.Min(endIndex, elementCount);

                for (var i = start; i < end; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        _elements[i] = null;
                        _elementToIndex.Remove(element);
                        recycleElement(element);
                    }
                }

                _elements.RemoveRange(start, end - start);
                _sizes.RemoveRange(start, end - start);

                // If the remove started before and ended within our realized elements, then our new
                // first index will be the index where the remove started. Mark StartU as unstable
                // because we can't rely on it now to estimate element heights.
                if (startIndex <= 0 && end < last)
                {
                    _firstIndex = first = index;
                }

                // Update the indexes of the elements after the removed range.
                end = _elements.Count;
                var newIndex = first + start;
                for (var i = start; i < end; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        updateElementIndex(element, newIndex + count, newIndex);
                        _elementToIndex[element] = newIndex;
                    }

                    ++newIndex;
                }
            }
        }

        /// <summary>
        /// Updates the elements in response to items being replaced in the source collection.
        /// </summary>
        /// <param name="index">The index in the source collection of the remove.</param>
        /// <param name="count">The number of items removed.</param>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void ItemsReplaced(int index, int count, Action<Control> recycleElement)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var elementCount = _elements.Count;
            if (elementCount == 0)
                return;

            // Get the index within the realized _elements collection.
            var startIndex = index - _firstIndex;
            var endIndex = Math.Min(startIndex + count, elementCount);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                for (var i = startIndex; i < endIndex; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        recycleElement(element);
                        _elementToIndex.Remove(element);
                        _elements[i] = null;
                        _sizes[i] = Size.Infinity;
                    }
                }
            }
        }

        /// <summary>
        /// Recycles all elements in response to the source collection being reset.
        /// </summary>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void ItemsReset(Action<Control> recycleElement)
        {
            var count = _elements.Count;
            if (count == 0)
                return;

            for (var i = 0; i < count; i++)
            {
                if (_elements[i] is { } e)
                {
                    _elements[i] = null;
                    _elementToIndex.Remove(e);
                    recycleElement(e);
                }
            }

            _elements.Clear();
            _sizes.Clear();
            _elementToIndex.Clear();
        }

        /// <summary>
        /// Recycles elements before a specific index.
        /// </summary>
        /// <param name="index">The index in the source collection of new first element.</param>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void RecycleElementsBefore(int index, Action<Control, int> recycleElement)
        {
            var count = _elements.Count;
            var first = _firstIndex;

            if (index <= first || count == 0)
                return;

            if (index > first + count - 1)
            {
                RecycleAllElements(recycleElement);
            }
            else
            {
                var endIndex = index - first;

                for (var i = 0; i < endIndex; ++i)
                {
                    if (_elements[i] is { } e)
                    {
                        _elements[i] = null;
                        _elementToIndex.Remove(e);
                        recycleElement(e, i + first);
                    }
                }

                _elements.RemoveRange(0, endIndex);
                _sizes.RemoveRange(0, endIndex);
                _firstIndex = index;
            }
        }

        /// <summary>
        /// Recycles elements after a specific index.
        /// </summary>
        /// <param name="index">The index in the source collection of new last element.</param>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void RecycleElementsAfter(int index, Action<Control, int> recycleElement)
        {
            var count = _elements.Count;
            var first = _firstIndex;

            if (index >= first + count - 1 || count == 0)
                return;

            if (index < first)
            {
                RecycleAllElements(recycleElement);
            }
            else
            {
                var startIndex = (index + 1) - first;

                for (var i = startIndex; i < count; ++i)
                {
                    if (_elements[i] is { } e)
                    {
                        _elements[i] = null;
                        _elementToIndex.Remove(e);
                        recycleElement(e, i + first);
                    }
                }

                var removeCount = count - startIndex;
                _elements.RemoveRange(startIndex, removeCount);
                _sizes.RemoveRange(startIndex, removeCount);
            }
        }

        /// <summary>
        /// Recycles all realized elements.
        /// </summary>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void RecycleAllElements(Action<Control, int> recycleElement)
        {
            var count = _elements.Count;
            if (count == 0)
                return;

            var first = _firstIndex;
            for (var i = 0; i < count; i++)
            {
                if (_elements[i] is { } e)
                {
                    _elements[i] = null;
                    _elementToIndex.Remove(e);
                    recycleElement(e, i + first);
                }
            }

            _firstIndex = 0;
            _elements.Clear();
            _sizes.Clear();
            _elementToIndex.Clear();
        }

        /// <summary>
        /// Resets the element list and prepares it for reuse.
        /// </summary>
        public void ResetForReuse()
        {
            _firstIndex = 0;
            _elements.Clear();
            _sizes.Clear();
            _elementToIndex.Clear();
        }
    }
}
