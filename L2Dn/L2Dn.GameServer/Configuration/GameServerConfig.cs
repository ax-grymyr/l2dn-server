using System.Net;
using System.Text.Json.Serialization;

namespace L2Dn.GameServer.Configuration;

public class GameServerConfig
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string ListenAddress { get; set; } = "0.0.0.0";

    [JsonIgnore]
    public IPAddress ListenIpAddress { get; set; } = IPAddress.Any;

    [JsonRequired]
    public string PublishAddress { get; set; } = "127.0.0.1";

    [JsonIgnore]
    public IPAddress PublishIpAddress { get; set; } = IPAddress.Loopback;
    
    public int Port { get; set; } = 7777;
    public int MaxPlayerCount { get; set; } = 5000;
    public bool Pvp { get; set; }
    public bool Test { get; set; }
    public bool Clock { get; set; }
    public bool Brackets { get; set; }
}
