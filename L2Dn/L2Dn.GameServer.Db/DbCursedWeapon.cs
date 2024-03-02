using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(CharacterId), IsUnique = true)]
public class DbCursedWeapon
{
    [Key]
    public int ItemId { get; set; }

    public int CharacterId { get; set; }
    public int PlayerReputation { get; set; }
    public int PlayerPkKills { get; set; }
    public int NbKills { get; set; }
    public DateTime? EndTime { get; set; }
}