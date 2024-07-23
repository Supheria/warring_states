using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.Net.Utilities;

public class AutoDisposeItemCollection<T> : ICollection<T> where T : AutoDisposeItem
{
    ConcurrentDictionary<DateTime, T> Items { get; } = [];

    public int Count => Items.Count;

    public bool IsReadOnly => false;

    public T this[DateTime key]
    {
        get => Items[key];
        set => Items[key] = value;
    }

    public bool TryAdd(T item)
    {
        item.OnDisposed += () => Items.Remove(item.TimeStamp, out _);
        return Items.TryAdd(item.TimeStamp, item);
    }

    public bool TryGetValue(DateTime timeStamp, [NotNullWhen(true)] out T? item)
    {
        return Items.TryGetValue(timeStamp, out item);
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool TryRemove(T item)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.Values.GetEnumerator();
    }

    [Obsolete]
    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }
}
