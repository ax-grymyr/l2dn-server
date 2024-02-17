using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Name))]
public class CharacterVariable
{
    public int CharacterId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Value { get; set; }
}