using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Extensions;

public static class ListExtensions
{
    public static void Push<T>(this List<T> collection, T item)
    {
        collection.Add(item);
    }

    public static T Pop<T>(this List<T> collection)
    {
        T item = collection.Last();
        collection.Remove(item);

        return item;
    }
}
