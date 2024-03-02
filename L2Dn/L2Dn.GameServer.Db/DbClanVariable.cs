using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(Name))]
public class DbClanVariable: DbVariable
{
    public int ClanId { get; set; }
}