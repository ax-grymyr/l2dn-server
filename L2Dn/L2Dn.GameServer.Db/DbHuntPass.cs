using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbHuntPass
{
    [Key]
    public int AccountId { get; set; }
    public int CurrentStep { get; set; }
    public int Points { get; set; }
    public int RewardStep { get; set; }
    public bool IsPremium { get; set; }
    public int PremiumRewardStep { get; set; }
    public int SayhaPointsAvailable { get; set; }
    public int SayhaPointsUsed { get; set; }
    public bool UnclaimedReward { get; set; }
}