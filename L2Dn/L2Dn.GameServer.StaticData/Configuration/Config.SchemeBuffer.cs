using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class SchemeBuffer
    {
        public static int BUFFER_MAX_SCHEMES;
        public static int BUFFER_ITEM_ID;
        public static int BUFFER_STATIC_BUFF_COST;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.SchemeBuffer);

            BUFFER_MAX_SCHEMES = parser.getInt("BufferMaxSchemesPerChar", 4);
            BUFFER_ITEM_ID = parser.getInt("BufferItemId", 57);
            BUFFER_STATIC_BUFF_COST = parser.getInt("BufferStaticCostPerBuff", -1);
        }
    }
}