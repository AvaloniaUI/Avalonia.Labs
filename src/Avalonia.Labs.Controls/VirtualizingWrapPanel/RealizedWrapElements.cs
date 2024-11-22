using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Utils;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// Stores the realized element state for a virtualizing panel that arranges its children
    /// in a stack layout, such as <see cref="VirtualizingStackPanel"/>.
    /// </summary>
    internal class RealizedWrapElements
    {
        private int _firstIndex;
        private List<Control?>? _elements;
        private List<Size>? _sizes;

        /// <summary>
        /// Gets the number of realized elements.
        /// </summary>
        public int Count => _elements?.Count ?? 0;

        /// <summary>
        /// Gets the index of the first realized element, or -1 if no elements are realized.
        /// </summary>
        public int FirstIndex => _elements?.Count > 0 ? _firstIndex : -1;

        /// <summary>
        /// Gets the index of the last realized element, or -1 if no elements are realized.
        /// </summary>
        public int LastIndex => _elements?.Count > 0 ? _firstIndex + _elements.Count - 1 : -1;

        /// <summary>
        /// Gets the elements.
        /// </summary>
        public IReadOnlyList<Control?> Elements => _elements ??= new List<Control?>();

        /// <summary>
        /// Gets the sizes of the elements on the primary axis.
        /// </summary>
        public IReadOnlyList<Size> Sizes => _sizes ??= new List<Size>();
        
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

            _elements ??= new List<Control?>();
            _sizes ??= new List<Size>();

            if (Count == 0)
            {
                _elements.Add(element);
                _sizes.Add(size);
                _firstIndex = index;
            }
            else if (index == LastIndex + 1)
            {
                _elements.Add(element);
                _sizes.Add(size);
            }
            else if (index == FirstIndex - 1)
            {
                --_firstIndex;
                _elements.Insert(0, element);
                _sizes.Insert(0, size);
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
            var i = index - FirstIndex;
            if (i >= 0 && i < _elements?.Count)
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
            
            if (index < FirstIndex || _sizes is null)
                return null;

            if (index >= _sizes.Count - 1)
                return null;
            
            return _sizes[index];
        }
        
        public Size? GetElementSize(int index)
        {
            if (index < FirstIndex || _sizes is null)
                return null;

            if (index >= _sizes.Count - 1)
                return null;
            
            return _sizes[index];
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The index or -1 if the element is not present in the collection.</returns>
        public int GetIndex(Control element)
        {
            return _elements?.IndexOf(element) is { } index and >= 0 ? index + FirstIndex : -1;
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
            if (_elements is null || _elements.Count == 0)
                return;

            // Get the index within the realized _elements collection.
            var first = FirstIndex;
            var realizedIndex = index - first;

            if (realizedIndex < Count)
            {
                // The insertion point affects the realized elements. Update the index of the
                // elements after the insertion point.
                var elementCount = _elements.Count;
                var start = Math.Max(realizedIndex, 0);

                for (var i = start; i < elementCount; ++i)
                {
                    if (_elements[i] is not { } element)
                        continue;
                    var oldIndex = i + first;
                    updateElementIndex(element, oldIndex, oldIndex + count);
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
                    _elements!.InsertMany(realizedIndex, null, count);
                    _sizes!.InsertMany(realizedIndex, Size.Infinity, count);
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
            if (_elements is null || _elements.Count == 0)
                return;

            // Get the removal start and end index within the realized _elements collection.
            var first = FirstIndex;
            var last = LastIndex;
            var startIndex = index - first;
            var endIndex = (index + count) - first;

            if (endIndex < 0)
            {
                // The removed range was before the realized elements. Update the first index and
                // the indexes of the realized elements.
                _firstIndex -= count;

                var newIndex = _firstIndex;
                for (var i = 0; i < _elements.Count; ++i)
                {
                    if (_elements[i] is { } element)
                        updateElementIndex(element, newIndex + count, newIndex);
                    ++newIndex;
                }
            }
            else if (startIndex < _elements.Count)
            {
                // Recycle and remove the affected elements.
                var start = Math.Max(startIndex, 0);
                var end = Math.Min(endIndex, _elements.Count);

                for (var i = start; i < end; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        _elements[i] = null;
                        recycleElement(element);
                    }
                }

                _elements.RemoveRange(start, end - start);
                _sizes!.RemoveRange(start, end - start);

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
                        updateElementIndex(element, newIndex + count, newIndex);
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
            if (_elements is null || _elements.Count == 0)
                return;

            // Get the index within the realized _elements collection.
            var startIndex = index - FirstIndex;
            var endIndex = Math.Min(startIndex + count, Count);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                for (var i = startIndex; i < endIndex; ++i)
                {
                    if (_elements[i] is { } element)
                    {
                        recycleElement(element);
                        _elements[i] = null;
                        _sizes![i] = Size.Infinity;
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
            if (_elements is null || _elements.Count == 0)
                return;

            for (var i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] is { } e)
                {
                    _elements[i] = null;
                    recycleElement(e);
                }
            }

            _elements?.Clear();
            _sizes?.Clear();

        }

        /// <summary>
        /// Recycles elements before a specific index.
        /// </summary>
        /// <param name="index">The index in the source collection of new first element.</param>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void RecycleElementsBefore(int index, Action<Control, int> recycleElement)
        {
            if (index <= FirstIndex || _elements is null || _elements.Count == 0)
                return;

            if (index > LastIndex)
            {
                RecycleAllElements(recycleElement);
            }
            else
            {
                var endIndex = index - FirstIndex;

                for (var i = 0; i < endIndex; ++i)
                {
                    if (_elements[i] is { } e)
                    {
                        _elements[i] = null;
                        recycleElement(e, i + FirstIndex);
                    }
                }

                _elements.RemoveRange(0, endIndex);
                _sizes!.RemoveRange(0, endIndex);
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
            if (index >= LastIndex || _elements is null || _elements.Count == 0)
                return;

            if (index < FirstIndex)
            {
                RecycleAllElements(recycleElement);
            }
            else
            {
                var startIndex = (index + 1) - FirstIndex;
                var count = _elements.Count;

                for (var i = startIndex; i < count; ++i)
                {
                    if (_elements[i] is { } e)
                    {
                        _elements[i] = null;
                        recycleElement(e, i + FirstIndex);
                    }
                }

                _elements.RemoveRange(startIndex, _elements.Count - startIndex);
                _sizes!.RemoveRange(startIndex, _sizes.Count - startIndex);
            }
        }

        /// <summary>
        /// Recycles all realized elements.
        /// </summary>
        /// <param name="recycleElement">A method used to recycle elements.</param>
        public void RecycleAllElements(Action<Control, int> recycleElement)
        {
            if (_elements is null || _elements.Count == 0)
                return;

            for (var i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] is { } e)
                {
                    _elements[i] = null;
                    recycleElement(e, i + FirstIndex);
                }
            }

            _firstIndex = 0;
            _elements?.Clear();
            _sizes?.Clear();
        }

        /// <summary>
        /// Resets the element list and prepares it for reuse.
        /// </summary>
        public void ResetForReuse()
        {
            _firstIndex = 0;
            _elements?.Clear();
            _sizes?.Clear();
        }
    }
}
