using System.Net;

namespace L2Dn.Configuration;

public class ListenerConfigBase
{
    public IPAddress ListenAddress { get; set; } = IPAddress.Any;
    public int Port { get; set; }
}