using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(AccountId), nameof(Name))]
public class DbAccountVariable: DbVariable
{
    public int AccountId { get; set; }
}