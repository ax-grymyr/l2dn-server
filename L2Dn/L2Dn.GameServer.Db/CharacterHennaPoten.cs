using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(SlotPosition))]
public class CharacterHennaPoten
{
    public int CharacterId { get; set; }
    public int SlotPosition { get; set; }
    public int PotenId { get; set; }
    public int EnchantLevel { get; set; }
    public int EnchantExp { get; set; }
}