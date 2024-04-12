namespace L2Dn.GameServer.Configuration;

public class GeoDataConfig
{
    public bool Download { get; set; }
    public bool Update { get; set; }
    public string FileListUrl { get; set; } = string.Empty;
}