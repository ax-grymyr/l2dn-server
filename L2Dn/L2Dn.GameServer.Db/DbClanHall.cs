using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(OwnerId))]
public class DbClanHall
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public DateTime PaidUntil { get; set; }
}