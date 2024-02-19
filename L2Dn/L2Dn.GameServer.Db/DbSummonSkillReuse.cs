using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(OwnerId), nameof(OwnerClassIndex), nameof(SummonSkillId), nameof(SkillId), nameof(SkillLevel))]
public class DbSummonSkillReuse
{
    public int OwnerId { get; set; }
    public byte OwnerClassIndex { get; set; }
    public int SummonSkillId { get; set; }
    public int SkillId { get; set; }
    public short SkillLevel { get; set; }
    public short SkillSubLevel { get; set; }
    public TimeSpan RemainingTime { get; set; }
    public byte BuffIndex { get; set; }
}