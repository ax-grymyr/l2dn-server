using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class MissionLevelHolder
{
    private int _maxLevel;
    private readonly int _bonusLevel;
    private readonly Map<int, int> _xpForLevel = new();
    private readonly Map<int, ItemHolder> _normalReward = new();
    private readonly Map<int, ItemHolder> _keyReward = new();
    private readonly ItemHolder? _specialReward;
    private readonly ItemHolder? _bonusReward;
    private readonly bool _bonusRewardIsAvailable;
    private readonly bool _bonusRewardByLevelUp;

    public MissionLevelHolder(int maxLevel, int bonusLevel, Map<int, int> xpForLevel, Map<int, ItemHolder> normalReward,
        Map<int, ItemHolder> keyReward, ItemHolder? specialReward, ItemHolder? bonusReward, bool bonusRewardByLevelUp,
        bool bonusRewardIsAvailable)
    {
        _maxLevel = maxLevel;
        _bonusLevel = bonusLevel;
        _xpForLevel.putAll(xpForLevel);
        _normalReward.putAll(normalReward);
        _keyReward.putAll(keyReward);
        _specialReward = specialReward;
        _bonusReward = bonusReward;
        _bonusRewardByLevelUp = bonusRewardByLevelUp;
        _bonusRewardIsAvailable = bonusRewardIsAvailable;
    }

    public int getMaxLevel()
    {
        return _maxLevel;
    }

    public void setMaxLevel(int maxLevel)
    {
        _maxLevel = maxLevel;
    }

    public int getBonusLevel()
    {
        return _bonusLevel;
    }

    public Map<int, int> getXPForLevel()
    {
        return _xpForLevel;
    }

    public int getXPForSpecifiedLevel(int level)
    {
        return _xpForLevel.get(level == 0 ? level + 1 : level);
    }

    public Map<int, ItemHolder> getNormalRewards()
    {
        return _normalReward;
    }

    public Map<int, ItemHolder> getKeyRewards()
    {
        return _keyReward;
    }

    public ItemHolder? getSpecialReward()
    {
        return _specialReward;
    }

    public ItemHolder? getBonusReward()
    {
        return _bonusReward;
    }

    public bool getBonusRewardByLevelUp()
    {
        return _bonusRewardByLevelUp;
    }

    public bool getBonusRewardIsAvailable()
    {
        return _bonusRewardIsAvailable;
    }
}