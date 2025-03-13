namespace L2Dn.GameServer.Configuration;

public class DataPackConfig
{
    /// <summary>
    /// DataPath root path, absolute or relative to the executable.
    /// </summary>
    public string Path { get; set; } = "DataPack";

    /// <summary>
    /// Scripts root path, absolute or relative to the executable.
    /// </summary>
    public string ScriptPath { get; set; } = "DataPack/scripts";

    /// <summary>
    /// Geodata loading configuration.
    /// </summary>
    public GeoDataConfig GeoData { get; set; } = new();
}