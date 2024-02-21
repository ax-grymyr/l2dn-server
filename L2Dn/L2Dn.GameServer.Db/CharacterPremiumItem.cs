using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ItemNumber), nameof(ItemId))]
public class CharacterPremiumItem
{
    public int CharacterId { get; set; }
    public int ItemNumber { get; set; }
    public int ItemId { get; set; }
    public long ItemCount { get; set; }

    [MaxLength(50)]
    public string ItemSender { get; set; } = string.Empty;
}