using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NLog;

namespace L2Dn.Utilities;

public static class JsonUtil
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter(), new JsonIpAddressConverter(), new JsonLogLevelConverter() }
    };

    public static T DeserializeStream<T>(Stream stream)
        where T: class
    {
        T? result = JsonSerializer.Deserialize<T>(stream, _jsonSerializerOptions);
        if (result is null)
            throw new InvalidOperationException($"Failed to deserialize json");

        return result;
    }

    public static T DeserializeFile<T>(string filePath)
        where T: class
    {
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        T? result = JsonSerializer.Deserialize<T>(fileStream, _jsonSerializerOptions);
        if (result is null)
            throw new InvalidOperationException($"'{filePath}' is invalid or empty");

        return result;
    }

    public static T? DeserializeNode<T>(JsonNode? node)
        where T: class =>
        node.Deserialize<T>(_jsonSerializerOptions);

    private sealed class JsonIpAddressConverter: JsonConverter<IPAddress>
    {
        public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
           string? s = reader.GetString();
            if (string.IsNullOrEmpty(s))
                return IPAddress.Any;

            if (!IPAddress.TryParse(s, out IPAddress? address))
                throw new JsonException($"Invalid IP address format '{s}'");

            return address;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    private sealed class JsonLogLevelConverter: JsonConverter<LogLevel>
    {
        public override LogLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s))
                return null;

            return LogLevel.FromString(s);
        }

        public override void Write(Utf8JsonWriter writer, LogLevel value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}