using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class SayuneForAll
    {
        public static bool FREE_JUMPS_FOR_ALL;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.SayuneForAll);

            FREE_JUMPS_FOR_ALL = parser.getBoolean("FreeJumpsForAll");
        }
    }
}