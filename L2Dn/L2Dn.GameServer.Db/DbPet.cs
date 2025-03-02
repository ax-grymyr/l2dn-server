using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(ItemObjectId), IsUnique = true)]
public class DbPet
{
    [Key]
    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public DbCharacter Owner { get; set; } = null!;

    public int ItemObjectId { get; set; }

    [MaxLength(36)]
    public string Name { get; set; } = string.Empty;

    public short Level { get; set; }

    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }

    public long Exp { get; set; }
    public long Sp { get; set; }
    public int Fed { get; set; }

    public bool Restore { get; set; }
}