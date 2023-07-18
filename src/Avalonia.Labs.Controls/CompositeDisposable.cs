using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Labs.Controls;

internal class CompositeDisposable : IList<IDisposable>, IDisposable
{
    readonly List<IDisposable> _disposables;
    public CompositeDisposable(int capacity)
    {
        _disposables = new List<IDisposable>(capacity);
    }

    public IDisposable this[int index]
    {
        get => _disposables[index];
        set
        {
            var old = _disposables[index];
            old?.Dispose();
            _disposables[index] = value;
        }
    }

    public int Count => _disposables.Count;

    public bool IsReadOnly => false;

    public void Add(IDisposable item) =>
        _disposables.Add(item);

    public void Clear() =>
        Dispose();


    public bool Contains(IDisposable item) =>
        _disposables.Contains(item);

    public void CopyTo(IDisposable[] array, int arrayIndex) =>
        _disposables.CopyTo(array, arrayIndex);

    public void Dispose()
    {
        var start = _disposables.Count - 1;
        for (int i = start; i >= 0; i--)
        {
            _disposables[i].Dispose();
            _disposables.RemoveAt(i);
        }
    }

    public IEnumerator<IDisposable> GetEnumerator() =>
        _disposables.GetEnumerator();

    public int IndexOf(IDisposable item) =>
        _disposables.IndexOf(item);

    public void Insert(int index, IDisposable item) =>
        _disposables.Insert(index, item);

    public bool Remove(IDisposable item)
    {
        var result = _disposables.Remove(item);
        item?.Dispose();
        return result;
    }

    public void RemoveAt(int index)
    {
        var item = _disposables[index];
        _disposables.RemoveAt(index);
        item?.Dispose();
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        _disposables.GetEnumerator();
}
