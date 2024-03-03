using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbAchievementBox
{
    [Key]
    public int CharacterId { get; set; }
    public int BoxOwned { get; set; }
    public int MonsterPoint { get; set; }
    public int PvpPoint { get; set; }
    public int PendingBox { get; set; }
    public DateTime? OpenTime { get; set; }
    public int BoxStateSlot1 { get; set; }
    public int BoxTypeSlot1 { get; set; }
    public int BoxStateSlot2 { get; set; }
    public int BoxTypeSlot2 { get; set; }
    public int BoxStateSlot3 { get; set; }
    public int BoxTypeSlot3 { get; set; }
    public int BoxStateSlot4 { get; set; }
    public int BoxTypeSlot4 { get; set; }
}