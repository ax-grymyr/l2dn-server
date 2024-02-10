using System.Net;
using System.Text.Json.Serialization;

namespace L2Dn.GameServer.Configuration;

public class ServerConfig: Config
{
    private static ServerConfig _instance = new();

    [JsonRequired]
    public GameServerConfig GameServer { get; set; } = new();

    [JsonRequired]
    public ProtocolConfig Protocol { get; set; } = new();

    public static ServerConfig Instance => _instance;
    
    public static void LoadConfig()
    {
        _instance = JsonUtility.DeserializeFile<ServerConfig>("config.json") ??
                    throw new InvalidOperationException("'config.json' is empty");

        GameServerConfig gameServer = _instance.GameServer;
        if (!IPAddress.TryParse(gameServer.ListenAddress, out IPAddress? address))
            throw new InvalidOperationException("Invalid game server address in 'config.json'");

        gameServer.ListenIpAddress = address;

        if (!IPAddress.TryParse(gameServer.PublishAddress, out address))
            throw new InvalidOperationException("Invalid game server address in 'config.json'");

        gameServer.PublishIpAddress = address;

        if (_instance.Protocol.Version == 0)
            throw new InvalidOperationException("Invalid protocol in 'config.json'");
    }
}
