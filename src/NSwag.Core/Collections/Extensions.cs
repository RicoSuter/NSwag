#nullable enable

namespace NSwag;

internal static class Extensions
{
    public static bool Any<T>(this List<T>? collection, Predicate<T> match)
    {
        return collection is not null && collection.Exists(match);
    }

    public static int Count<T>(this List<T>? collection, Predicate<T> match)
    {
        var count = 0;
        if (collection is null)
        {
            return count;
        }

        for (var i = 0; i < collection.Count; i++)
        {
            if (match(collection[i]))
            {
                count++;
            }
        }

        return count;
    }

    public static T? FirstOrDefault<T>(this List<T>? collection)
    {
        return collection is null || collection.Count == 0 ? default : collection[0];
    }

    public static bool All<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> match) where TKey : notnull
    {
        foreach (var key in dictionary)
        {
            if (!match(key))
            {
                return false;
            }
        }
        return true;
    }

    public static KeyValuePair<TKey, TValue> FirstOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> match) where TKey : notnull
    {
        foreach (var pair in dictionary)
        {
            if (match(pair))
            {
                return pair;
            }
        }
        return default;
    }
}