namespace L2Dn.Utilities;

public static class TimeUtil
{
    public static TimeSpan ParseDuration(string? s)
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

    public static DateTime GetCloseNextDay(DayOfWeek dayOfWeek, int hour, int minute)
    {
        DateTime calendar = DateTime.Now; // Today, now
        if (calendar.DayOfWeek != dayOfWeek)
        {
            calendar = calendar.AddDays((dayOfWeek + 7 - calendar.DayOfWeek) % 7);
        }
        else
        {
            int minOfDay = calendar.Hour * 60 + calendar.Minute;
            if (minOfDay >= hour * 60 + minute)
                calendar = calendar.AddDays(7); // Bump to next week
        }

        calendar = new DateTime(calendar.Year, calendar.Month, calendar.Day, hour, minute, 0);
        return calendar;
    }
}