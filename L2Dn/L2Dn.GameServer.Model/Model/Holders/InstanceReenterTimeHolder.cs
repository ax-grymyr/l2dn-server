namespace L2Dn.GameServer.Model.Holders;

/// <summary>
/// Simple class for storing Reenter Data for Instances.
/// </summary>
public sealed class InstanceReenterTimeHolder
{
    private readonly bool _isInterval;
    private readonly TimeSpan _interval;
    private readonly DayOfWeek? _day;
    private readonly int _hour;
    private readonly int _minute;

    public InstanceReenterTimeHolder(TimeSpan interval)
    {
        _isInterval = true;
        _interval = interval;
    }

    public InstanceReenterTimeHolder(DayOfWeek? day, int hour, int minute)
    {
        _isInterval = false;
        _day = day;
        _hour = hour;
        _minute = minute;
    }

    public bool IsInterval => _isInterval;
    public TimeSpan Interval => _interval;
    public DayOfWeek? Day => _day;
    public int Hour => _hour;
    public int Minute => _minute;
}