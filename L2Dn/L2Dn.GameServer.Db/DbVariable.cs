using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public abstract class DbVariable
{
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}