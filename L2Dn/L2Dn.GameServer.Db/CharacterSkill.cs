using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ClassIndex), nameof(SkillId))]
public class CharacterSkill
{
    public int CharacterId { get; set; }
    public int SkillId { get; set; }
    public short SkillLevel { get; set; }
    public short SkillSubLevel { get; set; }
    public byte ClassIndex { get; set; }
}