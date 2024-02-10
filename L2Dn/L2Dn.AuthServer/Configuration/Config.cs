using L2Dn.Configuration;

namespace L2Dn.AuthServer.Configuration;

public sealed class Config: ConfigBase, ISingleton<Config>
{
    private static Config _instance = new();
    public static Config Instance => _instance;

    public ClientListenerConfig ClientListener { get; set; } = new();
    public GameServerListenerConfig GameServerListener { get; set; } = new();
    public SettingsConfig Settings { get; set; } = new();

    public static void Load()
    {
        _instance = ConfigurationUtil.LoadConfig<Config>();
    }
}