using System.Globalization;

namespace L2Dn.Extensions;

public static class StringExtensions
{
    public static bool ContainsAlphaNumericOnly(this string s)
    {
        // No LINQ, as LINQ would allocate memory
        foreach (char c in s)
        {
            if (!char.IsLetterOrDigit(c))
                return false;
        }

        return true;
    }

    public static string? CapitalizeFirstLetter(this string? s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        char c = s[0];
        char upper = char.ToUpperInvariant(c);
        if (c != upper)
            return upper + s[1..];

        return s;
    }

    public static T TryParseOrDefault<T>(this string? s, T defaultValue, IFormatProvider? provider = null)
        where T: IParsable<T>
    {
        if (s is null)
            return defaultValue;

        return T.TryParse(s, provider ?? CultureInfo.InvariantCulture, out T? result) ? result : defaultValue;
    }
}