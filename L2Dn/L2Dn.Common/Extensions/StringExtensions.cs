namespace L2Dn.Extensions;

public static class StringExtensions
{
    public static T Parse<T>(this string? s, T defaultValue, IFormatProvider? provider = null)
        where T: IParsable<T>
    {
        if (s is null)
            return defaultValue;

        return T.TryParse(s, provider, out T? result) ? result : defaultValue;
    }
}