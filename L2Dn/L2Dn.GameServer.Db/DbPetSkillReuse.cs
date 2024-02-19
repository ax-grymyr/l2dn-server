using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(PetItemObjectId), nameof(SkillId), nameof(SkillLevel))]
public class DbPetSkillReuse
{
    public int PetItemObjectId { get; set; }
    public int SkillId { get; set; }
    public short SkillLevel { get; set; }
    public short SkillSubLevel { get; set; }
    public TimeSpan RemainingTime { get; set; }
    public byte BuffIndex { get; set; }
}