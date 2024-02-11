using System.Net;

namespace L2Dn.GameServer.Configuration;

public class AuthServerConnectionConfig
{
    public string Address { get; set; } = "localhost";
    public int Port { get; set; } = 2107;

    public string AccessKey { get; set; } = string.Empty;
    public IPAddress PublishAddress { get; set; } = IPAddress.Any;
}