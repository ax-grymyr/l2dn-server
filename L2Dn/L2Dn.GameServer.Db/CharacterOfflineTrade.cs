using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class CharacterOfflineTrade
{
    [Key]
    public int CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public Character Character { get; set; } = null!;
    
    public DateTime Time { get; set; }
    
    public byte Type { get; set; }

    [MaxLength(50)]
    public string? Title { get; set; }
}