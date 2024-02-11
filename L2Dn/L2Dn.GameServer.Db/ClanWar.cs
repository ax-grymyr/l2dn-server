using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(Clan1Id), nameof(Clan2Id))]
public class ClanWar
{
    [Key]
    public int Clan1Id { get; set; }
    
    [ForeignKey(nameof(Clan1Id))]
    public Clan Clan1 { get; set; } = null!;
    [Key]
    public int Clan2Id { get; set; }
    
    [ForeignKey(nameof(Clan2Id))]
    public Clan Clan2 { get; set; } = null!;
 
    
    public int Clan1Kills { get; set; }
    public int Clan2Kills { get; set; }
    
    public int? WinnerClanId { get; set; }
    
    [ForeignKey(nameof(WinnerClanId))]
    public Clan? WinnerClan { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    public short State { get; set; }
}