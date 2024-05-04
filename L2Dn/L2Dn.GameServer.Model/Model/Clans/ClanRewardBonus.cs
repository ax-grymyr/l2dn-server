using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Clans;

public class ClanRewardBonus
{
    private readonly ClanRewardType _type;
    private readonly int _level;
    private readonly int _requiredAmount;
    private readonly SkillHolder _skillReward;

    public ClanRewardBonus(ClanRewardType type, int level, int requiredAmount, SkillHolder skill)
    {
        _type = type;
        _level = level;
        _requiredAmount = requiredAmount;
        _skillReward = skill;
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
}