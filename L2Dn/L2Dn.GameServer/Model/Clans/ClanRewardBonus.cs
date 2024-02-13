using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Clans;

public class ClanRewardBonus
{
    private readonly ClanRewardType _type;
    private readonly int _level;
    private readonly int _requiredAmount;
    private SkillHolder _skillReward;

    public ClanRewardBonus(ClanRewardType type, int level, int requiredAmount)
    {
        _type = type;
        _level = level;
        _requiredAmount = requiredAmount;
    }

    public ClanRewardType getType()
    {
        return _type;
    }

    public int getLevel()
    {
        return _level;
    }

    public int getRequiredAmount()
    {
        return _requiredAmount;
    }

    public SkillHolder getSkillReward()
    {
        return _skillReward;
    }

    public void setSkillReward(SkillHolder skillReward)
    {
        _skillReward = skillReward;
    }
}