using System.Text.Json.Serialization;

namespace L2Dn.DbModel.Configuration;

public class DatabaseConfig
{
    [JsonRequired]
    public string Server { get; set; } = string.Empty;
    
    [JsonRequired]
    public string DatabaseName { get; set; } = string.Empty;
    
    [JsonRequired]
    public string UserName { get; set; } = string.Empty;
    
    [JsonRequired]
    public string Password { get; set; } = string.Empty;
}
