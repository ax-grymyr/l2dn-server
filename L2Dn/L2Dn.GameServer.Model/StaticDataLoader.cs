using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer;

public static class StaticDataLoader
{
    public static void Load()
    {
        // Config files
        Config.Load();

        // XML config files
        AccessLevelData.Instance.Load();
        AdminCommandData.Instance.Load();
        SecondaryAuthData.Instance.Load();

        // XML data files
        MapRegionData.Instance.Load();
        ZoneManager.Instance.Load(); // for now, zones cannot be separated from the Creature class
    }
}