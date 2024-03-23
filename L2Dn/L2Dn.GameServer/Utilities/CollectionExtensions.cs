using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace L2Dn.GameServer.Utilities;

public static class CollectionExtensions
{
    public static bool isEmpty<T>(this ICollection<T> list)
    {
        return list.Count == 0;
    }

    public static void forEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection)
        {
            action(item);
        }
    }

    public static int size<T>(this List<T> list)
    {
        return list.Count;
    }

    public static T get<T>(this List<T> list, int index)
    {
        return list[index];
    }

    public static void set<T>(this List<T> list, int index, T value)
    {
        list[index] = value;
    }

    public static void add<T>(this List<T> list, int index, T value)
    {
        list.Insert(index, value);
    }
    
    public static void add<T>(this List<T> list, T value)
    {
        list.Add(value);
    }
    
    public static T? get<T>(this WeakReference<T> weakReference)
        where T: class
    {
        if (weakReference.TryGetTarget(out T? target))
            return target;
        
        return null;
    }

    public static bool isEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static bool equals(this string s, string other)
    {
        return string.Equals(s, other, StringComparison.Ordinal);
    }

    public static bool equalsIgnoreCase(this string s, string other)
    {
        return string.Equals(s, other, StringComparison.OrdinalIgnoreCase);
    }

    public static bool endsWith(this string s, string other)
    {
        return (s ?? string.Empty).EndsWith(other, StringComparison.Ordinal);
    }

    public static bool startsWith(this string s, string other)
    {
        return (s ?? string.Empty).StartsWith(other, StringComparison.Ordinal);
    }

    public static string toLowerCase(this string s)
    {
        return s.ToLower();
    }

    public static string toUpperCase(this string s)
    {
        return s.ToUpper();
    }

    public static bool contains(this string s, string ss)
    {
        return s.Contains(ss);
    }

    public static string replaceAll(this string input, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern,
        string replacement)
    {
        return Regex.Replace(input, pattern, replacement);
    }
}