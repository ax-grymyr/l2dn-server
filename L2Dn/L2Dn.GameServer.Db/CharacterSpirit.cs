using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Type))]
public class CharacterSpirit
{
    public int CharacterId { get; set; }
    public byte Type { get; set; } // TODO enum
    public byte Level { get; set; }
    public byte Stage { get; set; }
    public long Exp { get; set; }
    public short AttackPoints { get; set; }
    public short DefensePoints { get; set; }
    public short CriticalRatePoints { get; set; }
    public short CriticalDamagePoints { get; set; }
    public bool IsInUse { get; set; }
}