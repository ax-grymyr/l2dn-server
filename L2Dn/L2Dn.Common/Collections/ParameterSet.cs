using System.Globalization;

namespace L2Dn.Collections;

public sealed class ParameterSet<TKey>
    where TKey: notnull
{
    private readonly Dictionary<TKey, object> _values = [];

    public object this[TKey key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    public string? GetStringOptional(TKey key) =>
        _values.TryGetValue(key, out object? obj) ? ConvertToString(obj) : null;

    public string GetString(TKey key) =>
        GetStringOptional(key) ??
        throw new InvalidCastException("String value of type required, but not specified");

    public string GetString(TKey key, string defaultValue) => GetStringOptional(key) ?? defaultValue;

    public TEnum? GetEnumOptional<TEnum>(TKey key)
        where TEnum: unmanaged, Enum
    {
        if (!_values.TryGetValue(key, out object? obj))
            return null;

        if (obj is TEnum result)
            return result;

        string s = ConvertToString(obj);
        if (Enum.TryParse(s, true, out result))
            return result;

        throw new InvalidCastException($"Enum value of type {typeof(TEnum).Name} required, but found: {s}");
    }

    public TEnum GetEnum<TEnum>(TKey key)
        where TEnum: unmanaged, Enum =>
        GetEnumOptional<TEnum>(key) ??
        throw new InvalidCastException($"Enum value of type {typeof(TEnum).Name} required, but not specified");

    public TEnum GetEnum<TEnum>(TKey key, TEnum defaultValue)
        where TEnum: unmanaged, Enum =>
        GetEnumOptional<TEnum>(key) ?? defaultValue;

    public int? GetInt32Optional(TKey key) => GetValueOptional<int>(key);
    public int GetInt32(TKey key) => GetValue<int>(key);
    public int GetInt32(TKey key, int defaultValue) => GetInt32Optional(key) ?? defaultValue;

    public long? GetInt64Optional(TKey key) => GetValueOptional<long>(key);
    public long GetInt64(TKey key) => GetValue<long>(key);
    public long GetInt64(TKey key, int defaultValue) => GetInt64Optional(key) ?? defaultValue;

    public decimal? GetDecimalOptional(TKey key) => GetValueOptional<decimal>(key);
    public decimal GetDecimal(TKey key) => GetValue<decimal>(key);
    public decimal GetDecimal(TKey key, decimal defaultValue) => GetDecimalOptional(key) ?? defaultValue;

    public double? GetDoubleOptional(TKey key) => GetValueOptional<double>(key);
    public double GetDouble(TKey key) => GetValue<double>(key);
    public double GetDouble(TKey key, double defaultValue) => GetDoubleOptional(key) ?? defaultValue;

    public float? GetFloatOptional(TKey key) => GetValueOptional<float>(key);
    public float GetFloat(TKey key) => GetValue<float>(key);
    public float GetFloat(TKey key, float defaultValue) => GetFloatOptional(key) ?? defaultValue;

    public bool? GetBooleanOptional(TKey key) => GetValueOptional<bool>(key);
    public bool GetBoolean(TKey key) => GetValue<bool>(key);
    public bool GetBoolean(TKey key, bool defaultValue) => GetBooleanOptional(key) ?? defaultValue;

    public TimeSpan? GetTimeSpanSecondsOptional(TKey key) => GetTimeSpanOptional(key, TimeSpan.TicksPerSecond);
    public TimeSpan GetTimeSpanSeconds(TKey key) => GetTimeSpan(key, TimeSpan.TicksPerSecond);

    public TimeSpan GetTimeSpanSeconds(TKey key, TimeSpan defaultValue) =>
        GetTimeSpanOptional(key, TimeSpan.TicksPerSecond) ?? defaultValue;

    public TimeSpan? GetTimeSpanMilliSecondsOptional(TKey key) =>
        GetTimeSpanOptional(key, TimeSpan.TicksPerMillisecond);

    public TimeSpan GetTimeSpanMilliSeconds(TKey key) => GetTimeSpan(key, TimeSpan.TicksPerMillisecond);

    public TimeSpan GetTimeSpanMilliSeconds(TKey key, TimeSpan defaultValue) =>
        GetTimeSpanOptional(key, TimeSpan.TicksPerMillisecond) ?? defaultValue;

    private static string ConvertToString(object obj) =>
        obj switch
        {
            string s => s,
            IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => obj.ToString() ?? string.Empty
        };

    private T GetValue<T>(TKey key)
        where T: struct, IParsable<T> =>
        GetValueOptional<T>(key) ??
        throw new InvalidCastException($"{typeof(T).Name} value required, but not specified");

    private T? GetValueOptional<T>(TKey key)
        where T: struct, IParsable<T>
    {
        if (!_values.TryGetValue(key, out object? obj))
            return null;

        if (obj is T result)
            return result;

        string s = ConvertToString(obj);
        if (T.TryParse(s, CultureInfo.InvariantCulture, out result))
            return result;

        throw new InvalidCastException($"{typeof(T).Name} value required, but found: {s}");
    }

    private TimeSpan GetTimeSpan(TKey key, long multiplier) =>
        GetTimeSpanOptional(key, multiplier) ??
        throw new InvalidCastException("Decimal value required, but not specified");

    private TimeSpan? GetTimeSpanOptional(TKey key, long multiplier)
    {
        decimal? value = GetDecimalOptional(key);
        return value == null ? null : TimeSpan.FromTicks((long)(value.Value * multiplier));
    }
}