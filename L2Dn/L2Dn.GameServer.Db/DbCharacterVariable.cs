using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Name))]
public class DbCharacterVariable: DbVariable
{
    public int CharacterId { get; set; }
}