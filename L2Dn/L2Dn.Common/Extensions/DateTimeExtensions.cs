namespace L2Dn.Extensions;

public static class DateTimeExtensions
{
    public static int getEpochSecond(this DateTime time)
    {
        return (int)(time - DateTime.UnixEpoch).TotalSeconds;
    }
}