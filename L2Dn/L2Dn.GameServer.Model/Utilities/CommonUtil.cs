namespace L2Dn.GameServer.Utilities;

public static class CommonUtil
{
	private static readonly char[] ILLEGAL_CHARACTERS =
	{
		'/',
		'\n',
		'\r',
		'\t',
		'\0',
		'\f',
		'`',
		'?',
		'*',
		'\\',
		'<',
		'>',
		'|',
		'\"',
		':'
	};
	
	/**
	 * Method to generate the hexadecimal representation of a byte array.<br>
	 * 16 bytes per row, while ascii chars or "." is shown at the end of the line.
	 * @param data the byte array to be represented in hexadecimal representation
	 * @param len the number of bytes to represent in hexadecimal representation
	 * @return byte array represented in hexadecimal format
	 */
	public static String printData(byte[] data, int len)
	{
		return new String(HexUtils.bArr2HexEdChars(data, len));
	}
	
	/**
	 * This call is equivalent to Util.printData(data, data.length)
	 * @see CommonUtil#printData(byte[],int)
	 * @param data data to represent in hexadecimal
	 * @return byte array represented in hexadecimal format
	 */
	public static String printData(byte[] data)
	{
		return printData(data, data.Length);
	}
	
	// /**
	//  * Method to represent the remaining bytes of a ByteBuffer as hexadecimal
	//  * @param buf ByteBuffer to represent the remaining bytes of as hexadecimal
	//  * @return hexadecimal representation of remaining bytes of the ByteBuffer
	//  */
	// public static String printData(ByteBuffer buf)
	// {
	// 	byte[] data = new byte[buf.remaining()];
	// 	buf.get(data);
	// 	String hex = printData(data, data.Length);
	// 	buf.position(buf.position() - data.Length);
	// 	return hex;
	// }
	
	// /**
	//  * Method to generate a random sequence of bytes returned as byte array
	//  * @param size number of random bytes to generate
	//  * @return byte array with sequence of random bytes
	//  */
	// public static byte[] generateHex(int size)
	// {
	// 	byte[] array = new byte[size];
	// 	Rnd.nextBytes(array);
	// 	
	// 	// Don't allow 0s inside the array!
	// 	for (int i = 0; i < array.length; i++)
	// 	{
	// 		while (array[i] == 0)
	// 		{
	// 			array[i] = (byte) Rnd.get(Byte.MAX_VALUE);
	// 		}
	// 	}
	// 	return array;
	// }
	
	/**
	 * Replaces most invalid characters for the given string with an underscore.
	 * @param str the string that may contain invalid characters
	 * @return the string with invalid character replaced by underscores
	 */
	public static String replaceIllegalCharacters(String str)
	{
		String valid = str;
		foreach (char c in ILLEGAL_CHARACTERS)
		{
			valid = valid.Replace(c, '_');
		}
		return valid;
	}
	
	// /**
	//  * Verify if a file name is valid.
	//  * @param name the name of the file
	//  * @return {@code true} if the file name is valid, {@code false} otherwise
	//  */
	// public static bool isValidFileName(String name)
	// {
	// 	File f = new File(name);
	// 	try
	// 	{
	// 		f.getCanonicalPath();
	// 		return true;
	// 	}
	// 	catch (IOException e)
	// 	{
	// 		return false;
	// 	}
	// }
	
	/**
	 * Split words with a space.
	 * @param input the string to split
	 * @return the split string
	 */
	public static String splitWords(String input)
	{
		return input.replaceAll("(\\p{Ll})(\\p{Lu})", "$1 $2");
	}
	
	// /**
	//  * Gets the next or same closest date from the specified days in {@code daysOfWeek Array} at specified {@code hour} and {@code min}.
	//  * @param daysOfWeek the days of week
	//  * @param hour the hour
	//  * @param min the min
	//  * @return the next or same date from the days of week at specified time
	//  */
	// public static LocalDateTime getNextClosestDateTime(DayOfWeek[] daysOfWeek, int hour, int min)
	// {
	// 	return getNextClosestDateTime(Arrays.asList(daysOfWeek), hour, min);
	// }
	//
	// /**
	//  * Gets the next or same closest date from the specified days in {@code daysOfWeek List} at specified {@code hour} and {@code min}.
	//  * @param daysOfWeek the days of week
	//  * @param hour the hour
	//  * @param min the min
	//  * @return the next or same date from the days of week at specified time
	//  */
	// public static LocalDateTime getNextClosestDateTime(List<DayOfWeek> daysOfWeek, int hour, int min)
	// {
	// 	if (daysOfWeek.isEmpty())
	// 	{
	// 		throw new IllegalArgumentException("daysOfWeek should not be empty.");
	// 	}
	// 	
	// 	final LocalDateTime dateNow = LocalDateTime.now();
	// 	final LocalDateTime dateNowWithDifferentTime = dateNow.withHour(hour).withMinute(min).withSecond(0);
	// 	
	// 	LocalDateTime minDateTime = null;
	// 	for (DayOfWeek dayOfWeek : daysOfWeek)
	// 	{
	// 		final LocalDateTime dateTime = dateNowWithDifferentTime.with(TemporalAdjusters.nextOrSame(dayOfWeek));
	// 		if ((minDateTime == null) || (dateTime.isAfter(dateNow) && dateTime.isBefore(minDateTime)))
	// 		{
	// 			minDateTime = dateTime;
	// 		}
	// 	}
	// 	
	// 	if (minDateTime == null)
	// 	{
	// 		minDateTime = dateNowWithDifferentTime.with(TemporalAdjusters.next(daysOfWeek.get(0)));
	// 	}
	// 	
	// 	return minDateTime;
	// }
	
	public static int min(int value1, int value2, params int[] values)
	{
		int min = Math.Min(value1, value2);
		foreach (int value in values)
		{
			if (min > value)
			{
				min = value;
			}
		}
		return min;
	}
	
	public static int max(int value1, int value2, params int[] values)
	{
		int max = Math.Max(value1, value2);
		foreach (int value in values)
		{
			if (max < value)
			{
				max = value;
			}
		}
		return max;
	}
	
	public static long min(long value1, long value2, params long[] values)
	{
		long min = Math.Min(value1, value2);
		foreach (long value in values)
		{
			if (min > value)
			{
				min = value;
			}
		}
		return min;
	}
	
	public static long max(long value1, long value2, params long[] values)
	{
		long max = Math.Max(value1, value2);
		foreach (long value in values)
		{
			if (max < value)
			{
				max = value;
			}
		}
		return max;
	}
	
	public static float min(float value1, float value2, params float[] values)
	{
		float min = Math.Min(value1, value2);
		foreach (float value in values)
		{
			if (min > value)
			{
				min = value;
			}
		}
		return min;
	}
	
	public static float max(float value1, float value2, params float[] values)
	{
		float max = Math.Max(value1, value2);
		foreach (float value in values)
		{
			if (max < value)
			{
				max = value;
			}
		}
		return max;
	}
	
	public static double min(double value1, double value2, params double[] values)
	{
		double min = Math.Min(value1, value2);
		foreach (double value in values)
		{
			if (min > value)
			{
				min = value;
			}
		}
		return min;
	}
	
	public static double max(double value1, double value2, params double[] values)
	{
		double max = Math.Max(value1, value2);
		foreach (double value in values)
		{
			if (max < value)
			{
				max = value;
			}
		}
		return max;
	}
	
	public static int getIndexOfMaxValue(params int[] array)
	{
		int index = 0;
		for (int i = 1; i < array.Length; i++)
		{
			if (array[i] > array[index])
			{
				index = i;
			}
		}
		return index;
	}
	
	public static int getIndexOfMinValue(params int[] array)
	{
		int index = 0;
		for (int i = 1; i < array.Length; i++)
		{
			if (array[i] < array[index])
			{
				index = i;
			}
		}
		return index;
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param inputValue
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static int map(int inputValue, int inputMin, int inputMax, int outputMin, int outputMax)
	{
		int input = constrain(inputValue, inputMin, inputMax);
		return (((input - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param inputValue
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static long map(long inputValue, long inputMin, long inputMax, long outputMin, long outputMax)
	{
		long input = constrain(inputValue, inputMin, inputMax);
		return (((input - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param inputValue
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static double map(double inputValue, double inputMin, double inputMax, double outputMin, double outputMax)
	{
		double input = constrain(inputValue, inputMin, inputMax);
		return (((input - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static int constrain(int input, int min, int max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static long constrain(long input, long min, long max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static double constrain(double input, double min, double max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * @param array - the array to look into
	 * @param obj - the object to search for
	 * @return {@code true} if the {@code array} contains the {@code obj}, {@code false} otherwise.
	 */
	public static bool startsWith(String[] array, String obj)
	{
		foreach (String element in array)
		{
			if (element.startsWith(obj))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param <T>
	 * @param array - the array to look into
	 * @param obj - the object to search for
	 * @return {@code true} if the {@code array} contains the {@code obj}, {@code false} otherwise.
	 */
	public static bool contains<T>(T[] array, T obj)
	{
		foreach (T element in array)
		{
			if (element.Equals(obj))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param array - the array to look into
	 * @param obj - the integer to search for
	 * @return {@code true} if the {@code array} contains the {@code obj}, {@code false} otherwise
	 */
	public static bool contains(int[] array, int obj)
	{
		foreach (int element in array)
		{
			if (element == obj)
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param array - the array to look into
	 * @param obj - the object to search for
	 * @param ignoreCase
	 * @return {@code true} if the {@code array} contains the {@code obj}, {@code false} otherwise.
	 */
	public static bool contains(String[] array, String obj, bool ignoreCase)
	{
		foreach (String element in array)
		{
			if (element.equals(obj) || (ignoreCase && element.equalsIgnoreCase(obj)))
			{
				return true;
			}
		}
		return false;
	}
	
	public static int parseNextInt(StringTokenizer st, int defaultVal)
	{
		try
		{
			String value = st.nextToken().Trim();
			return int.Parse(value);
		}
		catch (Exception e)
		{
			return defaultVal;
		}
	}
	
	public static int parseInt(String value, int defaultValue)
	{
		try
		{
			return int.Parse(value);
		}
		catch (Exception e)
		{
			return defaultValue;
		}
	}
	
	/**
	 * @param str - the string whose first letter to capitalize
	 * @return a string with the first letter of the {@code str} capitalized
	 */
	public static String capitalizeFirst(String str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		
		char[] arr = str.ToCharArray();
		char c = arr[0];
		if (char.IsLetter(c))
		{
			arr[0] = char.ToUpper(c);
		}
        
		return new String(arr);
	}
	
	// /**
	//  * Based on implode() in PHP
	//  * @param <T>
	//  * @param iteratable
	//  * @param delim
	//  * @return a delimited string for a given array of string elements.
	//  */
	// public static String implode<T>(Iterable<T> iteratable, String delim)
	// {
	// 	StringJoiner sj = new StringJoiner(delim);
	// 	iteratable.forEach(o -> sj.add(o.toString()));
	// 	return sj.toString();
	// }
	
	// /**
	//  * Based on implode() in PHP
	//  * @param <T>
	//  * @param array
	//  * @param delim
	//  * @return a delimited string for a given array of string elements.
	//  */
	// public static <T> String implode(T[] array, String delim)
	// {
	// 	final StringJoiner sj = new StringJoiner(delim);
	// 	for (T o : array)
	// 	{
	// 		sj.add(o.toString());
	// 	}
	// 	return sj.toString();
	// }
	
	// /**
	//  * @param value
	//  * @param format
	//  * @return
	//  */
	// public static String formatDouble(double value, String format)
	// {
	// 	final DecimalFormat formatter = new DecimalFormat(format, new DecimalFormatSymbols(Locale.ENGLISH));
	// 	return formatter.format(value);
	// }
	
	/**
	 * @param numToTest : The number to test.
	 * @param min : The minimum limit.
	 * @param max : The maximum limit.
	 * @return the number or one of the limit (mininum / maximum).
	 */
	public static int limit(int numToTest, int min, int max)
	{
		return (numToTest > max) ? max : ((numToTest < min) ? min : numToTest);
	}
	
	public static bool isNullOrEmpty(this string? value)
	{
		return string.IsNullOrEmpty(value);
	}
	
	public static bool isNotEmpty(this String? value)
	{
		return !string.IsNullOrEmpty(value);
	}
	
	// public static bool isNullOrEmpty(I collection)
	// {
	// 	return isNull(collection) || collection.isEmpty();
	// }
	//
	// public static <T> int zeroIfNullOrElse(T obj, ToIntFunction<T> action)
	// {
	// 	return isNull(obj) ? 0 : action.applyAsInt(obj);
	// }
	//
	// public static <T> bool falseIfNullOrElse(T obj, Predicate<T> predicate)
	// {
	// 	return nonNull(obj) && predicate.test(obj);
	// }
}