using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ContactId))]
public class DbCharacterContact
{
    public int CharacterId { get; set; }
    public int ContactId { get; set; }
}