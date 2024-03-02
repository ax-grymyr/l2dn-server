using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(NpcId), nameof(Name))]
public class DbNpcVariable: DbVariable
{
    public int NpcId { get; set; }
}