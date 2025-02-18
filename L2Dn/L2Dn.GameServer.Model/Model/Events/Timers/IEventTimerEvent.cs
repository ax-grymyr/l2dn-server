namespace L2Dn.GameServer.Model.Events.Timers;

public interface IEventTimerEvent<T>
    where T: notnull
{
    /**
     * notified upon timer execution method.
     * @param holder
     */
    void onTimerEvent(TimerHolder<T> holder);
}