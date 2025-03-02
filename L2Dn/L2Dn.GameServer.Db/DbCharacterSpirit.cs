using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Type))]
public class DbCharacterSpirit
{
    public int CharacterId { get; set; }
    public byte Type { get; set; } // TODO enum
    public byte Level { get; set; }
    public byte Stage { get; set; }
    public long Exp { get; set; }
    public byte AttackPoints { get; set; }
    public byte DefensePoints { get; set; }
    public byte CriticalRatePoints { get; set; }
    public byte CriticalDamagePoints { get; set; }
    public bool IsInUse { get; set; }
}