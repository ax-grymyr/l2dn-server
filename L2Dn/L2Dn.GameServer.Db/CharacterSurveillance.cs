using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(TargetId))]
public class CharacterSurveillance
{
    public int CharacterId { get; set; }
    public int TargetId { get; set; }
}