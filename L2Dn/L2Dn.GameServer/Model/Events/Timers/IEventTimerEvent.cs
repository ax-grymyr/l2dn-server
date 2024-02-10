namespace L2Dn.GameServer.Model.Events.Timers;

public interface IEventTimerEvent<T>
{
    /**
     * notified upon timer execution method.
     * @param holder
     */
    void onTimerEvent(TimerHolder<T> holder);
}
