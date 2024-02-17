using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using L2Dn.Extensions;
using NLog;

namespace L2Dn.Configuration;

public class ConfigurationParser
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ConfigurationParser));
    private readonly Dictionary<string, string> _values = new();
    private readonly string? _basePath;
    private string _filePath = string.Empty;

    public string FilePath => _filePath;

    public ConfigurationParser(string? basePath = null)
    {
        _basePath = basePath;
    }
    
    public void LoadConfig(string filePath)
    {
        if (!string.IsNullOrEmpty(_basePath) && !Path.IsPathRooted(filePath) && !Path.IsPathFullyQualified(filePath))
            _filePath = Path.Combine(_basePath, filePath);
        else 
            _filePath = filePath;

        _filePath = filePath;
        _values.Clear();

        if (!File.Exists(filePath))
        {
            _logger.Warn($"Configuration file '{filePath}' not found");
            return;
        }

        ReadLines(filePath).Select(ParseKeyValue).ForEach(pair =>
        {
            if (!_values.TryAdd(pair.Key, pair.Value))
            {
                _logger.Error($"Duplicated entry '{pair.Key}' in configuration file '{filePath}'");
            }
        });
    }

    public string getString(string key, string defaultValue = "") => _values.GetValueOrDefault(key, defaultValue);

    public string GetPath(string key, string defaultValue)
    {
        string path = getString(key, defaultValue);
        if (Path.IsPathRooted(path) || Path.IsPathFullyQualified(path))
            return path;

        return Path.Combine(Directory.GetCurrentDirectory(), path);
    }

    public bool containsKey(string key) => _values.ContainsKey(key);

    public byte getByte(string key, byte defaultValue = default) => GetValue(key, defaultValue, "integer in range 0-255");
    public int getInt(string key, int defaultValue = default) => GetValue(key, defaultValue, "integer");
    public long getLong(string key, long defaultValue = default) => GetValue(key, defaultValue, "64-bit integer");
    public double getDouble(string key, double defaultValue = default) => GetValue(key, defaultValue, "number");
    public float getFloat(string key, float defaultValue = default) => GetValue(key, defaultValue, "number");

    public bool getBoolean(string key, bool defaultValue = default)
    {
        if (!_values.TryGetValue(key, out string? value))
            return defaultValue;

        if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "1", StringComparison.Ordinal))
            return true;

        if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "no", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "0", StringComparison.Ordinal))
            return true;

        _logger.Error($"Invalid boolean value '{value}' in entry '{key}' in configuration file '{_filePath}'");
        return defaultValue;
    }

    public TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
    {
        if (!_values.TryGetValue(key, out string? value))
            return defaultValue;

        // number without suffix is in seconds
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
            return TimeSpan.FromSeconds(val);

        if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan result))
            return result;

        // 300, 300s, 300sec, 300seconds, 300 s, 300 sec, 300 seconds
        // 30 mins, 30 minutes, 30mins, 30minutes

        (string, long)[] suffixes = [
            ("hours", TimeSpan.TicksPerHour),
            ("hrs", TimeSpan.TicksPerHour),
            ("hour", TimeSpan.TicksPerHour),
            ("hr", TimeSpan.TicksPerHour),
            ("minutes", TimeSpan.TicksPerMinute),
            ("mins", TimeSpan.TicksPerMinute),
            ("minute", TimeSpan.TicksPerMinute),
            ("min", TimeSpan.TicksPerMinute),
            ("seconds", TimeSpan.TicksPerSecond),
            ("second", TimeSpan.TicksPerSecond),
            ("secs", TimeSpan.TicksPerSecond),
            ("sec", TimeSpan.TicksPerSecond),

            ("h", TimeSpan.TicksPerHour),
            ("m", TimeSpan.TicksPerMinute),
            ("s", TimeSpan.TicksPerSecond),
        ];

        foreach ((string suffix, long multiplier) in suffixes)
        {
            if (value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                string num = value[..^suffix.Length];
                if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                {
                    return TimeSpan.FromTicks((long)(multiplier * val));
                }
            }
        }
        
        _logger.Error($"Invalid duration value '{value}' in entry '{key}' in configuration file '{_filePath}'");
        return defaultValue;
    }

    public TimeOnly GetTime(string key, TimeOnly defaultValue = default)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValue;

        if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, out TimeOnly val))
            return val;

        _logger.Error($"Invalid time format '{value}' in entry '{key}' in configuration file '{_filePath}'");
        return defaultValue;
    }

    public ImmutableArray<TimeOnly> GetTimeList(string key, char separator = ',', params TimeOnly[] defaultValues)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValues.ToImmutableArray();

        // TODO: make split enumerator to avoid array allocation, as Split is used everywhere
        string[] values = value.Split(separator);
        TimeOnly[] result = new TimeOnly[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            if (!TimeOnly.TryParse(values[i], CultureInfo.InvariantCulture, out result[i]))
            {
                _logger.Error($"Invalid time list '{value}' in entry '{key}' in configuration file '{_filePath}'");
                return defaultValues.ToImmutableArray();
            }
        }
        
        return result.ToImmutableArray();
    }

    public ImmutableArray<T> GetEnumList<T>(string key, char separator = ',', params T[] defaultValues)
        where T: struct, Enum
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValues.ToImmutableArray();

        try
        {
            return GetList(key, separator, s =>
            {
                bool ok = Enum.TryParse(s, false, out T val);
                return (val, ok);
            }, false).ToImmutableArray();
        }
        catch (ConfigurationException)
        {
            return defaultValues.ToImmutableArray();
        }
    }
    
    public Regex GetRegex(string key, Regex defaultValue)
    {
        if (!_values.TryGetValue(key, out string? value))
            return defaultValue;

        try
        {
            return new Regex(value, RegexOptions.Compiled);
        }
        catch (ArgumentException exception)
        {
            _logger.Error(exception,
                $"Invalid regular expression in entry {key} in configuration file {_filePath}");
            
            return defaultValue;
        }
    }

    public ImmutableArray<string> GetStringList(string key, char separator = ',', params string[] defaultValues)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValues.ToImmutableArray();

        return value.Split(separator).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s))
            .ToImmutableArray();
    }

    public ImmutableArray<int> GetIntList(string key, char separator = ',', params int[] defaultValues)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValues.ToImmutableArray();

        try
        {
            return GetList(key, separator, s =>
            {
                bool ok = int.TryParse(s, CultureInfo.InvariantCulture, out int val);
                return (val, ok);
            }, false).ToImmutableArray();
        }
        catch (ConfigurationException)
        {
            return defaultValues.ToImmutableArray();
        }
    }

    public ImmutableArray<double> GetDoubleList(string key, char separator = ',', params double[] defaultValues)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            return defaultValues.ToImmutableArray();

        try
        {
            return GetList(key, separator, s =>
            {
                bool ok = double.TryParse(s, CultureInfo.InvariantCulture, out double val);
                return (val, ok);
            }, false).ToImmutableArray();
        }
        catch (ConfigurationException)
        {
            return defaultValues.ToImmutableArray();
        }
    }

    public Color GetColor(string key, Color defaultValue = default) => GetValue(key, defaultValue, "color");
    
    public T GetEnum<T>(string key, T defaultValue = default)
        where T: struct, Enum
    {
        if (!_values.TryGetValue(key, out string? value))
            return defaultValue;

        if (Enum.TryParse(value, true, out T result))
            return result;

        string message = typeof(T).GetCustomAttribute<FlagsAttribute>() is null
            ? $"{typeof(T).Name} value"
            : $"list of {typeof(T).Name} values";
        
        _logger.Error($"Invalid {message} '{value}' in entry '{key}' in configuration file '{_filePath}'");
        return defaultValue;
    }

    public IEnumerable<T> GetList<T>(string key, char separator, Func<string, (T, bool)> convert, bool continueOnError)
    {
        if (!_values.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
            yield break;

        IEnumerable<string> collection = value.Split(separator).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));
        foreach (string s in collection)
        {
            (T val, bool ok) = convert(s);
            if (ok)
                yield return val;
            else
            {
                string message = $"Invalid format '{value}' in entry '{key}' in configuration file '{_filePath}'";
                _logger.Error(message);
                if (!continueOnError)
                    throw new ConfigurationException(message);
            }
        }
    }

    public ImmutableDictionary<int, T> GetIdValueMap<T>(string key, char pairSeparator = ';',
        char keyValueSeparator = ',')
        where T: struct, IParsable<T>
    {
        if (!_values.TryGetValue(key, out string? value))
            return ImmutableDictionary<int, T>.Empty;

        return GetList(key, pairSeparator, s =>
        {
            string[] pair = s.Split(keyValueSeparator);
            int id = 0;
            T val = default;
            bool ok = pair.Length == 2 && !int.TryParse(pair[0], CultureInfo.InvariantCulture, out id) &&
                      T.TryParse(pair[1], CultureInfo.InvariantCulture, out val);

            return ((id, val), ok);
        }, true).ToImmutableDictionary(tuple => tuple.id, tuple => tuple.val);
    }

    private T GetValue<T>(string key, T defaultValue, string message)
        where T: struct, IParsable<T>
    {
        if (!_values.TryGetValue(key, out string? value))
            return defaultValue;

        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;

        _logger.Error($"Invalid {message} '{value}' in entry '{key}' in configuration file '{_filePath}'");
        return defaultValue;
    }

    private static bool IsEmptyOrCommented(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return true;

        ReadOnlySpan<char> span = line.AsSpan();
        while (span.Length != 0 && char.IsWhiteSpace(span[0]))
        {
            span = span[1..];
        }

        return span.Length == 0 || span[0] == '#'; // comment
    }

    private static (string Key, string Value) ParseKeyValue(string line)
    {
        int index = line.IndexOf('=');
        if (index >= 0)
            return (line[..index].Trim(), line[(index + 1)..].Trim());

        return (line, string.Empty);
    }

    private static IEnumerable<string> ReadLines(string filePath)
    {
        bool next = false;
        string option = string.Empty;
        foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
        {
            if (IsEmptyOrCommented(line))
                continue;

            string trimmed = line.Trim();
            
            if (trimmed.EndsWith('\\'))
            {
                option += trimmed[..^1];
                next = true;
            }
            else
            {
                if (next)
                {
                    option += trimmed;
                    next = false;
                    yield return option;
                    option = string.Empty;
                }
                else
                    yield return trimmed;
            }
        }

        if (option.Length != 0)
        {
            _logger.Error($"Unexpected end in configuration file '{filePath}'");
            yield return option;
        }
    }
}