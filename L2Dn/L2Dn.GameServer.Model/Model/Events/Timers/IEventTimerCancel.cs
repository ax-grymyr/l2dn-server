namespace L2Dn.GameServer.Model.Events.Timers;

public interface IEventTimerCancel<T>
    where T: notnull
{
    /**
     * Notified upon timer cancellation.
     * @param holder
     */
    void onTimerCancel(TimerHolder<T> holder);
}