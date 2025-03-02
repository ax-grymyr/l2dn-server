using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

/**
 * This class is meant to hold a set of (key,value) pairs.<br>
 * They are stored as object but can be retrieved in any type wanted. As long as cast is available.<br>
 * @author mkizub
 */
public class StatSet
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(StatSet));

	/** Static empty immutable map, used to avoid multiple null checks over the source. */
	public static readonly StatSet EMPTY_STATSET = new(new Map<string, object>());

	private readonly Map<string, object> _set;

	public StatSet(): this(new Map<string, object>())
	{
	}

	public StatSet(XElement element)
	{
		_set = new();
		foreach (XAttribute attribute in element.Attributes())
			_set.put(attribute.Name.LocalName, attribute.Value);
	}

	public StatSet(Func<Map<string, object>> mapFactory): this(mapFactory())
	{
	}

	public StatSet(Map<string, object> map)
	{
		_set = map;
	}

	/**
	 * Returns the set of values
	 * @return HashMap
	 */
	public Map<string, object> getSet() => _set;

    /**
	 * Add a set of couple values in the current set
	 * @param newSet : StatSet pointing out the list of couples to add in the current set
	 */
	public void merge(StatSet newSet)
	{
		_set.putAll(newSet.getSet());
	}

	/**
	 * Verifies if the stat set is empty.
	 * @return {@code true} if the stat set is empty, {@code false} otherwise
	 */
	public bool isEmpty() => _set.Count == 0;

    /**
     * Return the bool value associated with key.
     * @param key : String designating the key in the set
     * @return bool : value associated to the key
     * @throws IllegalArgumentException : If value is not set or value is not bool
     */
    public bool getBoolean(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Boolean value required, but not specified");
        if (val is bool b)
            return b;

        try
        {
            return bool.Parse((string)val);
        }
        catch (Exception e)
        {
            LOGGER.Error(e);
            throw new InvalidCastException("Boolean value required, but found: " + val);
        }
    }

    /**
     * Return the bool value associated with key.<br>
     * If no value is associated with key, or type of value is wrong, returns defaultValue.
     * @param key : String designating the key in the entry set
     * @return bool : value associated to the key
     */
    public bool getBoolean(string key, bool defaultValue)
    {
        object? val = _set.get(key);
        if (val == null)
            return defaultValue;

        if (val is bool b)
            return b;

        try
        {
            return bool.Parse((string)val);
        }
        catch (Exception e)
        {
            LOGGER.Error(e);
            return defaultValue;
        }
    }

    public byte getByte(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Byte value required, but not specified");
		if (val is byte b)
			return b;

        try
		{
			return byte.Parse((string)val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Byte value required, but found: " + val);
		}
	}

	public byte getByte(string key, byte defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is byte b)
			return b;

        try
		{
			return byte.Parse((string)val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Byte value required, but found: " + val);
		}
	}

	public byte[] getByteArray(string key, string splitOn)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Byte value required, but not specified");
		if (val is byte b)
			return [b];

        int c = 0;
		string[] vals = ((string)val).Split(splitOn);
		byte[] result = new byte[vals.Length];
		foreach (string v in vals)
		{
			try
			{
				result[c++] = byte.Parse(v);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				throw new InvalidCastException("Byte value required, but found: " + val);
			}
		}

		return result;
	}

	public short getShort(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Short value required, but not specified");
		if (val is short s)
			return s;

        try
		{
			return short.Parse((string) val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Short value required, but found: " + val);
		}
	}

	public short getShort(string key, short defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is short s)
			return s;

        try
		{
			return short.Parse((string) val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Short value required, but found: " + val);
		}
	}

	public int getInt(string key)
    {
        object val = _set.get(key) ??
            throw new InvalidCastException("Integer value required, but not specified: " + key + "!");

		if (val is int i)
			return i;

		try
		{
			return int.Parse((string)val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Integer value required, but found: " + val + "!");
		}
	}

	public int getInt(string key, int defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is int i)
			return i;

		if (val is double d && Math.Abs(Math.Round(d) - d) < 0.0000001)
			return (int)d;

		try
		{
			return int.Parse(val.ToString() ?? string.Empty);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Integer value required, but found: " + val);
		}
	}

	public int[] getIntArray(string key, string splitOn)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Array of integers required, but not specified");
		if (val is int i)
			return [i];

        int c = 0;
        string[] vals = ((string)val).Split(splitOn);
		int[] result = new int[vals.Length];
		foreach (string v in vals)
		{
			try
			{
				result[c++] = int.Parse(v);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				throw new InvalidCastException("Integer value required, but found: " + val);
			}
		}

		return result;
	}

	public List<int> getIntegerList(string key)
	{
		string val = getString(key, string.Empty);
		List<int> result;
		if (!string.IsNullOrEmpty(val))
		{
			string[] splitVal = val.Split(",");
			result = new List<int>(splitVal.Length + 1);
			foreach (string split in splitVal)
				result.Add(int.Parse(split));
		}
		else
		{
			result = new List<int>(1);
		}

		return result;
	}

	public DateTime getDateTime(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("DateTime value required, but not specified");
		if (val is DateTime time)
			return time;

        try
		{
			return DateTime.Parse((string) val);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("DateTime value required, but found: " + val);
		}
	}

	public long getLong(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Long value required, but not specified");
		if (val is long l)
			return l;

        try
        {
            return long.Parse((string)val);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Long value required, but found: " + val);
		}
	}

	public DateTime getDateTime(string key, DateTime defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is DateTime time)
			return time;

        try
        {
            return DateTime.Parse((string)val);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("DateTime value required, but found: " + val);
		}
	}

	public long getLong(string key, long defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is long l)
			return l;

        try
        {
            return long.Parse((string)val);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Long value required, but found: " + val);
		}
	}

	public float getFloat(string key)
    {
        object? val = _set.get(key) ?? throw new InvalidCastException("Float value required, but not specified");
		if (val is float f)
			return f;

		if (val is double d)
			return (float)d;

		try
        {
            return float.Parse((string)val, CultureInfo.InvariantCulture);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Float value required, but found: " + val);
		}
	}

	public float getFloat(string key, float defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

		if (val is float f)
			return f;

		if (val is double d)
			return (float)d;

		if (val is IConvertible convertible)
			return convertible.ToSingle(CultureInfo.InvariantCulture);

		try
		{
			return float.Parse(val.ToString() ?? string.Empty, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Float value required, but found: " + val);
		}
	}

	public double getDouble(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("Double value required, but not specified");
		if (val is double d)
			return d;

		if (val is float f)
			return f;

		try
        {
            return double.Parse((string)val, CultureInfo.InvariantCulture);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Double value required, but found: " + val);
		}
	}

	public double getDouble(string key, double defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        if (val is double d)
            return d;

        if (val is float f)
            return f;

		if (val is IConvertible convertible)
			return convertible.ToDouble(CultureInfo.InvariantCulture);

		try
		{
			return double.Parse(val.ToString() ?? string.Empty);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Double value required, but found: " + val);
		}
	}

	public string getString(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("String value required, but not specified");
		return val.ToString() ?? string.Empty;
	}

	public string getString(string key, string defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
			return defaultValue;

        return val.ToString() ?? defaultValue;
	}

	public TimeSpan getDuration(string key)
    {
        object val = _set.get(key) ?? throw new InvalidCastException("String value required, but not specified");
		return TimeUtil.ParseDuration(val.ToString());
	}

	public TimeSpan getDuration(string key, TimeSpan defaultValue)
	{
		object? val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}

		return TimeUtil.ParseDuration(val.ToString());
	}

	public T getEnum<T>(string key)
		where T: struct, Enum
    {
        object val = _set.get(key) ??
            throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but not specified");

		if (val is T value)
			return value;

		try
        {
            return Enum.Parse<T>((string)val, true);
        }
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but found: " + val);
		}
	}

	public T getEnum<T>(string key, T defaultValue)
		where T: struct, Enum
	{
		if (!_set.TryGetValue(key, out object? val) || val is null)
			return defaultValue;

		if (val is T value)
			return value;

        string str = val.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(str))
            return defaultValue;

		try
		{
			return Enum.Parse<T>(str, true);
		}
		catch (Exception e)
		{
            LOGGER.Error(e);
			throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but found: " + val);
		}
	}

	public A? getObject<A>(string name)
		where A: class
	{
		object? obj = _set.get(name);
		return obj as A;
    }

	public A getObject<A>(string name, A defaultValue)
		where A: class
	{
		object? obj = _set.get(name);
		return obj as A ?? defaultValue;
    }

	public SkillHolder? getSkillHolder(string key)
	{
		object? obj = _set.get(key);
		return obj as SkillHolder;
    }

	public Location? getLocation(string key)
	{
		object? obj = _set.get(key);
		if (obj is Location location)
			return location;
		return null;
	}

	public List<MinionHolder> getMinionList(string key)
	{
		object? obj = _set.get(key);
		if (!(obj is List<MinionHolder>))
		{
			return new();
		}

		return (List<MinionHolder>) obj;
	}

	public List<T>? getList<T>(string key)
	{
		object? obj = _set.get(key);
		if (obj is null)
			if (typeof(T).IsClass || typeof(T).IsInterface)
				return null;
			else
				throw new NotSupportedException();

		if (obj is List<T> ts)
		{
			return ts;
		}

		if (obj is IEnumerable enumerable)
		{
			List<T> items = new List<T>();
			foreach (object o in enumerable)
			{
				if (o is T t)
					items.Add(t);
				else if (o is string s)
				{
					try
					{
						items.Add((T)Convert.ChangeType(s, typeof(T)));
					}
					catch (Exception e)
					{
						throw new NotSupportedException("Invalid conversion", e);
					}
				}
				else
					throw new NotSupportedException();
			}

			return items;
		}

		List<object> originalList = (List<object>)obj;
		if (originalList.Count!=0 && !originalList.All(o => o is T))
		{
			if (typeof(T).IsEnum)
			{
				throw new InvalidOperationException("Please use getEnumList if you want to get list of Enums!");
			}

			// Attempt to convert the list
			List<T> convertedList = convertList<T>(originalList);
			if (convertedList == null)
			{
				LOGGER.Warn($"getList<{typeof(T).Name}>(\"{key}\") requested with wrong type: " +
				            obj.GetType().Name + "!");

				return null;
			}

			// Overwrite the existing list with proper generic type
			_set.put(key, convertedList);
			return convertedList;
		}
		return (List<T>) obj;
	}

	public List<T> getList<T>(string key, List<T> defaultValue)
	{
		List<T>? list = getList<T>(key);
		return list ?? defaultValue;
	}

	public List<T>? getEnumList<T>(string key)
		where T: unmanaged, Enum
	{
		object? obj = _set.get(key);
        T? value = TryConvertToEnum<T>(obj);
        if (value is not null)
            return [value.Value];

        if (obj is List<T> list)
            return list;

        if (obj is IEnumerable<T> enumerable)
            return enumerable.ToList();

        if (obj is not IEnumerable enumerable2)
            throw new InvalidCastException("List of enum of type " + typeof(T).Name + " required, but found: " + obj);

        try
        {
            return enumerable2.Cast<object?>().Select(ConvertToEnum<T>).ToList();
        }
        catch (Exception exception)
        {
            LOGGER.Error(exception);
            throw new InvalidCastException("List of enum of type " + typeof(T).Name + " required, but found: " + obj);
        }
    }

	/**
	 * @param <T>
	 * @param originalList
	 * @param clazz
	 * @return
	 */
	private static List<T> convertList<T>(List<object> originalList)
	{
		throw new NotImplementedException();
	}

	public void set(string name, object? value)
    {
        if (value is null)
            _set.TryRemove(name, out _);
        else
		    _set[name] = value;
	}

	public void set(string name, bool value)
	{
		_set.put(name, value ? bool.TrueString : bool.FalseString);
	}

	public void set(string key, byte value)
	{
		_set.put(key, value);
	}

	public void set(string key, short value)
	{
		_set.put(key, value);
	}

	public void set(string key, int value)
	{
		_set.put(key, value);
	}

	public void set(string key, long value)
	{
		_set.put(key, value);
	}

	public void set(string key, float value)
	{
		_set.put(key, value);
	}

	public void set(string key, double value)
	{
		_set.put(key, value);
	}

	public void set(string key, string value)
	{
		_set.put(key, value);
	}

	public void set<TValue>(string key, TValue value)
		where TValue: struct, Enum
	{
		_set.put(key, value);
	}

	public void remove(string key)
	{
		_set.remove(key);
	}

	public bool Contains(string name)
	{
		return _set.ContainsKey(name);
	}

	public bool contains(string name)
	{
		return _set.ContainsKey(name);
	}

	public override string ToString()
	{
		return "StatSet{_set=" + _set + '}';
	}

    private static T ConvertToEnum<T>(object? value)
        where T: unmanaged, Enum
    {
        return value switch
        {
            T val => val,
            string str => Enum.Parse<T>(str, true),
            int i => i.ToEnum<T>(),
            uint i => ((int)i).ToEnum<T>(),
            long l => l.ToEnum<T>(),
            ulong l => ((long)l).ToEnum<T>(),
            _ => Enum.Parse<T>(value?.ToString() ?? string.Empty, true),
        };
    }

    private static T? TryConvertToEnum<T>(object? value)
        where T: unmanaged, Enum
    {
        return value switch
        {
            null => null,
            T val => val,
            string str => Enum.TryParse(str, true, out T v) ? v : null,
            int i => i.ToEnum<T>(),
            uint i => ((int)i).ToEnum<T>(),
            long l => l.ToEnum<T>(),
            ulong l => ((long)l).ToEnum<T>(),
            _ => Enum.TryParse(value.ToString() ?? string.Empty, true, out T v) ? v : null,
        };
    }
}