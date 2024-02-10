namespace L2Dn.GameServer.Model;

public class SiegeScheduleDate
{
    private readonly int _day;
    private readonly int _hour;
    private readonly int _maxConcurrent;
    private readonly bool _siegeEnabled;
	
    public SiegeScheduleDate(StatSet set)
    {
        _day = set.getInt("day", Calendar.SUNDAY);
        _hour = set.getInt("hour", 16);
        _maxConcurrent = set.getInt("maxConcurrent", 5);
        _siegeEnabled = set.getBoolean("siegeEnabled", false);
    }
	
    public int getDay()
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
