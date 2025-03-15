using System.Collections.Immutable;
using System.Text;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ChatFilter
    {
        public static ImmutableArray<string> FILTER_LIST = ImmutableArray<string>.Empty;

        internal static void LoadFilter()
        {
            string configPath = ServerConfig.Instance.DataPack.ConfigPath;
            string filePath = string.IsNullOrEmpty(configPath)
                ? FileNames.Configs.ChatFilter
                : Path.Combine(configPath, FileNames.Configs.ChatFilter);

            if (!File.Exists(filePath))
            {
                _logger.Warn($"Configuration file '{filePath}' not found");
                FILTER_LIST = ImmutableArray<string>.Empty;
                return;
            }

            FILTER_LIST = File.ReadLines(filePath, Encoding.UTF8).Select(s => s.Trim()).
                Where(s => !string.IsNullOrEmpty(s) && s[0] != '#').ToImmutableArray();
        }
    }
}