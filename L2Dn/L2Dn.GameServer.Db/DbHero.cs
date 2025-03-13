using System.ComponentModel.DataAnnotations;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Db;

public class DbHero
{
    [Key]
    public int CharacterId { get; set; }

    public CharacterClass ClassId { get; set; }
    public short Count { get; set; }
    public short LegendCount { get; set; }
    public bool Played { get; set; }
    public bool Claimed { get; set; }

    [MaxLength(300)]
    public string? Message { get; set; }
}