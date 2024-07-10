namespace L2Dn.GameServer.Utilities;

public static class CommonUtil
{
	private static readonly char[] _illegalCharacters =
	[
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
		':',
	];

	/**
	 * Replaces most invalid characters for the given string with an underscore.
	 * @param str the string that may contain invalid characters
	 * @return the string with invalid character replaced by underscores
	 */
	public static string replaceIllegalCharacters(string str)
	{
		string valid = str;
		foreach (char c in _illegalCharacters)
		{
			valid = valid.Replace(c, '_');
		}

		return valid;
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
		long input = Math.Clamp(inputValue, inputMin, inputMax);
		return (((input - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
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

	public static int parseNextInt(StringTokenizer st, int defaultVal)
	{
		try
		{
			string value = st.nextToken().Trim();
			return int.Parse(value);
		}
		catch (Exception e)
		{
			return defaultVal;
		}
	}

	public static int parseInt(string value, int defaultValue)
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
	public static string capitalizeFirst(string str)
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

		return new string(arr);
	}
}