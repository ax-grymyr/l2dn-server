using System.Text.Json;
using System.Text.Json.Serialization;

namespace L2Dn;

public static class JsonUtility
{
    public static T DeserializeFile<T>(string filePath)
        where T: class
    {
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter() }
        };
        
        T? result = JsonSerializer.Deserialize<T>(fileStream, options);
        if (result is null) 
            throw new InvalidOperationException($"'{filePath}' is invalid or empty");

        return result;
    }
}
