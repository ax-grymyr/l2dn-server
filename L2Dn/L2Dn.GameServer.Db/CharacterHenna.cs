using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ClassIndex), nameof(Slot))]
public class CharacterHenna
{
    public int CharacterId { get; set; }
    public int SymbolId { get; set; }
    public int Slot { get; set; } 
    public byte ClassIndex { get; set; } 
}