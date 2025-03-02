using L2Dn.Model;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(SubClass))]
public class DbCharacterSubClass
{
    public int CharacterId { get; set; }
    public CharacterClass SubClass { get; set; }
    public long Exp { get; set; }
    public long Sp { get; set; }
    public short Level { get; set; }
    public int VitalityPoints { get; set; }
    public byte ClassIndex { get; set; }
    public bool DualClass { get; set; }
}