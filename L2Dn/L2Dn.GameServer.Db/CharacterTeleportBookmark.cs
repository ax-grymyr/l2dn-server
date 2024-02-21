using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Id))]
public class CharacterTeleportBookmark
{
    public int CharacterId { get; set; }
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Icon { get; set; }
    
    [MaxLength(50)]
    public string? Tag { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}