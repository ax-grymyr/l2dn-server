using System.ComponentModel.DataAnnotations;
using L2Dn.GameServer.Enums;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(Id), IsUnique = true)]
public class DbCastle
{
    public int Id { get; set; }

    [Key]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    public CastleSide Side { get; set; } = CastleSide.NEUTRAL;

    public long Treasury { get; set; }

    public DateTime SiegeTime { get; set; }

    public bool RegistrationTimeOver { get; set; } = true;
    public DateTime RegistrationEndTime { get; set; }

    public bool ShowNpcCrest { get; set; }

    public short TicketBuyCount { get; set; }
}