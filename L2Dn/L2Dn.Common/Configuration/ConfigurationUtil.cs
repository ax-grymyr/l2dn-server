using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using L2Dn.Utilities;

namespace L2Dn.Configuration;

public static class ConfigurationUtil
{
    public static TConfig LoadConfig<TConfig>()
        where TConfig: class
    {
        const string configFileName = "config.json";
        JsonNode? config = ReadJson(configFileName);
#if DEBUG
        const string devConfigFileName = "config.dev.json";
        JsonNode? devConfig = ReadJson(devConfigFileName);
        config = CombineNodes(config, devConfig);
#endif

        TConfig? result = JsonUtil.DeserializeNode<TConfig>(config);
        if (result is null)
            throw new InvalidOperationException("No configuration found");

        return result;
    }

    private static JsonNode? ReadJson(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
 
        Logger.Trace($"Loading configuration from '{filePath}'");
        
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return JsonNode.Parse(fileStream, new JsonNodeOptions
        {
            PropertyNameCaseInsensitive = true
        }, new JsonDocumentOptions()
        {
            CommentHandling = JsonCommentHandling.Skip
        });
    }

    private static JsonNode? CombineNodes(JsonNode? target, JsonNode? source)
    {
        if (target is null)
            return source;

        if (source is null)
            return target;
        
        if (target.GetValueKind() != JsonValueKind.Object || source.GetValueKind() != JsonValueKind.Object)
            return source;

        CombineObjects(target.AsObject(), source.AsObject());
        return target;
    }
    
    private static void CombineObjects(JsonObject target, JsonObject source)
    {
        List<KeyValuePair<string, JsonNode?>> nodes = source.ToList();
        foreach (var (key, srcNode) in nodes)
        {
            source.Remove(key);
            target[key] = target.TryGetPropertyValue(key, out JsonNode? targetNode)
                ? CombineNodes(targetNode, srcNode)
                : srcNode;
        }
    }
}