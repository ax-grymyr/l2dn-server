using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(AccountId), nameof(CollectionId), nameof(Index))]
public class AccountCollection
{
    public int AccountId { get; set; }
    public short CollectionId { get; set; }
    public short Index { get; set; }
    public int ItemId { get; set; }
}