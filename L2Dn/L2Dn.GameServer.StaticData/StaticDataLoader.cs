using L2Dn.GameServer.Configuration;

namespace L2Dn.GameServer;

public static class StaticDataLoader
{
    public static void Load(string configBasePath)
    {
        Config.Load(configBasePath);
    }
}