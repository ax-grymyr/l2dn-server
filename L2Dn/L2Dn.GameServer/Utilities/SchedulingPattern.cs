namespace L2Dn.GameServer.Utilities;

// TODO: this is stub class currently
// need to decide what to do: rewrite original class from L2J or find 3rd party library
public class SchedulingPattern
{
    public DateTime next(DateTime utcNow)
    {
        throw new NotImplementedException();
    }
}


// using System.Globalization;
//
// namespace L2Dn.GameServer.Utilities;
//
// /**
//  * <p>
//  * A UNIX crontab-like pattern is a string split in five space separated parts. Each part is intented as:
//  * </p>
//  * <ol>
//  * <li><strong>Minutes sub-pattern</strong>. During which minutes of the hour should the task been launched? The values range is from 0 to 59.</li>
//  * <li><strong>Hours sub-pattern</strong>. During which hours of the day should the task been launched? The values range is from 0 to 23.</li>
//  * <li><strong>Days of month sub-pattern</strong>. During which days of the month should the task been launched? The values range is from 1 to 31. The special value L can be used to recognize the last day of month.</li>
//  * <li><strong>Months sub-pattern</strong>. During which months of the year should the task been launched? The values range is from 1 (January) to 12 (December), otherwise this sub-pattern allows the aliases &quot;jan&quot;, &quot;feb&quot;, &quot;mar&quot;, &quot;apr&quot;, &quot;may&quot;,
//  * &quot;jun&quot;, &quot;jul&quot;, &quot;aug&quot;, &quot;sep&quot;, &quot;oct&quot;, &quot;nov&quot; and &quot;dec&quot;.</li>
//  * <li><strong>Days of week sub-pattern</strong>. During which days of the week should the task been launched? The values range is from 0 (Sunday) to 6 (Saturday), otherwise this sub-pattern allows the aliases &quot;sun&quot;, &quot;mon&quot;, &quot;tue&quot;, &quot;wed&quot;, &quot;thu&quot;,
//  * &quot;fri&quot; and &quot;sat&quot;.</li>
//  * </ol>
//  * <p>
//  * The star wildcard character is also admitted, indicating &quot;every minute of the hour&quot;, &quot;every hour of the day&quot;, &quot;every day of the month&quot;, &quot;every month of the year&quot; and &quot;every day of the week&quot;, according to the sub-pattern in which it is used.
//  * </p>
//  * <p>
//  * Once the scheduler is started, a task will be launched when the five parts in its scheduling pattern will be true at the same time.
//  * </p>
//  * <p>
//  * Some examples:
//  * </p>
//  * <p>
//  * <strong>5 * * * *</strong><br />
//  * This pattern causes a task to be launched once every hour, at the begin of the fifth minute (00:05, 01:05, 02:05 etc.).
//  * </p>
//  * <p>
//  * <strong>* * * * *</strong><br />
//  * This pattern causes a task to be launched every minute.
//  * </p>
//  * <p>
//  * <strong>* 12 * * Mon</strong><br />
//  * This pattern causes a task to be launched every minute during the 12th hour of Monday.
//  * </p>
//  * <p>
//  * <strong>* 12 16 * Mon</strong><br />
//  * This pattern causes a task to be launched every minute during the 12th hour of Monday, 16th, but only if the day is the 16th of the month.
//  * </p>
//  * <p>
//  * Every sub-pattern can contain two or more comma separated values.
//  * </p>
//  * <p>
//  * <strong>59 11 * * 1,2,3,4,5</strong><br />
//  * This pattern causes a task to be launched at 11:59AM on Monday, Tuesday, Wednesday, Thursday and Friday.
//  * </p>
//  * <p>
//  * Values intervals are admitted and defined using the minus character.
//  * </p>
//  * <p>
//  * <strong>59 11 * * 1-5</strong><br />
//  * This pattern is equivalent to the previous one.
//  * </p>
//  * <p>
//  * The slash character can be used to identify step values within a range. It can be used both in the form <em>*&#47;c</em> and <em>a-b/c</em>. The subpattern is matched every <em>c</em> values of the range <em>0,maxvalue</em> or <em>a-b</em>.
//  * </p>
//  * <p>
//  * <strong>*&#47;5 * * * *</strong><br />
//  * This pattern causes a task to be launched every 5 minutes (0:00, 0:05, 0:10, 0:15 and so on).
//  * </p>
//  * <p>
//  * <strong>3-18&#47;5 * * * *</strong><br />
//  * This pattern causes a task to be launched every 5 minutes starting from the third minute of the hour, up to the 18th (0:03, 0:08, 0:13, 0:18, 1:03, 1:08 and so on).
//  * </p>
//  * <p>
//  * <strong>*&#47;15 9-17 * * *</strong><br />
//  * This pattern causes a task to be launched every 15 minutes between the 9th and 17th hour of the day (9:00, 9:15, 9:30, 9:45 and so on... note that the last execution will be at 17:45).
//  * </p>
//  * <p>
//  * All the fresh described syntax rules can be used together.
//  * </p>
//  * <p>
//  * <strong>* 12 10-16&#47;2 * *</strong><br />
//  * This pattern causes a task to be launched every minute during the 12th hour of the day, but only if the day is the 10th, the 12th, the 14th or the 16th of the month.
//  * </p>
//  * <p>
//  * <strong>* 12 1-15,17,20-25 * *</strong><br />
//  * This pattern causes a task to be launched every minute during the 12th hour of the day, but the day of the month must be between the 1st and the 15th, the 20th and the 25, or at least it must be the 17th.
//  * </p>
//  * <p>
//  * Finally lets you combine more scheduling patterns into one, with the pipe character:
//  * </p>
//  * <p>
//  * <strong>0 5 * * *|8 10 * * *|22 17 * * *</strong><br />
//  * This pattern causes a task to be launched every day at 05:00, 10:08 and 17:22.
//  * </p>
//  * @author Carlo Pelliccia
//  */
// public class SchedulingPattern
// {
// 	private const int MINUTE_MIN_VALUE = 0;
// 	private const int MINUTE_MAX_VALUE = 59;
// 	private const int HOUR_MIN_VALUE = 0;
// 	private const int HOUR_MAX_VALUE = 23;
// 	private const int DAY_OF_MONTH_MIN_VALUE = 1;
// 	private const int DAY_OF_MONTH_MAX_VALUE = 31;
// 	private const int MONTH_MIN_VALUE = 1;
// 	private const int MONTH_MAX_VALUE = 12;
// 	private const int DAY_OF_WEEK_MIN_VALUE = 0;
// 	private const int DAY_OF_WEEK_MAX_VALUE = 7;
// 	
// 	/**
// 	 * The parser for the minute values.
// 	 */
// 	private static readonly ValueParser MINUTE_VALUE_PARSER = new MinuteValueParser();
// 	
// 	/**
// 	 * The parser for the hour values.
// 	 */
// 	private static readonly ValueParser HOUR_VALUE_PARSER = new HourValueParser();
// 	
// 	/**
// 	 * The parser for the day of month values.
// 	 */
// 	private static readonly ValueParser DAY_OF_MONTH_VALUE_PARSER = new DayOfMonthValueParser();
// 	
// 	/**
// 	 * The parser for the month values.
// 	 */
// 	private static readonly ValueParser MONTH_VALUE_PARSER = new MonthValueParser();
// 	
// 	/**
// 	 * The parser for the day of week values.
// 	 */
// 	private static readonly ValueParser DAY_OF_WEEK_VALUE_PARSER = new DayOfWeekValueParser();
// 	
// 	/**
// 	 * The pattern as a string.
// 	 */
// 	private readonly String _asString;
// 	
// 	/**
// 	 * The ValueMatcher list for the "minute" field.
// 	 */
// 	private List<ValueMatcher> _minuteMatchers = new();
// 	
// 	/**
// 	 * The ValueMatcher list for the "hour" field.
// 	 */
// 	private List<ValueMatcher> _hourMatchers = new();
// 	
// 	/**
// 	 * The ValueMatcher list for the "day of month" field.
// 	 */
// 	private List<ValueMatcher> _dayOfMonthMatchers = new();
// 	
// 	/**
// 	 * The ValueMatcher list for the "month" field.
// 	 */
// 	private List<ValueMatcher> _monthMatchers = new();
// 	
// 	/**
// 	 * The ValueMatcher list for the "day of week" field.
// 	 */
// 	private List<ValueMatcher> _dayOfWeekMatchers = new();
// 	
// 	/**
// 	 * How many matcher groups in this pattern?
// 	 */
// 	protected int _matcherSize = 0;
// 	
// 	protected Map<int, int> _hourAdder = new();
// 	protected Map<int, int> _hourAdderRnd = new();
// 	protected Map<int, int> _dayOfYearAdder = new();
// 	protected Map<int, int> _minuteAdderRnd = new();
// 	protected Map<int, int> _weekOfYearAdder = new();
// 	
// 	/**
// 	 * Validates a string as a scheduling pattern.
// 	 * @param schedulingPattern The pattern to validate.
// 	 * @return true if the given string represents a valid scheduling pattern; false otherwise.
// 	 */
// 	public static bool validate(String schedulingPattern)
// 	{
// 		try
// 		{
// 			new SchedulingPattern(schedulingPattern);
// 		}
// 		catch (Exception e)
// 		{
// 			return false;
// 		}
// 		return true;
// 	}
// 	
// 	/**
// 	 * Builds a SchedulingPattern parsing it from a string.
// 	 * @param pattern The pattern as a crontab-like string.
// 	 * @throws RuntimeException If the supplied string is not a valid pattern.
// 	 */
// 	public SchedulingPattern(String pattern)
// 	{
// 		_asString = pattern;
// 		StringTokenizer st1 = new StringTokenizer(pattern, "|");
// 		if (st1.countTokens() < 1)
// 		{
// 			throw new ArgumentException("invalid pattern: \"" + pattern + "\"");
// 		}
// 		
// 		while (st1.hasMoreTokens())
// 		{
// 			String localPattern = st1.nextToken();
// 			StringTokenizer st2 = new StringTokenizer(localPattern, " \t");
// 			if (st2.countTokens() != 5)
// 			{
// 				throw new ArgumentException("invalid pattern: \"" + localPattern + "\"");
// 			}
// 			
// 			try
// 			{
// 				String minutePattern = st2.nextToken();
// 				String[] minutePatternParts = minutePattern.Split(":");
// 				if (minutePatternParts.Length > 1)
// 				{
// 					for (int i = 0; i < (minutePatternParts.Length - 1); ++i)
// 					{
// 						if (minutePatternParts[i].Length <= 1)
// 						{
// 							continue;
// 						}
// 						
// 						if (minutePatternParts[i].startsWith("~"))
// 						{
// 							_minuteAdderRnd.put(_matcherSize, int.Parse(minutePatternParts[i].Substring(1)));
// 							continue;
// 						}
// 						
// 						throw new ArgumentException("Unknown hour modifier \"" + minutePatternParts[i] + "\"");
// 					}
// 					minutePattern = minutePatternParts[minutePatternParts.Length - 1];
// 				}
// 				
// 				_minuteMatchers.add(buildValueMatcher(minutePattern, MINUTE_VALUE_PARSER));
// 			}
// 			catch (Exception e)
// 			{
// 				throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing minutes field: " + e + ".");
// 			}
// 			
// 			try
// 			{
// 				String hourPattern = st2.nextToken();
// 				String[] hourPatternParts = hourPattern.Split(":");
// 				if (hourPatternParts.Length > 1)
// 				{
// 					for (int i = 0; i < (hourPatternParts.Length - 1); ++i)
// 					{
// 						if (hourPatternParts[i].Length <= 1)
// 						{
// 							continue;
// 						}
// 						
// 						if (hourPatternParts[i].startsWith("+"))
// 						{
// 							_hourAdder.put(_matcherSize, int.Parse(hourPatternParts[i].Substring(1)));
// 							continue;
// 						}
// 						
// 						if (hourPatternParts[i].startsWith("~"))
// 						{
// 							_hourAdderRnd.put(_matcherSize, int.Parse(hourPatternParts[i].Substring(1)));
// 							continue;
// 						}
// 						
// 						throw new ArgumentException("Unknown hour modifier \"" + hourPatternParts[i] + "\"");
// 					}
// 					hourPattern = hourPatternParts[hourPatternParts.Length - 1];
// 				}
// 				
// 				_hourMatchers.add(buildValueMatcher(hourPattern, HOUR_VALUE_PARSER));
// 			}
// 			
// 			catch (Exception e)
// 			{
// 				throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing hours field: " + e + ".");
// 			}
// 			
// 			try
// 			{
// 				String dayOfMonthPattern = st2.nextToken();
// 				String[] dayOfMonthPatternParts = dayOfMonthPattern.Split(":");
// 				if (dayOfMonthPatternParts.Length > 1)
// 				{
// 					for (int i = 0; i < (dayOfMonthPatternParts.Length - 1); ++i)
// 					{
// 						if (dayOfMonthPatternParts[i].Length <= 1)
// 						{
// 							continue;
// 						}
// 						
// 						if (dayOfMonthPatternParts[i].startsWith("+"))
// 						{
// 							_dayOfYearAdder.put(_matcherSize, int.Parse(dayOfMonthPatternParts[i].Substring(1)));
// 							continue;
// 						}
// 						
// 						throw new ArgumentException("Unknown day modifier \"" + dayOfMonthPatternParts[i] + "\"");
// 					}
// 					dayOfMonthPattern = dayOfMonthPatternParts[dayOfMonthPatternParts.Length - 1];
// 				}
// 				
// 				_dayOfMonthMatchers.add(buildValueMatcher(dayOfMonthPattern, DAY_OF_MONTH_VALUE_PARSER));
// 			}
// 			catch (Exception e)
// 			{
// 				throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing days of month field: " + e + ".");
// 			}
// 			
// 			try
// 			{
// 				_monthMatchers.add(buildValueMatcher(st2.nextToken(), MONTH_VALUE_PARSER));
// 			}
// 			catch (Exception e)
// 			{
// 				throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing months field: " + e + ".");
// 			}
// 			
// 			try
// 			{
// 				_dayOfWeekMatchers.add(buildValueMatcher(st2.nextToken(), DAY_OF_WEEK_VALUE_PARSER));
// 			}
// 			catch (Exception e)
// 			{
// 				throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing days of week field: " + e + ".");
// 			}
// 			
// 			if (st2.hasMoreTokens())
// 			{
// 				try
// 				{
// 					String weekOfYearAdderText = st2.nextToken();
// 					if (weekOfYearAdderText[0] != '+')
// 					{
// 						throw new ArgumentException("Unknown week of year addition in pattern \"" + localPattern + "\".");
// 					}
// 					weekOfYearAdderText = weekOfYearAdderText.Substring(1);
// 					_weekOfYearAdder.put(_matcherSize, int.Parse(weekOfYearAdderText));
// 				}
// 				catch (Exception e)
// 				{
// 					throw new ArgumentException("invalid pattern \"" + localPattern + "\". Error parsing days of week field: " + e + ".");
// 				}
// 			}
// 			
// 			_matcherSize++;
// 		}
// 	}
// 	
// 	/**
// 	 * A ValueMatcher utility builder.
// 	 * @param str The pattern part for the ValueMatcher creation.
// 	 * @param parser The parser used to parse the values.
// 	 * @return The requested ValueMatcher.
// 	 * @throws Exception If the supplied pattern part is not valid.
// 	 */
// 	private ValueMatcher buildValueMatcher(String str, ValueParser parser)
// 	{
// 		if ((str.Length == 1) && str.equals("*"))
// 		{
// 			return new AlwaysTrueValueMatcher();
// 		}
// 		
// 		List<int> values = new();
// 		StringTokenizer st = new StringTokenizer(str, ",");
// 		while (st.hasMoreTokens())
// 		{
// 			String element = st.nextToken();
// 			List<int> local;
// 			try
// 			{
// 				local = parseListElement(element, parser);
// 			}
// 			catch (Exception e)
// 			{
// 				throw new Exception("invalid field \"" + str + "\", invalid element \"" + element + "\", " + e);
// 			}
// 			
// 			foreach (int value in local)
// 			{
// 				if (values.Contains(value))
// 				{
// 					continue;
// 				}
// 				
// 				values.add(value);
// 			}
// 		}
// 		
// 		if (values.size() == 0)
// 		{
// 			throw new Exception("invalid field \"" + str + "\"");
// 		}
// 		
// 		if (parser == DAY_OF_MONTH_VALUE_PARSER)
// 		{
// 			return new DayOfMonthValueMatcher(values);
// 		}
// 		
// 		return new IntArrayValueMatcher(values);
// 	}
// 	
// 	/**
// 	 * Parses an element of a list of values of the pattern.
// 	 * @param str The element string.
// 	 * @param parser The parser used to parse the values.
// 	 * @return A list of integers representing the allowed values.
// 	 * @throws Exception If the supplied pattern part is not valid.
// 	 */
// 	private List<int> parseListElement(String str, ValueParser parser)
// 	{
// 		StringTokenizer st = new StringTokenizer(str, "/");
// 		int size = st.countTokens();
// 		if ((size < 1) || (size > 2))
// 		{
// 			throw new Exception("syntax error");
// 		}
// 		
// 		List<int> values;
// 		try
// 		{
// 			values = parseRange(st.nextToken(), parser);
// 		}
// 		catch (Exception e)
// 		{
// 			throw new Exception("invalid range, " + e);
// 		}
// 		
// 		if (size == 2)
// 		{
// 			String dStr = st.nextToken();
// 			int div;
// 			try
// 			{
// 				div = int.Parse(dStr);
// 			}
// 			catch (FormatException e)
// 			{
// 				throw new Exception("invalid divisor \"" + dStr + "\"");
// 			}
// 			
// 			if (div < 1)
// 			{
// 				throw new Exception("non positive divisor \"" + div + "\"");
// 			}
// 			
// 			List<int> values2 = new();
// 			for (int i = 0; i < values.size(); i += div)
// 			{
// 				values2.add(values.get(i));
// 			}
// 			return values2;
// 		}
// 		
// 		return values;
// 	}
// 	
// 	/**
// 	 * Parses a range of values.
// 	 * @param str The range string.
// 	 * @param parser The parser used to parse the values.
// 	 * @return A list of integers representing the allowed values.
// 	 * @throws Exception If the supplied pattern part is not valid.
// 	 */
// 	private List<int> parseRange(String str, ValueParser parser)
// 	{
// 		if (str.equals("*"))
// 		{
// 			int min = parser.getMinValue();
// 			int max = parser.getMaxValue();
// 			List<int> values = new();
// 			for (int i = min; i <= max; i++)
// 			{
// 				values.add(i);
// 			}
// 			return values;
// 		}
// 		
// 		StringTokenizer st = new StringTokenizer(str, "-");
// 		int size = st.countTokens();
// 		if ((size < 1) || (size > 2))
// 		{
// 			throw new Exception("syntax error");
// 		}
// 		
// 		String v1Str = st.nextToken();
// 		int v1;
// 		try
// 		{
// 			v1 = parser.parse(v1Str);
// 		}
// 		catch (Exception e)
// 		{
// 			throw new Exception("invalid value \"" + v1Str + "\", " + e);
// 		}
// 		
// 		if (size == 1)
// 		{
// 			List<int> values = new();
// 			values.add(v1);
// 			return values;
// 		}
// 		
// 		String v2Str = st.nextToken();
// 		int v2;
// 		try
// 		{
// 			v2 = parser.parse(v2Str);
// 		}
// 		catch (Exception e)
// 		{
// 			throw new Exception("invalid value \"" + v2Str + "\", " + e);
// 		}
// 		
// 		List<int> values1 = new();
// 		if (v1 < v2)
// 		{
// 			for (int i = v1; i <= v2; i++)
// 			{
// 				values1.add(i);
// 			}
// 		}
// 		else if (v1 > v2)
// 		{
// 			int min = parser.getMinValue();
// 			int max = parser.getMaxValue();
// 			for (int i = v1; i <= max; i++)
// 			{
// 				values1.add(i);
// 			}
// 			for (int i = min; i <= v2; i++)
// 			{
// 				values1.add(i);
// 			}
// 		}
// 		else
// 		{
// 			// v1 == v2
// 			values1.add(v1);
// 		}
// 		return values1;
// 	}
//
// 	/**
// 	 * This methods returns true if the given timestamp (expressed as a UNIX-era millis value) matches the pattern, according to the given time zone.
// 	 * @param timezone A time zone.
// 	 * @param millis The timestamp, as a UNIX-era millis value.
// 	 * @return true if the given timestamp matches the pattern.
// 	 */
// 	public bool match(TimeZoneInfo timeZone, DateTime utcTime)
// 	{
// 		DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
//
// 		for (int i = 0; i < _matcherSize; ++i)
// 		{
// 			if (_weekOfYearAdder.containsKey(i))
// 				localTime = localTime.AddDays(-_weekOfYearAdder.get(i) * 7);
//
// 			if (_dayOfYearAdder.containsKey(i))
// 				localTime = localTime.AddDays(-_dayOfYearAdder.get(i));
//
// 			if (_hourAdder.containsKey(i))
// 				localTime = localTime.AddHours(-_hourAdder.get(i));
//
// 			int minute = localTime.Minute;
// 			int hour = localTime.Hour;
// 			int dayOfMonth = localTime.Day;
// 			int month = localTime.Month;
// 			int dayOfWeek = 1 + (int)localTime.DayOfWeek;
// 			int year = localTime.Year;
// 			
// 			ValueMatcher minuteMatcher = _minuteMatchers.get(i);
// 			ValueMatcher hourMatcher = _hourMatchers.get(i);
// 			ValueMatcher dayOfMonthMatcher = _dayOfMonthMatchers.get(i);
// 			ValueMatcher monthMatcher = _monthMatchers.get(i);
// 			ValueMatcher dayOfWeekMatcher = _dayOfWeekMatchers.get(i);
//
// 			GregorianCalendar calendar = new();
// 			bool isLeapYear = calendar.IsLeapYear(year); 
// 			
// 			if (minuteMatcher.match(minute) && hourMatcher.match(hour) && ((dayOfMonthMatcher is DayOfMonthValueMatcher)
// 				    ? ((DayOfMonthValueMatcher)dayOfMonthMatcher).match(dayOfMonth, month, isLeapYear)
// 				    : dayOfMonthMatcher.match(dayOfMonth)) && monthMatcher.match(month) &&
// 			    dayOfWeekMatcher.match(dayOfWeek))
// 			{
// 				return true;
// 			}
// 		}
//
// 		return false;
// 	}
//
// 	/**
// 	 * This methods returns true if the given timestamp (expressed as a UNIX-era millis value) matches the pattern, according to the system default time zone.
// 	 * @param millis The timestamp, as a UNIX-era millis value.
// 	 * @return true if the given timestamp matches the pattern.
// 	 */
// 	public bool match(DateTime utcTime)
// 	{
// 		return match(TimeZoneInfo.Local, utcTime);
// 	}
// 	
// 	/**
// 	 * It returns the next matching moment as a millis value.
// 	 * @param timezone
// 	 * @param millis
// 	 * @return The next matching moment as a millis value.
// 	 */
// 	public long next(TimeZoneInfo timeZone, DateTime utcTime)
// 	{
// 		long result = -1;
// 		for (int i = 0; i < _matcherSize; ++i)
// 		{
// 			long next = -1;
// 			gc.setTimeInMillis(millis);
// 			gc.set(13, 0);
// 			gc.set(14, 0);
// 			if (_weekOfYearAdder.containsKey(i))
// 			{
// 				gc.add(3, _weekOfYearAdder.get(i));
// 			}
// 			if (_dayOfYearAdder.containsKey(i))
// 			{
// 				gc.add(6, _dayOfYearAdder.get(i));
// 			}
// 			if (_hourAdder.containsKey(i))
// 			{
// 				gc.add(10, _hourAdder.get(i));
// 			}
// 			ValueMatcher minuteMatcher = _minuteMatchers.get(i);
// 			ValueMatcher hourMatcher = _hourMatchers.get(i);
// 			ValueMatcher dayOfMonthMatcher = _dayOfMonthMatchers.get(i);
// 			ValueMatcher monthMatcher = _monthMatchers.get(i);
// 			ValueMatcher dayOfWeekMatcher = _dayOfWeekMatchers.get(i);
// 			
// 			SEARCH: do
// 			{
// 				int year = gc.get(1);
// 				bool isLeapYear = gc.isLeapYear(year);
// 				for (int month = gc.get(2) + 1; month <= MONTH_MAX_VALUE; ++month)
// 				{
// 					if (monthMatcher.match(month))
// 					{
// 						gc.set(2, month - 1);
// 						int maxDayOfMonth = DayOfMonthValueMatcher.getLastDayOfMonth(month, isLeapYear);
// 						for (int dayOfMonth = gc.get(5); dayOfMonth <= maxDayOfMonth; ++dayOfMonth)
// 						{
// 							if (dayOfMonthMatcher is DayOfMonthValueMatcher ? ((DayOfMonthValueMatcher) dayOfMonthMatcher).match(dayOfMonth, month, isLeapYear) : dayOfMonthMatcher.match(dayOfMonth))
// 							{
// 								gc.set(5, dayOfMonth);
// 								int dayOfWeek = gc.get(DAY_OF_WEEK_MAX_VALUE) - 1;
// 								if (dayOfWeekMatcher.match(dayOfWeek))
// 								{
// 									for (int hour = gc.get(11); hour <= HOUR_MAX_VALUE; ++hour)
// 									{
// 										if (hourMatcher.match(hour))
// 										{
// 											gc.set(11, hour);
// 											for (int minute = gc.get(MONTH_MAX_VALUE); minute <= MINUTE_MAX_VALUE; ++minute)
// 											{
// 												if (!minuteMatcher.match(minute))
// 												{
// 													continue;
// 												}
// 												
// 												gc.set(MONTH_MAX_VALUE, minute);
// 												long next0 = gc.getTimeInMillis();
// 												if (next0 <= millis)
// 												{
// 													continue;
// 												}
// 												
// 												if ((next != -1) && (next0 >= next))
// 												{
// 													break SEARCH;
// 												}
// 												
// 												next = next0;
// 												if (_hourAdderRnd.containsKey(i))
// 												{
// 													next += Rnd.get(_hourAdderRnd.get(i)) * 60 * 60 * 1000;
// 												}
// 												
// 												if (!_minuteAdderRnd.containsKey(i))
// 												{
// 													break SEARCH;
// 												}
// 												
// 												next += Rnd.get(_minuteAdderRnd.get(i)) * 60 * 1000;
// 												break SEARCH;
// 											}
// 										}
// 										gc.set(MONTH_MAX_VALUE, 0);
// 									}
// 								}
// 							}
// 							gc.set(11, 0);
// 							gc.set(MONTH_MAX_VALUE, 0);
// 						}
// 					}
// 					gc.set(5, 1);
// 					gc.set(11, 0);
// 					gc.set(MONTH_MAX_VALUE, 0);
// 				}
// 				gc.set(2, 0);
// 				gc.set(11, 0);
// 				gc.set(MONTH_MAX_VALUE, 0);
// 				gc.roll(1, true);
// 			}
// 			
// 			while (true);
// 			if ((next <= millis) || ((result != -1) && (next >= result)))
// 			{
// 				continue;
// 			}
// 			
// 			result = next;
// 		}
// 		return result;
// 	}
// 	
// 	/**
// 	 * It returns the next matching moment as a long.
// 	 * @param millis
// 	 * @return The next matching moment as a long.
// 	 */
// 	public long next(DateTime utcTime)
// 	{
// 		return next(TimeZoneInfo.Local, utcTime);
// 	}
// 	
// 	/**
// 	 * Returns the pattern as a string.
// 	 * @return The pattern as a string.
// 	 */
// 	public override String ToString()
// 	{
// 		return _asString;
// 	}
// 	
// 	/**
// 	 * This utility method changes an alias to an int value.
// 	 * @param value The value.
// 	 * @param aliases The aliases list.
// 	 * @param offset The offset appplied to the aliases list indices.
// 	 * @return The parsed value.
// 	 * @throws Exception If the expressed values doesn't match any alias.
// 	 */
// 	protected static int parseAlias(String value, String[] aliases, int offset)
// 	{
// 		for (int i = 0; i < aliases.Length; i++)
// 		{
// 			if (aliases[i].equalsIgnoreCase(value))
// 			{
// 				return offset + i;
// 			}
// 		}
// 		throw new Exception("invalid alias \"" + value + "\"");
// 	}
// 	
// 	/**
// 	 * <p>
// 	 * A ValueMatcher whose rules are in a plain array of integer values. When asked to validate a value, this ValueMatcher checks if it is in the array and, if not, checks whether the last-day-of-month setting applies.
// 	 * </p>
// 	 * @author Paul Fernley
// 	 */
// 	private class DayOfMonthValueMatcher: IntArrayValueMatcher
// 	{
// 		private static int[] LAST_DAYS =
// 		{
// 			31,
// 			28,
// 			31,
// 			30,
// 			31,
// 			30,
// 			31,
// 			31,
// 			30,
// 			31,
// 			30,
// 			31
// 		};
// 		
// 		/**
// 		 * Builds the ValueMatcher.
// 		 * @param integers An ArrayList of int elements, one for every value accepted by the matcher. The match() method will return true only if its parameter will be one of this list or the last-day-of-month setting applies.
// 		 */
// 		public DayOfMonthValueMatcher(List<int> integers): base(integers)
// 		{
// 		}
// 		
// 		/**
// 		 * Returns true if the given value is included in the matcher list or the last-day-of-month setting applies.
// 		 * @param value
// 		 * @param month
// 		 * @param isLeapYear
// 		 * @return
// 		 */
// 		public bool match(int value, int month, bool isLeapYear)
// 		{
// 			return (base.match(value) || ((value > 27) && match(32) && isLastDayOfMonth(value, month, isLeapYear)));
// 		}
// 		
// 		public static int getLastDayOfMonth(int month, bool isLeapYear)
// 		{
// 			if (isLeapYear && (month == 2))
// 			{
// 				return 29;
// 			}
// 			
// 			return LAST_DAYS[month - 1];
// 		}
// 		
// 		public static bool isLastDayOfMonth(int value, int month, bool isLeapYear)
// 		{
// 			return value == getLastDayOfMonth(month, isLeapYear);
// 		}
// 	}
// 	
// 	/**
// 	 * <p>
// 	 * A ValueMatcher whose rules are in a plain array of integer values. When asked to validate a value, this ValueMatcher checks if it is in the array.
// 	 * </p>
// 	 * @author Carlo Pelliccia
// 	 */
// 	private class IntArrayValueMatcher: ValueMatcher
// 	{
// 		/**
// 		 * The accepted values.
// 		 */
// 		private int[] _values;
// 		
// 		/**
// 		 * Builds the ValueMatcher.
// 		 * @param integers a List of int elements, one for every value accepted by the matcher. The match() method will return true only if its parameter will be one of this list.
// 		 */
// 		public IntArrayValueMatcher(List<int> integers)
// 		{
// 			_values = integers.ToArray();
// 		}
// 		
// 		/**
// 		 * Returns true if the given value is included in the matcher list.
// 		 */
// 		public bool match(int value)
// 		{
// 			for (int i = 0; i < _values.Length; i++)
// 			{
// 				if (_values[i] == value)
// 				{
// 					return true;
// 				}
// 			}
// 			return false;
// 		}
// 	}
// 	
// 	/**
// 	 * This ValueMatcher always returns true!
// 	 * @author Carlo Pelliccia
// 	 */
// 	protected class AlwaysTrueValueMatcher: ValueMatcher
// 	{
// 		/**
// 		 * Always true!
// 		 */
// 		public bool match(int value)
// 		{
// 			return true;
// 		}
// 	}
// 	
// 	/**
// 	 * <p>
// 	 * This interface describes the ValueMatcher behavior. A ValueMatcher is an object that validate an integer value against a set of rules.
// 	 * </p>
// 	 * @author Carlo Pelliccia
// 	 */
// 	private interface ValueMatcher
// 	{
// 		/**
// 		 * Validate the given integer value against a set of rules.
// 		 * @param value The value.
// 		 * @return true if the given value matches the rules of the ValueMatcher, false otherwise.
// 		 */
// 		public bool match(int value);
// 	}
// 	
// 	/**
// 	 * The value parser for the day of week field.
// 	 */
// 	private class DayOfWeekValueParser: SimpleValueParser
// 	{
// 		/**
// 		 * Days of week aliases.
// 		 */
// 		private static String[] ALIASES =
// 		{
// 			"sun",
// 			"mon",
// 			"tue",
// 			"wed",
// 			"thu",
// 			"fri",
// 			"sat"
// 		};
// 		
// 		/**
// 		 * Builds the day value parser.
// 		 */
// 		public DayOfWeekValueParser(): base(DAY_OF_WEEK_MIN_VALUE, DAY_OF_WEEK_MAX_VALUE)
// 		{
// 		}
// 		
// 		public override int parse(String value)
// 		{
// 			try
// 			{
// 				// try as a simple value
// 				return base.parse(value) % 7;
// 			}
// 			catch (Exception e)
// 			{
// 				// try as an alias
// 				return parseAlias(value, ALIASES, 0);
// 			}
// 		}
// 	}
// 	
// 	/**
// 	 * The value parser for the months field.
// 	 */
// 	private class MonthValueParser: SimpleValueParser
// 	{
// 		/**
// 		 * Months of year aliases.
// 		 */
// 		private static String[] ALIASES = new String[]
// 		{
// 			"jan",
// 			"feb",
// 			"mar",
// 			"apr",
// 			"may",
// 			"jun",
// 			"jul",
// 			"aug",
// 			"sep",
// 			"oct",
// 			"nov",
// 			"dec"
// 		};
// 		
// 		/**
// 		 * Builds the months value parser.
// 		 */
// 		public MonthValueParser(): base(MONTH_MIN_VALUE, MONTH_MAX_VALUE)
// 		{
// 		}
// 		
// 		public override int parse(String value)
// 		{
// 			try
// 			{
// 				return base.parse(value);
// 			}
// 			catch (Exception e)
// 			{
// 				return parseAlias(value, ALIASES, 1);
// 			}
// 		}
// 	}
// 	
// 	/**
// 	 * The value parser for the day of month field.
// 	 */
// 	private class DayOfMonthValueParser: SimpleValueParser
// 	{
// 		/**
// 		 * Builds the value parser.
// 		 */
// 		public DayOfMonthValueParser(): base(DAY_OF_MONTH_MIN_VALUE, DAY_OF_MONTH_MAX_VALUE)
// 		{
// 		}
// 		
// 		public override int parse(String value)
// 		{
// 			if (value.equalsIgnoreCase("L"))
// 			{
// 				return 32;
// 			}
// 			
// 			return base.parse(value);
// 		}
// 	}
// 	
// 	/**
// 	 * The value parser for the hour field.
// 	 */
// 	private class HourValueParser: SimpleValueParser
// 	{
// 		/**
// 		 * Builds the value parser.
// 		 */
// 		public HourValueParser(): base(HOUR_MIN_VALUE, HOUR_MAX_VALUE)
// 		{
// 		}
// 	}
// 	
// 	/**
// 	 * The minutes value parser.
// 	 */
// 	private class MinuteValueParser: SimpleValueParser
// 	{
// 		/**
// 		 * Builds the value parser.
// 		 */
// 		public MinuteValueParser(): base(MINUTE_MIN_VALUE, MINUTE_MAX_VALUE)
// 		{
// 		}
// 	}
// 	
// 	/**
// 	 * A simple value parser.
// 	 */
// 	private class SimpleValueParser: ValueParser
// 	{
// 		/**
// 		 * The minimum allowed value.
// 		 */
// 		protected int _minValue;
// 		
// 		/**
// 		 * The maximum allowed value.
// 		 */
// 		protected int _maxValue;
// 		
// 		/**
// 		 * Builds the value parser.
// 		 * @param minValue The minimum allowed value.
// 		 * @param maxValue The maximum allowed value.
// 		 */
// 		public SimpleValueParser(int minValue, int maxValue)
// 		{
// 			_minValue = minValue;
// 			_maxValue = maxValue;
// 		}
// 		
// 		public virtual int parse(String value)
// 		{
// 			int i;
// 			try
// 			{
// 				i = int.Parse(value);
// 			}
// 			catch (FormatException e)
// 			{
// 				throw new Exception("invalid integer value");
// 			}
// 			if ((i < _minValue) || (i > _maxValue))
// 			{
// 				throw new Exception("value out of range");
// 			}
// 			return i;
// 		}
// 		
// 		public int getMinValue()
// 		{
// 			return _minValue;
// 		}
// 		
// 		public int getMaxValue()
// 		{
// 			return _maxValue;
// 		}
// 	}
// 	
// 	/**
// 	 * Definition for a value parser.
// 	 */
// 	private interface ValueParser
// 	{
// 		/**
// 		 * Attempts to parse a value.
// 		 * @param value The value.
// 		 * @return The parsed value.
// 		 * @throws Exception If the value can't be parsed.
// 		 */
// 		public int parse(String value);
// 		
// 		/**
// 		 * Returns the minimum value accepted by the parser.
// 		 * @return The minimum value accepted by the parser.
// 		 */
// 		public int getMinValue();
// 		
// 		/**
// 		 * Returns the maximum value accepted by the parser.
// 		 * @return The maximum value accepted by the parser.
// 		 */
// 		public int getMaxValue();
// 	}
// }
