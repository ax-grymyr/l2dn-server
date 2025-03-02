using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Category))]
public class DbCharacterPurge
{
    public int CharacterId { get; set; }
    public short Category { get; set; }
    public int Points { get; set; }
    public int Keys { get; set; }
    public int RemainingKeys { get; set; }
}