namespace L2Dn.GameServer.Configuration;

public class GameServerParamsConfig
{
    public byte ServerId { get; set; }
    public bool AcceptAlternateServerId { get; set; } // TODO: implement
    public GameServerType ServerType { get; set; }
    public byte AgeLimit { get; set; } = 15;
    public short MaxPlayerCount { get; set; } = 5000;
    public bool IsPvpServer { get; set; }
    public bool IsTestServer { get; set; }
    public bool Brackets { get; set; }
}