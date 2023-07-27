using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Paradoxical.Extensions;

public static class ListExtensions
{
    public static void Push<T>(this List<T> collection, T item)
    {
        collection.Add(item);
    }

    public static T Pop<T>(this List<T> collection)
    {
        T item = collection[^1];
        collection.RemoveAt(collection.Count - 1);

        return item;
    }

    public static T Peek<T>(this List<T> collection)
    {
        T item = collection.Last();

        return item;
    }

    public static T? PeekOrDefault<T>(this List<T> collection)
    {
        if (collection.Any() == false)
        {
            return default;
        }

        return Peek(collection);
    }

    public static void RemoveAll<T>(this List<T> collection, T obj)
    {
        collection.RemoveAll(item => EqualityComparer<T>.Default.Equals(item, obj));
    }

    public static void RemoveConsecutiveDuplicates<T>(this List<T> collection)
    {
        int i = 0;
        while (i < collection.Count - 1)
        {
            if (EqualityComparer<T>.Default.Equals(collection[i], collection[i + 1]))
            {
                collection.RemoveAt(i + 1);
            }
            else
            {
                i++;
            }
        }
    }
}
