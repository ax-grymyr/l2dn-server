using NLog;

namespace L2Dn.GameServer.Scripts;

/**
 * @author Luis Arias
 */
public class DateRange
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(DateRange));
	private readonly DateTime _startTime;
	private readonly DateTime _endTime;
	
	public DateRange(DateTime from, DateTime to)
	{
		if (from >= to)
			throw new ArgumentException();
			
		_startTime = from;
		_endTime = to;
	}
	
	public static DateRange parse(string dateRange, string format)
	{
		string[] date = dateRange.Split("-");
		if (date.Length != 2)
			throw new FormatException();

		DateTime from = DateTime.ParseExact(date[0], format, null);
		DateTime to = DateTime.ParseExact(date[1], format, null);
		return new DateRange(from, to);
	}
	
	public bool isWithinRange(DateTime time)
	{
		return _startTime <= time && time <= _endTime;
	}
	
	public DateTime getEndDate()
	{
		return _endTime;
	}
	
	public DateTime getStartDate()
	{
		return _startTime;
	}
	
	public override string ToString()
	{
		return "DateRange: From: " + _startTime + " To: " + _endTime;
	}
}
