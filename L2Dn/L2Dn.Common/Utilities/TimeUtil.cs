namespace L2Dn.Utilities;

public static class TimeUtil
{
    public static TimeSpan ParseDuration(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new FormatException();

        if (s.EndsWith("sec"))
            return TimeSpan.FromSeconds(int.Parse(s.Substring(0, s.Length - 3)));

        if (s.EndsWith("min"))
            return TimeSpan.FromMinutes(int.Parse(s.Substring(0, s.Length - 3)));

        if (s.EndsWith("hour"))
            return TimeSpan.FromHours(int.Parse(s.Substring(0, s.Length - 4)));

        if (s.EndsWith("days"))
            return TimeSpan.FromDays(int.Parse(s.Substring(0, s.Length - 4)));

        throw new FormatException();
    }
}