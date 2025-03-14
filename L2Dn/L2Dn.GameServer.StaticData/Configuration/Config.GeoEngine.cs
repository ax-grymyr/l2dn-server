using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class GeoEngine
    {
        public static string GEODATA_PATH = string.Empty;
        public static string PATHNODE_PATH = string.Empty;
        public static string GEOEDIT_PATH = string.Empty;
        public static int PATHFINDING;
        public static string PATHFIND_BUFFERS = "100x6;128x6;192x6;256x4;320x4;384x4;500x2";
        public static float LOW_WEIGHT;
        public static float MEDIUM_WEIGHT;
        public static float HIGH_WEIGHT;
        public static bool ADVANCED_DIAGONAL_STRATEGY;
        public static float DIAGONAL_WEIGHT;
        public static int MAX_POSTFILTER_PASSES;
        public static bool DEBUG_PATH;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.GeoEngine);

            string dataPackPath = ServerConfig.Instance.DataPack.Path;
            GEODATA_PATH = Path.Combine(dataPackPath, parser.getString("GeoDataPath", "geodata"));
            PATHNODE_PATH = Path.Combine(dataPackPath, parser.getString("PathnodePath", "pathnode"));
            GEOEDIT_PATH = Path.Combine(dataPackPath, parser.getString("GeoEditPath", "geodata_save"));
            PATHFINDING = parser.getInt("PathFinding");
            PATHFIND_BUFFERS = parser.getString("PathFindBuffers", "100x6;128x6;192x6;256x4;320x4;384x4;500x2");
            LOW_WEIGHT = parser.getFloat("LowWeight", 0.5f);
            MEDIUM_WEIGHT = parser.getFloat("MediumWeight", 2);
            HIGH_WEIGHT = parser.getFloat("HighWeight", 3);
            ADVANCED_DIAGONAL_STRATEGY = parser.getBoolean("AdvancedDiagonalStrategy", true);
            DIAGONAL_WEIGHT = parser.getFloat("DiagonalWeight", 0.707f);
            MAX_POSTFILTER_PASSES = parser.getInt("MaxPostfilterPasses", 3);
            DEBUG_PATH = parser.getBoolean("DebugPath");
        }
    }
}