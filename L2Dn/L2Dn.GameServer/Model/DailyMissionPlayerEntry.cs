using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model;

public class DailyMissionPlayerEntry
{
    private readonly int _objectId;
    private readonly int _rewardId;
    private DailyMissionStatus _status = DailyMissionStatus.NOT_AVAILABLE;
    private int _progress;
    private long _lastCompleted;
    private bool _recentlyCompleted;
	
    public DailyMissionPlayerEntry(int objectId, int rewardId)
    {
        _objectId = objectId;
        _rewardId = rewardId;
    }
	
    public DailyMissionPlayerEntry(int objectId, int rewardId, DailyMissionStatus status, int progress, long lastCompleted):this(objectId, rewardId)
    {
        _status = status;
        _progress = progress;
        _lastCompleted = lastCompleted;
    }
	
    public int getObjectId()
    {
        return _objectId;
    }
	
    public int getRewardId()
    {
        return _rewardId;
    }
	
    public DailyMissionStatus getStatus()
    {
        return _status;
    }
	
    public void setStatus(DailyMissionStatus status)
    {
        _status = status;
    }
	
    public int getProgress()
    {
        return _progress;
    }
	
    public void setProgress(int progress)
    {
        _progress = progress;
    }
	
    public int increaseProgress()
    {
        _progress++;
        return _progress;
    }
	
    public long getLastCompleted()
    {
        return _lastCompleted;
    }
	
    public void setLastCompleted(long lastCompleted)
    {
        _lastCompleted = lastCompleted;
    }
	
    public bool isRecentlyCompleted()
    {
        return _recentlyCompleted;
    }
	
    public void setRecentlyCompleted(bool recentlyCompleted)
    {
        _recentlyCompleted = recentlyCompleted;
    }
}
