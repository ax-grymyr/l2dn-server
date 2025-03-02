using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(RewardId))]
public class DbCharacterDailyReward
{
    public int CharacterId { get; set; }
    public int RewardId { get; set; }
    public DailyMissionStatus Status { get; set; } = DailyMissionStatus.AVAILABLE;
    public int Progress { get; set; }
    public DateTime LastCompleted { get; set; }
}

public enum DailyMissionStatus: byte
{
    AVAILABLE = 1,
    NOT_AVAILABLE = 2,
    COMPLETED = 3,
}