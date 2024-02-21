using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ClassIndex), nameof(SkillId), nameof(SkillLevel))]
public class CharacterSkillReuse
{
    public int CharacterId { get; set; }
    public int SkillId { get; set; }
    public short SkillLevel { get; set; }
    public short SkillSubLevel { get; set; }
    public TimeSpan? RemainingTime { get; set; }
    public TimeSpan ReuseDelay { get; set; }
    public DateTime? SysTime { get; set; }
    public byte RestoreType { get; set; }
    public byte ClassIndex { get; set; }
    public byte BuffIndex { get; set; }
}