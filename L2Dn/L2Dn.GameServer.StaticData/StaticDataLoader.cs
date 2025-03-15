using L2Dn.GameServer.Configuration;

namespace L2Dn.GameServer;

public static class StaticDataLoader
{
    public static void Load()
    {
        Config.Load(ServerConfig.Instance.DataPack.ConfigPath);
    }
}