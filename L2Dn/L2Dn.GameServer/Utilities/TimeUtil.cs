namespace L2Dn.GameServer.Utilities;

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

        throw new FormatException();
    }
}