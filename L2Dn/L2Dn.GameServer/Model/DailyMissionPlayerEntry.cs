using L2Dn.GameServer.Db;

namespace L2Dn.GameServer.Model;

public class DailyMissionPlayerEntry
{
    private readonly int _objectId;
    private readonly int _rewardId;
    private DailyMissionStatus _status = DailyMissionStatus.NOT_AVAILABLE;
    private int _progress;
    private DateTime _lastCompleted;
    private bool _recentlyCompleted;
	
    public DailyMissionPlayerEntry(int objectId, int rewardId)
    {
        _objectId = objectId;
        _rewardId = rewardId;
    }
	
    public DailyMissionPlayerEntry(int objectId, int rewardId, DailyMissionStatus status, int progress, DateTime lastCompleted):this(objectId, rewardId)
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
	
    public DateTime getLastCompleted()
    {
        return _lastCompleted;
    }
	
    public void setLastCompleted(DateTime lastCompleted)
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
