namespace L2Dn.GameServer.Model.Holders;

/**
 * Simple class for storing Reenter Data for Instances.
 * @author FallenAngel
 */
public class InstanceReenterTimeHolder
{
	private DayOfWeek _day = null;
	private int _hour = -1;
	private int _minute = -1;
	private long _time = -1;

	public InstanceReenterTimeHolder(long time)
	{
		_time = TimeUnit.MINUTES.toMillis(time);
	}

	public InstanceReenterTimeHolder(DayOfWeek day, int hour, int minute)
	{
		_day = day;
		_hour = hour;
		_minute = minute;
	}

	public long getTime()
	{
		return _time;
	}

	public DayOfWeek getDay()
	{
		return _day;
	}

	public int getHour()
	{
		return _hour;
	}

	public int getMinute()
	{
		return _minute;
	}
}