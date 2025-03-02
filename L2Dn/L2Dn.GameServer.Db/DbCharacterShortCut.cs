using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ClassIndex), nameof(Slot), nameof(Page))]
[Index(nameof(ShortCutId))]
public class DbCharacterShortCut
{
    public int CharacterId { get; set; }
    public byte Slot { get; set; }
    public byte Page { get; set; }
    public byte Type { get; set; }
    public int ShortCutId { get; set; }
    public short SkillLevel { get; set; }
    public short SkillSubLevel { get; set; }
    public byte ClassIndex { get; set; }
}