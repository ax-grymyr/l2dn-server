using System.Runtime.CompilerServices;

namespace L2Dn.GameServer.Model.ItemAuction;

public class AuctionDateGenerator
{
	public const string FIELD_INTERVAL = "interval";
	public const string FIELD_DAY_OF_WEEK = "day_of_week";
	public const string FIELD_HOUR_OF_DAY = "hour_of_day";
	public const string FIELD_MINUTE_OF_HOUR = "minute_of_hour";
	
	private DateTime _calendar;
	
	private readonly int _interval;
	private DayOfWeek? _dayOfWeek;
	private int _hourOfDay;
	private int _minuteOfHour;
	
	public AuctionDateGenerator(StatSet config)
	{
		_calendar = DateTime.UtcNow;
		_interval = config.getInt(FIELD_INTERVAL, -1);
		// NC week start in Monday.
		int fixedDayWeek = config.getInt(FIELD_DAY_OF_WEEK, -1);
		_dayOfWeek = fixedDayWeek == -1 ? null : (DayOfWeek)fixedDayWeek;
		_hourOfDay = config.getInt(FIELD_HOUR_OF_DAY, -1);
		_minuteOfHour = config.getInt(FIELD_MINUTE_OF_HOUR, -1);
		checkDayOfWeek(null);
		checkHourOfDay(-1);
		checkMinuteOfHour(0);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public DateTime nextDate(DateTime date)
	{
		_calendar = new DateTime(date.Year, date.Month, date.Day, _hourOfDay, _minuteOfHour, 0);
		if (_dayOfWeek > 0)
		{
			while (_calendar.DayOfWeek != _dayOfWeek)
			{
				_calendar = _calendar.AddDays(1);
			}

			return calcDestTime(_calendar, date, TimeSpan.FromDays(7));
		}
		
		return calcDestTime(_calendar, date, TimeSpan.FromDays(_interval));
	}
	
	private static DateTime calcDestTime(DateTime timeValue, DateTime date, TimeSpan add)
	{
		DateTime time = timeValue;
		if (time < date)
		{
			time += ((date - time) / add) * add;
			if (time < date)
			{
				time += add;
			}
		}
		
		return time;
	}
	
	private void checkDayOfWeek(DayOfWeek? defaultValue)
	{
		if ((_dayOfWeek < DayOfWeek.Sunday) || (_dayOfWeek > DayOfWeek.Saturday))
		{
			if ((defaultValue == null) && (_interval < 1))
			{
				throw new ArgumentException("Illegal params for '" + FIELD_DAY_OF_WEEK + "': " + (_dayOfWeek == null ? "not found" : _dayOfWeek));
			}
			
			_dayOfWeek = defaultValue;
		}
		else if (_interval > 1)
		{
			throw new ArgumentException("Illegal params for '" + FIELD_INTERVAL + "' and '" + FIELD_DAY_OF_WEEK + "': you can use only one, not both");
		}
	}
	
	private void checkHourOfDay(int defaultValue)
	{
		if ((_hourOfDay < 0) || (_hourOfDay > 23))
		{
			if (defaultValue == -1)
			{
				throw new ArgumentException("Illegal params for '" + FIELD_HOUR_OF_DAY + "': " + (_hourOfDay == -1 ? "not found" : _hourOfDay));
			}
			_hourOfDay = defaultValue;
		}
	}
	
	private void checkMinuteOfHour(int defaultValue)
	{
		if ((_minuteOfHour < 0) || (_minuteOfHour > 59))
		{
			if (defaultValue == -1)
			{
				throw new ArgumentException("Illegal params for '" + FIELD_MINUTE_OF_HOUR + "': " + (_minuteOfHour == -1 ? "not found" : _minuteOfHour));
			}
			_minuteOfHour = defaultValue;
		}
	}
}