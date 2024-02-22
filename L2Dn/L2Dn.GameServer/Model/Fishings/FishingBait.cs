using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Fishings;

public class FishingBait
{
    private readonly int _itemId;
    private readonly int _level;
    private readonly int _minPlayerLevel;
    private readonly int _maxPlayerLevel;
    private readonly double _chance;
    private readonly TimeSpan _timeMin;
    private readonly TimeSpan _timeMax;
    private readonly TimeSpan _waitMin;
    private readonly TimeSpan _waitMax;
    private readonly bool _isPremiumOnly;
    private readonly List<FishingCatch> _rewards = new();

    public FishingBait(int itemId, int level, int minPlayerLevel, int maxPlayerLevel, double chance, TimeSpan timeMin,
        TimeSpan timeMax, TimeSpan waitMin, TimeSpan waitMax, bool isPremiumOnly)
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

    public TimeSpan getTimeMin()
    {
        return _timeMin;
    }

    public TimeSpan getTimeMax()
    {
        return _timeMax;
    }

    public TimeSpan getWaitMin()
    {
        return _waitMin;
    }

    public TimeSpan getWaitMax()
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