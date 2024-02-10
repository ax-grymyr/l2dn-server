using System.Text.Json.Serialization;

namespace L2Dn.DbModel.Configuration;

public class Config
{
    [JsonRequired]
    public DatabaseConfig Database { get; set; } = new();
}
