using System.Collections.Concurrent;
using System.Text.Json;
using NLog;

namespace L2Dn.Collections;

public class PropertyDictionary(StringComparer? comparer = null)
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PropertyDictionary));
    private static readonly JsonSerializerOptions _serializeOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            WriteIndented = false,
        };

    private readonly ConcurrentDictionary<string, (string Value, PropertyState State)> _dictionary = new(comparer);
    private bool _changed;

    public bool Changed => _changed;

    public void Set(string name, string value, PropertyState state)
    {
        _dictionary[name] = (value, state);
    }

    public void Set<T>(string name, T value)
    {
        if (value is null)
        {
            Remove(name);
            return;
        }

        string text = JsonSerializer.Serialize(value, _serializeOptions);

        _dictionary.AddOrUpdate(name, static (_, newValue) => (newValue, PropertyState.New),
            static (_, oldItem, newValue) =>
                (newValue, oldItem.State != PropertyState.New ? PropertyState.Modified : PropertyState.New), text);

        _changed = true;
    }

    public TValue? Get<TValue>(string name)
    {
        if (_dictionary.TryGetValue(name, out (string Text, PropertyState State) item) &&
            item.State != PropertyState.Deleted)
        {
            TValue? value = default;
            try
            {
                value = JsonSerializer.Deserialize<TValue>(item.Text, _serializeOptions);
            }
            catch (JsonException exception)
            {
                _logger.Error(
                    $"Could not deserialize object of type {typeof(TValue)} from string '{item.Text}': {exception}");
            }

            return value;
        }

        return default;
    }

    public TValue Get<TValue>(string name, TValue defaultValue) => Get<TValue>(name) ?? defaultValue;

    public bool Remove(string name)
    {
        if (!_dictionary.TryGetValue(name, out (string Value, PropertyState State) item))
            return false;

        if (_dictionary.TryUpdate(name, (string.Empty, PropertyState.Deleted), item))
        {
            _changed = true;
            return true;
        }

        return false;
    }

    public void RemoveAll(string namePrefix)
    {
        List<string> names = _dictionary.Keys.Where(x => x.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase)).
            ToList();

        foreach (string name in names)
            Remove(name);
    }

    public bool ContainsKey(string name) =>
        _dictionary.TryGetValue(name, out (string Value, PropertyState State) item) &&
        item.State != PropertyState.Deleted;

    public void Clear()
    {
        _dictionary.Clear();
        _changed = false;
    }

    public List<(string Name, string Value, PropertyState State)> ResetChanges()
    {
        _changed = false;
        List<(string Name, string Text, PropertyState State)> changedItems = [];
        foreach (KeyValuePair<string, (string Text, PropertyState State)> pair in _dictionary)
        {
            if (pair.Value.State != PropertyState.Unchanged)
            {
                changedItems.Add((pair.Key, pair.Value.Text, pair.Value.State));
                if (pair.Value.State == PropertyState.Deleted)
                    _dictionary.TryRemove(pair);
                else
                    _dictionary.TryUpdate(pair.Key, (pair.Value.Text, PropertyState.Unchanged), pair.Value);
            }
        }

        return changedItems;
    }
}