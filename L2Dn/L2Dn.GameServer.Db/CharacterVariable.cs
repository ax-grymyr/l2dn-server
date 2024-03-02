using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Name))]
public class CharacterVariable: DbVariable
{
    public int CharacterId { get; set; }
}