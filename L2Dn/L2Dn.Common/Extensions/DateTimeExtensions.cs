namespace L2Dn.Extensions;

public static class DateTimeExtensions
{
    public static int getEpochSecond(this DateTime time)
    {
        var diff = time - DateTime.UnixEpoch;
        if (diff == TimeSpan.Zero)
            return 0;
        
        return (int)diff.TotalSeconds;
    }
}