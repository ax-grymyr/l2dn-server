﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace L2Dn.GameServer.Utilities;

public static class CollectionExtensions
{
    public static T? get<T>(this WeakReference<T> weakReference)
        where T: class
    {
        if (weakReference.TryGetTarget(out T? target))
            return target;

        return null;
    }

    public static bool equals(this string? s, string? other)
    {
        return string.Equals(s, other, StringComparison.Ordinal);
    }

    public static bool equalsIgnoreCase(this string? s, string? other)
    {
        return string.Equals(s, other, StringComparison.OrdinalIgnoreCase);
    }

    public static bool endsWith(this string? s, string other)
    {
        return (s ?? string.Empty).EndsWith(other, StringComparison.Ordinal);
    }

    public static bool startsWith(this string? s, string other)
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