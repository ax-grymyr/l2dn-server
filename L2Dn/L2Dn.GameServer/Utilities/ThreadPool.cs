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
        return new ScheduledFuture(action, Validate(initialDelayInMs), Validate(periodInMs));
    }
    
    public static ScheduledFuture scheduleAtFixedRate(Action action, TimeSpan initialDelayInMs, TimeSpan periodInMs)
    {
        return new ScheduledFuture(action, Validate((int)initialDelayInMs.TotalMilliseconds),
            Validate((int)periodInMs.TotalMilliseconds));
    }
    
    public static ScheduledFuture scheduleAtFixedRate(Runnable runnable, int initialDelayInMs, int periodInMs)
    {
        return scheduleAtFixedRate(runnable.run, initialDelayInMs, periodInMs);
    }
    
    public static ScheduledFuture schedule(Runnable runnable, int delayInMs)
    {
        return new ScheduledFuture(runnable.run, Validate(delayInMs), Timeout.Infinite);
    }
    
    public static ScheduledFuture schedule(Runnable runnable, TimeSpan delay)
    {
        return schedule(runnable, (int)delay.TotalMilliseconds);
    }
    
    public static ScheduledFuture schedule(Action action, int delayInMs)
    {
        return new ScheduledFuture(action, Validate(delayInMs), Timeout.Infinite);
    }
    
    public static ScheduledFuture schedule(Action action, TimeSpan delay)
    {
        return schedule(action, (int)delay.TotalMilliseconds);
    }
    
    private static int Validate(int delay)
    {
        if (delay < 0)
        {
            LOGGER.Warn("ThreadPool found delay " + delay + "!");
            StackTrace stackTrace = new StackTrace();
            LOGGER.Warn(stackTrace.ToString());
            return 0;
        }
        
        return delay;
    }
}