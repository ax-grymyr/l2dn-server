using System.Text.Json.Serialization;

namespace L2Dn.GameServer.Configuration;

public class ProtocolConfig
{
    [JsonRequired]
    public int Version { get; set; }

    public bool Encryption { get; set; }
}
