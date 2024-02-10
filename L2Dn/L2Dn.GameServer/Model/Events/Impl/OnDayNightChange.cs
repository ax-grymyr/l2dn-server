namespace L2Dn.GameServer.Model.Events.Impl;

public class OnDayNightChange: IBaseEvent
{
    private readonly bool _isNight;

    public OnDayNightChange(bool isNight)
    {
        _isNight = isNight;
    }

    public bool isNight()
    {
        return _isNight;
    }

    public EventType getType()
    {
        return EventType.ON_DAY_NIGHT_CHANGE;
    }
}