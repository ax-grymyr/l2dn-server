using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(MerchantId), nameof(CharacterId), nameof(Type))]
public class DbMerchantLease
{
    public int MerchantId { get; set; }
    public int CharacterId { get; set; }
    public int Bid { get; set; }
    public int Type { get; set; }

    [MaxLength(35)]
    public string CharacterName { get; set; } = string.Empty;
}