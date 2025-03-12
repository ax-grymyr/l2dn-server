using System.Collections.Frozen;
using System.Globalization;

namespace L2Dn.Utilities;

public static class ParseUtil
{
    public static FrozenSet<T> ParseEnumSet<T>(string value, char separator = ';', bool ignoreCase = true)
        where T: struct, Enum =>
        string.IsNullOrEmpty(value) ? FrozenSet<T>.Empty : ParseEnumSetPrivate<T>(value, separator, ignoreCase);

    public static FrozenSet<T> ParseEnumSet<T>(string value, string addPrefix, string addSuffix, char separator = ';',
        bool ignoreCase = true)
        where T: struct, Enum
    {
        if (string.IsNullOrEmpty(value))
            return FrozenSet<T>.Empty;

        if (string.IsNullOrEmpty(addPrefix) && string.IsNullOrEmpty(addSuffix))
            return ParseEnumSetPrivate<T>(value, separator, ignoreCase);

        int suffixLength = addSuffix.Length;
        int addLength = addPrefix.Length + suffixLength;

        const int storageSize = 256;
        Span<char> storage = stackalloc char[storageSize];
        List<T> list = new(16);
        ReadOnlySpan<char> span = value.AsSpan();
        foreach (System.Range range in span.Split(separator))
        {
            ReadOnlySpan<char> v = span[range];
            if (storageSize < v.Length + addLength)
            {
                string s = addPrefix + new string(v) + addSuffix;
                list.Add(Enum.Parse<T>(s, ignoreCase));
            }
            else
            {
                int size = addPrefix.Length;
                if (size != 0)
                    addPrefix.AsSpan().CopyTo(storage);

                v.CopyTo(storage[size..]);
                size += v.Length;
                if (suffixLength != 0)
                {
                    addSuffix.AsSpan().CopyTo(storage[size..]);
                    size += suffixLength;
                }

                list.Add(Enum.Parse<T>(storage[..size], ignoreCase));
            }
        }

        return list.ToFrozenSet();
    }

    public static FrozenSet<T> ParseSet<T>(string value, char separator = ';')
        where T: ISpanParsable<T>
    {
        if (string.IsNullOrEmpty(value))
            return FrozenSet<T>.Empty;

        List<T> list = new(16);
        ReadOnlySpan<char> span = value.AsSpan();
        foreach (System.Range range in span.Split(separator))
            list.Add(T.Parse(span[range], CultureInfo.InvariantCulture));

        return list.ToFrozenSet();
    }

    private static FrozenSet<T> ParseEnumSetPrivate<T>(string value, char separator, bool ignoreCase)
        where T: struct, Enum
    {
        List<T> list = new(16);
        ReadOnlySpan<char> span = value.AsSpan();
        foreach (System.Range range in span.Split(separator))
            list.Add(Enum.Parse<T>(span[range], ignoreCase));

        return list.ToFrozenSet();
    }
}