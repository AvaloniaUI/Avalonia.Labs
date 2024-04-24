// Based on: https://github.com/StephenCleary/Mvvm.Core/blob/cbac65eda98760cc0dabd0b82a6468be0e1dd44f/src/Nito.Mvvm.Core/WeakCollection.cs
using System;
using System.Collections.Generic;

namespace Avalonia.Utilities;

/// <summary>
/// A collection of weak references to objects.
/// </summary>
/// <typeparam name="T">The type of object to hold weak references to.</typeparam>
internal sealed class WeakCollection<T> where T : class
{
    /// <summary>
    /// The actual collection of strongly-typed weak references.
    /// </summary>
    private readonly List<WeakReference<T>> _list = [];

    /// <summary>
    /// Gets a list of live objects from this collection, causing a purge.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<T> GetLiveItems()
    {
        var ret = new List<T>(_list.Count);

        // This implementation uses logic similar to List<T>.RemoveAll, which always has O(n) time.
        //  Some other implementations seen in the wild have O(n*m) time, where m is the number of dead entries.
        //  As m approaches n (e.g., mass object extinctions), their running time approaches O(n^2).
        int writeIndex = 0;
        for (int readIndex = 0; readIndex != _list.Count; ++readIndex)
        {
            WeakReference<T> weakReference = _list[readIndex];
            T? item;
            if (weakReference.TryGetTarget(out item))
            {
                ret.Add(item);

                if (readIndex != writeIndex)
                    _list[writeIndex] = _list[readIndex];

                ++writeIndex;
            }
        }

        _list.RemoveRange(writeIndex, _list.Count - writeIndex);

        return ret;
    }

    /// <summary>
    /// Adds a weak reference to an object to the collection. Does not cause a purge.
    /// </summary>
    /// <param name="item">The object to add a weak reference to.</param>
    public void Add(T item)
    {
        _list.Add(new WeakReference<T>(item));
    }

    /// <summary>
    /// Removes a weak reference to an object from the collection. Does not cause a purge.
    /// </summary>
    /// <param name="item">The object to remove a weak reference to.</param>
    /// <returns>True if the object was found and removed; false if the object was not found.</returns>
    public bool Remove(T item)
    {
        for (int i = 0; i != _list.Count; ++i)
        {
            var weakReference = _list[i];
            T? entry;
            if (weakReference.TryGetTarget(out entry) && entry == item)
            {
                _list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
