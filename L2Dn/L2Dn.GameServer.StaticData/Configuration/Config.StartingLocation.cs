using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class StartingLocation
    {
        public static bool CUSTOM_STARTING_LOC;
        public static int CUSTOM_STARTING_LOC_X;
        public static int CUSTOM_STARTING_LOC_Y;
        public static int CUSTOM_STARTING_LOC_Z;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.StartingLocation);

            CUSTOM_STARTING_LOC = parser.getBoolean("CustomStartingLocation");
            CUSTOM_STARTING_LOC_X = parser.getInt("CustomStartingLocX", 50821);
            CUSTOM_STARTING_LOC_Y = parser.getInt("CustomStartingLocY", 186527);
            CUSTOM_STARTING_LOC_Z = parser.getInt("CustomStartingLocZ", -3625);
        }
    }
}