using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class CharacterRevenge
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int CharacterId { get; set; }
    public int Type { get; set; } // TODO enum RevengeType

    [MaxLength(35)]
    public string KillerName { get; set; } = string.Empty;

    [MaxLength(45)]
    public string KillerClan { get; set; } = string.Empty;

    public int KillerLevel { get; set; }
    public CharacterClass KillerClass { get; set; }

    [MaxLength(35)]
    public string VictimName { get; set; } = string.Empty;

    [MaxLength(45)]
    public string VictimClan { get; set; } = string.Empty;

    public int VictimLevel { get; set; }
    public CharacterClass VictimClass { get; set; }
    public bool Shared { get; set; }
    public int ShowLocationRemaining { get; set; }
    public int TeleportRemaining { get; set; }
    public int SharedTeleportRemaining { get; set; }
    public DateTime KillTime { get; set; }
    public DateTime ShareTime { get; set; }
}