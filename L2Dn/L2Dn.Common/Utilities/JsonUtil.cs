using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace L2Dn.Utilities;

public static class JsonUtil
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() }
    };
    
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
}