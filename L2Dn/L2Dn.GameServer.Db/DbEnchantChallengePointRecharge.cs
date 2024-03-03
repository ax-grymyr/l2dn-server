using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(GroupId), nameof(OptionIndex))]
public class DbEnchantChallengePointRecharge
{
    public int CharacterId { get; set; }
    public int GroupId { get; set; }
    public int OptionIndex { get; set; }
    public int Count { get; set; }
}