using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class MultilingualSupport
    {
        public static bool MULTILANG_ENABLE;
        public static string MULTILANG_DEFAULT = "en";
        public static ImmutableArray<string> MULTILANG_ALLOWED = ImmutableArray<string>.Empty;
        public static bool MULTILANG_VOICED_ALLOW;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.MultilingualSupport);

            MULTILANG_ENABLE = parser.getBoolean("MultiLangEnable");
            if (MULTILANG_ENABLE)
                General.CHECK_HTML_ENCODING = false;

            MULTILANG_DEFAULT = parser.getString("MultiLangDefault", "en").ToLower();
            MULTILANG_ALLOWED = parser.GetStringList("MultiLangAllowed", ';', MULTILANG_DEFAULT);
            if (!MULTILANG_ALLOWED.Contains(MULTILANG_DEFAULT))
                _logger.Error(
                    $"Default language missing in entry 'MultiLangAllowed' in configuration file '{parser.FilePath}'");

            MULTILANG_VOICED_ALLOW = parser.getBoolean("MultiLangVoiceCommand", true);
        }
    }
}