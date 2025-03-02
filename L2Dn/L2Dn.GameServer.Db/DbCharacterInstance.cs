using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(InstanceId))]
public class DbCharacterInstance
{
    public int CharacterId { get; set; }
    public int InstanceId { get; set; }
    public DateTime Time { get; set; }
}