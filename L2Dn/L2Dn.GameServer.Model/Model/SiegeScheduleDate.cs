namespace L2Dn.GameServer.Model;

public class SiegeScheduleDate
{
    private readonly DayOfWeek _day;
    private readonly int _hour;
    private readonly int _maxConcurrent;
    private readonly bool _siegeEnabled;
	
    public SiegeScheduleDate(DayOfWeek day, int hour, int maxConcurrent, bool siegeEnabled)
    {
        _day = day;
        _hour = hour;
        _maxConcurrent = maxConcurrent;
        _siegeEnabled = siegeEnabled;
    }
	
    public DayOfWeek getDay()
    {
        return _day;
    }
	
    public int getHour()
    {
        return _hour;
    }
	
    public int getMaxConcurrent()
    {
        return _maxConcurrent;
    }
	
    public bool siegeEnabled()
    {
        return _siegeEnabled;
    }
}