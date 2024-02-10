namespace L2Dn.AuthServer.Configuration;

public sealed class Config: ISingleton<Config>
{
    private static Config _instance = new();
    public static Config Instance => _instance;

    public ClientListenerConfig ClientListener { get; set; } = new();
    public GameServerListenerConfig GameServerListener { get; set; } = new();
    public DatabaseConfig Database { get; set; } = new();
    public SettingsConfig Settings { get; set; } = new();

    public static void Load(string filePath)
    {
        _instance = JsonUtility.DeserializeFile<Config>(filePath) ??
                    throw new InvalidOperationException($"'{filePath}' is empty");
    }
}