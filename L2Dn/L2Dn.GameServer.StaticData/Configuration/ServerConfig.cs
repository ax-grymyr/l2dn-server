using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public sealed class ServerConfig: ConfigBase, ISingleton<ServerConfig>
{
    private static ServerConfig _instance = new();
    public static ServerConfig Instance => _instance;

    public ClientListenerConfig ClientListener { get; set; } = new();
    public AuthServerConnectionConfig AuthServerConnection { get; set; } = new();
    public GameServerParamsConfig GameServerParams { get; set; } = new();

    public DataPackConfig DataPack { get; set; } = new();

    public static void Load()
    {
        _instance = ConfigurationUtil.LoadConfig<ServerConfig>();
    }
}