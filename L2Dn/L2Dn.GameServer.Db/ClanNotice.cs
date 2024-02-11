using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class ClanNotice
{
    [Key]
    public int ClanId { get; set; }
    
    [ForeignKey(nameof(ClanId))]
    public Clan Clan { get; set; } = null!;
    
    public bool Enabled { get; set; }

    public string Notice { get; set; } = string.Empty;
}