using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ItemObjectId), nameof(Index), nameof(Level))]
public class DbPetEvolve
{
    public int ItemObjectId { get; set; }
    public int Index { get; set; }
    public int Level { get; set; }
}