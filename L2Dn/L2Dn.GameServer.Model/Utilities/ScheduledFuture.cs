using NLog;

namespace L2Dn.GameServer.Utilities;

public class ScheduledFuture
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ScheduledFuture));
    private readonly Action _action;
    private readonly Timer _timer;
    private readonly bool _oneTime;
    private bool _executing;
    private bool _cancelled;
    private bool _done;

    public ScheduledFuture(Action action, TimeSpan initialDelay, TimeSpan period)
    {
        _action = action;
        _oneTime = period == Timeout.InfiniteTimeSpan;
        _timer = new Timer(Run, null, initialDelay, period);
    }

    private void Run(object? state)
    {
        if (_executing)
        {
            return;
        }
        
        _executing = true;
        try
        {
            _action();
            if (_oneTime)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _done = true;
            }
        }
        catch (Exception e)
        {
            _logger.Error("Unhandled exception in scheduled task: " + e);
        }
        finally
        {
            _executing = false;
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