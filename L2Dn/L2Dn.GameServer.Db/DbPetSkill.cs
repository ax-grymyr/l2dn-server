using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(PetItemObjectId), nameof(SkillId), nameof(SkillLevel))]
public class DbPetSkill
{
    public int PetItemObjectId { get; set; }
    public int SkillId { get; set; }
    public short SkillLevel { get; set; } = 1;
}