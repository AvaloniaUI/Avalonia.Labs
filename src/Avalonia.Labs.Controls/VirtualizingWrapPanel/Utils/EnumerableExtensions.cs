using System.Collections;

namespace Avalonia.Labs.Controls.Utils;

/// <summary>
/// <see cref="IEnumerable"/> extensions methods
/// </summary>
internal static class EnumerableExtensions
{
    /// <summary>
    /// Gets the index of an item from an IEnumerable
    /// </summary>
    internal static int IndexOf(this IEnumerable items, object item)
    {
        if (items is IList list)
        {
            return list.IndexOf(item);
        }

        int index = 0;

        foreach (var i in items)
        {
            if (ReferenceEquals(i, item))
            {
                return index;
            }

            ++index;
        }

        return -1;
    }
}
