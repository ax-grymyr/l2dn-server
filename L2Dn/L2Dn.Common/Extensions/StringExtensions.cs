namespace L2Dn.Extensions;

public static class StringExtensions
{
    public static bool ContainsAlphaNumericOnly(this string s)
    {
        // No LINQ, as it would allocate memory
        foreach (char c in s)
        {
            if (!char.IsLetterOrDigit(c))
                return false;
        }

        return true;
    }

    public static T Parse<T>(this string? s, T defaultValue, IFormatProvider? provider = null)
        where T: IParsable<T>
    {
        if (s is null)
            return defaultValue;

        return T.TryParse(s, provider, out T? result) ? result : defaultValue;
    }
}