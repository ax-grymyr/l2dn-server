using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class FindPvP
    {
        public static bool ENABLE_FIND_PVP;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.FindPvP);

            ENABLE_FIND_PVP = parser.getBoolean("EnableFindPvP");
        }
    }
}