using NLog;

namespace L2Dn.GameServer.Utilities;

public class ScheduledFuture
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ScheduledFuture));
    private readonly Action _action;
    private readonly Timer _timer;
    private readonly bool _oneTime;
    private bool _cancelled;
    private bool _done;

    public ScheduledFuture(Action action, int initialDelayInMs, int periodInMs)
    {
        _action = action;
        _oneTime = periodInMs == Timeout.Infinite;
        _timer = new Timer(Run, null, initialDelayInMs, periodInMs);
    }

    private void Run(object? state)
    {
        try
        {
            _action();
        }
        catch (Exception e)
        {
            _logger.Error("Unhandled exception in scheduled task: " + e);
        }

        if (_oneTime)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _done = true;
        }
    }

    public bool cancel(bool mayInterruptIfRunning)
    {
        if (_cancelled || _done)
            return false;
        
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        _timer.Dispose();
        _cancelled = true;
        return true;
    }

    public bool isCancelled()
    {
        return _cancelled;
    }

    public bool isDone()
    {
        return _done;
    }

    public TimeSpan getDelay()
    {
        throw new NotImplementedException();
    }
}