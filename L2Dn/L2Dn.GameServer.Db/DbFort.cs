using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(OwnerId), IsUnique = true)]
public class DbFort
{
    public int Id { get; set; }
    
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    public DateTime SiegeDate { get; set; }
    public DateTime LastOwnedTime { get; set; }
    public int OwnerId { get; set; }
    public int Type { get; set; }
    public int State { get; set; }
    public int CastleId { get; set; }
    public int SupplyLevel { get; set; }
}