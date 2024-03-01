using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Name), nameof(Variable))]
public class CharacterQuest
{
    public int CharacterId { get; set; }

    [MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Variable { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Value { get; set; }
}