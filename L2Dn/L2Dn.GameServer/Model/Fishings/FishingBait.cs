using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Fishings;

public class FishingBait
{
    private readonly int _itemId;
    private readonly int _level;
    private readonly int _minPlayerLevel;
    private readonly int _maxPlayerLevel;
    private readonly double _chance;
    private readonly int _timeMin;
    private readonly int _timeMax;
    private readonly int _waitMin;
    private readonly int _waitMax;
    private readonly bool _isPremiumOnly;
    private readonly List<FishingCatch> _rewards = new();

    public FishingBait(int itemId, int level, int minPlayerLevel, int maxPlayerLevel, double chance, int timeMin,
        int timeMax, int waitMin, int waitMax, bool isPremiumOnly)
    {
        _itemId = itemId;
        _level = level;
        _minPlayerLevel = minPlayerLevel;
        _maxPlayerLevel = maxPlayerLevel;
        _chance = chance;
        _timeMin = timeMin;
        _timeMax = timeMax;
        _waitMin = waitMin;
        _waitMax = waitMax;
        _isPremiumOnly = isPremiumOnly;
    }

    public int getItemId()
    {
        return _itemId;
    }

    public int getLevel()
    {
        return _level;
    }

    public int getMinPlayerLevel()
    {
        return _minPlayerLevel;
    }

    public int getMaxPlayerLevel()
    {
        return _maxPlayerLevel;
    }

    public double getChance()
    {
        return _chance;
    }

    public int getTimeMin()
    {
        return _timeMin;
    }

    public int getTimeMax()
    {
        return _timeMax;
    }

    public int getWaitMin()
    {
        return _waitMin;
    }

    public int getWaitMax()
    {
        return _waitMax;
    }

    public bool isPremiumOnly()
    {
        return _isPremiumOnly;
    }

    public List<FishingCatch> getRewards()
    {
        return _rewards;
    }

    public void addReward(FishingCatch catchData)
    {
        _rewards.Add(catchData);
    }

    public FishingCatch getRandom()
    {
        float random = Rnd.get(100);
        foreach (FishingCatch fishingCatchData in _rewards)
        {
            if (fishingCatchData.getChance() > random)
            {
                return fishingCatchData;
            }

            random -= fishingCatchData.getChance();
        }

        return null;
    }
}