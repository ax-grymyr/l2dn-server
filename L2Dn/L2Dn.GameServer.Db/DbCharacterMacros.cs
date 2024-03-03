using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Id))]
public class DbCharacterMacros
{
    public int CharacterId { get; set; }
    public int Id { get; set; }
    public int? Icon { get; set; }

    [MaxLength(40)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(4)]
    public string Acronym { get; set; } = string.Empty;

    [MaxLength(1255)]
    public string Commands { get; set; } = string.Empty;
}