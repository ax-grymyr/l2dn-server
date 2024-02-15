using System.Text;
using System.Xml.Linq;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

/**
 * This class is meant to hold a set of (key,value) pairs.<br>
 * They are stored as object but can be retrieved in any type wanted. As long as cast is available.<br>
 * @author mkizub
 */
public class StatSet : IParserAdvUtils
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(StatSet));
	
	/** Static empty immutable map, used to avoid multiple null checks over the source. */
	public static readonly StatSet EMPTY_STATSET = new StatSet(new Map<string, object>());
	
	private readonly Map<String, Object> _set;
	
	public StatSet(): this(new Map<string, object>())
	{
	}

	public StatSet(XElement element)
	{
		_set = new();
		foreach (XAttribute attribute in element.Attributes())
			_set.put(attribute.Name.LocalName, attribute.Value);
	}
	
	public StatSet(Func<Map<String, Object>> mapFactory): this(mapFactory())
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
	public Map<String, Object> getSet()
	{
		return _set;
	}
	
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
	public bool isEmpty()
	{
		return _set.isEmpty();
	}
	
	/**
	 * Return the boolean value associated with key.
	 * @param key : String designating the key in the set
	 * @return boolean : value associated to the key
	 * @throws IllegalArgumentException : If value is not set or value is not boolean
	 */
	public bool getBoolean(String key)
	{
		object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Boolean value required, but not specified");
		}
		
		if (val is bool)
		{
			return (bool)val;
		}

		try
		{
			return bool.Parse((string)val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Boolean value required, but found: " + val);
		}
	}
	
	/**
	 * Return the boolean value associated with key.<br>
	 * If no value is associated with key, or type of value is wrong, returns defaultValue.
	 * @param key : String designating the key in the entry set
	 * @return boolean : value associated to the key
	 */
	public bool getBoolean(String key, bool defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is bool)
		{
			return (bool) val;
		}
		try
		{
			return bool.Parse((String) val);
		}
		catch (Exception e)
		{
			return defaultValue;
		}
	}
	
	public byte getByte(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Byte value required, but not specified");
		}
		if (val is byte)
		{
			return (byte)val;
		}
		try
		{
			return byte.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Byte value required, but found: " + val);
		}
	}
	
	public byte getByte(String key, byte defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is byte)
		{
			return (byte)val;
		}
		try
		{
			return byte.Parse((String)val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Byte value required, but found: " + val);
		}
	}
	
	public short increaseByte(String key, byte increaseWith)
	{
		byte newValue = (byte) (getByte(key) + increaseWith);
		set(key, newValue);
		return newValue;
	}
	
	public short increaseByte(String key, byte defaultValue, byte increaseWith)
	{
		byte newValue = (byte) (getByte(key, defaultValue) + increaseWith);
		set(key, newValue);
		return newValue;
	}
	
	public byte[] getByteArray(String key, String splitOn)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Byte value required, but not specified");
		}
		if (val is byte)
		{
			return new byte[] { (byte)val };
		}
		int c = 0;
		String[] vals = ((string) val).Split(splitOn);
		byte[] result = new byte[vals.Length];
		foreach (String v in vals)
		{
			try
			{
				result[c++] = byte.Parse(v);
			}
			catch (Exception e)
			{
				throw new InvalidCastException("Byte value required, but found: " + val);
			}
		}
		return result;
	}
	
	public List<Byte> getByteList(String key, String splitOn)
	{
		List<Byte> result = new();
		foreach (Byte i in getByteArray(key, splitOn))
		{
			result.Add(i);
		}
		return result;
	}
	
	public short getShort(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Short value required, but not specified");
		}
		if (val is short)
		{
			return (short) val;
		}
		try
		{
			return short.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Short value required, but found: " + val);
		}
	}
	
	public short getShort(String key, short defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is short)
		{
			return (short) val;
		}
		try
		{
			return short.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Short value required, but found: " + val);
		}
	}
	
	public short increaseShort(String key, short increaseWith)
	{
		short newValue = (short) (getShort(key) + increaseWith);
		set(key, newValue);
		return newValue;
	}
	
	public short increaseShort(String key, short defaultValue, short increaseWith)
	{
		short newValue = (short) (getShort(key, defaultValue) + increaseWith);
		set(key, newValue);
		return newValue;
	}
	
	public virtual int getInt(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Integer value required, but not specified: " + key + "!");
		}
		
		if (val is int)
		{
			return ((int) val);
		}
		
		try
		{
			return int.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Integer value required, but found: " + val + "!");
		}
	}
	
	public int getInt(String key, int defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is int)
		{
			return ((int) val);
		}
		try
		{
			return int.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Integer value required, but found: " + val);
		}
	}
	
	public int increaseInt(String key, int increaseWith)
	{
		int newValue = getInt(key) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public int increaseInt(String key, int defaultValue, int increaseWith)
	{
		int newValue = getInt(key, defaultValue) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public int[] getIntArray(String key, String splitOn)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Integer value required, but not specified");
		}
		if (val is int)
		{
			return new int[] { ((int) val) };
		}
		int c = 0;
		String[] vals = ((String) val).Split(splitOn);
		int[] result = new int[vals.Length];
		foreach (String v in vals)
		{
			try
			{
				result[c++] = int.Parse(v);
			}
			catch (Exception e)
			{
				throw new InvalidCastException("Integer value required, but found: " + val);
			}
		}
		return result;
	}
	
	public List<int> getIntegerList(String key)
	{
		String val = getString(key, null);
		List<int> result;
		if (val != null)
		{
			String[] splitVal = val.Split(",");
			result = new(splitVal.Length + 1);
			foreach (String split in splitVal)
			{
				result.Add(int.Parse(split));
			}
		}
		else
		{
			result = new(1);
		}
		return result;
	}
	
	public void setIntegerList(String key, List<int> list)
	{
		if (key == null)
		{
			return;
		}
		
		if ((list == null) || list.Count == 0)
		{
			remove(key);
			return;
		}
		
		StringBuilder sb = new();
		foreach (int element in list)
		{
			sb.Append(element);
			sb.Append(',');
		}
		
		sb.Remove(sb.Length - 1, 1); // Prettify value.
		
		set(key, sb.ToString());
	}
	
	public Map<int, int> getIntegerMap(String key)
	{
		String val = getString(key, null);
		Map<int, int> result;
		if (val != null)
		{
			String[] splitVal = val.Split(",");
			result = new();
			foreach (String split in splitVal)
			{
				String[] entry = split.Split("-");
				result.put(int.Parse(entry[0]), int.Parse(entry[1]));
			}
		}
		else
		{
			result = new();
		}
		return result;
	}
	
	public void setIntegerMap(String key, Map<int, int> map)
	{
		if (key == null)
		{
			return;
		}
		
		if ((map == null) || map.isEmpty())
		{
			remove(key);
			return;
		}
		
		StringBuilder sb = new();
		foreach (System.Collections.Generic.KeyValuePair<int, int> entry in map)
		{
			sb.Append(entry.Key);
			sb.Append('-');
			sb.Append(entry.Value);
			sb.Append(',');
		}

		sb.Remove(sb.Length - 1, 1); // Prettify value.
		
		set(key, sb.ToString());
	}
	
	public long getLong(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Long value required, but not specified");
		}
		if (val is long)
		{
			return ((long) val);
		}
		try
		{
			return long.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Long value required, but found: " + val);
		}
	}
	
	public long getLong(String key, long defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is long)
		{
			return ((long) val);
		}
		try
		{
			return long.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Long value required, but found: " + val);
		}
	}
	
	public long increaseLong(String key, long increaseWith)
	{
		long newValue = getLong(key) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public long increaseLong(String key, long defaultValue, long increaseWith)
	{
		long newValue = getLong(key, defaultValue) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public float getFloat(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Float value required, but not specified");
		}
		if (val is float)
		{
			return ((float) val);
		}
		try
		{
			return float.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Float value required, but found: " + val);
		}
	}
	
	public float getFloat(String key, float defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is float)
		{
			return ((float) val);
		}
		try
		{
			return float.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Float value required, but found: " + val);
		}
	}
	
	public float increaseFloat(String key, float increaseWith)
	{
		float newValue = getFloat(key) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public float increaseFloat(String key, float defaultValue, float increaseWith)
	{
		float newValue = getFloat(key, defaultValue) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public double getDouble(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Double value required, but not specified");
		}
		if (val is double)
		{
			return ((double) val);
		}
		try
		{
			return double.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Double value required, but found: " + val);
		}
	}
	
	public double getDouble(String key, double defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is double)
		{
			return ((double) val);
		}
		try
		{
			return double.Parse((String) val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Double value required, but found: " + val);
		}
	}
	
	public double increaseDouble(String key, double increaseWith)
	{
		double newValue = getDouble(key) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public double increaseDouble(String key, double defaultValue, double increaseWith)
	{
		double newValue = getDouble(key, defaultValue) + increaseWith;
		set(key, newValue);
		return newValue;
	}
	
	public String getString(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("String value required, but not specified");
		}
		return val.ToString() ?? string.Empty;
	}
	
	public String getString(String key, String defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		return val.ToString() ?? string.Empty;
	}
	
	public TimeSpan getDuration(String key)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("String value required, but not specified");
		}
		return TimeUtil.parseDuration(String.valueOf(val));
	}
	
	public TimeSpan getDuration(String key, TimeSpan defaultValue)
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		return TimeUtil.parseDuration(String.valueOf(val));
	}
	
	public T getEnum<T>(String key)
		where T: struct, Enum
	{
		Object val = _set.get(key);
		if (val == null)
		{
			throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but not specified");
		}

		if (val is T)
		{
			return (T)val;
		}
		try
		{
			return Enum.Parse<T>((string)val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but found: " + val);
		}
	}
	
	public T getEnum<T>(String key, T defaultValue)
		where T: struct, Enum
	{
		Object val = _set.get(key);
		if (val == null)
		{
			return defaultValue;
		}
		if (val is T)
		{
			return (T)val;
		}
		try
		{
			return Enum.Parse<T>((string)val);
		}
		catch (Exception e)
		{
			throw new InvalidCastException("Enum value of type " + typeof(T).Name + " required, but found: " + val);
		}
	}
	
	public A getObject<A>(String name)
		where A: class
	{
		Object obj = _set.get(name);
		if ((obj == null) || !(obj is A))
		{
			return null;
		}
		
		return (A) obj;
	}
	
	public A getObject<A>(String name, A defaultValue)
		where A: class
	{
		Object obj = _set.get(name);
		if ((obj == null) || !(obj is A))
		{
			return defaultValue;
		}
		return (A) obj;
	}
	
	public SkillHolder getSkillHolder(String key)
	{
		Object obj = _set.get(key);
		if (!(obj is SkillHolder))
		{
			return null;
		}
		return (SkillHolder) obj;
	}
	
	public Location getLocation(String key)
	{
		Object obj = _set.get(key);
		if (!(obj is Location))
		{
			return null;
		}
		return (Location) obj;
	}
	
	public List<MinionHolder> getMinionList(String key)
	{
		Object obj = _set.get(key);
		if (!(obj is List<MinionHolder>))
		{
			return new();
		}
		
		return (List<MinionHolder>) obj;
	}
	
	public List<T> getList<T>(String key)
	{
		Object obj = _set.get(key);
		if (!(obj is List<T>))
		{
			return null;
		}
		
		List<Object> originalList = (List<Object>) obj;
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
	
	public List<T> getList<T>(String key, List<T> defaultValue)
	{
		List<T> list = getList<T>(key);
		return list == null ? defaultValue : list;
	}
	
	public List<T> getEnumList<T>(String key)
		where T: struct, Enum
	{
		Object obj = _set.get(key);
		if (!(obj is List<T>))
		{
			return null;
		}
		
		List<Object> originalList = (List<Object>) obj;
		if (!originalList.isEmpty() && (obj.getClass().getGenericInterfaces()[0] != clazz) && originalList.stream().allMatch(name => Util.isEnum(name.toString(), clazz)))
		{
			List<T> convertedList = originalList.stream().map(Object::toString).map(name => Enum.valueOf(clazz, name)).map(clazz::cast).collect(Collectors.toList());
			
			// Overwrite the existing list with proper generic type
			_set.put(key, convertedList);
			return convertedList;
		}
		return (List<T>) obj;
	}
	
	/**
	 * @param <T>
	 * @param originalList
	 * @param clazz
	 * @return
	 */
	private List<T> convertList<T>(List<object> originalList)
	{
		
		if (clazz == Integer.class)
		{
			if (originalList.stream().map(Object::toString).allMatch(Util::isInteger))
			{
				return originalList.stream().map(Object::toString).map(Integer::valueOf).map(clazz::cast).collect(Collectors.toList());
			}
		}
		else if (clazz == Float.class)
		{
			if (originalList.stream().map(Object::toString).allMatch(Util::isFloat))
			{
				return originalList.stream().map(Object::toString).map(Float::valueOf).map(clazz::cast).collect(Collectors.toList());
			}
		}
		else if (clazz == Double.class)
		{
			if (originalList.stream().map(Object::toString).allMatch(Util::isDouble))
			{
				return originalList.stream().map(Object::toString).map(Double::valueOf).map(clazz::cast).collect(Collectors.toList());
			}
		}
		return null;
	}
	
	public Map<K, V> getMap<K, V>(String key)
	{
		Object obj = _set.get(key);
		if (!(obj is Map<K, V>))
		{
			return null;
		}
		
		Map<K, V> originalList = (Map<K, V>)obj;
		if (!originalList.isEmpty() && ((!originalList.Keys.All(k=>k is K)) || 
		                                (!originalList.Values.All(v=>v is V))))
		{
			LOGGER.Warn($"getMap<{typeof(K).Name}, {typeof(V).Name}>(\"{key}\") requested with wrong type: " +
			            obj.GetType().Name + "!");
		}
		
		return (Map<K, V>) obj;
	}
	
	public virtual void set(String name, Object value)
	{
		_set.put(name, value);
	}
	
	public virtual void set(String name, bool value)
	{
		_set.put(name, value ? Boolean.TrueString : Boolean.FalseString);
	}
	
	public virtual void set(String key, byte value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, short value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, int value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, long value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, float value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, double value)
	{
		_set.put(key, value);
	}
	
	public virtual void set(String key, String value)
	{
		if (value == null)
		{
			return;
		}
		_set.put(key, value);
	}
	
	public virtual void set<T>(String key, T value)
		where T: struct, Enum
	{
		_set.put(key, value);
	}
	
	public virtual void remove(String key)
	{
		_set.remove(key);
	}
	
	public bool Contains(String name)
	{
		return _set.ContainsKey(name);
	}
	
	public bool contains(String name)
	{
		return _set.ContainsKey(name);
	}
	
	public override string ToString()
	{
		return "StatSet{_set=" + _set + '}';
	}
}
