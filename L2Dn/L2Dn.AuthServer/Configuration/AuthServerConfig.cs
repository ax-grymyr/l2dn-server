using System.Net;
using System.Text.Json.Serialization;

namespace L2Dn.AuthServer.Configuration;

public class AuthServerConfig
{
    [JsonRequired]
    public string ListenAddress { get; set; } = "127.0.0.1";

    [JsonIgnore]
    public IPAddress ListenIpAddress { get; set; } = IPAddress.Loopback;
    
    public int Port { get; set; } = 2106;
    
    public bool AutoCreateAccounts { get; set; }
}
