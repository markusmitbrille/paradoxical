using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paradoxical.Extensions;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }

    public static int RemoveAll<T>(this ObservableCollection<T> collection, Predicate<T> match)
    {
        var items = collection.Where(item => match(item)).ToList();
        foreach (T item in items)
        {
            collection.Remove(item);
        }

        return items.Count;
    }
}
