using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(Name))]
public class DbGlobalVariable: DbVariable
{
}