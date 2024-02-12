using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ItemId))]
public class CharacterOfflineTradeItem
{
    public int CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public Character Character { get; set; } = null!;
    
    public int ItemId { get; set; } // itemId(for buy) or ObjectId(for sell)
    
    public long Count { get; set; }
    public long Price { get; set; }
}