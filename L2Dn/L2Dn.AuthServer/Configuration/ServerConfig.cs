using System.Net;
using System.Text.Json.Serialization;

namespace L2Dn.AuthServer.Configuration;

public class ServerConfig: Config
{
    private static ServerConfig _instance = new();

    [JsonRequired]
    public AuthServerConfig AuthServer { get; set; } = new();

    public static ServerConfig Instance => _instance;
    
    public static void LoadConfig()
    {
        _instance = JsonUtility.DeserializeFile<ServerConfig>("config.json") ??
                    throw new InvalidOperationException("'config.json' is empty");

        if (!IPAddress.TryParse(_instance.AuthServer.ListenAddress, out IPAddress? address))
            throw new InvalidOperationException("Invalid auth server address in 'config.json'");

        _instance.AuthServer.ListenIpAddress = address;
    }
}
