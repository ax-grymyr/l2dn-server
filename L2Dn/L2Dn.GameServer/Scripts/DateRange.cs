using NLog;

namespace L2Dn.GameServer.Scripts;

/**
 * @author Luis Arias
 */
public class DateRange
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(DateRange));
	private readonly DateOnly _startDate;
	private readonly DateOnly _endDate;
	
	public DateRange(DateOnly from, DateOnly to)
	{
		_startDate = from;
		_endDate = to;
	}
	
	public static DateRange parse(String dateRange, string format)
	{
		String[] date = dateRange.Split("-");
		if (date.Length == 2)
		{
			try
			{
				return new DateRange(format.parse(date[0]), format.parse(date[1]));
			}
			catch (ParseException e)
			{
				LOGGER.Warn("Invalid Date Format: " + e);
			}
		}
		return new DateRange(null, null);
	}
	
	public bool isValid()
	{
		return (_startDate != null) && (_endDate != null) && _startDate.before(_endDate);
	}
	
	public bool isWithinRange(DateOnly date)
	{
		return (date.equals(_startDate) || date.after(_startDate)) //
			&& (date.equals(_endDate) || date.before(_endDate));
	}
	
	public DateOnly getEndDate()
	{
		return _endDate;
	}
	
	public DateOnly getStartDate()
	{
		return _startDate;
	}
	
	public override String ToString()
	{
		return "DateRange: From: " + _startDate + " To: " + _endDate;
	}
}
