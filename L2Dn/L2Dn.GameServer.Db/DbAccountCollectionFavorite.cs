using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(AccountId), nameof(CollectionId))]
public class DbAccountCollectionFavorite
{
    public int AccountId { get; set; }
    public short CollectionId { get; set; }
}