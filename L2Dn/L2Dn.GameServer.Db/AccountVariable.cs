using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(AccountId), nameof(Name))]
public class AccountVariable: DbVariable
{
    public int AccountId { get; set; }
}