using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class GraciaSeeds
    {
        public static int SOD_TIAT_KILL_COUNT;
        public static long SOD_STAGE_2_LENGTH;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.GraciaSeeds);

            SOD_TIAT_KILL_COUNT = parser.getInt("TiatKillCountForNextState", 10);
            SOD_STAGE_2_LENGTH = parser.getLong("Stage2Length", 720) * 60000;
        }
    }
}