using System.Diagnostics;
using NLog;

namespace L2Dn.GameServer.Utilities;

public static class ThreadPool
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ThreadPool));

    public static void execute(Runnable runnable)
    {
        System.Threading.ThreadPool.QueueUserWorkItem(state =>
        {
            try
            {
                runnable.run();
            }
            catch (Exception e)
            {
                LOGGER.Error("Unhandled exception in task: " + e);
            }
        });
    }

    public static void execute(Action action)
    {
        System.Threading.ThreadPool.QueueUserWorkItem(state =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                LOGGER.Error("Unhandled exception in task: " + e);
            }
        });
    }

    public static ScheduledFuture scheduleAtFixedRate(Action action, int initialDelayInMs, int periodInMs)
    {
        return scheduleAtFixedRate(action, TimeSpan.FromMilliseconds(initialDelayInMs), TimeSpan.FromMilliseconds(periodInMs));
    }
    
    public static ScheduledFuture scheduleAtFixedRate(Action action, TimeSpan initialDelayInMs, TimeSpan periodInMs)
    {
        return new ScheduledFuture(action, Validate(initialDelayInMs), Validate(periodInMs));
    }
    
    public static ScheduledFuture scheduleAtFixedRate(Runnable runnable, TimeSpan initialDelayInMs, TimeSpan periodInMs)
    {
        return new ScheduledFuture(runnable.run, Validate(initialDelayInMs), Validate(periodInMs));
    }
    
    public static ScheduledFuture scheduleAtFixedRate(Runnable runnable, int initialDelayInMs, int periodInMs)
    {
        return scheduleAtFixedRate(runnable.run, initialDelayInMs, periodInMs);
    }
    
    public static ScheduledFuture schedule(Runnable runnable, int delayInMs)
    {
        return schedule(runnable.run, TimeSpan.FromMilliseconds(delayInMs));
    }
    
    public static ScheduledFuture schedule(Runnable runnable, TimeSpan delay)
    {
        return new ScheduledFuture(runnable.run, delay, Timeout.InfiniteTimeSpan);
    }
    
    public static ScheduledFuture schedule(Action action, int delayInMs)
    {
        return schedule(action, TimeSpan.FromMilliseconds(delayInMs));
    }
    
    public static ScheduledFuture schedule(Action action, TimeSpan delay)
    {
        return new ScheduledFuture(action, delay, Timeout.InfiniteTimeSpan);
    }
    
    private static TimeSpan Validate(TimeSpan delay)
    {
        if (delay < TimeSpan.Zero)
        {
            StackTrace stackTrace = new StackTrace();
            LOGGER.Error("ThreadPool found delay " + delay + "!\n" + stackTrace);
            return TimeSpan.Zero;
        }
        
        return delay;
    }
}