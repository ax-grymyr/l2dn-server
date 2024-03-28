namespace L2Dn.GameServer.Model.Holders;

/**
 * Simple class for storing Reenter Data for Instances.
 * @author FallenAngel
 */
public class InstanceReenterTimeHolder
{
	private DayOfWeek? _day;
	private int? _hour;
	private int? _minute;
	private TimeSpan? _time;

	public InstanceReenterTimeHolder(TimeSpan time)
	{
		_time = time;
	}

	public InstanceReenterTimeHolder(DayOfWeek day, int hour, int minute)
	{
		_day = day;
		_hour = hour;
		_minute = minute;
	}

	public TimeSpan? getTime()
	{
		return _time;
	}

	public DayOfWeek? getDay()
	{
		return _day;
	}

	public int? getHour()
	{
		return _hour;
	}

	public int? getMinute()
	{
		return _minute;
	}
}