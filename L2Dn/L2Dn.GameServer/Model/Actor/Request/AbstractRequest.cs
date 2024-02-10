using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Request;

public abstract class AbstractRequest
{
    private readonly Player _player;
    private long _timestamp = 0;
    private volatile bool _isProcessing;
    private ScheduledFuture<?> _timeOutTask;

    public AbstractRequest(Player player)
    {
        Objects.requireNonNull(player);
        _player = player;
    }

    public Player getActiveChar()
    {
        return _player;
    }

    public long getTimestamp()
    {
        return Interlocked.Read(ref _timestamp);
    }

    public void setTimestamp(long timestamp)
    {
        Interlocked.Exchange(ref _timestamp, timestamp);
    }

    public void scheduleTimeout(long delay)
    {
        _timeOutTask = ThreadPool.schedule(this::onTimeout, delay);
    }

    public bool isTimeout()
    {
        return (_timeOutTask != null) && !_timeOutTask.isDone();
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
