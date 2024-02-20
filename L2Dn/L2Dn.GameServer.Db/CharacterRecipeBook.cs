using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(Id), nameof(CharacterId), nameof(ClassIndex))]
public class CharacterRecipeBook
{
    public int CharacterId { get; set; }
    public int Id { get; set; }
    public short ClassIndex { get; set; }
    public int Type { get; set; }
}