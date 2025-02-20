using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Request;

public abstract class AbstractRequest
{
    private readonly Player _player;
    private DateTime _timestamp;
    private volatile bool _isProcessing;
    private ScheduledFuture? _timeOutTask;

    protected AbstractRequest(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        _player = player;
    }

    public Player getActiveChar()
    {
        return _player;
    }

    public DateTime getTimestamp()
    {
        return _timestamp;
    }

    public void setTimestamp(DateTime timestamp)
    {
        _timestamp = timestamp;
    }

    public void scheduleTimeout(TimeSpan delay)
    {
        _timeOutTask = ThreadPool.schedule(onTimeout, delay);
    }

    public bool isTimeout()
    {
        return _timeOutTask != null && !_timeOutTask.isDone();
    }

    public bool isProcessing()
    {
        return _isProcessing;
    }

    public bool setProcessing(bool isProcessing)
    {
        return _isProcessing = isProcessing;
    }

    public virtual bool canWorkWith(AbstractRequest request)
    {
        return true;
    }

    public virtual bool isItemRequest()
    {
        return false;
    }

    public abstract bool isUsing(int objectId);

    public virtual void onTimeout()
    {
    }
}
