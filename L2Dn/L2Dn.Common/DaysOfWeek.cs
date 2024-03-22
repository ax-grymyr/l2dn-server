namespace L2Dn;

[Flags]
public enum DaysOfWeek
{
    None = 0,
    
    Monday = 1 << DayOfWeek.Monday,
    Tuesday = 1 << DayOfWeek.Tuesday,
    Wednesday = 1 << DayOfWeek.Wednesday,
    Thursday = 1 << DayOfWeek.Thursday,
    Friday = 1 << DayOfWeek.Friday,
    Saturday = 1 << DayOfWeek.Saturday,
    Sunday = 1 << DayOfWeek.Sunday,
    
    All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday
}

public static class DayOfWeekExtensions
{
    public static bool Contains(this DaysOfWeek daysOfWeek, DayOfWeek dayOfWeek) =>
        (daysOfWeek & ToFlags(dayOfWeek)) != 0;

    public static DaysOfWeek ToFlags(this DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Monday => DaysOfWeek.Monday,
            DayOfWeek.Tuesday => DaysOfWeek.Tuesday,
            DayOfWeek.Wednesday => DaysOfWeek.Wednesday,
            DayOfWeek.Thursday => DaysOfWeek.Thursday,
            DayOfWeek.Friday => DaysOfWeek.Friday,
            DayOfWeek.Saturday => DaysOfWeek.Saturday,
            DayOfWeek.Sunday => DaysOfWeek.Sunday,
            _ => DaysOfWeek.None
        };
}