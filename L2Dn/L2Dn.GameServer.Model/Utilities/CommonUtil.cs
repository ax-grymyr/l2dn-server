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
}